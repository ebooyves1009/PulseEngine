using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Globals;
using System;

namespace PulseEngine.Modules.Commander
{
    /// <summary>
    /// Le manager de commandes.
    /// </summary>
    public static class CommanderManager
    {
        #region Attributes ####################################################################

        #endregion
        #region Methods ####################################################################

        /// <summary>
        /// Execute une commande action.
        /// </summary>
        /// <param name="_actionCmd"></param>
        public static void ExecuteCommand(GameObject emitter, CommandAction _actionCmd)
        {
            if (PulseEngineMgr.DEBUG_MODE_Runtime)
                Debug.Log("CommandAction " + _actionCmd.code + ", triggered by " + emitter.name);
        }

        /// <summary>
        /// Execute une commande evenement.
        /// </summary>
        /// <param name="_eventCmd"></param>
        public static void ExecuteCommand(GameObject emitter, CommandEvent _eventCmd)
        {
            if (PulseEngineMgr.DEBUG_MODE_Runtime)
                Debug.Log("CommandEvent " + _eventCmd.code + ", triggered by " + emitter.name);
        }

        /// <summary>
        /// Execute une commande globale.
        /// </summary>
        /// <param name="_worldCmd"></param>
        public static void ExecuteCommand(GameObject emitter, CommandWorld _worldCmd)
        {
            if (PulseEngineMgr.DEBUG_MODE_Runtime)
                Debug.Log("CommandWorld " + _worldCmd.code + ", triggered by " + emitter.name);
        }

        #endregion

        #region Extension&Helpers ####################################################################

        #endregion
    }
}