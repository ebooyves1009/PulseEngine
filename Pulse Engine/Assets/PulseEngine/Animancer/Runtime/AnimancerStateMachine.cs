using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Animations;
using UnityEngine.InputSystem;
using UnityEngine.Playables;

public class AnimancerStateMachine : MonoBehaviour
{
    #region Constants #############################################################

    #endregion

    #region Variables #############################################################

    [SerializeField] private Animator _animator;
    [SerializeField] private Character _character;

    [SerializeField] private AnimaMotion _jump;
    [SerializeField] private AnimaMotion _dodge;
    [SerializeField] private AnimaMotion _run;

    [SerializeField] private List<AnimancerMachineLayer> _layers;

    [SerializeField] private float _transition = 0.1f;
    [SerializeField] private float _speed = 1;
    [SerializeField] private int _currentLayerIndex;

    private PlayableGraph _playableGraph;
    private AnimationLayerMixerPlayable _mainMixer;
    private AnimationPlayableOutput _outPut;


    #endregion

    #region Statics   #############################################################

    #endregion

    #region Inner Types ###########################################################

    #endregion

    #region Properties ############################################################

    public float Transition { get => _transition * (1 / Speed); }
    public bool InLayerTransition { get; private set; }
    public float Speed { get => _speed; }
    public int CurrentLayerIndex { get => _currentLayerIndex; }

    #endregion

    #region Public Functions ######################################################

    /// <summary>
    /// Play a motion on a layer
    /// </summary>
    /// <param name="layerIndex"></param>
    /// <param name="motion"></param>
    /// <param name="priority"></param>
    /// <param name="updateAction"></param>
    /// <returns></returns>
    public bool PlayState(int layerIndex, AnimaMotion motion, Action<float> updateAction = null)
    {
        if (_layers != null && _layers.IsInRange(layerIndex))
        {
            if (_layers[layerIndex] != null)
            {
                return _layers[layerIndex].PlayState(_playableGraph, motion, updateAction);
            }
        }
        return false;
    }

    #endregion

    #region Private Functions #####################################################

    private void EvaluateLayers(float delta)
    {
        if (_playableGraph.IsValid())
        {
            _playableGraph.Evaluate(delta);
            for (int i = _layers.Count - 1; i >= 0; i--)
            {
                //Calcul of the layers weights
                _mainMixer.SetInputWeight(i, Mathf.Lerp(_mainMixer.GetInputWeight(i), i == _currentLayerIndex ? 1 : 0, delta * (1 / Transition)));
                if (i == _currentLayerIndex)
                    InLayerTransition = _mainMixer.GetInputWeight(i) < (1 - delta);

                //evaluate layers
                if (_layers[i] == null)
                    continue;
                _layers[i].EvaluateLayer(_playableGraph, delta);
                if (i == _currentLayerIndex)
                {
                    //execute current motion update action
                    if (_layers[i].CurrentMotion)
                        _layers[i].CurrentMotion.UpdateAction?.Invoke(delta);
                }
            }
        }
    }


    private void Jump() { if (CurrentLayerIndex == 0 && !InLayerTransition) { PlayState(0, _jump); }; }
    private void Dodge() { if (CurrentLayerIndex == 0 && !InLayerTransition) { PlayState(0, _dodge); }; }

    private void LinkActions()
    {
        if (_character)
        {
            _character.JumpAction?.AddListener(Jump);
            _character.InteractAction?.AddListener(Dodge);
        }
    }

    private void UnlinkActions()
    {
        if (_character)
        {
            _character.JumpAction?.RemoveListener(Jump);
            _character.InteractAction?.RemoveListener(Dodge);
        }
    }

    #endregion

    #region Jobs      #############################################################

    #endregion

    #region MonoBehaviours ########################################################


    private void OnEnable()
    {
        //animator
        _animator.SetBoneLocalRotation(HumanBodyBones.LeftUpperArm, Quaternion.identity);

        //Graph
        _playableGraph = PlayableGraph.Create("Animancer Playable");
        _playableGraph.SetTimeUpdateMode(DirectorUpdateMode.Manual);

        int layerCount = (int)_layers?.Count;

        //Mixers
        _mainMixer = AnimationLayerMixerPlayable.Create(_playableGraph, layerCount);

        //Output
        _outPut = AnimationPlayableOutput.Create(_playableGraph, "Anim OutPut", _animator);

        //Connection
        {
            //Layers to main mixers
            for (int i = 0; i < layerCount; i++)
            {
                if (_layers[i] == null)
                    continue;
                _playableGraph.Connect(_layers[i].InitLayer(_playableGraph, this), 0, _mainMixer, i);
            }

            //Main mixers to output
            _outPut.SetSourcePlayable(_mainMixer);
        }

        //Weight setting
        if (_mainMixer.GetInputCount() > 0)
            _mainMixer.SetInputWeight(0, 1);

        //Launch
        _playableGraph.Play();

        LinkActions();
    }

    private void FixedUpdate()
    {
        float delta = Time.fixedDeltaTime;
        EvaluateLayers(delta);
    }

    private void Update()
    {
        if (_character)
        {
            _currentLayerIndex = _character.CurrentPhysicSpace == PhysicSpace.onGround? 0 : 1;
            float stickVal = _character.DesiredDirection.magnitude;
            if (_layers.IsInRange(0) && _layers[0] != null)
            {
                if (_layers[0].CurrentMotion)
                {
                    if (_layers[0].CurrentMotion.Priority <= 0)
                        _layers[0].BlendWeight = stickVal;
                }
            }
            if (stickVal > 0)
            {
                PlayState(0, _run, (delta) =>
                {
                    if (_character)
                    {
                        _character.RotateToward(transform.position + _character.DesiredDirection, _character.TurnSpeed * delta);
                    }
                });
            }
        }
    }

    private void OnDisable()
    {
        UnlinkActions();
        for (int i = _layers.Count - 1; i >= 0; i--)
        {
            //dispose layers
            if (_layers[i] == null)
                continue;
            _layers[i].Dispose();
        }
        if (_playableGraph.IsValid())
            _playableGraph.Destroy();
    }

    #endregion
}

