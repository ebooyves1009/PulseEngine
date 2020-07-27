using System.Collections;
using System.Collections.Generic;
using UnityEngine;


//TODO: Les Valeurs et fonctions globales seront ajoutees au fur et a mesure.
namespace PulseEngine.Core
{
    /// <summary>
    /// Le Manager Global du Pulse Engine.
    /// </summary>
    public static class PulseCore_GlobalValue_Manager
    {
        #region Attributs ###############################################################

        /// <summary>
        /// Le repertoire ou seront stockes et d'ou seront charges les Assets du jeu in game et dans l'editeur. il sera Addressable.
        /// </summary>
        public const string Path_GAMERESSOURCES = "Assets/GameResources";

        /// <summary>
        /// Le menu dans lequel seront crees les menus des editeurs.
        /// </summary>
        public const string Menu_EDITOR_MENU = "PulseEngine/Module/";


        #endregion

        #region Methods #################################################################

        #endregion

        #region Enumerations #############################################################

        /// <summary>
        /// Les langues traductibles du jeu.
        /// </summary>
        public enum Languages {
            Francais, English
        }

        /// <summary>
        /// Les types de data dans l'environnement.
        /// </summary>
        public enum DataType {
            None, CharacterInfos
        }

        #endregion

        #region Structures ################################################################

        #endregion
    }
}