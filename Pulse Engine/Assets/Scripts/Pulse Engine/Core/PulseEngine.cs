using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.ResourceManagement.ResourceLocations;
using UnityEditor;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using PulseEngine.Datas;
using System.Reflection;
using System.Threading;


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
        /// The correspondance between module types and enum at runtime, used by refelction.
        /// </summary>
        public static Dictionary<ModulesManagers, Type> ManagersCache = new Dictionary<ModulesManagers, Type>();

        /// <summary>
        /// switch debug on or off.
        /// </summary>
        public static bool DebugMode = true;

        #endregion

        #region Methods ###########################################################################

        /// <summary>
        /// To reinitailize on domain reload.
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        public static void OnDomainReload()
        {

        }

        #endregion

        #region HelperMethods ###########################################################################

        /// <summary>
        /// Copie par valeur un scriptable.
        /// </summary>
        /// <returns></returns>
        public static Q LibraryClone<Q>(Q original) where Q : ScriptableObject
        {
            Q newOne = ScriptableObject.Instantiate(original);
            return newOne;
        }

        /// <summary>
        /// Copie par valeur un objet
        /// </summary>
        /// <returns></returns>
        public static T ObjectClone<T>(T original) where T : new()
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

            T nouvo = default;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Context = new System.Runtime.Serialization.StreamingContext(System.Runtime.Serialization.StreamingContextStates.Clone);
                    bf.Serialize(ms, original);
                    ms.Position = 0;
                    nouvo = (T)bf.Deserialize(ms);
                }
            }
            catch
            {
                try
                {
                    string sourceJson = JsonUtility.ToJson(original);
                    nouvo = JsonUtility.FromJson<T>(sourceJson);
                }
                catch { }
            }

            return nouvo;
        }

        #endregion

        #region Extensions ###########################################################################

        /// <summary>
        /// Limit a string to a certain number of characters.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxCharacters"></param>
        /// <returns></returns>
        public static string Limit(this string str, int maxCharacters = 40)
        {
            if (str.Length <= maxCharacters || string.IsNullOrEmpty(str))
                return str;
            int maximum = (maxCharacters > 3 ? maxCharacters - 3 : maxCharacters);
            char[] chain = new char[maxCharacters];
            for (int i = 0; i < maximum; i++) { chain[i] = str[i]; }
            if (maximum < maxCharacters)
                for (int i = maximum; i < maxCharacters; i++) { chain[i] = '.'; }
            return new string(chain);
        }

        /// <summary>
        /// Encadre un texte dans des balises Html couleur
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static string Hightlight(this string txt, Color col)
        {
            if (string.IsNullOrEmpty(txt))
                return txt;
            string htmlCol = ColorUtility.ToHtmlStringRGB(col);
            return "<color=#" + htmlCol + ">" + txt + "</color>";
        }

        /// <summary>
        /// Encadre un texte dans des balises gras
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static string Bold(this string txt)
        {
            if (string.IsNullOrEmpty(txt))
                return txt;
            return "<b>" + txt + "</b>";
        }

        /// <summary>
        /// Encadre un texte dans des balises italic
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static string Italic(this string txt)
        {
            if (string.IsNullOrEmpty(txt))
                return txt;
            return "<i>" + txt + "</i>";
        }

        /// <summary>
        /// Encadre un texte dans des balises de soulignage
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static string Underline(this string txt)
        {
            if (string.IsNullOrEmpty(txt))
                return txt;
            return "<u>" + txt + "</u>";
        }

        /// <summary>
        /// Return the selected field
        /// </summary>
        /// <param name="dta"></param>
        /// <param name="fld"></param>
        /// <returns></returns>
        public static TradField GetTradField(this Localisationdata dta, DatalocationField fld)
        {
            switch (fld)
            {
                case DatalocationField.title:
                    return dta.Title;
                case DatalocationField.header:
                    return dta.Header;
                case DatalocationField.banner:
                    return dta.Banner;
                case DatalocationField.groupName:
                    return dta.GroupName;
                case DatalocationField.toolTip:
                    return dta.ToolTip;
                case DatalocationField.description:
                    return dta.Description;
                case DatalocationField.details:
                    return dta.Details;
                case DatalocationField.infos:
                    return dta.Infos;
                case DatalocationField.child1:
                    return dta.Child1;
                case DatalocationField.child2:
                    return dta.Child2;
                case DatalocationField.child3:
                    return dta.Child3;
                case DatalocationField.child4:
                    return dta.Child4;
                case DatalocationField.child5:
                    return dta.Child5;
                case DatalocationField.child6:
                    return dta.Child6;
                case DatalocationField.footPage:
                    return dta.FootPage;
                case DatalocationField.conclusion:
                    return dta.Conclusion;
                case DatalocationField.end:
                    return dta.End;
                default:
                    return dta.Title;
            }
        }

        #endregion
    }

    #endregion

    #region Enums <><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>


    #region Globals >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// The Modules Managers.
    /// </summary>
    public enum ModulesManagers
    {
        Localisator = 0,
        CharacterCreator = 2,
        MessageSystem,
    }

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
        @conditionnal, //Commande conditionnelle
        @exit, //commade de fin de sequence d'instructions.
        @jump, //commande de saut vers l'index d'une instruction
        @out, //commande de sortie d'une sous liste d'instructions
        @execute, //commande executable maintenant
        @delay, //commande a executer apres la sortie, avec un delai
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

    /// <summary>
    ///  Le reperage d'une data dans les assets.
    /// </summary>
    [System.Serializable]
    public struct DataLocation : IEquatable<DataLocation>
    {
        public int id;
        public int globalLocation;
        public int localLocation;
        public DataTypes dType;


        public override bool Equals(object o)
        {
            DataLocation other = (DataLocation)o;
            if (other != default && other != null)
                return Equals(other);
            else
                return ReferenceEquals(this, o);
        }

        public override int GetHashCode()
        {
            return base.GetHashCode();
        }

        public bool Equals(DataLocation other)
        {
            return dType == other.dType && globalLocation == other.globalLocation && localLocation == other.localLocation && id == other.id;
        }

        public static bool operator ==(DataLocation x, object y)
        {
            if (y == null)
                return false;
            var dt = (DataLocation)y;
            if (dt != null)
                return x.Equals(dt);
            else
                return ((object)x).Equals(y);
        }

        public static bool operator !=(DataLocation x, object y)
        {
            if (y == null)
                return true;
            var dt = (DataLocation)y;
            if (dt != null)
                return !x.Equals(dt);
            else
                return !((object)x).Equals(y);
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

        public TradField(string str = "")
        {
            textField = str;
            imageField = null;
            audioField = null;
        }
    }

    #endregion

    #region PhysicSpace Structs >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    #endregion

    #region Commander Structs >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// Une commande action.
    /// </summary>
    [System.Serializable]
    public struct Command : IEquatable<Command>
    {
        #region Attributs #######################################################################

        [SerializeField]
        private CommandType type;
        [SerializeField]
        private CmdExecutableType childType;
        [SerializeField]
        private int code;
        [SerializeField]
        private Vector4 primaryParameters;
        [SerializeField]
        private Vector4 secondaryParameters;
        [SerializeField]
        private Vector2 editorNodePos;
        [SerializeField]
        private string parentJson;
        [SerializeField]
        private string childrenJson;

        #endregion

        #region Properties #######################################################################

        /// <summary>
        /// The command's current state.
        /// </summary>
        public CommandState STate { get; set; }

        /// <summary>
        /// The command's main type
        /// </summary>
        public CommandType Type { get => type; set => type = value; }

        /// <summary>
        /// The command's executable type
        /// </summary>
        public CmdExecutableType ChildType { get => childType; set => childType = value; }

        /// <summary>
        /// The command's code
        /// </summary>
        public CmdEventCode CodeEv { get => (CmdEventCode)code; set => code = (int)value; }

        /// <summary>
        /// The command's code
        /// </summary>
        public CmdActionCode CodeAc { get => (CmdActionCode)code; set => code = (int)value; }

        /// <summary>
        /// The command's code
        /// </summary>
        public CmdGlobalCode CodeGl { get => (CmdGlobalCode)code; set => code = (int)value; }

        /// <summary>
        /// The command's code
        /// </summary>
        public CmdStoryCode CodeSt { get => (CmdStoryCode)code; set => code = (int)value; }

        /// <summary>
        /// The command's children count.
        /// </summary>
        public int ChildrenCount
        {
            get
            {
                if (Children.targets == null)
                    return 0;
                return Children.targets.Count;
            }
        }

        /// <summary>
        /// The command's primary parameters.
        /// </summary>
        public Vector4 PrimaryParameters { get => primaryParameters; set => primaryParameters = value; }

        /// <summary>
        /// The command's secondary parameters.
        /// </summary>
        public Vector4 SecondaryParameters { get => secondaryParameters; set => secondaryParameters = value; }

        /// <summary>
        /// The parent Command
        /// </summary>
        public Command Parent {
            get {
                if (string.IsNullOrEmpty(parentJson))
                    return Command.NullCmd;
                Command c = default;
                try
                {
                    c = (Command)JsonUtility.FromJson(parentJson, typeof(Command));
                }
                catch { return default; }
                return c;
            }
            set
            {
                parentJson = JsonUtility.ToJson(value);
            }
        }

        /// <summary>
        /// The children node array
        /// </summary>
        public CommandList Children
        {
            get
            {
                if (string.IsNullOrEmpty(childrenJson))
                    return CommandList.NullCmdList;
                CommandList c = default;
                try
                {
                    c = (CommandList)JsonUtility.FromJson(childrenJson, typeof(CommandList));
                }
                catch { return default; }
                return c;
            }
            set
            {
                childrenJson = JsonUtility.ToJson(value);
            }
        }

        #endregion

        #region methods #######################################################################

        public bool Equals(Command other)
        {
            return type == other.type && childType == other.childType && code == other.code && PrimaryParameters == other.primaryParameters && SecondaryParameters == other.secondaryParameters;
        }
        public static bool operator==(Command a, Command b) {
            return a.Equals(b);
        }
        public static bool operator!=(Command a, Command b) {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            var hashCode = -1644972737;
            hashCode = hashCode * -1521134295 + code.GetHashCode();
            hashCode = hashCode * -1521134295 + Type.GetHashCode();
            hashCode = hashCode * -1521134295 + ChildType.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector4>.Default.GetHashCode(PrimaryParameters);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector4>.Default.GetHashCode(SecondaryParameters);
            hashCode = hashCode * -1521134295 + EqualityComparer<Command>.Default.GetHashCode(Parent);
            hashCode = hashCode * -1521134295 + EqualityComparer<CommandList>.Default.GetHashCode(Children);
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }


        /// <summary>
        /// Return the master parent command.
        /// </summary>
        /// <returns></returns>
        public Command MasterCommand()
        {
            Command t = this;
            while (!t.Parent.Equals(Command.NullCmd))
            {
                var f = t.Parent;
                var tmp = f.Children;
                if (tmp.targets == null)
                    tmp.targets = new List<Command>();
                tmp.targets.Add(t);
                f.Children = tmp;
                t = f;
            }
            return t;
        }

        /// <summary>
        /// Add a child to this command
        /// </summary>
        /// <param name="c"></param>
        public void AddChild(Command c)
        {
            var t = Children;
            if (t.targets == null)
                t.targets = new List<Command>();
            t.targets.Add(c);
            Children = t;
            int l = t.targets.Count;
            for(int i = 0; i < l; i++)
            {
                var ch = t.targets[i];
                ch.Parent = this;
                t.targets[i] = ch;
            }
            Children = t;
        }

        /// <summary>
        /// Remove a child to this command
        /// </summary>
        /// <param name="c"></param>
        public void RemoveChild(Command c)
        {
            var t = Children;
            if (t.targets == null)
                return;
            if (t.targets.Contains(c))
                t.targets.Remove(c);
            Children = t;
        }

        /// <summary>
        /// Remove a child to this command
        /// </summary>
        /// <param name="c"></param>
        public void RemoveChildAt(int index)
        {
            var t = Children;
            if (t.targets == null)
                return;
            if (t.targets.Count > index && index >= 0)
                t.targets.RemoveAt(index);
            Children = t;
        }

        /// <summary>
        /// Remove a child to this command
        /// </summary>
        /// <param name="c"></param>
        public void InsertChildAt(Command c, int index)
        {
            var t = Children;
            if (t.targets == null)
                return;
            if (t.targets.Count > index && index >= 0)
                t.targets.Insert(index, c);
            Children = t;
        }

        /// <summary>
        /// Return child at index.
        /// </summary>
        /// <param name="depth"></param>
        /// <returns></returns>
        public Command GetChild(int index = 0)
        {
            if (Children.targets == null)
                return Command.NullCmd;
            if (index < 0 || Children.targets.Count <= index)
                return Command.NullCmd;
            return Children.targets[index];
        }

        public static Command NullCmd { get => new Command { code = -1 }; }

#if UNITY_EDITOR

        public Vector2 NodePosition { get => editorNodePos; set => editorNodePos = value; }
        //public int ParentNodeIndex { get => parentCmdIndex; set => parentCmdIndex = value; }

        public void NodeDisplay()
        {
            Type = (CommandType)EditorGUILayout.EnumPopup("Node Type", Type);
            switch (Type)
            {
                case CommandType.comment:
                    break;
                case CommandType.conditionnal:
                    break;
                case CommandType.exit:
                    break;
                case CommandType.jump:
                    break;
                case CommandType.@out:
                    break;
                case CommandType.execute:
                    childType = (CmdExecutableType)EditorGUILayout.EnumPopup("Execute", ChildType);
                    switch (ChildType)
                    {
                        case CmdExecutableType._event:
                            CodeEv = (CmdEventCode)EditorGUILayout.EnumPopup("Code", CodeEv);
                            break;
                        case CmdExecutableType._action:
                            CodeAc = (CmdActionCode)EditorGUILayout.EnumPopup("Code", CodeAc);
                            break;
                        case CmdExecutableType._global:
                            CodeGl = (CmdGlobalCode)EditorGUILayout.EnumPopup("Code", CodeGl);
                            break;
                        case CmdExecutableType._story:
                            CodeSt = (CmdStoryCode)EditorGUILayout.EnumPopup("Code", CodeSt);
                            break;
                    }
                    break;
                case CommandType.delay:
                    break;
            }
        }

#endif
        #endregion
    }

    /// <summary>
    /// The transit struct to serialize command children.
    /// </summary>
    public struct CommandList: IEquatable<CommandList>
    {
        public List<Command> targets;
        public static CommandList NullCmdList { get => new CommandList { targets = null }; }

        public bool Equals(CommandList other)
        {
            if (targets == null && other.targets == null)
                return true;
            if (targets == null ^ other.targets == null)
                return false;
            if (targets.Count != other.targets.Count)
                return false;
            int commonlenght = targets.Count;
            for(int i = 0; i < commonlenght; i++)
            {
                if (targets[i] != other.targets[i])
                    return false;
            }
            return true;
        }

        public override int GetHashCode()
        {
            return -1745064695 + EqualityComparer<List<Command>>.Default.GetHashCode(targets);
        }

        public static bool operator ==(CommandList a, CommandList b)
        {
            return a.Equals(b);
        }

        public static bool operator !=(CommandList a, CommandList b)
        {
            return !a.Equals(b);
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
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
        public Command command;
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

    /// <summary>
    /// L'interface des datas du systeme.
    /// </summary>
    public interface IData
    {
        DataLocation Location { get; set; }
    }

    #endregion

    #region ECS <><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>

    #region Structs >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    #endregion

    #region Archetypes >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    #endregion

    #endregion
}

namespace PulseEngine.Datas
{

    #region Libraries <><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>

    /// <summary>
    /// La classe mere des libraies d'assets
    /// </summary>
    [System.Serializable]
    public class CoreLibrary : ScriptableObject
    {

        #region Attributes >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        /// <summary>
        /// Usually the GD scope, it's the first asset filter parameter
        /// </summary>
        [SerializeField]
        protected int libraryMainLocation;

        /// <summary>
        /// Usually the zone, it's the second asset filter parameter
        /// </summary>
        [SerializeField]
        protected int librarySecLocation;

        #endregion

        #region Properties >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        public virtual List<IData> DataList { get; set; }

        #endregion

        #region Methods >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

#if UNITY_EDITOR

        /// <summary>
        /// Cree l'asset.
        /// </summary>
        /// <returns></returns>
        public static bool Save<T>(string assetsPath, params object[] locationFilters) where T : CoreLibrary
        {
            string fileName = typeof(T).Name;
            for (int i = 0; i < locationFilters.Length; i++)
            {
                fileName += "_" + (int)locationFilters[i];
            }
            fileName += ".asset";
            string path = assetsPath;
            string folderPath = string.Join("/", Core.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", Core.Path_GAMERESSOURCES, path, fileName);
            if (!AssetDatabase.IsValidFolder(Core.Path_GAMERESSOURCES))
                return false;
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(Core.Path_GAMERESSOURCES, path);
                AssetDatabase.SaveAssets();
            }
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                T asset = ScriptableObject.CreateInstance<T>();

                try
                {
                    asset.libraryMainLocation = locationFilters.Length > 0 ? (int)locationFilters[0] : 0;
                }
                catch (Exception e)
                {
                    if (e.GetType() != typeof(InvalidCastException))
                        throw new Exception("Unknow exeption occured when saving " + fileName);
                }
                try
                {
                    asset.librarySecLocation = locationFilters.Length > 1 ? (int)locationFilters[1] : 0;
                }
                catch (Exception e)
                {
                    if (e.GetType() != typeof(InvalidCastException))
                        throw new Exception("Unknow exeption occured when saving " + fileName);
                }
                AssetDatabase.CreateAsset(asset, fullPath);
                AssetDatabase.SaveAssets();
                //Make a gameobject an addressable
                var settings = AddressableAssetSettingsDefaultObject.Settings;
                if (settings != null)
                {
                    AddressableAssetGroup g = settings.DefaultGroup;
                    if (g != null)
                    {
                        var guid = AssetDatabase.AssetPathToGUID(fullPath);
                        //This is the function that actually makes the object addressable
                        var entry = settings.CreateOrMoveEntry(guid, g);

                        //simplify entry names
                        var parts = fullPath.Split(new[] { '/', '.' });
                        entry.SetAddress(parts[parts.Length - 2], true);

                        //You'll need these to run to save the changes!
                        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
                        AssetDatabase.SaveAssets();
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifie si l'asset existe.
        /// </summary>
        /// <returns></returns>
        public static bool Exist<T>(string assetsPath, params object[] locationFilters) where T : CoreLibrary
        {
            string fileName = typeof(T).Name;
            for (int i = 0; i < locationFilters.Length; i++)
            {
                fileName += "_" + (int)locationFilters[i];
            }
            fileName += ".asset";
            string path = assetsPath;
            string folderPath = string.Join("/", Core.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", Core.Path_GAMERESSOURCES, path, fileName);
            if (!AssetDatabase.IsValidFolder(Core.Path_GAMERESSOURCES))
                return false;
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(Core.Path_GAMERESSOURCES, path);
                AssetDatabase.SaveAssets();
            }
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                if (AssetDatabase.LoadAssetAtPath<T>(fullPath) == null)
                    return false;
                else
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Charge l'asset.
        /// </summary>
        /// <returns></returns>
        public static T Load<T>(string assetsPath, params object[] locationFilters) where T : CoreLibrary
        {
            string fileName = typeof(T).Name;
            for (int i = 0; i < locationFilters.Length; i++)
            {
                fileName += "_" + (int)locationFilters[i];
            }
            fileName += ".asset";
            string path = assetsPath;
            string folderPath = string.Join("/", Core.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", Core.Path_GAMERESSOURCES, path, fileName);
            if (Exist<T>(assetsPath, locationFilters))
            {
                return AssetDatabase.LoadAssetAtPath(fullPath, typeof(T)) as T;
            }
            else
                return null;
        }

#endif

        #endregion
    }

    #endregion

    #region Data Access <><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>

    /// <summary>
    /// Classe d'acces aux datas en runtime.
    /// </summary>
    public static class CoreData
    {

        #region Attributs >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        /// <summary>
        /// The event pool allowing to wait for asset load complete
        /// </summary>
        private static Dictionary<string, ManualResetEvent> LoaderPool = new Dictionary<string, ManualResetEvent>();

        #endregion

        #region Properties >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>


        #endregion

        #region Methodes >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        /// <summary>
        /// Call on async load complete.
        /// </summary>
        /// <param name="loaderPoolIndex"></param>
        private static void SetAssetLoadComplete(string assetPath)
        {
            if (!LoaderPool.ContainsKey(assetPath))
                return;
            LoaderPool[assetPath].Set();
        }

        /// <summary>
        /// Await this method to wait for an asset to finnish loading.
        /// </summary>
        /// <param name="waitMillisec"></param>
        /// <returns></returns>
        private static async Task WaitAssetLoadUntil(string assetPath, int waitMillisec = 100)
        {
            if (LoaderPool.ContainsKey(assetPath))
                return;
            LoaderPool.Add(assetPath, new ManualResetEvent(false));
            await Task.Factory.StartNew(() =>
            {
                LoaderPool[assetPath].WaitOne(waitMillisec);
                LoaderPool[assetPath].Reset();
                LoaderPool[assetPath].Dispose();
                LoaderPool[assetPath] = null;
                LoaderPool.Remove(assetPath);
            });
        }


        /// <summary>
        /// Execute a manager method async at runtime.
        /// </summary>
        /// <param name="mgrEnum"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<T> ManagerAsyncMethod<T>(ModulesManagers mgrEnum, string methodName, params object[] parameters)
        {
            if (Core.ManagersCache == null)
                Core.ManagersCache = new Dictionary<ModulesManagers, Type>();
            if (!Core.ManagersCache.ContainsKey(mgrEnum))
            {
                string classPath = mgrEnum.ToString() + "." + mgrEnum.ToString();
                Type Mgrclass = Type.GetType("PulseEngine.Modules." + classPath);
                if (Mgrclass == null)
                {
                    //TODO: Remove
                    PulseDebug.Log("null Manager at " + classPath);
                    return default;
                }
                Core.ManagersCache.Add(mgrEnum, Mgrclass);
            }
            if (Core.ManagersCache[mgrEnum] != null)
            {
                var Method = MethodFromClass(Core.ManagersCache[mgrEnum], methodName);
                if (Method == null)
                {
                    //TODO: Remove
                    PulseDebug.Log("Null method");
                    return default;
                }
                //TODO: Remove
                PulseDebug.Log("Method infos summary\n" +
                    "name: " + Method.Name + "\n" +
                    "is static: " + Method.IsStatic + "\n" +
                    "returning: " + Method.ReturnType);
                try
                {
                    Task<T> task = (Task<T>)Method.Invoke(null, parameters);
                    //TODO: Remove
                    PulseDebug.Log("its task of type " + typeof(T));
                    await task.ConfigureAwait(false);
                    //TODO: Remove
                    PulseDebug.Log("task result is " + task.Result);
                    T result = task.Result;
                    return result;
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(InvalidCastException))
                    {
                        try
                        {
                            T result = (T)Method.Invoke(null, parameters);
                            //TODO: Remove
                            PulseDebug.Log("its normal method");
                            return result;
                        }
                        catch (Exception r)
                        {
                            //TODO: Remove
                            PulseDebug.Log("second exception occured : " + r.Message);
                            return default;
                        }
                    }
                    //TODO: Remove
                    PulseDebug.Log("exception occured but it's not an invalid cast. it's " + e + " || " + e.Message);
                    return default;
                }
            }
            //TODO: Remove
            PulseDebug.Log("Null type in Core Manager cache");
            return default;
        }

        /// <summary>
        /// Get a method from a class by reflection.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static MethodInfo MethodFromClass(Type t, string methodName)
        {
            if (t == null)
                return null;
            MethodInfo i = t.GetMethod(methodName);
            return i;
        }


        /// <summary>
        /// Get all module datas with specified parameters
        /// </summary>
        /// <returns></returns>
        public static async Task<List<T>> GetAllDatas<T, Q>() where Q : CoreLibrary where T : IData
        {
            string keyNamePart = typeof(Q).Name;
            List<string> keys = new List<string>();
            foreach (var loc in Addressables.ResourceLocators)
            {
                List<object> _keys = new List<object>(loc.Keys);
                for (int i = 0; i < _keys.Count; i++)
                {
                    var ks = _keys[i] as string;
                    if (ks == null)
                        continue;
                    if (string.IsNullOrEmpty(ks))
                        continue;
                    if (ks.Contains(keyNamePart))
                        keys.Add(ks);
                }
            }
            List<T> output = new List<T>();
            Q library = null;
            int k = 0;
            for (int i = 0; i < keys.Count; i++)
            {
                k = i;
                try
                {
                    Addressables.LoadAssetAsync<Q>(keys[k]).Completed += hdl =>
                    {
                        if (hdl.Status == AsyncOperationStatus.Succeeded)
                            library = hdl.Result;
                        SetAssetLoadComplete(keys[k]);
                    };
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(UnityEngine.UnityException))
                    {
                        Addressables.InitializeAsync();
                        try
                        {
                            Addressables.LoadAssetAsync<Q>(keys[k]).Completed += hdl =>
                            {
                                if (hdl.Status == AsyncOperationStatus.Succeeded)
                                    library = hdl.Result;
                                SetAssetLoadComplete(keys[k]);
                            };
                        }
                        catch
                        {
                            return null;
                        }
                    }
                    return null;
                }
                await WaitAssetLoadUntil(keys[k], 1000);
                if (library == null)
                    continue;
                var datalist = Core.LibraryClone(library).DataList;
                output.AddRange(datalist.ConvertAll(new Converter<object, T>(data => { return (T)data; })));
                library = null;
            }
            return output.Count > 0 ? output.FindAll(item => { return item != null; }) : null;
        }


        /// <summary>
        /// Get all module datas with specified parameters
        /// </summary>
        /// <returns></returns>
        public static async Task<List<T>> GetDatas<T, Q>(DataLocation _location) where Q : CoreLibrary where T : IData
        {
            string path = typeof(Q).Name + "_" + _location.globalLocation + "_" + _location.localLocation;
            IList<IResourceLocation> location = null;
            try
            {
                Addressables.LoadResourceLocationsAsync(path).Completed += hdl =>
                {
                    if (hdl.Status == AsyncOperationStatus.Succeeded)
                        location = hdl.Result;
                    SetAssetLoadComplete(path);
                };
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(UnityEngine.UnityException))
                {
                    await Addressables.InitializeAsync().Task;
                    try
                    {
                        Addressables.LoadResourceLocationsAsync(path).Completed += hdl =>
                        {
                            if (hdl.Status == AsyncOperationStatus.Succeeded)
                                location = hdl.Result;
                            SetAssetLoadComplete(path);
                        };
                    }
                    catch
                    {
                        return null;
                    }
                }
                return null;
            }
            await WaitAssetLoadUntil(path, 10000);

            if (location == null || location.Count <= 0)
                return null;
            var key = location[0].PrimaryKey;
            Q library = null;
            Addressables.LoadAssetAsync<Q>(key).Completed += hdl =>
            {
                if (hdl.Status == AsyncOperationStatus.Succeeded)
                    library = hdl.Result;
                SetAssetLoadComplete(path);
            };
            await WaitAssetLoadUntil(path, 10000);

            if (library == null)
                return null;
            var datalist = Core.LibraryClone(library).DataList;
            return datalist.FindAll(d => { return d != null; }).ConvertAll(new Converter<object, T>(data => { return (T)data; }));
        }

        /// <summary>
        /// Get module data with ID
        /// </summary>
        /// <returns></returns>
        public static async Task<T> GetData<T, Q>(DataLocation _location) where Q : CoreLibrary where T : class, IData
        {
            var list = await GetDatas<T, Q>(_location);
            return list != null ? list.Find(data => { return data.Location.id == _location.id; }) : null;
        }

        #endregion
    }

    #endregion

    #region Localisation >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// La Data de localisation contenu dans un asset de localisation, dans une langue precise.
    /// </summary>
    [System.Serializable]
    public class Localisationdata : IData
    {
        #region Attributes ###############################################################

        [SerializeField]
        private DataLocation location;

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
        public DataLocation Location
        {
            get { return location; }
            set
            {
                var tmp = value;
                tmp.dType = DataTypes.Localisation;
                location = tmp;
            }
        }

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
        private DataLocation tradLocation;

        #endregion

        #region Properties #########################################################

        /// <summary>
        /// Traduction file ID.
        /// </summary>
        public DataLocation TradLocation
        {
            get { return tradLocation; }
            set
            {
                var tmp = value;
                tmp.dType = DataTypes.Localisation;
                tradLocation = tmp;
            }
        }

        #endregion
    }

    #endregion

    #region Animation >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// La data d'animation.
    /// </summary>
    [System.Serializable]
    public class AnimaData : IData
    {
        #region Attributs #########################################################

        [SerializeField]
        private DataLocation location;
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
        public DataLocation Location
        {
            get { return location; }
            set
            {
                var tmp = value;
                tmp.dType = DataTypes.Anima;
                location = tmp;
            }
        }

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
    public class CharacterData : LocalisableData, IData
    {
        #region Attributs #########################################################

        [SerializeField]
        private DataLocation location;
        [SerializeField]
        private MindStat stats;
        [SerializeField]
        private GameObject character;
        [SerializeField]
        private RuntimeAnimatorController animatorController;
        [SerializeField]
        private Avatar animatorAvatar;
        [SerializeField]
        private List<DataLocation> armurie;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// l'id dans BD.
        /// </summary>
        public DataLocation Location
        {
            get { return location; }
            set
            {
                var tmp = value;
                tmp.dType = DataTypes.Character;
                location = tmp;
            }
        }

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
        public List<DataLocation> Armurie { get => armurie; set => armurie = value; }

        #endregion
    }

    #endregion

    #region Combat Sysytem >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// La data d'une arme.
    /// </summary>
    [System.Serializable]
    public class WeaponData : LocalisableData, IData
    {
        #region Attributs #########################################################

        [SerializeField]
        private DataLocation location;
        [SerializeField]
        private float range;
        [SerializeField]
        private int damageType;
        [SerializeField]
        private float damageValues;
        [SerializeField]
        private float merchantValue;
        [SerializeField]
        private List<int> materials = new List<int>();
        [SerializeField]
        private List<GameObject> componentParts = new List<GameObject>();
        [SerializeField]
        private PhysicStats physicProperties;
        [SerializeField]
        private DataLocation idle_move;
        [SerializeField]
        private DataLocation draw_move;
        [SerializeField]
        private DataLocation sheath_move;
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
        public DataLocation Location
        {
            get { return location; }
            set
            {
                var tmp = value;
                tmp.dType = DataTypes.Weapon;
                location = tmp;
            }
        }

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
        public PhysicStats PhysicProperties { get => physicProperties; set => physicProperties = value; }

        /// <summary>
        /// l'idle avec l'arme.
        /// </summary>
        public DataLocation IdleMove { get => idle_move; set => idle_move = value; }

        /// <summary>
        /// le degainage avec l'arme.
        /// </summary>
        public DataLocation DrawMove { get => draw_move; set => draw_move = value; }

        /// <summary>
        /// le rengainage avec l'arme.
        /// </summary>
        public DataLocation SheathMove { get => sheath_move; set => sheath_move = value; }

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
    public struct PhysicStats
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
    public struct VitalStat
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
        [SerializeField]
        private PhysicStats phycics_stats;

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

        /// <summary>
        /// The physic stats.
        /// </summary>
        public PhysicStats PhycicsStats { get => phycics_stats; set => phycics_stats = value; }

        #endregion
    }


    /// <summary>
    /// La data statistique physiques associee a des etres vivants.
    /// </summary>
    [System.Serializable]
    public struct BodyStats
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
        [SerializeField]
        private VitalStat vital_stat;

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

        /// <summary>
        /// the Vital Stats.
        /// </summary>
        public VitalStat VitalStats { get => vital_stat; set => vital_stat = value; }

        #endregion
    }

    /// <summary>
    /// La data statistique Mentale associee a des etres vivants.
    /// </summary>
    [System.Serializable]
    public struct MindStat
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
        [SerializeField]
        private BodyStats body_stat;

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

        /// <summary>
        /// The body stats.
        /// </summary>
        public BodyStats BodyStats { get => body_stat; set => body_stat = value; }

        #endregion
    }

    #endregion

    #region MessageSystem >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    public class MessageData : IData
    {
        #region Attributs #########################################################

        [SerializeField]
        private DataLocation text;
        [SerializeField]
        private List<DataLocation> choices = new List<DataLocation>();
        [SerializeField]
        private float selectionDelay;
        [SerializeField]
        private int defaultChoiceIndex;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// The Message location in the message database
        /// </summary>
        public DataLocation Location { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

        /// <summary>
        /// The message's text trad data location.
        /// </summary>
        public DataLocation Text { get => text; set => text = value; }

        /// <summary>
        /// the message's choices trad datas locations.
        /// </summary>
        public List<DataLocation> Choices { get => choices; set => choices = value; }

        /// <summary>
        /// The message selection delay
        /// </summary>
        public float SelectionDelay { get => selectionDelay; set => selectionDelay = value; }

        /// <summary>
        /// The default choice index
        /// </summary>
        public int DefaultChoiceIndex { get => defaultChoiceIndex; set => defaultChoiceIndex = Mathf.Clamp(value, 0, choices.Count); }

        #endregion

        #region Methods #########################################################

        #endregion
    }


    #endregion

    #region ....... >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    #endregion
}