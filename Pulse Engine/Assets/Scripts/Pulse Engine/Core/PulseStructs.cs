using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

namespace PulseEngine
{
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
    public struct Command : IEquatable<Command>, IEditorNode
    {
        #region Attributs #######################################################################

        [SerializeField]
        private CommandPath path;
        [SerializeField]
        private int code;
        [SerializeField]
        private Vector4 primaryParameters;
        [SerializeField]
        private Vector4 secondaryParameters;
        [SerializeField]
        private List<CommandPath> inputs;
        [SerializeField]
        private List<CommandPath> outputs;
        [SerializeField]
        private Vector2 editorNodePos;

        #endregion

        #region Properties #######################################################################

        /// <summary>
        /// The Command location in the Sequence.
        /// </summary>
        public CommandPath CmdPath
        {
            get
            {
                return path;
            }
            set => path = value;
        }

        /// <summary>
        /// The command's current state.
        /// </summary>
        public CommandState State { get; set; }

        /// <summary>
        /// The parents paths in the sequence.
        /// </summary>
        public List<CommandPath> Inputs
        {
            get
            {
                if (inputs == null)
                    inputs = new List<CommandPath>();
                return inputs;
            }
            set => inputs = value;
        }

        /// <summary>
        /// The command childrens in the sequence.
        /// </summary>
        public List<CommandPath> Outputs
        {
            get
            {
                if (outputs == null)
                    outputs = new List<CommandPath>();
                return outputs;
            }
            set => outputs = value;
        }

        /// <summary>
        /// get the default Output, or null commandpath.
        /// </summary>
        public CommandPath DefaultOutPut
        {
            get
            {
                if (Outputs.Count > 0)
                    return Outputs[0];
                else
                    return CommandPath.NullPath;
            }
        }

        /// <summary>
        /// The command's code.
        /// </summary>
        public int Code { get => code; set => code = value; }

        /// <summary>
        /// The command's primary parameters.
        /// </summary>
        public Vector4 PrimaryParameters { get => primaryParameters; set => primaryParameters = value; }

        /// <summary>
        /// The command's secondary parameters.
        /// </summary>
        public Vector4 SecondaryParameters { get => secondaryParameters; set => secondaryParameters = value; }


        #endregion

        #region methods #######################################################################

        public bool Equals(Command other)
        {
            return CmdPath == other.CmdPath && ReferenceEquals(this, other);
        }
        public static bool operator ==(Command a, Command b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(Command a, Command b)
        {
            return !a.Equals(b);
        }

        public override int GetHashCode()
        {
            var hashCode = -1644972737;
            hashCode = hashCode * -1521134295 + code.GetHashCode();
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector4>.Default.GetHashCode(PrimaryParameters);
            hashCode = hashCode * -1521134295 + EqualityComparer<Vector4>.Default.GetHashCode(SecondaryParameters);
            return hashCode;
        }

        public override bool Equals(object obj)
        {
            return base.Equals(obj);
        }

        /// <summary>
        /// check if the command already has this input.
        /// </summary>
        /// <param name="parent"></param>
        /// <returns></returns>
        public bool IsParent(CommandPath parent)
        {
            if (parent == CommandPath.NullPath)
                return true;
            if (inputs == null)
                inputs = new List<CommandPath>();
            if (inputs.Contains(parent))
                return true;
            return false;
        }

        /// <summary>
        /// check if the command already has this output.
        /// </summary>
        /// <param name="child"></param>
        /// <returns></returns>
        public bool IsChild(CommandPath child)
        {
            if (child == CommandPath.NullPath)
                return false;
            if (outputs == null)
                outputs = new List<CommandPath>();
            if (outputs.Contains(child))
                return true;
            return false;
        }

        /////////////////////////////////////////////////////////////////////////////////////////////////

        public static Command NullCmd { get => new Command { path = CommandPath.NullPath, code = -1 }; }

#if UNITY_EDITOR

        public Rect NodeShape { get; set; }
        public NodeState NodeState { get; set; }
        public GenericMenu ContextMenu { get; set; }
        public Func<Command, Command> DisplayDetails { get; set; }

        public IEditorNode CreateNode()
        {
            NodeShape = new Rect(editorNodePos, new Vector2(200, 80));
            return this;
        }

        public IEditorNode ShowDetails()
        {
            var p = CmdPath;
            //
            if (DisplayDetails != null)
                this = DisplayDetails.Invoke(this);
            //
            CmdPath = p;
            return this;
        }


        public void DrawNode(GUIStyle style)
        {
            switch (NodeState)
            {
                case NodeState.selected:
                    style = "Button";
                    break;
                case NodeState.dragged:
                    style = "Button";
                    break;
            }
            if (style == null)
                style = "Button";

            GUI.Box(NodeShape, NodeShape.size.x > 80 ? path.Label : string.Empty, style);
        }

        public IEditorNode ProcessNodeEvents(Event e, IEditorGraph linkSource, out bool changed)
        {
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 0)
                    {
                        if (NodeShape.Contains(e.mousePosition))
                        {
                            if (e.clickCount == 2)
                            {
                                NodeState = NodeState.doubleClicked;
                                e.Use();
                                changed = true;
                                return this;
                            }
                            if (e.clickCount == 1)
                            {
                                NodeState = NodeState.selected;
                            }
                        }
                        else
                            NodeState = NodeState.free;
                    }
                    if (e.button == 1 && NodeShape.Contains(e.mousePosition))
                    {
                        GUI.FocusControl(null);
                        if (ContextMenu != null)
                            ContextMenu.ShowAsContext();
                        e.Use();
                    }
                    break;

                case EventType.MouseUp:
                    if (e.button == 0 && NodeShape.Contains(e.mousePosition) && NodeState == NodeState.free)
                    {
                        if (linkSource != null && linkSource.LinkRequester != default)
                        {
                            try
                            {
                                if (((CommandSequence)linkSource).Link(((Command)(linkSource.LinkRequester)).CmdPath, CmdPath))
                                    PulseDebug.Log($"Connected {((Command)(linkSource.LinkRequester)).CmdPath.Label} to {path.Label}");
                                else
                                    PulseDebug.Log($"Connection to {path.Label} failed");
                            }
                            finally
                            {
                                linkSource.LinkRequester = default;
                                changed = true;
                            }
                            return this;
                        }
                    }
                    if (NodeState != NodeState.selected && e.button == 0)
                    {
                        NodeState = NodeState.free;
                        changed = true;
                        return this;
                    }
                    break;

                case EventType.MouseDrag:
                    if (e.button == 0)
                    {
                        if (NodeShape.Contains(e.mousePosition) && NodeState != NodeState.free)
                        {
                            if ((e.delta.magnitude > 0 || NodeState == NodeState.dragged) && NodeState != NodeState.doubleClicked)
                            {
                                NodeState = NodeState.dragged;
                                Drag(e.delta);
                                GUI.changed = true;
                                e.Use();
                                changed = true;
                                return this;
                            }
                            if (NodeState == NodeState.doubleClicked)
                            {
                                if (linkSource != null)
                                {
                                    linkSource.LinkRequester = this;
                                }
                                GUI.changed = true;
                                e.Use();
                                changed = true;
                                return this;
                            }
                        }
                    }
                    break;
            }
            changed = false;
            return this;
        }

        public IEditorNode Drag(Vector2 _pos)
        {
            Rect shape = NodeShape;
            shape.center += _pos;
            editorNodePos = shape.position;
            NodeShape = shape;
            return this;
        }

        public IEditorNode Scale(float scale, float delta, Vector2 mousePos)
        {
            Vector2 displacement = NodeShape.center - mousePos;
            Vector2 center = NodeShape.center;
            displacement.Normalize();
            if (scale > 50 && scale < 200)
                center += displacement * (delta * 10);
            Vector2 newSize = new Vector3(1, NodeShape.size.y / NodeShape.size.x, 0) * scale;
            Vector2 center2 = center;
            var shape = new Rect(NodeShape.position, newSize);
            shape.center = center2;
            NodeShape = shape;
            editorNodePos = NodeShape.position;
            return this;
        }

        private void MoveCmdParams()
        {
            Vector3 destination = EditorGUILayout.Vector3Field("Destination", (Vector3)PrimaryParameters);
            bool waitMoveEnd = EditorGUILayout.Toggle("Wait move end", PrimaryParameters.w > 0);
            PrimaryParameters = new Vector4(destination.x, destination.y, destination.z, waitMoveEnd ? 1 : 0);
        }
        private void ConditionCmdParams()
        {
            primaryParameters.x = (int)((TypeCondition)EditorGUILayout.EnumPopup("Type condition", (TypeCondition)PrimaryParameters.x));
            primaryParameters.y = (int)((ComparaisonOperators)EditorGUILayout.EnumPopup("Comparaison", (ComparaisonOperators)PrimaryParameters.y));
            switch ((TypeCondition)primaryParameters.x)
            {
                case TypeCondition.none:
                    break;
                case TypeCondition.realTime:
                    break;
                case TypeCondition.gameTime:
                    break;
                case TypeCondition.visibility:
                    primaryParameters.y = Mathf.Clamp(primaryParameters.y, 0, 1);
                    break;
                case TypeCondition.state:
                    break;
            }
        }

#endif
        #endregion
    }


    /// <summary>
    /// The Command path in the command sequence.
    /// </summary>
    [System.Serializable]
    public struct CommandPath : IEquatable<CommandPath>
    {
        #region Attributes #########################################################################

        [SerializeField]
        private long timeHash;
        [SerializeField]
        private string label;

        #endregion

        #region Properties #########################################################################

        /// <summary>
        /// The Command Unique id in the sequence.
        /// </summary>
        public DateTime TimeHash { get => DateTime.FromBinary(timeHash); set => timeHash = value.ToBinary(); }

        /// <summary>
        /// The command label
        /// </summary>
        public string Label { get => label; set => label = value; }

        #endregion

        #region Methods #########################################################################

        public CommandPath(DateTime _timeHash)
        {
            timeHash = _timeHash.ToBinary();
            label = "";
        }

        public bool Equals(CommandPath other)
        {
            bool tCompare = TimeHash == other.TimeHash;
            if (tCompare) return label == other.label;
            return tCompare;
        }

        public override bool Equals(object o)
        {
            if (o.GetType() != typeof(CommandPath))
                return false;
            CommandPath other = (CommandPath)o;
            return Equals(other);
        }

        public override int GetHashCode()
        {
            var hashCode = -1529887145;
            hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Label);
            return hashCode;
        }

        public static bool operator ==(CommandPath a, CommandPath b)
        {
            return a.Equals(b);
        }
        public static bool operator !=(CommandPath a, CommandPath b)
        {
            return !a.Equals(b);
        }

        public static CommandPath NullPath
        {
            get => new CommandPath { TimeHash = default(DateTime), Label = null };
        }

        public static CommandPath EntryPath
        {
            get => new CommandPath { TimeHash = default(DateTime), Label = "Entry" };
        }

        public static CommandPath ExitPath
        {
            get => new CommandPath { TimeHash = default(DateTime), Label = "Exit" };
        }

        public static CommandPath BreakPath
        {
            get => new CommandPath { TimeHash = default(DateTime), Label = "Break" };
        }

        #endregion
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

}
