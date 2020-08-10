using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
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

        /// <summary>
        /// La langue actuellement selectionnee dans le jeu.
        /// </summary>
        public static Languages currentLanguage;


        #endregion

        #region Methods #################################################################

        /// <summary>
        /// Copie par valeur.
        /// </summary>
        /// <returns></returns>
        public static T DeepCopy<T>(T original)
        {

            if (!typeof(T).IsSerializable)
            {
                //throw new ArgumentException("The type must be serializable.", "source");
                return default(T);
            }

            // Don't serialize a null object, simply return the default for that object
            if (object.ReferenceEquals(original, null))
            {
                return default(T);
            }

            string sourceJson = JsonUtility.ToJson(original);

            T nouvo = JsonUtility.FromJson<T>(sourceJson);

            return nouvo;
        }

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
            None,
            CharacterInfos,
            Weapon,
            Animations,
        }

        /// <summary>
        /// Les scope designer.
        /// </summary>
        public enum Scopes
        {
            tous,
        }

        #endregion

        #region Structures ################################################################

        /// <summary>
        /// Tampon temporel, pouvant garder utile dans les logs et animations. 
        /// </summary>
        public struct TimeStamp
        {
            /// <summary>
            /// Le temps.
            /// </summary>
           public DateTime fullTime;
            
           /// <summary>
           /// le temps.
           /// </summary>
           public float time;
            
            /// <summary>
            /// la duree.
            /// </summary>
           public float duration;
        }

        #endregion
    }
}