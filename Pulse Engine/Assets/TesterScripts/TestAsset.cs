using PulseEngine;
using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;


public class TestAsset : MonoBehaviour, IMovable
{
    //Soon will be moved to a global variable in the pathfinder manager.
    public float distanceToReach = 1;

    public event EventHandler OnArrival;
    public Vector3 movingPlace = Vector3.negativeInfinity;
    public float speed = 5;
    public float reachDist = 1;
    Animator character;

    public PathMovingState MovingState { get; set; }
    public CancellationTokenSource PathCancellationSource { get; set; }
    public int CurrentPathPriority { get; set; }

    public void ArrivedAt()
    {
        if (OnArrival != null)
        {
            OnArrival.Invoke(gameObject, EventArgs.Empty);
        }
    }

    public Task<CommandPath> MoveCommand(Command _cmd, CancellationToken ct)
    {
        throw new NotImplementedException();
    }

    public void MoveTo(Vector3 position)
    {
        movingPlace = position;
    }

    private Vector3[] CurrentPath;
    private void Update()
    {
        if (MovingState == PathMovingState.followingPath)
        {
            //transform.Translate((movingPlace - transform.position).normalized * speed* 0.1f * Time.deltaTime);
            transform.position += (movingPlace - transform.position).normalized * speed * Time.deltaTime;
            //if (character)
            //    character.transform.rotation = Quaternion.Slerp(character.transform.rotation, Quaternion.LookRotation(movingPlace - character.transform.position), Time.deltaTime * speed);
            if ((movingPlace - transform.position).sqrMagnitude <= Mathf.Pow(reachDist, 2))
            {
                movingPlace = Vector3.negativeInfinity;
                IMovable mover = this;
                mover.ArrivedAt();
            }
        }
        PulseDebug.DrawPath(CurrentPath, Color.red, Color.green);
    }

    private void OnEnable()
    {
        character = GetComponent<Animator>();
        if (PathCancellationSource == null)
            PathCancellationSource = new CancellationTokenSource();
        CurrentPathPriority = -1;
    }

    private void OnDisable()
    {
        CancelCurrentPath();
        if (PathCancellationSource != null)
            PathCancellationSource.Dispose();
    }

    public async Task FollowPath(Vector3[] _path, int priority, float weight)
    {
        if (priority < CurrentPathPriority)
            return;
        if (MovingState == PathMovingState.followingPath && !CancelCurrentPath())
            return;
        //PulseDebug.Log($"{name} started walking...");
        CurrentPathPriority = priority;
        //await Task.Delay(500);
        await Task.Yield();
        await Task.Yield();
        var cpyPath = new List<Vector3>(_path);
        var mover = ((IMovable)this);
        if (character)
        {
            character.SetFloat("MoveSpeed", speed * 1.25f);
            character.Play("Walk", 0);
        }
        MovingState = PathMovingState.followingPath;
        int k = 0;
        do
        {
            var pos = cpyPath[k];
            //Just for debug
            CurrentPath = cpyPath.ToArray();
            //
            if (character)
                character.transform.rotation = Quaternion.LookRotation(pos - character.transform.position);
            mover.MoveTo(pos);
            await Core.WaitPredicate(() => { return (transform.position - pos).sqrMagnitude <= Mathf.Pow(reachDist, 2); }, PathCancellationSource.Token);
            k++;
        } while (cpyPath.Count > k);
        CurrentPath = null;
        if (character)
            character.Play("Idle", 0);
        MovingState = PathMovingState.none;
        CurrentPathPriority = -1;
        PathCancellationSource = new CancellationTokenSource();
        //PulseDebug.Log($"{name} Walked end of path...");
    }

    public bool CancelCurrentPath()
    {
        if (PathCancellationSource == null)
            return false;
        PathCancellationSource.Cancel();
        CurrentPathPriority = -1;
        MovingState = PathMovingState.none;
        //PulseDebug.Log($"path cancelled on {name}...");
        return true;
    }
}