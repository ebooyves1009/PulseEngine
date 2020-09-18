﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using PulseEngine.Modules.Commander;


//TODO: Les Valeurs et fonctions globales seront ajoutees au fur et a mesure.

namespace PulseEngine
{
    #region Core >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// Les constantes et variables globales du pulse.
    /// </summary>
    public static class Core
    {

        #region Constants ###########################################################################

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

        /// <summary>
        /// switch debug on or off.
        /// </summary>
        public const bool DebugMode = true;

        #endregion

        #region HelperMethods ###########################################################################

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

    #region Enums <><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>


    #region Globals >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// Les langues traductibles du jeu.
    /// </summary>
    public enum Languages
    {
        Francais, English
    }

    /// <summary>
    /// Les scope designer.
    /// </summary>
    public enum Scopes
    {
        tous,
    }

    #endregion

    #region CharacterCreator Enums >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// Le type de character. son importance dans l'histoire du jeu.
    /// </summary>
    public enum CharacterType
    {
        InteractiveObject,
        PrincipalActors,
        SecondaryActor,
        Pnj
    }

    #endregion

    #region Localisator Enums >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

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

    #region PhysicSpace Enums >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

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

    #region Commander Enums >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

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

    #region Anima Enums >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// La phase d'une animation.
    /// </summary>
    public enum AnimaPhase
    {
        Preparing,
        Processing,
        PostProcessing,
        recovering
    }

    /// <summary>
    /// Le type d'animation.
    /// </summary>
    public enum AnimaType
    {
        none,
        Idle,
        Locamotion,
        Interraction,
        Offensive,
        Defensive,
        DamageTaken,
    }

    /// <summary>
    /// La categorie d'une animation.
    /// </summary>
    public enum AvatarType
    {
        humanoid,
        quadruped,
        generic
    }

    #endregion

    #region CombatSystem Enums >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

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

    #endregion


    #endregion

    #region Structures <><><><><><<><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><<><><><><><><><><><><><><><><><><><><><><><><><><>

    #region Globals >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

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

    #region CharacterCreator Structures >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    #endregion

    #region Localisator Structs >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// La structure d'un champs truductible.
    /// </summary>
    [System.Serializable]
    public struct TradField
    {
        /// <summary>
        /// le champ textuel.
        /// </summary>
        public string textField;

        /// <summary>
        /// le champ image.
        /// </summary>
        public Sprite imageField;

        /// <summary>
        /// le champ vocal.
        /// </summary>
        public AudioClip audioField;
    }

    #endregion

    #region PhysicSpace Structs >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    #endregion

    #region Commander Structs >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

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

    #region Anima Structs >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

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

    #region CombatSystem Structures >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// L'emplacement d'une arme sur le corps.
    /// </summary>
    [System.Serializable]
    public struct WeaponPlace
    {
        [SerializeField]
        private int parentBone;
        public Vector3 positionOffset;
        public Vector3 rotationOffset;

        public HumanBodyBones ParentBone { get { return (HumanBodyBones)parentBone; } set { parentBone = (int)value; } }
    }

    #endregion

    #endregion

    #region Interfaces <><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>

    #endregion
}

namespace PulseEngine.Datas
{
    #region Localisation >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// La Data de localisation contenu dans un asset de localisation, dans une langue precise.
    /// </summary>
    [System.Serializable]
    public class Localisationdata
    {
        #region Attributes ###############################################################

        [SerializeField]
        private int trad_ID;

        [SerializeField]
        private TradField title;

        [SerializeField]
        private TradField header;

        [SerializeField]
        private TradField banner;

        [SerializeField]
        private TradField groupName;

        [SerializeField]
        private TradField toolTip;

        [SerializeField]
        private TradField description;

        [SerializeField]
        private TradField details;

        [SerializeField]
        private TradField infos;

        [SerializeField]
        private TradField child1;

        [SerializeField]
        private TradField child2;

        [SerializeField]
        private TradField child3;

        [SerializeField]
        private TradField child4;

        [SerializeField]
        private TradField child5;

        [SerializeField]
        private TradField child6;

        [SerializeField]
        private TradField footPage;

        [SerializeField]
        private TradField conclusion;

        [SerializeField]
        private TradField end;

        #endregion

        #region Proprietes ##################################################################

        /// <summary>
        /// L'id de traduction.
        /// </summary>
        public int ID { get { return trad_ID; } set { trad_ID = value; } }

        /// <summary>
        /// Le titre.
        /// </summary>
        public TradField Title { get { return title; } set { title = value; } }

        /// <summary>
        /// L'entete.
        /// </summary>
        public TradField Header { get { return header; } set { header = value; } }

        /// <summary>
        /// La banniere.
        /// </summary>
        public TradField Banner { get { return banner; } set { banner = value; } }

        /// <summary>
        /// Le nom de groupe.
        /// </summary>
        public TradField GroupName { get { return groupName; } set { groupName = value; } }

        /// <summary>
        /// Le texte au survol.
        /// </summary>
        public TradField ToolTip { get { return toolTip; } set { toolTip = value; } }

        /// <summary>
        /// La description.
        /// </summary>
        public TradField Description { get { return description; } set { description = value; } }

        /// <summary>
        /// Les details.
        /// </summary>
        public TradField Details { get { return details; } set { details = value; } }

        /// <summary>
        /// Les details avances.
        /// </summary>
        public TradField Infos { get { return infos; } set { infos = value; } }

        /// <summary>
        /// Le sous texte 1
        /// </summary>
        public TradField Child1 { get { return child1; } set { child1 = value; } }

        /// <summary>
        /// Le sous texte 2
        /// </summary>
        public TradField Child2 { get { return child2; } set { child2 = value; } }

        /// <summary>
        /// Le sous texte 3
        /// </summary>
        public TradField Child3 { get { return child3; } set { child3 = value; } }

        /// <summary>
        /// Le sous texte 4
        /// </summary>
        public TradField Child4 { get { return child4; } set { child4 = value; } }

        /// <summary>
        /// Le sous texte 5
        /// </summary>
        public TradField Child5 { get { return child5; } set { child5 = value; } }

        /// <summary>
        /// Le sous texte 6
        /// </summary>
        public TradField Child6 { get { return child6; } set { child6 = value; } }

        /// <summary>
        /// Le pied de page.
        /// </summary>
        public TradField FootPage { get { return footPage; } set { footPage = value; } }

        /// <summary>
        /// La conclusion.
        /// </summary>
        public TradField Conclusion { get { return conclusion; } set { conclusion = value; } }

        /// <summary>
        /// Le ending / les credits
        /// </summary>
        public TradField End { get { return end; } set { end = value; } }

        #endregion
    }

    /// <summary>
    /// Le type de Base de toute data traductible.
    /// </summary>
    public abstract class LocalisableData
    {
        #region Attributes #########################################################

        [SerializeField]
        protected int IdTrad;
        [SerializeField]
        protected TradDataTypes TradDataType;

        #endregion
    }

    #endregion

    #region Animation >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// La data d'animation.
    /// </summary>
    [System.Serializable]
    public class AnimaData
    {
        #region Attributs #########################################################

        [SerializeField]
        private int id;
        [SerializeField]
        private List<AnimeCommand> eventList = new List<AnimeCommand>();
        [SerializeField]
        private List<AnimePhaseTimeStamp> phaseAnims = new List<AnimePhaseTimeStamp>();
        [SerializeField]
        private int physicPlace;
        [SerializeField]
        private AnimationClip motion;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// L'id de l'animation dans la BD des anima data.
        /// </summary>
        public int ID { get { return id; } set { id = value; } }

        /// <summary>
        /// La liste des evenements au cours de l'animation.
        /// </summary>
        public List<AnimeCommand> EventList { get { return eventList; } set { eventList = value; } }

        /// <summary>
        /// La phase d'animation en cours.
        /// </summary>
        public List<AnimePhaseTimeStamp> PhaseAnims { get { return phaseAnims; } set { phaseAnims = value; } }

        /// <summary>
        /// Le lieux physique, terre, aux air ... ou il est possible d'effectuer l'action.
        /// </summary>
        public PhysicSpaces PhysicPlace { get { return (PhysicSpaces)physicPlace; } set { physicPlace = (int)value; } }

        /// <summary>
        /// L'animation lue.
        /// </summary>
        public AnimationClip Motion { get { return motion; } set { motion = value; } }

        #endregion
    }

    #endregion

    #region Character >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// La Data d'un character.
    /// </summary>
    [System.Serializable]
    public class CharacterData
    {
        #region Attributs #########################################################

        [SerializeField]
        private int id;
        [SerializeField]
        private int idTrad;
        [SerializeField]
        private MindStat stats;
        [SerializeField]
        private GameObject character;
        [SerializeField]
        private RuntimeAnimatorController animatorController;
        [SerializeField]
        private Avatar animatorAvatar;
        [SerializeField]
        private List<Vector3Int> armurie;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// l'id dans BD.
        /// </summary>
        public int ID { get => id; set => id = value; }

        /// <summary>
        /// l'id des donnees de localisation.
        /// </summary>
        public int IdTrad { get => idTrad; set => idTrad = value; }

        /// <summary>
        /// Les stats du character.
        /// </summary>
        public MindStat Stats { get => stats; set => stats = value; }

        /// <summary>
        /// Le prefab du character.
        /// </summary>
        public GameObject Character { get => character; set => character = value; }

        /// <summary>
        /// Controller d'animator du character, ses mouvements de base.
        /// </summary>
        public RuntimeAnimatorController AnimatorController { get => animatorController; set => animatorController = value; }

        /// <summary>
        /// L'avatar de la disposition des bones du character si il est humanoid.
        /// </summary>
        public Avatar AnimatorAvatar { get => animatorAvatar; set => animatorAvatar = value; }

        /// <summary>
        /// La liste des armes detenues par le personnage; IDs, types et scope.
        /// </summary>
        public List<Vector3Int> Armurie { get => armurie; set => armurie = value; }

        #endregion
    }

    #endregion

    #region Combat Sysytem >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// La data d'une arme.
    /// </summary>
    [System.Serializable]
    public class WeaponData : LocalisableData
    {
        #region Attributs #########################################################

        [SerializeField]
        private int id;
        [SerializeField]
        private float range;
        [SerializeField]
        private int damageType;
        [SerializeField]
        private float damageValues;
        [SerializeField]
        private float merchantValue;
        [SerializeField]
        private List<int> materials;
        [SerializeField]
        private List<GameObject> componentParts = new List<GameObject>();
        [SerializeField]
        private PhycisStats physicProperties;
        [SerializeField]
        private AnimaData idle_move;
        [SerializeField]
        private List<AnimaData> attack_moves = new List<AnimaData>();
        [SerializeField]
        private List<AnimaData> defense_moves = new List<AnimaData>();
        [SerializeField]
        private List<WeaponPlace> restPlaces = new List<WeaponPlace>();
        [SerializeField]
        private List<WeaponPlace> carryPlaces = new List<WeaponPlace>();
        [SerializeField]
        private List<Vector3> projectilesOutPoints = new List<Vector3>();

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// l'id de l'arme dans la bd des armes.
        /// </summary>
        public int ID { get => id; set => id = value; }

        /// <summary>
        /// La portee de l'arme.
        /// </summary>
        public float Range { get => range; set => range = value; }

        /// <summary>
        /// le type de degat que l'arme inflige.
        /// </summary>
        public TypeDegatArme TypeDegats { get => (TypeDegatArme)damageType; set => damageType = (int)value; }

        /// <summary>
        /// la valeur des degats infliges.
        /// </summary>
        public float Degats { get => damageValues; set => damageValues = value; }

        /// <summary>
        /// La valeur, prix de l'arme.
        /// </summary>
        public float Cost { get => merchantValue; set => merchantValue = value; }

        /// <summary>
        /// La liste des materiaux en lesquels sont faites chaque partie de l'arme, un pour chaque objet.
        /// </summary>
        public List<PhysicMaterials> Materiaux
        {
            get
            {
                return materials.ConvertAll(new System.Converter<int, PhysicMaterials>(integer => { return (PhysicMaterials)integer; }));
            }
            set
            {
                materials = value.ConvertAll(new System.Converter<PhysicMaterials, int>(physic => { return (int)physic; }));
            }
        }


        /// <summary>
        /// la liste des gameObjects qui constituent l'arme.
        /// </summary>
        public List<GameObject> ComponentParts { get => componentParts; set => componentParts = value; }

        /// <summary>
        /// Les proprietes physiques.
        /// </summary>
        public PhycisStats PhysicProperties { get => physicProperties; set => physicProperties = value; }

        /// <summary>
        /// l'idle avec l'arme.
        /// </summary>
        public AnimaData IdleMove { get => idle_move; set => idle_move = value; }

        /// <summary>
        /// Les emplacements rengaine de l'arme. un pour chaque object
        /// </summary>
        public List<WeaponPlace> RestPlaces { get => restPlaces; set => restPlaces = value; }


        /// <summary>
        /// Les emplacements degaine de l'arme. un pour chaque object
        /// </summary>
        public List<WeaponPlace> CarryPlaces { get => carryPlaces; set => carryPlaces = value; }

        /// <summary>
        /// Les points de sortie de projectiles pour les armes a distance.
        /// </summary>
        public List<Vector3> ProjectilesOutPoints { get => projectilesOutPoints; set => projectilesOutPoints = value; }


        #endregion
    }

    #endregion

    #region Properties and stats >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// La data statistique Physique associee a des objets.
    /// </summary>
    [System.Serializable]
    public class PhycisStats
    {
        #region Attributs #########################################################

        //space occupation
        [SerializeField]
        private float volume;
        [SerializeField]
        private float density;

        //physic properties
        [SerializeField]
        private float mass;
        [SerializeField]
        private float inflammability;
        [SerializeField]
        private float frozability;

        //mecanics properties
        [SerializeField]
        private float resistance;
        [SerializeField]
        private float elasticity;
        [SerializeField]
        private float roughness;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// The spacial volume of the object
        /// </summary>
        public float Volume { get => volume; set => volume = Mathf.Clamp(value, 0, value); }

        /// <summary>
        /// The object density; how hard is to penetrate the object.
        /// </summary>
        public float Density { get => density; set => density = Mathf.Clamp(value, 0, value); }

        /// <summary>
        /// The object mass
        /// </summary>
        public float Mass { get => mass; set => mass = Mathf.Clamp(value, 0, value); }

        /// <summary>
        /// The flamability, how easy is the object catch fire.
        /// </summary>
        public float Inflammability { get => inflammability; set => inflammability = Mathf.Clamp(value, 0, value); }

        /// <summary>
        /// how easy the object get frozen at low tempratures.
        /// </summary>
        public float Frozability { get => frozability; set => frozability = Mathf.Clamp(value, 0, value); }

        /// <summary>
        /// the object resistance; how hard the object is to break.
        /// </summary>
        public float Resistance { get => resistance; set => resistance = Mathf.Clamp(value, 0, value); }

        /// <summary>
        /// The object's elasticity. how much the object can be bended.
        /// </summary>
        public float Elasticity { get => elasticity; set => elasticity = Mathf.Clamp(value, 0, value); }

        /// <summary>
        /// The object roughness. how much grip the friction against this object make.
        /// </summary>
        public float Roughness { get => roughness; set => roughness = Mathf.Clamp(value, 0, value); }

        #endregion
    }

    /// <summary>
    /// La data statistique vitale associee a des objets.
    /// </summary>
    [System.Serializable]
    public class VitalStat : PhycisStats
    {
        #region Attributs #########################################################

        [SerializeField]
        private float health;
        [SerializeField]
        private float longevity;
        [SerializeField]
        private float age;
        [SerializeField]
        private float karma;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// quantite de vie d'un objet.
        /// </summary>
        public float Health { get { return health; } set { health = Mathf.Clamp(value, 0, longevity); } }

        /// <summary>
        /// The object longevity or maximum health
        /// </summary>
        public float Longevity { get { return longevity; } set { longevity = Mathf.Clamp(value, 0, value); } }

        /// <summary>
        /// The age. how old is the object.
        /// </summary>
        public float Age { get { return age; } set { age = Mathf.Clamp(value, 0, value); } }

        /// <summary>
        /// The Karma.how much luck you got.
        /// </summary>
        public float Karma { get { return karma; } set { karma = value; } }

        #endregion
    }


    /// <summary>
    /// La data statistique physiques associee a des etres vivants.
    /// </summary>
    [System.Serializable]
    public class BodyStats : VitalStat
    {
        #region Attributs #########################################################

        [SerializeField]
        private float strenght;
        [SerializeField]
        private float endurance;
        [SerializeField]
        private float enduranceMax;
        [SerializeField]
        private float souffle;
        [SerializeField]
        private float souffleMax;
        [SerializeField]
        private float speed;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// The strenght of the target.
        /// </summary>
        public float Strenght { get => strenght; set => strenght = value; }

        /// <summary>
        /// The Endurance of the target. how much time it can keep high level of physic performances.
        /// </summary>
        public float Endurance { get => endurance; set => endurance = value; }

        /// <summary>
        /// The maximum endurance. 
        /// </summary>
        public float EnduranceMax { get => enduranceMax; set => enduranceMax = value; }

        /// <summary>
        /// How much time it can keep without breathing.
        /// </summary>
        public float Souffle { get => souffle; set => souffle = value; }

        /// <summary>
        /// The maximum possible souffle.
        /// </summary>
        public float SouffleMax { get => souffleMax; set => souffleMax = value; }

        /// <summary>
        /// the movement and reactivity speeds
        /// </summary>
        public float Speed { get => speed; set => speed = value; }


        #endregion
    }

    /// <summary>
    /// La data statistique Mentale associee a des etres vivants.
    /// </summary>
    [System.Serializable]
    public class MindStat : BodyStats
    {
        #region Attributs #########################################################

        [SerializeField]
        private float intelligence;
        [SerializeField]
        private float experience;
        [SerializeField]
        private float dexterity;
        [SerializeField]
        private float sociability;
        [SerializeField]
        private float determination;
        [SerializeField]
        private float madness;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// The intelligence. Capacity to learn contents or the required level to do some actions.
        /// </summary>
        public float Intelligence { get => intelligence; set => intelligence = value; }

        /// <summary>
        /// The experience.Capacity to learn from mistakes and avoid them next time or the mind level
        /// </summary>
        public float Experience { get => experience; set => experience = value; }

        /// <summary>
        /// Dexterity. The target skill level.
        /// </summary>
        public float Dexterity { get => dexterity; set => dexterity = value; }

        /// <summary>
        /// the dependance from others.
        /// </summary>
        public float Sociability { get => sociability; set => sociability = value; }

        /// <summary>
        /// Capacity to try and retry.
        /// </summary>
        public float Determination { get => determination; set => determination = value; }

        /// <summary>
        /// Probability to be imprevisible.
        /// </summary>
        public float Madness { get => madness; set => madness = value; }

        #endregion
    }

    #endregion

    #region Localisator >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    #endregion
}