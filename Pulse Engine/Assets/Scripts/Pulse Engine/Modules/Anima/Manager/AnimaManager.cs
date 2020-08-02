using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Core;


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

        #endregion
        #region Structures ####################################################################

        /// <summary>
        /// La commande temporelle d'une animation.
        /// </summary>
        public struct AnimeCommand
        {
            /// <summary>
            /// La commande executee.
            /// </summary>
            //public CommandAction command;

            PulseCore_GlobalValue_Manager.TimeStamp timeStamp;
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