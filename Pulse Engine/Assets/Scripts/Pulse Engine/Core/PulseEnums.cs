using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PulseEngine
{
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

    /// <summary>
    /// Les Zones.
    /// </summary>
    public enum Zones
    {
        tous,
    }

    /// <summary>
    /// The datatype of a data. Useful for static retrival of datas in Editor mode.
    /// </summary>
    public enum DataTypes
    {
        none,
        Localisation,
        Anima,
        Weapon,
        Character,
        Message,
        CommandSequence,
    }

    /// <summary>
    /// The differents sides of a node.
    /// </summary>
    public enum NodeEdgeSide
    {
        upper, lower, lefty, righty
    }

    /// <summary>
    /// The differents nodes states
    /// </summary>
    public enum NodeState
    {
        free, selected, dragged, doubleClicked
    }

    /// <summary>
    /// Arithmetic operators , mainly for command execution.
    /// </summary>
    public enum ArithmeticOperator
    {
        addition,
        substraction,
        multiplication,
        division,
        modulo,
    }

    /// <summary>
    /// Comparaison operators, mainly for conditions evaluation.
    /// </summary>
    public enum ComparaisonOperators
    {
        equals,
        notEquals,
        greaterThan,
        lessThan,
        greaterOrEquals,
        lessOrEquals,
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

    /// <summary>
    /// Le type de controller utilise par le character.
    /// </summary>
    public enum CharacterControllerType
    {
        player,
        AI
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
    /// The state of a command
    /// </summary>
    public enum CommandState
    {
        waiting,
        playing,
        done
    }

    /// <summary>
    /// Le type d'une commande.
    /// </summary>
    public enum CommandType
    {
        @comment, //Commande sans effet
        @execute, //commande executable maintenant
        @start, //Commande d'entree
        @exit, //commade de fin de sequence d'instructions.
        @break, //commande de sortie resumable
    }

    /// <summary>
    /// Le type d'une commande executable.
    /// </summary>
    public enum CmdExecutableType
    {
        _event, //commande evenement
        _action, //commande action
        _global, //commande globale
        _story //commande globale
    }

    /// <summary>
    /// Le code d'une commande evenement.
    /// </summary>
    public enum CmdEventCode
    {
        evaluateCondition,
        Affectstat,
    }

    /// <summary>
    /// Le code d'une commande action.
    /// </summary>
    public enum CmdActionCode
    {
        Idle,
        MoveTo,
        Jump,
    }

    /// <summary>
    /// Le code d'une commande globale.
    /// </summary>
    public enum CmdGlobalCode
    {
        Weather,
        Time,
    }

    /// <summary>
    /// Le code d'une commande evenement.
    /// </summary>
    public enum CmdStoryCode
    {
        StoryLine,
        Relationship,
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
    public enum AnimaAvatar
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

    #region Conditions Enums >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// Le type de condition
    /// </summary>
    public enum TypeCondition
    {
        none,
        realTime,
        gameTime,
        visibility,
        state,//TODO: remove
    }

    #endregion

    #region PathFinding Enums >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// The Moving state of an Character
    /// </summary>
    public enum PathMovingState
    {
        none,
        searchingPath,
        followingPath,
    }

    #endregion


    #endregion

}
