using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using PulseEngine.Globals;
using PulseEngine.Modules.Commander;


//TODO: Les Valeurs et fonctions globales seront ajoutees au fur et a mesure.
namespace PulseEngine
{
    namespace Globals
    {
        #region Enums ##################################################################################

        /// <summary>
        /// Les langues traductibles du jeu.
        /// </summary>
        public enum Languages
        {
            Francais, English
        }

        /// <summary>
        /// Les types de data dans l'environnement.
        /// </summary>
        public enum DataTypes
        {
            None,
            tradData,
            CharacterData,
            WeaponData,
            AnimaData,
        }

        /// <summary>
        /// Les scope designer.
        /// </summary>
        public enum Scopes
        {
            tous,
        }


        #endregion

        #region Structures ###############################################################################

        /// <summary>
        /// Tampon temporel, pouvant garder utile dans les logs et animations. 
        /// </summary>
        [System.Serializable]
        public struct TimeStamp : IEquatable<TimeStamp>
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

            public bool Equals(TimeStamp other)
            {
                return time == other.time && duration == other.duration;
            }
        }

        #endregion

        #region Interfaces ############################################################################

        /// <summary>
        /// L'interface de toutes les datas des modules.
        /// </summary>
        interface IModuleData
        {
            int ID { get; set; }
        }

        /// <summary>
        /// L'interface de toutes les Assets des modules.
        /// </summary>
        interface IModuleAsset
        {
            PulseEngine.Globals.DataTypes DataType { get; }
            PulseEngine.Globals.Scopes Scope { get; set; }
        }

        /// <summary>
        /// L'interface de tous les Composants sur scene des modules.
        /// </summary>
        interface IModuleComponent
        {

        }

        #endregion

        #region Class #################################################################################

        /// <summary>
        /// Le Manager Global du Pulse Engine.
        /// </summary>
        public static class PulseEngineMgr
        {
            #region Attributs ###############################################################

            /// <summary>
            /// Le booleen pour activer/ desactiver le mode debug en runtime.
            /// </summary>
            public const bool DEBUG_MODE_Runtime = true; //HACK: Enable or disable runtime debug mode.

            /// <summary>
            /// Le repertoire ou seront stockes et d'ou seront charges les Assets du jeu in game et dans l'editeur. il sera Addressable.
            /// </summary>
            public const string Path_GAMERESSOURCES = "Assets/GameResources";

            /// <summary>
            /// La langue actuellement selectionnee dans le jeu.
            /// </summary>
            public static Languages currentLanguage;

            /// <summary>
            /// Le scope actuellement selectionnee dans L'editeur.
            /// </summary>
            public static Scopes currentScope;

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
        }

        #endregion
    }

    namespace Modules
    {

        #region Interfaces >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        #region Localisator Interfaces ###########################################################################

        /// <summary>
        /// L'interface de toute donne traductible.
        /// </summary>
        interface ITraductible : IModuleData
        {
            int IdTrad { get; set; }
            TradDataTypes TradType { get; set; }
            Task<string> GetTradText(DatalocationField field);
            Task<AudioClip> GetTradVoice(DatalocationField field);
            Task<Sprite> GetTradSprite(DatalocationField field);
        }

        #endregion

        #region PhysicSpace Interfaces ###########################################################################

        #endregion

        #region StatHandler Interfaces ###########################################################################

        #endregion

        #region Commander Interfaces ###########################################################################

        /// <summary>
        /// L'interface implementee par tout object pouvant executer des commandes.
        /// </summary>
        public interface ICommandable
        {
            void ExecuteCommand();
        }

        #endregion

        #region Anima Interfaces ###########################################################################

        #endregion

        #region CombatSystem Interfaces ###########################################################################

        #endregion

        #endregion

        #region Enums >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        #region CharacterCreator Enums ####################################################################

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

        #region Localisator Enums ################################################################################

        /// <summary>
        /// l'enumeration des champs d'un donnee de localisation.
        /// </summary>
        public enum DatalocationField
        {
            title,
            header,
            banner,
            groupName,
            toolTip,
            description,
            details,
            infos,
            child1,
            child2,
            child3,
            child4,
            child5,
            child6,
            footPage,
            conclusion,
            end,
        }

        /// <summary>
        /// l'enumeration des types traductibles.
        /// </summary>
        public enum TradDataTypes
        {
            Person,
            Animal,
            InterrractibleObject,
            StaticObject,
            Document,
            Dialog,
            Notification,
            Message,
            HUD,
            Place,
            Prop,
            Gadget,
            Weapon,
            Item,
            Cinematic,
            Skill,
            Behaviour,
        }

        #endregion

        #region PhysicSpace Enums ################################################################################

        /// <summary>
        /// Les espaces physique dans lequel peux se trouver un objet.
        /// </summary>
        public enum PhysicSpaces
        {
            Void,
            Grounded,
            InAir,
            AttachedTo,
            Semi_Submerged,
            Submerged,
            InverseGravity
        }

        /// <summary>
        /// le type de materiau d'un objet, utile pour emettre des sons/particules a la collision ou friction d'objets.
        /// </summary>
        public enum PhysicMaterials
        {
            none,
            flesh,
            bone,
            wood,
            iron,
            steel,
            concrete,
            sand,
            ground,
            ice,
            glass,
            plastic,
            gravas,
        }

        #endregion

        #region StatHandler Enums ################################################################################

        /// <summary>
        /// Le type des stats.
        /// </summary>
        public enum StatType
        {
            Sante,
            Intelligence,
            Sagesse,
            Force,
            Endurance,
            Souffle,
            Dexterite,
            Masse,
            Taille,
            Age,
            Karma,
            Paranormal,
            Fierte,
            Engoument
        }

        #endregion

        #region Commander Enums ################################################################################

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

        #region Anima Enums ################################################################################

        /// <summary>
        /// La phase d'une animation.
        /// </summary>
        public enum AnimaPhase
        {
            Repos,
            Preparing,
            Processing,
            PostProcessing,
            recovering
        }

        /// <summary>
        /// Le layer sur lequel est place une animation dans l'animator.
        /// </summary>
        public enum AnimaLayer
        {
            IdleLayer,
            LocamotionLayer,
            InterractionLayer,
            OffensiveLayer,
            DefensiveLayer,
            DamageLayer
        }

        /// <summary>
        /// Le type d'animation.
        /// </summary>
        public enum AnimaType
        {
            Idle,
            Locamotion,
            Interraction,
            Offensive,
            Defensive,
            Damage
        }

        /// <summary>
        /// La categorie d'une animation.
        /// </summary>
        public enum AnimaCategory
        {
            humanoid,
            quadruped,
            generic
        }

        #endregion

        #region CombatSystem Enums ####################################################################

        /// <summary>
        /// Le type de degat qu'une arme inflige
        /// </summary>
        public enum TypeDegatArme
        {
            Impact,
            Cut,
            Piercing,
        }

        /// <summary>
        /// Le type d'une attaque, l'effet qu'elle provoque a l'adversaire dans la meilleurs conditions.
        /// </summary>
        public enum AttackType
        {
            Simple,
            Push,
            Pull,
            Downward,
            Upward,
            SpinAway,
            Pierce
        }

        /// <summary>
        /// La hauteur d'une attaque.
        /// </summary>
        public enum AttackHeight
        {
            middle,
            above,
            below
        }

        /// <summary>
        /// Le type d'une defense, le zones couvertes.
        /// </summary>
        public enum DefenseType
        {
            overral,
            forward,
            backward,
            Downward,
            Upward,
            left,
            right
        }

        /// <summary>
        /// Le type d'une arme.
        /// </summary>
        public enum WeaponType
        {
            aucun,
            shortRange,
            LongRange
        }

        #endregion

        #endregion

        #region Structures >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        #region CharacterCreator Structures ####################################################################

        #endregion

        #region Localisator Structs ################################################################################

        /// <summary>
        /// La structure d'un champs truductible.
        /// </summary>
        [System.Serializable]
        public struct TradField
        {
            /// <summary>
            /// le champ textuel.
            /// </summary>
            public string s_textField;

            /// <summary>
            /// le champ image.
            /// </summary>
            public Sprite s_imageField;

            /// <summary>
            /// le champ vocal.
            /// </summary>
            public AudioClip s_audioField;
        }

        #endregion

        #region PhysicSpace Structs ################################################################################

        #endregion

        #region StatHandler Structs ################################################################################

        #endregion

        #region Commander Structs ################################################################################

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

        #region Anima Structs ################################################################################

        /// <summary>
        /// La commande temporelle d'une animation.
        /// </summary>
        [System.Serializable]
        public struct AnimeCommand : IEquatable<AnimeCommand>
        {
            public CommandAction command;
            public TimeStamp timeStamp;
            public bool isOneTimeAction;

            public bool Equals(AnimeCommand other)
            {
                return command.Equals(other.command) && timeStamp.Equals(other.timeStamp) && isOneTimeAction == other.isOneTimeAction;
            }
        }

        /// <summary>
        /// Les phases d'une animation.
        /// </summary>
        [System.Serializable]
        public struct AnimePhaseTimeStamp : IEquatable<AnimePhaseTimeStamp>
        {
            public AnimaPhase phase;
            public TimeStamp timeStamp;

            public bool Equals(AnimePhaseTimeStamp other)
            {
                return phase == other.phase && timeStamp.Equals(other.timeStamp);
            }
        }

        #endregion

        #region CombatSystem Structures ####################################################################

        /// <summary>
        /// L'emplacement d'une arme sur le corps.
        /// </summary>
        [System.Serializable]
        public struct WeaponPlace
        {
            [SerializeField]
            private int parentBone;
            public Vector3 PositionOffset;
            public Quaternion RotationOffset;

            public HumanBodyBones ParentBone { get { return (HumanBodyBones)parentBone; } set { parentBone = (int)value; } }
        }

        /// <summary>
        /// Le donnes d'evenement d'animation d'une attaque.
        /// </summary>
        [System.Serializable]
        public struct AttackEvent
        {
            private int type;
            private int height;

            public TimeStamp timeStamp;
            public AttackType Type { get { return (AttackType)type; } set { type = (int)value; } }
            public AttackHeight Height { get { return (AttackHeight)height; } set { height = (int)value; } }
        }

        #endregion

        #endregion

        #region Children NameSpaces >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        namespace Localisator
        {
            #region Module constants ################################################################################

            /// <summary>
            /// Les constantes dans le module.
            /// </summary>
            public static class ModuleConstants
            {
                /// <summary>
                /// Le chemin d'access des datas.
                /// </summary>
                public static string AssetsPath { get => "LocalisationDatas"; }
            }

            #endregion
        }

        namespace PhysicSpace
        {

            #region Module constants ################################################################################

            /// <summary>
            /// Les constantes dans le module.
            /// </summary>
            public static class ModuleConstants
            {
                /// <summary>
                /// Le chemin d'access des datas.
                /// </summary>
                public static string AssetsPath { get => "PhysicDatas"; }
            }

            #endregion
        }

        namespace StatHandler
        {
            #region Module constants ################################################################################

            /// <summary>
            /// Les constantes dans le module.
            /// </summary>
            public static class ModuleConstants
            {
                /// <summary>
                /// Le chemin d'access des datas.
                /// </summary>
                public static string AssetsPath { get => "StatDatas"; }
            }

            #endregion
        }

        namespace Commander
        {
            #region Module constants ################################################################################

            /// <summary>
            /// Les constantes dans le module.
            /// </summary>
            public static class ModuleConstants
            {
                /// <summary>
                /// Le chemin d'access des datas.
                /// </summary>
                public static string AssetsPath { get => "CommanderDatas"; }
            }

            #endregion
        }

        namespace Anima
        {
            #region Module constants ################################################################################

            /// <summary>
            /// Les constantes dans le module.
            /// </summary>
            public static class ModuleConstants
            {
                /// <summary>
                /// Le chemin d'access des datas.
                /// </summary>
                public static string AssetsPath { get => "AnimaDatas"; }
            }

            #endregion
        }

        namespace CharacterCreator
        {
            #region Module constants ################################################################################

            /// <summary>
            /// Les constantes dans le module.
            /// </summary>
            public static class ModuleConstants
            {
                /// <summary>
                /// Le chemin d'access des datas.
                /// </summary>
                public static string AssetsPath { get => "CharactersDatas"; }
            }

            #endregion
        }

        namespace CombatSystem
        {
            #region Module constants ################################################################################

            /// <summary>
            /// Les constantes dans le module.
            /// </summary>
            public static class ModuleConstants
            {
                /// <summary>
                /// Le chemin d'access des datas.
                /// </summary>
                public static string AssetsPath { get => "CombatDatas"; }
            }

            #endregion
        }

        #endregion
    }
}