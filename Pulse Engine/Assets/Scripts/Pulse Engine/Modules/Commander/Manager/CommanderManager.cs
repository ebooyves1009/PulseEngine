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
        /// Execute une commande.
        /// </summary>
        /// <param name="_actionCmd"></param>
        public static void ExecuteCommand(GameObject emitter, Command _Cmd)
        {
            Func<Command, dynamic> getCodeType = c =>
             {
                 switch (c.ChildType)
                 {
                     case CmdExecutableType._event:
                         return c.CodeEv;
                     case CmdExecutableType._action:
                         return c.CodeAc;
                     case CmdExecutableType._global:
                         return c.CodeGl;
                     case CmdExecutableType._story:
                         return c.CodeSt;
                     default:
                         return (int)c.CodeAc;
                 }
             };
            PulseDebug.Log("Command " + _Cmd.Type + (_Cmd.Type == CommandType.execute? (_Cmd.ChildType+" "+getCodeType(_Cmd)) : "") + ", triggered by " + emitter.name);
        }

        #endregion

        #region Extension&Helpers ####################################################################

        #endregion
    }
}