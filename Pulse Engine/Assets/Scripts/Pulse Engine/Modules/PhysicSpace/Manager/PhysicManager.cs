using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Core;



namespace PulseEngine.Module.PhysicSpace
{
    /// <summary>
    /// Le Manager du monde physique.
    /// </summary>
    public static class PhysicManager
    {
        #region Enums ####################################################################

        /// <summary>
        /// Les espaces physique dans lequel peux se trouver un objet.
        /// </summary>
        public enum PhysicSpace
        {
            Void,
            Grounded,
            InAir,
            AttachedTo,
            Semi_Submerged,
            Submerged,
            InverseGravity
        }

        /// <summary>
        /// le type de materiau d'un objet, utile pour emettre des sons/particules a la collision ou friction d'objets.
        /// </summary>
        public enum PhysicMaterials
        {
            none,
            flesh,
            bone,
            wood,
            iron,
            steel,
            concrete,
            sand,
            ground,
            ice,
            glass,
            plastic,
            gravas,
        }

        #endregion
        #region Structures ####################################################################

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
