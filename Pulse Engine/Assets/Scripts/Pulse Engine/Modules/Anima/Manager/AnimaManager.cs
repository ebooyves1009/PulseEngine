using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Globals;


namespace PulseEngine.Modules.Anima
{
    /// <summary>
    /// Le manager du module Anima
    /// </summary>
    public static class AnimaManager
    {
        #region Attributes ####################################################################

        #endregion
        #region Methods ####################################################################

        /// <summary>
        /// Get the animator layer corresponding to AnimType.
        /// </summary>
        /// <param name="_type"></param>
        /// <returns></returns>
        public static AnimaLayer LayerFromType(AnimaType _type)
        {
            switch (_type)
            {
                case AnimaType.Idle:
                    return AnimaLayer.IdleLayer;
                case AnimaType.Locamotion:
                    return AnimaLayer.LocamotionLayer;
                case AnimaType.Interraction:
                    return AnimaLayer.InterractionLayer;
                case AnimaType.Offensive:
                    return AnimaLayer.OffensiveLayer;
                case AnimaType.Defensive:
                    return AnimaLayer.DefensiveLayer;
                case AnimaType.Damage:
                    return AnimaLayer.DamageLayer;
                default:
                    return AnimaLayer.IdleLayer;
            }
        }

        #endregion
        #region Extension&Helpers ####################################################################

        #endregion
    }
}