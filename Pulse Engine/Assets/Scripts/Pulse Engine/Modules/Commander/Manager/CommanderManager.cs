using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Core;
using System;

namespace PulseEngine.Module.Commands
{
    /// <summary>
    /// Le manager de commandes.
    /// </summary>
    public static class CommanderManager
    {
        #region Enums ####################################################################

        #endregion
        #region Structures ####################################################################

        #endregion
        #region Nested Classes ####################################################################

        /// <summary>
        /// Une commande action.
        /// </summary>
        public class CommandAction : IEquatable<CommandAction>
        {
            public int code;
            public Vector4 primaryParameters;
            public Vector4 secondaryParameters;

            public bool Equals(CommandAction other)
            {
                return code == other.code && primaryParameters == other.primaryParameters && secondaryParameters == other.secondaryParameters;
            }
        }

        /// <summary>
        /// Une commande qui declenche un evenement.
        /// </summary>
        public class CommandEvent : IEquatable<CommandEvent>
        {
            public int code;
            public Vector4 primaryParameters;
            public Vector4 secondaryParameters;

            public bool Equals(CommandEvent other)
            {
                return code == other.code && primaryParameters == other.primaryParameters && secondaryParameters == other.secondaryParameters;
            }
        }

        /// <summary>
        /// Une commande qui modifie des proprietes du monde.
        /// </summary>
        public class CommandWorld : IEquatable<CommandWorld>
        {
            public int code;
            public Vector4 primaryParameters;
            public Vector4 secondaryParameters;

            public bool Equals(CommandWorld other)
            {
                return code == other.code && primaryParameters == other.primaryParameters && secondaryParameters == other.secondaryParameters;
            }
        }

        #endregion
        #region Attributes ####################################################################

        #endregion
        #region Methods ####################################################################

        #endregion
        #region Extension&Helpers ####################################################################

        #endregion
    }


    /// <summary>
    /// L'interface implementee par tout object pouvant executer des commandes.
    /// </summary>
    public interface ICommandable
    {
        void ExecuteCommand();
    }
}