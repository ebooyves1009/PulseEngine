using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Core;
using PulseEngine.Module.Commands;


namespace PulseEngine.Module.Anima
{
    public static class AnimaManager
    {
        #region Enums ####################################################################

        /// <summary>
        /// La phase d'une animation.
        /// </summary>
        public enum AnimPhase
        {
            Repos,
            Preparing,
            Processing,
            PostProcessing,
            recovering
        }

        /// <summary>
        /// Le layer sur lequel est place une animation dans l'animator.
        /// </summary>
        public enum AnimationLayer
        {
            IdleLayer,
            LocamotionLayer,
            InterractionLayer,
            OffensiveLayer,
            DefensiveLayer,
            DamageLayer
        }

        #endregion
        #region Structures ####################################################################

        /// <summary>
        /// La commande temporelle d'une animation.
        /// </summary>
        public struct AnimeCommand
        {
            public CommanderManager.CommandAction command;
            public PulseCore_GlobalValue_Manager.TimeStamp timeStamp;
        }

        #endregion
        #region Nested Classes ####################################################################

        #endregion
        #region Attributes ####################################################################

        #endregion
        #region Methods ####################################################################

        #endregion
        #region Extension&Helpers ####################################################################

        #endregion
    }
}