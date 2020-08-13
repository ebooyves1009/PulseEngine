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

        /// <summary>
        /// Le type d'une commande.
        /// </summary>
        public enum CommandType
        {
            none,
            SheatheWeapon,
            UnsheatheWeapon,
        }

        #endregion
        #region Structures ####################################################################

        /// <summary>
        /// Une commande action.
        /// </summary>
        [System.Serializable]
        public struct CommandAction : IEquatable<CommandAction>
        {
            public CommandType code;
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
        [System.Serializable]
        public struct CommandEvent : IEquatable<CommandEvent>
        {
            public CommandType code;
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
        [System.Serializable]
        public struct CommandWorld : IEquatable<CommandWorld>
        {
            public CommandType code;
            public Vector4 primaryParameters;
            public Vector4 secondaryParameters;

            public bool Equals(CommandWorld other)
            {
                return code == other.code && primaryParameters == other.primaryParameters && secondaryParameters == other.secondaryParameters;
            }
        }

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


    /// <summary>
    /// L'interface implementee par tout object pouvant executer des commandes.
    /// </summary>
    public interface ICommandable
    {
        void ExecuteCommand();
    }
}