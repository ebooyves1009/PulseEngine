using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Globals;
using PulseEngine.Modules;
using PulseEngine.Modules.Anima;



namespace PulseEngine.Modules.CombatSystem
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
        public DefenseType DefenseType { get { return (DefenseType)defenseType; } set { defenseType = (int)value; } }

        /// <summary>
        /// La hauteur de la garde.
        /// </summary>
        public AttackHeight DefenseHeight { get { return (AttackHeight)defenseHeight; } set { defenseHeight = (int)value; } }

        #endregion

        #region Methods #########################################################

        #endregion
    }
}