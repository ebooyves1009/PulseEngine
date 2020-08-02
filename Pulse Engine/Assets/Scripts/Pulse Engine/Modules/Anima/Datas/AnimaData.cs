using System.Collections;
using System.Collections.Generic;
using UnityEngine;



namespace PulseEngine.Module.Anima
{

    /// <summary>
    /// La data d'animation.
    /// </summary>
    [System.Serializable]
    public class AnimaData
    {
        #region Attributs #########################################################

        [SerializeField]
        private int animID;

        [SerializeField]
        private bool isHumanMotion;

        [SerializeField]
        private List<AnimaManager.AnimeCommand> eventList;

        [SerializeField]
        private int phaseAnim;

        [SerializeField]
        private int physicPlace;

        [SerializeField]
        private AnimationClip animClip;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// L'id de l'animation dans la BD des anima data.
        /// </summary>
        public int AnimID { get { return animID; } set { animID = value; } }

        /// <summary>
        /// Est ce una animation.
        /// </summary>
        public bool IsHumanMotion { get { return isHumanMotion; } set { isHumanMotion = value; } }

        /// <summary>
        /// La liste des evenements au cours de l'animation.
        /// </summary>
        public List<AnimaManager.AnimeCommand> EventList { get { return eventList; } set { eventList = value; } }

        /// <summary>
        /// La phase d'animation en cours.
        /// </summary>
        public AnimaManager.AnimPhase PhaseAnim { get { return (AnimaManager.AnimPhase)phaseAnim; } set { phaseAnim = (int)value; } }

        /// <summary>
        /// Le lieux physique, terre, aux air ... ou il est possible d'effectuer l'action.
        /// </summary>
        //public int physicPlace;

        /// <summary>
        /// L'animation lue.
        /// </summary>
        private AnimationClip AnimClip { get { return animClip; } set { animClip = value; } }

        #endregion

        #region Methods #########################################################

        #endregion
    }

}
