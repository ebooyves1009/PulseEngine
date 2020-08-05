using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Core;


namespace PulseEngine.Module.CharacterCreator
{
    /// <summary>
    /// Le manager du mondule de creation de charactere.
    /// </summary>
    public static class CharacterManager
    {
        #region Enums ####################################################################

        /// <summary>
        /// Le type de character.
        /// </summary>
        public enum CharacterType
        {
            InteractiveObject,
            PrincipalActors,
            SecondaryActor,
            Pnj
        }

        #endregion
        #region Structures ####################################################################

        #endregion
        #region Nested Classes ####################################################################

        #endregion
        #region Attributes ####################################################################

        /// <summary>
        /// L'emplacement de sauvegarde des datas de charcters.
        /// </summary>
        public static string AssetsPath { get => "Characters"; }

        #endregion
        #region Methods ####################################################################

        #endregion
        #region Extension&Helpers ####################################################################

        #endregion
    }
}