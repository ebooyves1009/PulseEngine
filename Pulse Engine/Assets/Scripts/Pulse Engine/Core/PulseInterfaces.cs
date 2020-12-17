using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace PulseEngine
{
    #region Interfaces <><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>

    /// <summary>
    /// L'interface des datas du systeme.
    /// </summary>
    public interface IData
    {
        DataLocation Location { get; set; }
    }

    /// <summary>
    /// Interface of all nodable editor's data.
    /// </summary>
    public interface IEditorNode
    {
        IEditorNode CreateNode();
        IEditorNode ShowDetails();
        void DrawNode(GUIStyle style);
        IEditorNode ProcessNodeEvents(Event e, IEditorGraph linkSource, out bool changed);
        IEditorNode Drag(Vector2 _pos);
        IEditorNode Scale(float scale, float delta, Vector2 mousePos);
        Rect NodeShape { get; set; }
        NodeState NodeState { get; set; }
        GenericMenu ContextMenu { get; set; }
    }

    /// <summary>
    /// The editor node graph interface.
    /// </summary>
    public interface IEditorGraph
    {
        bool InitializedGraph { get; set; }
        Vector2 GraphPosition { get; set; }
        float GraphScale { get; set; }
        Matrix4x4 TransformMatrix { get; set; }
        IEditorNode LinkRequester { get; set; }
        GenericMenu ContextMenu { get; set; }
        void InitializeGraph();
    }

    /// <summary>
    /// Interface implemented by all entities able to follow a path and reach a destination.
    /// </summary>
    public interface IMovable
    {
        Task<CommandPath> MoveCommand(Command _cmd, CancellationToken ct);
        void MoveTo(Vector3 position);
        Task FollowPath(Vector3[] _path, int priority, float weight);
        event EventHandler OnArrival;
        PathMovingState MovingState { get; set; }
        CancellationTokenSource PathCancellationSource { get; set; }
        int CurrentPathPriority { get; set; }
        void ArrivedAt();
        bool CancelCurrentPath();
    }

    public interface IConditionnable
    {
        bool EvaluateConditions(params object[] parameters);
        CommandPath EvaluateCommandCondition(Command cmd);
    }

    #endregion

}
