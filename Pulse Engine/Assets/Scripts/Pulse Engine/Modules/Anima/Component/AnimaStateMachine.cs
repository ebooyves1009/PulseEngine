using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Datas;




namespace PulseEngine.Modules.Components
{

    /// <summary>
    /// La state machine dans l'animator.
    /// </summary>
    public class AnimaStateMachine : StateMachineBehaviour
    {
        #region Attributs #########################################################

        private string m_currentClipName;
        private AnimatorOverrideController m_controller;
        private AnimaData m_animationData;
        private List<AnimeCommand> m_oneTimeCommands = new List<AnimeCommand>();

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// La data d'animation liee a cette state machine.
        /// </summary>
        public AnimaData AnimationData { get => m_animationData; set => m_animationData = value; }

        /// <summary>
        /// Le noms du state.
        /// </summary>
        public string StateName { set => m_currentClipName = value; }

        #endregion

        #region Methods #########################################################

        /// <summary>
        /// Override the state motion.
        /// </summary>
        /// <param name="_clip"></param>
        public void OverrideAnimation(AnimationClip _clip, Animator _animator)
        {
            if (_clip && !string.IsNullOrEmpty(m_currentClipName))
            {
                if(m_controller == null)
                    m_controller = new AnimatorOverrideController(_animator.runtimeAnimatorController);
                _animator.runtimeAnimatorController = m_controller;
                m_controller[m_currentClipName] = _clip;
                if (PulseEngine.Core.DebugMode)
                    Debug.Log("Overriding state " + m_currentClipName + " of " + _animator.name + " with " + _clip.name);
            }
        }

        /// <summary>
        /// Check Anima event during animation process.
        /// </summary>
        /// <param name="_data"></param>
        /// <param name="_normalizedTime"></param>
        public void CheckEvent(Animator _animator, AnimaData _data, float _normalizedTime)
        {
            if (_data == null)
                return;
            //get the time cursor
            float timeCursor = _data.Motion.length * _normalizedTime;
            //find the corresponding event.
            for(int i = 0,len = _data.EventList.Count; i < len; i++)
            {
                var Event = _data.EventList[i];
                if (m_oneTimeCommands.Contains(Event))
                    continue;
                if(Event.timeStamp.time <= timeCursor && (Event.timeStamp.time + Event.timeStamp.duration) > timeCursor)
                {
                    TriggerEvent(_animator.gameObject, Event);
                    if (Event.isOneTimeAction)
                        m_oneTimeCommands.Add(Event);
                }
            }
        }

        /// <summary>
        /// trigger an event in Anima data.
        /// </summary>
        public void TriggerEvent(GameObject _eventEmitter, AnimeCommand _event)
        {
            var executeTask = Core.ManagerAsyncMethod(ModulesManagers.Commander, "ExecuteCommand", _eventEmitter, _event.command);
        }

        /// <summary>
        /// clear all the one time events pool.
        /// </summary>
        private void ClearOneTimeCommands()
        {
            if (m_oneTimeCommands != null)
                m_oneTimeCommands.Clear();
        }

        #endregion
        
        #region Behaviours #########################################################

        /// <summary>
        /// Start
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="animatorStateInfo"></param>
        /// <param name="layerIndex"></param>
        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {

        }

        /// <summary>
        /// Update
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateInfo"></param>
        /// <param name="layerIndex"></param>
        public override void OnStateUpdate(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            int loopCount = (int)stateInfo.normalizedTime;
            float normalisedTime = Mathf.Abs(stateInfo.normalizedTime - loopCount);
            CheckEvent(animator, m_animationData, normalisedTime);
        }

        /// <summary>
        /// Ik Logic
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateInfo"></param>
        /// <param name="layerIndex"></param>
        public override void OnStateIK(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        /// <summary>
        /// On Apply root motion.
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateInfo"></param>
        /// <param name="layerIndex"></param>
        public override void OnStateMove(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {

        }

        /// <summary>
        /// Exit
        /// </summary>
        /// <param name="animator"></param>
        /// <param name="stateInfo"></param>
        /// <param name="layerIndex"></param>
        public override void OnStateExit(Animator animator, AnimatorStateInfo stateInfo, int layerIndex)
        {
            ClearOneTimeCommands();
        }


        #endregion
    }

}