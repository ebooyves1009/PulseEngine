using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PulseEngine.Datas;
using System.Threading;
using System.Threading.Tasks;

namespace PulseEngine.Modules.Commander
{
    /// <summary>
    /// Le manager de commandes.
    /// </summary>
    public static class Commander
    {
        #region Attributes ####################################################################

        /// <summary>
        /// The current sequence executed.
        /// </summary>
        private static CommandSequence currentSequence;

        /// <summary>
        /// The current command executed.
        /// </summary>
        private  static Command currentCmd = Command.NullCmd;

        /// <summary>
        /// The list of Unfinnished commands.
        /// </summary>
        private static Dictionary<DataLocation, CommandPath> ResumablesSequences = new Dictionary<DataLocation, CommandPath>();

        private static CancellationTokenSource taskCancellationSource;

        #endregion

        #region Methods ####################################################################

        [RuntimeInitializeOnLoadMethod]
        public static void OnDomainReload()
        {
            currentCmd = Command.NullCmd;
            currentSequence = null;
            ResumablesSequences = new Dictionary<DataLocation, CommandPath>();
        }

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