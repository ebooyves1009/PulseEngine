using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

namespace PulseEngine.Modules.Commander
{
    /// <summary>
    /// Le manager de commandes.
    /// </summary>
    public static class CommanderManager
    {
        #region Attributes ####################################################################

        /// <summary>
        /// Le chemin d'access des datas.
        /// </summary>
        public static string AssetsPath { get => "CommanderDatas"; }

        #endregion

        #region Methods ####################################################################

        /// <summary>
        /// Execute une commande action.
        /// </summary>
        /// <param name="_actionCmd"></param>
        public static void ExecuteCommand(GameObject emitter, CommandAction _actionCmd)
        {
            PulseDebug.Log("CommandAction " + _actionCmd.code + ", triggered by " + emitter.name);
        }

        /// <summary>
        /// Execute une commande evenement.
        /// </summary>
        /// <param name="_eventCmd"></param>
        public static void ExecuteCommand(GameObject emitter, CommandEvent _eventCmd)
        {
            PulseDebug.Log("CommandEvent " + _eventCmd.code + ", triggered by " + emitter.name);
        }

        /// <summary>
        /// Execute une commande globale.
        /// </summary>
        /// <param name="_worldCmd"></param>
        public static void ExecuteCommand(GameObject emitter, CommandWorld _worldCmd)
        {
            PulseDebug.Log("CommandWorld " + _worldCmd.code + ", triggered by " + emitter.name);
        }

        #endregion

        #region Extension&Helpers ####################################################################

        #endregion
    }
}