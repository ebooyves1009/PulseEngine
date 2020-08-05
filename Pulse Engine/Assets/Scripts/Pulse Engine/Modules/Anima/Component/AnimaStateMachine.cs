using System.Collections;
using System.Collections.Generic;
using UnityEngine;




namespace PulseEngine.Module.Anima
{

    /// <summary>
    /// La state machine dans l'animator.
    /// </summary>
    public class AnimaStateMachine : StateMachineBehaviour
    {
        #region Attributs #########################################################

        public AnimationClip clip;
        public string currentClipName;
        private AnimatorOverrideController controller;

        #endregion

        #region Propriete #########################################################

        #endregion

        #region Methods #########################################################

        public override void OnStateEnter(Animator animator, AnimatorStateInfo animatorStateInfo, int layerIndex)
        {
            if (clip)
            {
                controller = new AnimatorOverrideController(animator.runtimeAnimatorController);
                if (clip != controller[currentClipName])
                {
                    animator.runtimeAnimatorController = controller;
                    controller[currentClipName] = clip;
                    Debug.Log("Override");
                }
            }
        }


        #endregion
    }

}