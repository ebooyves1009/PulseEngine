using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.Playables;

/// <summary>
/// Represent one layer one the anima state machine
/// </summary>
[System.Serializable]
public class AnimancerMachineLayer
{
    #region Constants #############################################################

    private const int STATES_COUNT = 3;

    #endregion

    #region Variables #############################################################

    /// <summary>
    /// The default layer motion
    /// </summary>
    [SerializeField] private AnimaMotion _defaultMotion;

    /// <summary>
    /// The index of the current motion
    /// </summary>
    [SerializeField] private int _currentStateIndex;

    /// <summary>
    /// The blend tree value of child states
    /// </summary>
    [SerializeField][Range(0, 1)] private float _blendWeight = 1;

    /// <summary>
    /// Active whe the layer had been initialized
    /// </summary>
    private bool _isInitialized = false;


    /// <summary>
    /// The layer's motions mixers
    /// </summary>
    private AnimationLayerMixerPlayable[] _animationJobMixer = new AnimationLayerMixerPlayable[STATES_COUNT];

    /// <summary>
    /// The layer main mixer
    /// </summary>
    private AnimationMixerPlayable _mainMixer;

    /// <summary>
    /// The current motion sub mixers
    /// </summary>
    private AnimationMixerPlayable _currentSubMixer;

    /// <summary>
    /// The next motion to be played in some cases
    /// </summary>
    private AnimaMotion _chainMotion;

    /// <summary>
    /// The currently played motion
    /// </summary>
    private AnimaMotion _currentMotion;


    /// <summary>
    /// the state machine this layer belong to.
    /// </summary>
    private AnimancerStateMachine _stateMachine;

    /// <summary>
    /// Reference to all valid animations
    /// </summary>
    private List<AnimationClipPlayable> _allAnimations = new List<AnimationClipPlayable>();

    #endregion

    #region cache   #############################################################

    /// <summary>
    /// Used to get the <see cref="CurrentPlayedAnimation"/>
    /// </summary>
    List<(int, float)> _inputsWeights = new List<(int, float)>();

    #endregion

    #region Inner Types ###########################################################

    #endregion

    #region Properties ############################################################

    /// <summary>
    /// Get the currently processed playable
    /// </summary>
    private AnimationClipPlayable CurrentPlayedAnimation
    {
        get
        {
            if (_currentSubMixer.IsValid())
            {
                _inputsWeights.Clear();
                for (int i = 0, len = _currentSubMixer.GetInputCount(); i < len; i++)
                {
                    _inputsWeights.Add((i, _currentSubMixer.GetInputWeight(i)));
                }
                if (_inputsWeights.Count <= 0)
                    return default;
                _inputsWeights.Sort((i1, i2) => { return i1.Item2.CompareTo(i2.Item2); });
                _inputsWeights.Reverse();
                return (AnimationClipPlayable)_currentSubMixer.GetInput(_inputsWeights[0].Item1);
            }
            return default;
        }
    }

    /// <summary>
    /// Is this layer in transition?
    /// </summary>
    [field: SerializeField] public bool InTransition { get; private set; }

    /// <summary>
    /// The blend tree value of child states
    /// </summary>
    public float BlendWeight { get => _blendWeight; set => _blendWeight = value; }

    /// <summary>
    /// Return the current motion on this state.
    /// </summary>
    public AnimaMotion CurrentMotion { get => _currentMotion; }

    #endregion

    #region Public Functions ######################################################

    /// <summary>
    /// Play a motion on this Layer
    /// </summary>
    /// <param name="motion"></param>
    /// <param name="priority"></param>
    /// <param name="updateAction"></param>
    public bool PlayState(PlayableGraph _playableGraph, AnimaMotion motion, Action<float> updateAction = null)
    {
        if (!_playableGraph.IsValid())
            return false;
        if (!_isInitialized || _stateMachine == null)
            return false;
        if (motion == null)
            return false;
        if (motion.Clips == null || motion.Clips.Length <= 0)
            return false;
        if (motion.UpdateAction == null)
            motion.UpdateAction = updateAction;
        if (_currentMotion != null)
        {
            //Handle lower priority motion request
            if (_currentMotion.Priority > motion.Priority)
            {
                if (motion.Priority <= 0)
                {
                    if (_chainMotion && _chainMotion.Priority > motion.Priority)
                        return false;
                    _chainMotion = motion;
                }
                return false;
            }
            //handle same priority motion request
            if (_currentMotion.Priority == motion.Priority)
            {
                var currentAnim = CurrentPlayedAnimation;
                if (currentAnim.IsValid())
                {
                    //if the current animation is not at the end
                    if (currentAnim.GetTime() < currentAnim.GetAnimationClip().length - _stateMachine.Transition)
                    {
                        if (motion.Priority > 0)
                        {
                            if (_chainMotion && _chainMotion.Priority > motion.Priority)
                                return false;
                            _chainMotion = motion;
                            return false;
                        }
                    }
                    //if (InTransition)
                    //    return false;
                }
            }
            //The motion currently playing is the same as the requested?
            if (_currentMotion.Statename == motion.Statename)
                return false;
        }

        //The current motion is the one we keep in backup chain
        if (_chainMotion == motion)
        {
            motion.UpdateAction = _chainMotion.UpdateAction;
            _chainMotion = null;
        }

        //Set the last motion update to null
        if (_currentMotion)
            _currentMotion.UpdateAction = null;

        //Swith to the next state.
        _currentStateIndex++;
        _currentStateIndex %= _animationJobMixer.Length;

        //Mixer
        var mixer = AnimationMixerPlayable.Create(_playableGraph, motion.Clips.Length, true);

        for (int i = 0; i < motion.Clips.Length; i++)
        {
            //Clips
            var animClip = AnimationClipPlayable.Create(_playableGraph, motion.Clips[i]);
            animClip.SetTime(0);
            _allAnimations.Add(animClip);
            //connect
            _playableGraph.Connect(animClip, 0, mixer, i);
            _playableGraph.Disconnect(_animationJobMixer[_currentStateIndex], 0);
            _playableGraph.Connect(mixer, 0, _animationJobMixer[_currentStateIndex], 0);
        }
        //Weight
        mixer.SetInputWeight(0, 1);
        _currentSubMixer = mixer;
        _currentMotion = motion;
        return true;
    }


    #endregion

    #region Private Functions #####################################################

    private Vector2 NormalizedTransitionSpeed(float fromDuration, float toDuration, float fromWeight, float toWeight, float speed)
    {
        float avgDuration = (fromDuration + toDuration) / 2;
        double fromDurationRatio = avgDuration / toDuration;
        double toDurationRatio = avgDuration / fromDuration;
        float fromFactor = (float)fromDurationRatio;
        float toFactor = (float)toDurationRatio;
        float fromSpeedRange = Mathf.Lerp(speed * fromFactor, speed, fromWeight <= 0.5f ? 0 : fromWeight);
        float toSpeedRange = Mathf.Lerp(speed * toFactor, speed, toWeight <= 0.5f ? 0 : toWeight);
        return new Vector2(fromSpeedRange, toSpeedRange);
    }

    /// <summary>
    /// Back to default State
    /// </summary>
    /// <param name="forceIdlePriority">if the priority is above the current motion's one, the default layer motion will be forced</param>
    private bool ReturnToDefaultState(int forceIdlePriority = -1)
    {
        PlayableGraph currentGraph = CurrentPlayedAnimation.GetGraph();
        if (_currentMotion && forceIdlePriority >= _currentMotion.Priority)
        {
            return PlayState(currentGraph, _defaultMotion);
        }
        if (_chainMotion)
        {
            return PlayState(currentGraph, _chainMotion);
        }
        return PlayState(currentGraph, _defaultMotion);
    }

    /// <summary>
    /// set animations time to 0.
    /// </summary>
    /// <param name="node"></param>
    private void LoopTimeOnAllAnimations(float delta)
    {
        for (int i = _allAnimations.Count - 1; i >= 0; i--)
        {
            if (_allAnimations[i].IsValid())
            {
                if (_allAnimations[i].GetTime() > _allAnimations[i].GetAnimationClip().length)
                {
                    //_allAnimations[i].SetTime(0);
                }
            }
            else
            {
                _allAnimations.RemoveAt(i);
            }
        }
    }

    #endregion

    #region Jobs      #############################################################

    public struct AnimationJob : IAnimationJob
    {
        public float weight;
        public float delta;

        public void TriggerEvent() { }

        public void ProcessRootMotion(AnimationStream stream)
        {
            // This method is called during the root motion process pass.
        }

        public void ProcessAnimation(AnimationStream stream)
        {
            // This method is called during the animation process pass.
        }
    }

    #endregion

    #region Flow ########################################################

    /// <summary>
    /// Initialize the layer
    /// </summary>
    public AnimationMixerPlayable InitLayer(PlayableGraph _playableGraph, AnimancerStateMachine stateMachine)
    {
        //Graph
        if (!_playableGraph.IsValid())
            return default;

        if (_defaultMotion == null)
        {
            PulseDebug.LogError("Please provide a default state for this Anima Layer");
            return default;
        }

        //State machine 
        _stateMachine = stateMachine;

        //Animation Mixers
        for (int i = 0; i < STATES_COUNT; i++)
        {
            _animationJobMixer[i] = AnimationLayerMixerPlayable.Create(_playableGraph, 1);
        }

        //Mixers
        _mainMixer = AnimationMixerPlayable.Create(_playableGraph, _animationJobMixer.Length, true);


        //Clips
        var clip_0 = AnimationClipPlayable.Create(_playableGraph, _defaultMotion.Clips[0]);

        //connect clip to child mixers
        _playableGraph.Connect(clip_0, 0, _animationJobMixer[0], 0);

        for (int i = 0; i < _animationJobMixer.Length; i++)
        {
            //connect child mixers to main mixers
            _playableGraph.Connect(_animationJobMixer[i], 0, _mainMixer, i);
            //Weight setting
            _animationJobMixer[i].SetInputWeight(0, 1);
        }

        //Weight setting 2
        _mainMixer.SetInputWeight(0, 1);

        _isInitialized = true;

        return _mainMixer;
    }

    /// <summary>
    /// Evaluate the layer
    /// </summary>
    /// <param name="_playableGraph"></param>
    /// <param name="delta"></param>
    public void EvaluateLayer(PlayableGraph _playableGraph, float delta)
    {
        if (!_isInitialized || _stateMachine == null)
        {
            return;
        }
        if (_playableGraph.IsValid())
        {
            //blend weight between states
            if (_mainMixer.IsValid())
            {
                int inputs = _mainMixer.GetInputCount();
                for (int i = 0; i < inputs; i++)
                {
                    float nodeWeight = _mainMixer.GetInputWeight(i);
                    _mainMixer.SetInputWeight(i, Mathf.Lerp(nodeWeight, i == _currentStateIndex ? 1 : 0, delta * (1 / _stateMachine.Transition)));
                    if (i == _currentStateIndex)
                        InTransition = nodeWeight < (1 - delta * 2);
                }
            }

            if (_currentSubMixer.IsValid())
            {
                //Blend weight and speed between child animations in current state
                int inputs = _currentSubMixer.GetInputCount();
                for (int i = 0; i < inputs; i++)
                {
                    float inputWeight = _blendWeight.SpreadEvenly(inputs, i);
                    _currentSubMixer.SetInputWeight(i, inputWeight);
                    if ((i - 1).InInterval(0, inputs))
                    {
                        Vector2 n_speed = Vector2.one;
                        float lastDuration = (float)((AnimationClipPlayable)_currentSubMixer.GetInput(i - 1)).GetAnimationClip().length;
                        float duration = (float)((AnimationClipPlayable)_currentSubMixer.GetInput(i)).GetAnimationClip().length;
                        float lastWeight = _currentSubMixer.GetInputWeight(i - 1);
                        bool reverse = inputWeight < lastWeight;
                        n_speed = NormalizedTransitionSpeed(lastDuration
                           , duration
                           , lastWeight
                           , inputWeight
                           , _stateMachine.Speed);
                        _currentSubMixer.GetInput(i - 1).SetSpeed(n_speed.x);
                        _currentSubMixer.GetInput(i).SetSpeed(n_speed.y);
                    }
                }
            }

            //Handle current maotion job
            //if (_animationJobMixer.IsInRange(_currentStateIndex))
            //{
            //    var job = _animationJobMixer[_currentStateIndex].GetJobData<AnimationJob>();
            //    job.weight = _mainMixer.GetInputWeight(_currentStateIndex);
            //    _animationJobMixer[_currentStateIndex].SetJobData<AnimationJob>(job);
            //}

            //return to default state if the last finite animation ended.
            var currentAnim = CurrentPlayedAnimation;
            if (currentAnim.IsValid())
            {
                if ((!currentAnim.GetAnimationClip().isLooping && currentAnim.GetTime() >= currentAnim.GetAnimationClip().length - _stateMachine.Transition)
                    || (currentAnim.GetAnimationClip().isLooping && _blendWeight <= 0))
                {
                    var c_motion = _currentMotion;
                    int c_motion_priority = 0;
                    if (_currentMotion)
                    {
                        c_motion_priority = _currentMotion.Priority;
                        _currentMotion.Priority = -1;
                    }
                    ReturnToDefaultState();
                    if (c_motion != null)
                        c_motion.Priority = c_motion_priority;
                }
            }

            //Loop time
            LoopTimeOnAllAnimations(delta);
        }
    }

    /// <summary>
    /// Dispose of the layer on destruction
    /// </summary>
    public void Dispose()
    {
        _isInitialized = false;
        _stateMachine = null;
    }

    #endregion
}

