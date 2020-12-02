using System.Collections.Generic;
using UnityEngine;
using System.Threading;
using System.Threading.Tasks;
using System.Text;

namespace PulseEngine.Modules.Commander
{
    /// <summary>
    /// Le manager de commandes.
    /// </summary>
    public static class Commander
    {
        #region Attributes ####################################################################

        /// <summary>
        /// The list of Unfinnished commands.
        /// </summary>
        private static Dictionary<DataLocation, CommandPath> ResumablesSequences = new Dictionary<DataLocation, CommandPath>();

        #endregion

        #region Methods ####################################################################

        [RuntimeInitializeOnLoadMethod]
        public static void OnDomainReload()
        {
            ResumablesSequences = new Dictionary<DataLocation, CommandPath>();
        }

        /// <summary>
        /// Execute a sequence of commands.
        /// </summary>
        /// <param name="sequence"></param>
        /// <returns></returns>
        public static async Task PlayCommandSequence(CancellationToken ct, DataLocation sequenceLocation, bool avoidResume = false)
        {
            CommandSequence sequence = await CoreLibrary.GetData<CommandSequence>(sequenceLocation, ct);
            if (sequence == null || sequence.Sequence == null || sequence.Sequence.Count <= 0)
                return;

            //Pre execution stuffs.

            Command currentCommand = sequence.DefaultCommand;
            if(ResumablesSequences.ContainsKey(sequenceLocation) && !avoidResume)
            {
                int indx = sequence.Sequence.FindIndex(cmd => { return cmd.CmdPath == ResumablesSequences[sequenceLocation]; });
                currentCommand = indx >= 0 ? sequence.Sequence[indx] : sequence.DefaultCommand;
                ResumablesSequences.Remove(sequenceLocation);
            }
            CommandPath nextCmd = CommandPath.ExitPath;
            do
            {
                nextCmd = await ExecuteCommand(ct, null, currentCommand);
                if (nextCmd == CommandPath.NullPath)
                    break;
                if(nextCmd == CommandPath.BreakPath)
                {
                    if(!avoidResume)
                        ResumablesSequences.Add(sequenceLocation, currentCommand.CmdPath);
                    break;
                }
                if (nextCmd == CommandPath.EntryPath)
                    break;
                int index = sequence.Sequence.FindIndex(cmd => { return cmd.CmdPath == nextCmd; });
                if (index < 0)
                    break;
                currentCommand = sequence.Sequence[index];
            }
            while (!ct.IsCancellationRequested && nextCmd != CommandPath.ExitPath);

            //Post execution stuffs
        }

        //TODO: remove
        public static dynamic virtualEmitter;

        /// <summary>
        /// Execute une commande, et renvoi vrai si la commande a ete executee correctement et false pour tout autre cas.
        /// </summary>
        /// <param name="_actionCmd"></param>
        public static async Task<CommandPath> ExecuteCommand(CancellationToken ct, GameObject emitter, Command _Cmd)
        {
            CommandPath nextCommandPath = CommandPath.NullPath;
            await Task.Delay(10);
            return nextCommandPath;
        }

        /// <summary>
        /// Execute an Event.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static async Task<CommandPath> ExecuteEvent<T>(CancellationToken ct, T caster, Command cmd)
        {
            CommandPath nextCommand = cmd.Outputs[0];
            await Task.Delay(10);
            return nextCommand;
        }

        /// <summary>
        /// Execute an action.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static async Task<CommandPath> ExecuteAction<T>(CancellationToken ct, T caster, Command cmd)
        {
            CommandPath nextCommand = cmd.Outputs[0];
            await Task.Delay(10);
            return nextCommand;
        }

        /// <summary>
        /// Execute a globla event Event.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static async Task<CommandPath> ExecuteGlobal(CancellationToken ct, GameObject caster, Command cmd)
        {
            StringBuilder logtext = new StringBuilder();
            PulseDebug.Log(logtext.Append(caster?.name).Append(" Trigerred Global event "));
            logtext.Clear();
            await Task.Delay(2000);
            CommandPath nextCommand = cmd.Outputs[0];
            return nextCommand;
        }

        /// <summary>
        /// Execute a narratic Event.
        /// </summary>
        /// <param name="cmd"></param>
        /// <returns></returns>
        private static async Task<CommandPath> ExecuteStory(CancellationToken ct, GameObject caster, Command cmd)
        {
            StringBuilder logtext = new StringBuilder();
            PulseDebug.Log(logtext.Append(caster?.name).Append(" Trigerred Narrative "));
            logtext.Clear();
            await Task.Delay(100);
            CommandPath nextCommand = cmd.Outputs[0];
            return nextCommand;
        }

        #endregion

        #region Extension&Helpers ####################################################################

        #endregion
    }
}