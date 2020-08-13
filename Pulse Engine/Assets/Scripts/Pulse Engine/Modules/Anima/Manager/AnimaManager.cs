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

        /// <summary>
        /// Le type d'animation.
        /// </summary>
        public enum AnimationType
        {
            Idle,
            Locamotion,
            Interraction,
            Offensive,
            Defensive,
            Damage
        }

        /// <summary>
        /// La categorie d'une animation.
        /// </summary>
        public enum AnimaCategory
        {
            humanoid,
            quadruped,
            generic
        }

        #endregion
        #region Structures ####################################################################

        #endregion
        #region Nested Classes ####################################################################

        /// <summary>
        /// La commande temporelle d'une animation.
        /// </summary>
        [System.Serializable]
        public class AnimeCommand
        {
            public CommanderManager.CommandAction command;
            public PulseCore_GlobalValue_Manager.TimeStamp timeStamp;
            public bool isOneTimeAction;
        }

        /// <summary>
        /// La commande temporelle d'une animation.
        /// </summary>
        [System.Serializable]
        public struct AnimePhaseTimeStamp
        {
            public AnimPhase phase;
            public PulseCore_GlobalValue_Manager.TimeStamp timeStamp;
        }

        #endregion
        #region Attributes ####################################################################

        /// <summary>
        /// Le chemin d'acces local des datas
        /// </summary>
        public static string AssetsPath { get => "AnimaAsset"; }

        #endregion
        #region Methods ####################################################################

        #endregion
        #region Extension&Helpers ####################################################################

        #endregion
    }
}