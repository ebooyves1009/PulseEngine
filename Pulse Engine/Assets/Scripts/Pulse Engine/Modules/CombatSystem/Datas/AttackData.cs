using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Module.Anima;




namespace PulseEngine.Module.CombatSystem
{
    /// <summary>
    /// La data d'une attaque.
    /// </summary>
    [System.Serializable]
    public class AttackData : AnimaData
    {
        #region Attributs #########################################################

        [SerializeField]
        private List<CombatSystemManager.AttackEvent> attackEvents;
        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// La liste des hits produits par cette attaque en fonction du temps.
        /// </summary>
        public List<CombatSystemManager.AttackEvent> AttackEvents { get { return attackEvents; } set { attackEvents = value; } }

        #endregion

        #region Methods #########################################################

        #endregion
    }
}
