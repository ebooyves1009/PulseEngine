using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Module.PhysicSpace;



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
        private int id;

        [SerializeField]
        private bool isHumanMotion;

        [SerializeField]
        private int animLayer;

        [SerializeField]
        private List<AnimaManager.AnimeCommand> eventList;

        [SerializeField]
        private int phaseAnim;

        [SerializeField]
        private int physicPlace;

        [SerializeField]
        private AnimationClip motion;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// L'id de l'animation dans la BD des anima data.
        /// </summary>
        public int ID { get { return id; } set { id = value; } }

        /// <summary>
        /// Est ce una animation.
        /// </summary>
        public bool IsHumanMotion { get { return isHumanMotion; } set { isHumanMotion = value; } }

        /// <summary>
        /// La calque d'animator sur lequel se trouve l'animation.
        /// </summary>
        public AnimaManager.AnimationLayer AnimLayer { get => (AnimaManager.AnimationLayer)animLayer; set => animLayer = (int)value; }

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
        public PhysicManager.PhysicSpace PhysicPlace { get { return (PhysicManager.PhysicSpace)physicPlace; } set { physicPlace = (int)value; } }

        /// <summary>
        /// L'animation lue.
        /// </summary>
        public AnimationClip Motion { get { return motion; } set { motion = value; } }

        #endregion

        #region Methods #########################################################

        #endregion
    }

}
