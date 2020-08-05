using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Core;
using PulseEngine.Module.Anima;



namespace PulseEngine.Module.CombatSystem
{
    /// <summary>
    /// LA data d'une action defensive.
    /// </summary>
    [System.Serializable]
    public class DefenseData : AnimaData
    {
        #region Attributs #########################################################

        [SerializeField]
        private int defenseType;

        [SerializeField]
        private int defenseHeight;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// Le type de defense.
        /// </summary>
        public CombatSystemManager.DefenseType DefenseType { get { return (CombatSystemManager.DefenseType)defenseType; } set { defenseType = (int)value; } }

        /// <summary>
        /// La hauteur de la garde.
        /// </summary>
        public CombatSystemManager.AttackHeight DefenseHeight { get { return (CombatSystemManager.AttackHeight)defenseHeight; } set { defenseHeight = (int)value; } }

        #endregion

        #region Methods #########################################################

        #endregion
    }
}