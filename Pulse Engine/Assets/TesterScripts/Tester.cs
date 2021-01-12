using PulseEngine;
using System.Collections.Generic;
using UnityEngine;
using System;
using PulseEngine.Modules.Localisator;
using System.Threading.Tasks;
using PulseEngine.Modules.Commander;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using PulseEngine.Modules.Components;
using UnityEngine.InputSystem;
using Unity.Mathematics;
using System.Linq;
using UnityEditor;
using UnityEngine.AddressableAssets;
using Unity.Burst;
using System.Diagnostics;

public class Tester : MonoBehaviour, IMovable
{
    bool busy = false;
    CancellationTokenSource source = new CancellationTokenSource();
    string title;
    string description;
    public Animator[] characterAnims;
    public GameObject character;
    [SerializeField]
    private Vector3 worldPoint;
    [SerializeField]
    private Camera currentCam;

    private void OnGUI()
    {
        if (busy)
            return;
        //if (GUILayout.Button("Execute Spawn"))
        //{
        //    CharacterData(Vector3.zero, Quaternion.identity);
        //}
        if (GUILayout.Button("Experimentation: Adressable sync"))
        {
            //Thread t = new Thread(new ThreadStart(() =>
            //{
            //    TestPlayGround();
            //}));
            //t.Start();
            TestPlayGround();
        }
    }

    private static void TestPlayGround()
    {
        Addressables.LoadAssetAsync<CoreLibrary>("CharacterData_0_3").Completed += h =>
        {
            PulseDebug.Log($"Load done on Main thread#{Thread.CurrentThread.ManagedThreadId}");
            Thread.Sleep(1000);
            int taskDone = 0;
            AutoResetEvent taskWaiter = new AutoResetEvent(false);
            for (int i = 0; i < 10; i++)
                ThreadPool.QueueUserWorkItem(o =>
                {
                    try
                    {
                        //Addressables.LoadAssetAsync<CoreLibrary>("CharacterData_0_3").Completed += hdl =>
                        //{
                        //    PulseDebug.Log($"Load done on thread#{Thread.CurrentThread.ManagedThreadId}");
                        //    if (Interlocked.Increment(ref taskDone) >= 9)
                        //        taskWaiter.Set();
                        //};
                        if (SynchronizationContext.Current != null)
                        {
                            Addressables.LoadAsset<CoreLibrary>("CharacterData_0_3");
                        }
                        if (Interlocked.Increment(ref taskDone) >= 9)
                            taskWaiter.Set();
                    }
                    catch (Exception e)
                    {
                        PulseDebug.LogError($"Error happenned on thread #{Thread.CurrentThread.ManagedThreadId} : {e.Message}");
                        if (Interlocked.Increment(ref taskDone) >= 9)
                            taskWaiter.Set();
                    }
                });
            taskWaiter.WaitOne();
            PulseDebug.Log("All done on bacground threads");
        };
    }

    private async Task WaitSequenceExecution(CancellationToken ct)
    {
        //await Core.ManagerAsyncMethod(ModulesManagers.Commander, "PlayCommandSequence", new object[] { ct, new DataLocation { id = 1 }, false });
        busy = false;
    }

    private void OnDisable()
    {
        if (source != null)
        {
            source.Cancel();
            source.Dispose();
        }
    }


    Color[] palette = new Color[]
    {
        Color.green,
        Color.red,
        Color.yellow,
        Color.blue,
        Color.cyan,
        Color.black,
    };


    private void OnDrawGizmos()
    {
        //OpenList.ForEach(node =>
        //{
        //    Vector3 pos = new Vector3(node.NodeCenter(cell_Size).x, 0, node.NodeCenter(cell_Size).y);
        //    Handles.Label(new Vector3(node.WorldPosition.x, 0, pos.z), $"G:{node.GCost}; H:{node.HCost}; F:{node.FCost}");
        //});
    }


    private void FindRecursive(Command c)
    {
        //for(int i = 0; i < lenght; i++)
        //{
        //    var ch = c.GetChild(i);
        //    Debug.Log("found child at: " + ch.NodePosition + " parented to: " + ch.Parent.NodePosition);
        //    representation.Add((ch.NodePosition, ch.Parent.NodePosition));
        //    FindRecursive(ch);
        //}
    }


    //private async void Spawn()
    //{
    //    Vector2 randPos = UnityEngine.Random.insideUnitCircle * 5;
    //    Vector3 position = new Vector3(randPos.x, 0, randPos.y);
    //    //await CharacterCreator.SpawnCharacter(new DataLocation { id = spawnCount, globalLocation = 0, localLocation = 0 }, position);
    //}

    private async Task LocData()
    {
        using (var source = new CancellationTokenSource())
        {
            CancellationToken ct = source.Token;
            title = await Localisationdata.TextData(new DataLocation { id = 1, globalLocation = 0, localLocation = 0 }, DatalocationField.title, ct, new[] { Color.red, Color.green, Color.blue});
            description = await Localisationdata.TextData(new DataLocation { id = 2, globalLocation = 0, localLocation = 0 }, DatalocationField.description, ct, new[] { Color.red, Color.green, Color.blue });
        }
    }

    private async Task AnimData()
    {
        if (characterAnims == null || characterAnims.Length <= 0)
            return;
        var data = await CoreLibrary.GetData<AnimaData>(new DataLocation { id = 1, dType = DataTypes.Anima, globalLocation = 0, localLocation = 0 }, source.Token);
        if (data == null)
            return;
        if (data.Motion == null)
            return;
        //characterAnim.Play("State", 0);
        var component = characterAnims[0].GetBehaviour<AnimaStateMachine>();
        if (!component)
            return;
        component.AnimationData = data;
        component.OverrideAnimation(data.Motion, characterAnims[0]);
        PulseDebug.Log(component.name);
        characterAnims[0].Play("State", 0);
    }
    private async Task CharacterData(Vector3 position, Quaternion rotation)
    {
        if (!character)
            return;
        var data = await CoreLibrary.GetData<CharacterData>(new DataLocation { dType = DataTypes.Character, id = 1, globalLocation = 0, localLocation = 0 }, source.Token);
        if (data == null)
            return;
        var Char = GameObject.Instantiate(character, position, rotation);
        Char.name = await Localisationdata.TextData(data.TradLocation, DatalocationField.title, source.Token);
    }

    public event EventHandler OnArrival;

    void IMovable.MoveTo(Vector3 position)
    {
        movingPlace = position;
    }

    async Task<CommandPath> IMovable.MoveCommand(Command _cmd, CancellationToken ct)
    {
        //Pre movement stuffs
        Vector3 position = new Vector3(_cmd.PrimaryParameters.x, _cmd.PrimaryParameters.y, _cmd.PrimaryParameters.z);
        bool waitTheEndOfMove = _cmd.PrimaryParameters.w > 0;
        IMovable mover = this;
        try
        {
            mover.MoveTo(position);
            if (waitTheEndOfMove)
            {
                bool arrived = false;
                OnArrival = (o, e) =>
                {
                    arrived = true;
                };
                await Core.WaitPredicate(() => { return arrived; }, ct);
            }
            return _cmd.DefaultOutPut;
        }
        catch (Exception e)
        {
            throw e;
        }
        finally
        {
            //post movement stuffs
        }
    }

    void IMovable.ArrivedAt()
    {
        if (OnArrival != null)
            OnArrival.Invoke(gameObject, EventArgs.Empty);
    }


    public Vector3 movingPlace = Vector3.negativeInfinity;
    public float speed = 5;
    public float reachDist = 1;
    public float chkDist = 2;
    private float cell_Size = 0.5f;


    /// <summary>
    /// The Job made to check if a node is in a grid.
    /// </summary>
    [BurstCompile]
    public struct CheckNodeJob : IJobParallelFor
    {
        [ReadOnly]
        public NativeArray<PathNode> grid;
        [ReadOnly]
        public float cellSize;
        [ReadOnly]
        public NativeArray<float3> posittions;
        [WriteOnly]
        public NativeArray<int> checkedTrue;

        public void Execute(int index)
        {
            PathNode n = grid[index];
            bool chk = false;
            for (int i = 0; i < posittions.Length; i++)
            {
                if (n.CheckPositionOverlap(cellSize, posittions[i]))
                {
                    checkedTrue[index] = index;
                    chk = true;
                    break;
                }
                else
                {
                    checkedTrue[index] = -1;
                }
            }
            //Vector3 pos = new Vector3(n.NodeCenter(cellSize).x, 0, n.NodeCenter(cellSize).y);
            //PulseDebug.Draw2dPolygon(pos, cellSize * 0.5f, Vector3.up, chk ? Color.blue : Color.yellow, 15);
        }
    }


    private async Task Update()
    {
        if (currentCam)
        {
            var ray = currentCam.ScreenPointToRay(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue()));
            RaycastHit hit;
            if (Physics.Raycast(ray, out hit))
            {
                PulseDebug.Draw2dPolygon(hit.point, 1, hit.normal, Color.green);
                worldPoint = hit.point;


                Vector3 k = hit.normal;
                Vector3 i = Vector3.zero;
                Vector3 j = Vector3.zero;
                Vector3.OrthoNormalize(ref k, ref i, ref j);
                PulseDebug.DrawRLine(hit.point, hit.point + i, Color.red);
                PulseDebug.DrawRLine(hit.point, hit.point + j, Color.blue);
                PulseDebug.DrawRLine(hit.point, hit.point + k, Color.green);

                if (Mouse.current.leftButton.wasPressedThisFrame)
                {
                    //CharacterData(worldPoint, Quaternion.LookRotation(j, k));
                    if (characterAnims.Length > 0)
                    {
                        Stopwatch sw = new Stopwatch();
                        sw.Start();
                        for (int a = 0; a < 1/*characterAnims.Length*/; a++)
                        {
                            characterAnims[a].Play("Search", 0);
                            var theMover = characterAnims[a].GetComponent<TestAsset>();
                            if (theMover)
                            {
                                //await SearchPath(characterAnims[a].transform.position, worldPoint, characterAnims[a]);
                                await SearchPathJob(characterAnims[a].transform.position, worldPoint, theMover, grid, source.Token);
                            }
                        }
                        //Parallel.For(0, characterAnims.Length, a =>
                        //{
                        //    characterAnims[a].Play("Search", 0);
                        //    var theMover = characterAnims[a].GetComponent<TestAsset>();
                        //    if (theMover)
                        //    {
                        //        SearchPath(characterAnims[a].transform.position, worldPoint, theMover);
                        //        //SearchPathJob(characterAnims[a].transform.position, worldPoint, theMover);
                        //    }
                        //});
                        sw.Stop();
                        PulseDebug.Log($"Search took {sw.ElapsedMilliseconds} ms for {characterAnims.Length} units");
                    }
                }
            }
            //else
            //    PulseDebug.Log(hit.point);
        }
        if (movingPlace.x != Vector3.negativeInfinity.x)
        {
            transform.Translate((movingPlace - transform.position).normalized * speed * Time.deltaTime, Space.Self);
            if((movingPlace - transform.position).sqrMagnitude <= Mathf.Pow(reachDist, 2))
            {
                movingPlace = Vector3.negativeInfinity;
                IMovable mover = this;
                mover.ArrivedAt();
            }
        }
        //Display path grid
        {
            if (characterAnims == null || characterAnims.Length <= 0)
                return;
            NativeArray<PathNode> _grid = new NativeArray<PathNode>(grid.NodeList.ToArray(), Allocator.TempJob);
            NativeArray<int> chkTrue = new NativeArray<int>(grid.NodeList.Count, Allocator.TempJob);
            List<float3> positionArray = new List<float3>(characterAnims.ToList().Select(character => { return (float3)character.transform.position; }));
            positionArray.Add((float3)worldPoint);
            NativeArray<float3> posArray = new NativeArray<float3>(positionArray.ToArray(), Allocator.TempJob);
            try
            {
                var job = new CheckNodeJob { grid = _grid, cellSize = cell_Size, checkedTrue = chkTrue, posittions = posArray }.Schedule(_grid.Length, 4);
                //grid.NodeList = grid.NodeList.ToArray().CheckWalkability(cell_Size, physicMask, 2, job).ToList();
                job.Complete();
                var ckhArray = chkTrue.ToArray();
                //Parallel.For(0, ckhArray.Length, i =>
                //{
                //    ////if (ckhArray[i] < 0)
                //    ////    return;
                //    //var node = grid[i];
                //    //Vector3 pos = new Vector3(node.NodeCenter(cell_Size).x, 0, node.NodeCenter(cell_Size).y);
                //    //if (node == startNode || node == endNode)
                //    //{
                //    //    PulseDebug.Draw2dPolygon(pos, cell_Size * 0.5f, Vector3.up, Color.cyan, 60);
                //    //}
                //    //else if (closedList.Contains(node))
                //    //    PulseDebug.Draw2dPolygon(pos, cell_Size * 0.5f, Vector3.up, Color.green, 60);
                //    //else if (openList.Contains(node))
                //    //    PulseDebug.Draw2dPolygon(pos, cell_Size * 0.5f, Vector3.up, Color.red, 60);
                //    //else
                //    //{
                //    //    PulseDebug.Draw2dPolygon(pos, cell_Size * 0.5f, Vector3.up, ckhArray[i] >= 0 ? Color.blue : Color.yellow, 60);
                //    //    //PulseDebug.Draw2dPolygon(pos, cell_Size * 0.5f, Vector3.up, Color.yellow, 60);
                //    //}
                //});
                //PulseDebug.Draw2dPolygon(grid.ClosestNode(characterAnim.transform.position, true, physicMask, 2).NodeWorldCenter(cell_Size), cell_Size * 0.5f, Vector3.up, Color.blue, 60);
                for (int i = 0; i < ckhArray.Length; i++)
                {
                    //if (ckhArray[i] < 0)
                    //    return;
                    var node = grid.NodeList[i];
                    Vector3 pos = new Vector3(node.NodeCenter(cell_Size).x, 0, node.NodeCenter(cell_Size).y);
                    if (node == startNode || node == endNode)
                    {
                        PulseDebug.Draw2dPolygon(pos, cell_Size * 0.5f, Vector3.up, Color.cyan, 90);
                    }
                    else if (ClosedList.Contains(node))
                        PulseDebug.Draw2dPolygon(pos, cell_Size * 0.5f, Vector3.up, Color.red, 90);
                    else if (OpenList.Contains(node))
                    {
                        if (node == OpenList[0])
                            PulseDebug.Draw2dPolygon(pos, cell_Size * 0.5f, Vector3.up, Color.magenta, 90);
                        else
                            PulseDebug.Draw2dPolygon(pos, cell_Size * 0.5f, Vector3.up, Color.green, 90);
                    }
                    else
                    {
                        PulseDebug.Draw2dPolygon(pos, cell_Size * 0.5f, Vector3.up, ckhArray[i] >= 0 ? Color.blue : (node.Walkable ? Color.gray : Color.black), 90);
                        //PulseDebug.Draw2dPolygon(pos, cell_Size * 0.5f, Vector3.up, Color.yellow, 60);
                    }
                }
            }
            finally
            {
                _grid.Dispose();
                chkTrue.Dispose();
                posArray.Dispose();
            }
        }
        //grid.ForEach(node =>
        //{
        //    Vector3 pos = new Vector3(node.NodeCenter(cell_Size).x, 0, node.NodeCenter(cell_Size).y);
        //    PulseDebug.Draw2dPolygon(pos, cell_Size * 0.5f, Vector3.up, node.CheckPositionOverlap(cell_Size, worldPoint) ? Color.blue : Color.yellow, 60);
        //});
    }


    PathGrid grid = new PathGrid();
    List<PathNode> OpenList = new List<PathNode>();
    List<PathNode> ClosedList = new List<PathNode>();
    PathNode startNode = PathNode.NullNode;
    PathNode endNode = PathNode.NullNode;
    bool searching = false;

    public async Task SearchPath(Vector3 start, Vector3 end, IMovable character)
    {
        if (character == null || character.MovingState == PathMovingState.searchingPath)
            return;
        character.MovingState = PathMovingState.searchingPath;
        var Pospath = new List<Vector3>();
        PathNode[] path;
        List<PathNode> openList = new List<PathNode>();
        List<PathNode> closedList = new List<PathNode>();
        PathNode currentNode = PathNode.NullNode;
        try
        {
            //Inits checks
            {
                startNode = grid.WorldToNode(start);
                if (startNode == PathNode.NullNode)
                    return;
                endNode = grid.ClosestNode(end, true, physicMask, 2);
                if (endNode == PathNode.NullNode)
                    return;
            }
            //Algorithm start
            {
                openList.Clear();
                closedList.Clear();
                currentNode = startNode;
                currentNode.Parent = new float2(float.NaN, float.NaN);
                openList.Add(currentNode);
                for (int i = 0; i < 500; i++)
                {
                    currentNode = openList[0];
                    //PulseDebug.Log($"current node: F:{currentNode.FCost}, G:{currentNode.GCost}, H:{currentNode.HCost}");
                    closedList.Add(currentNode);
                    openList.Remove(currentNode);
                    if (currentNode == endNode)
                    {
                        //PulseDebug.Log($"{character.name} Reached end...");
                        closedList.Add(currentNode);
                        break;
                    }
                    var ngb = currentNode.GetNeithbors(cell_Size, startNode, endNode)
                        .CheckWalkability(cell_Size, physicMask, 2);
                    bool breakMade = false;
                    for (int j = 0; j < ngb.Length; j++)
                    {
                        var node = ngb[j];
                        if (!grid.NodeList.Contains(node))
                            continue;
                        if (closedList.Contains(node))
                            continue;
                        if (!node.Walkable)
                            continue;
                        var newNode = node.CalculateCost(currentNode, endNode, cell_Size);
                        newNode.Parent = currentNode.GridPosition;
                        //if (node == endNode)
                        //{
                        //    closedList.Add(newNode);
                        //    PulseDebug.Log($"Reached end...");
                        //    breakMade = true;
                        //    break;
                        //}
                        if (openList.Contains(node))
                        {
                            int nodeIndex = openList.FindIndex(n => { return n == node; });
                            if (newNode.FCost < node.FCost)
                            {
                                openList[nodeIndex] = newNode;
                            }
                        }
                        else
                        {
                            openList.Add(newNode);
                        }
                    }
                    if (breakMade)
                        break;
                    openList.Sort();
                    //await Task.Delay(100);
                    await Task.Yield();
                }
                path = new PathNode[closedList.Count];
                closedList.CopyTo(path);

                var Ppath = path.ToList().TracePath();
                for (int i = 0; i < Ppath.Length; i++)
                {
                    Pospath.Add(Ppath[i].NodeWorldCenter(cell_Size));
                }

                character.MovingState = PathMovingState.none;
                if (character != null)
                {
                    //SEnd path to target here
                    character.FollowPath(Pospath.ToArray(), 1, 0);
                }
            }
        }
        catch (Exception e) { throw e; }
        finally
        {
            if (character.MovingState == PathMovingState.searchingPath)
                character.MovingState = PathMovingState.none;
        }
    }


    private async Task SearchPathJob(Vector3 start, Vector3 end, IMovable character, PathGrid grid, CancellationToken ct)
    {
        if (character == null || character.MovingState == PathMovingState.searchingPath)
            return;
        character.MovingState = PathMovingState.searchingPath;
        var Pospath = new List<Vector3>();
        PathNode[] path;
        //Path finding Buffers
        NativeArray<PathNode> gridArray = new NativeArray<PathNode>(grid.NodeList.ToArray(), Allocator.Persistent);
        NativeList<PathNode> openList = new NativeList<PathNode>(1, Allocator.Persistent);
        NativeList<PathNode> closedList = new NativeList<PathNode>(1, Allocator.Persistent);
        NativeArray<PathNode> neigborsArray = new NativeArray<PathNode>(8, Allocator.Persistent);
        PathNode currentNode = PathNode.NullNode;
        //Physic buffers
        NativeArray<SpherecastCommand> obstacleCommands = new NativeArray<SpherecastCommand>(gridArray.Length, Allocator.Persistent);
        NativeArray<SpherecastCommand> groundCommands = new NativeArray<SpherecastCommand>(gridArray.Length, Allocator.Persistent);
        NativeArray<RaycastHit> obstacleResult = new NativeArray<RaycastHit>(obstacleCommands.Length, Allocator.Persistent);
        NativeArray<RaycastHit> groundResult = new NativeArray<RaycastHit>(groundCommands.Length, Allocator.Persistent);

        try
        {
            //Inits checks
            {
                startNode = grid.WorldToNode(start);
                if (startNode == PathNode.NullNode)
                    return;
                endNode = grid.ClosestNode(end, true, physicMask, 2);
                if (endNode == PathNode.NullNode)
                    return;
            }
            //Algorithm start
            {
                //Physic check grid
                JobHandle chkPhysickGrid = gridArray.CheckWalkability(obstacleCommands, groundCommands, obstacleResult, groundResult, grid.CellSize.x, physicMask, 2);
                JobHandle griRefresh = new PathNode.GridPhysicRefresh
                {
                    GridList = gridArray,
                    GroundChks = groundResult,
                    ObstaclesChks = obstacleResult
                }.Schedule(gridArray.Length, 8, chkPhysickGrid);
                griRefresh.Complete();

                PathNode[] g = new PathNode[grid.NodeList.Count];
                gridArray.CopyTo(g);
                grid.NodeList = new List<PathNode>(g);

                currentNode = startNode;
                currentNode.Parent = new float2(float.NaN, float.NaN);
                openList.Add(currentNode);
                for (int i = 0; i < 500; i++)
                {
                    currentNode = openList[0];
                    //PulseDebug.Log($"current node: F:{currentNode.FCost}, G:{currentNode.GCost}, H:{currentNode.HCost}");
                    closedList.Add(currentNode);
                    openList.RemoveAt(0);
                    if (currentNode == endNode)
                    {
                        //PulseDebug.Log($"{character.name} Reached end...");
                        closedList.Add(currentNode);
                        break;
                    }
                    currentNode.GetNeithborsOnGrid(cell_Size, neigborsArray, gridArray);
                    PathNode.NeighborNodesWork(currentNode, startNode, endNode, neigborsArray, closedList, gridArray, openList, cell_Size);
                    openList.Sort();

                    PathNode[] o = new PathNode[openList.Length], c = new PathNode[closedList.Length];
                    openList.AsArray().CopyTo(o);
                    OpenList = new List<PathNode>(o);
                    closedList.AsArray().CopyTo(c);
                    ClosedList = new List<PathNode>(c);

                    PulseDebug.Log($"{OpenList.Count} in Open list...");
                    PulseDebug.Log($"{ClosedList.Count} in close list...");

                    await Task.Delay(250);
                    //await Task.Yield();
                    ct.ThrowIfCancellationRequested();
                }
            }
            path = new PathNode[closedList.Length];
            closedList.AsArray().CopyTo(path);

            var Ppath = path.ToList().TracePath();
            for (int i = 0; i < Ppath.Length; i++)
            {
                Pospath.Add(Ppath[i].NodeWorldCenter(cell_Size));
            }

            character.MovingState = PathMovingState.none;
            if (character != null)
            {
                //SEnd path to target here
                character.FollowPath(Pospath.ToArray(), 1, 0);
            }
        }
        catch (Exception e) { throw e; }
        finally
        {
            //dispose Physic
            obstacleCommands.Dispose();
            groundCommands.Dispose();
            obstacleResult.Dispose();
            groundResult.Dispose();
            //Dispose path finding
            openList.Dispose();
            closedList.Dispose();
            neigborsArray.Dispose();
            gridArray.Dispose();
            if (character.MovingState == PathMovingState.searchingPath)
                character.MovingState = PathMovingState.none;
        }
    }

    private void Start()
    {
        {
            int gridSize = 5;
            grid.CreateQuad(cell_Size, new Vector2(gridSize, gridSize));
        }
        //var rootGameO = gameObject.scene.GetRootGameObjects();
        //for(int i = 0; i < rootGameO.Length; i++)
        //{
        //    //CombineMeshes(rootGameO[i]);
        //}
        //physicsCheckCenters = new[]
        //{
        //    transform.position,
        //};
        //Task physicLoop = PhysicUpdate(source.Token);
    }

    private void CombineMeshes(GameObject staticObj)
    {
        if (!staticObj.isStatic)
            return;
        MeshFilter filter = null;
        MeshRenderer renderer = null;
        MeshCollider collider = null;
        if(!staticObj.TryGetComponent<MeshFilter>(out filter))
        {
            filter = staticObj.AddComponent<MeshFilter>();
        }
        if (filter == null)
            return;
        if(!staticObj.TryGetComponent<MeshRenderer>(out renderer))
        {
            renderer = staticObj.AddComponent<MeshRenderer>();
        }
        if (renderer == null)
            return;
        if(!staticObj.TryGetComponent<MeshCollider>(out collider))
        {
            collider = staticObj.AddComponent<MeshCollider>();
        }
        if (collider == null)
            return;
        MeshFilter[] meshFilters = staticObj.GetComponentsInChildren<MeshFilter>();
        MeshRenderer[] meshRenderers = staticObj.GetComponentsInChildren<MeshRenderer>();
        List<MeshFilter> validsFilters = new List<MeshFilter>();
        List<Material> validsMaterials = new List<Material>();
        for (int k = 0; k < meshFilters.Length; k++)
        {
            if (meshFilters[k].gameObject.isStatic)
            {
                validsFilters.Add(meshFilters[k]);
                validsMaterials.Add(meshRenderers[k].material);
            }
        }
        CombineInstance[] combine = new CombineInstance[validsFilters.Count];

        int i = 0;
        while (i < validsFilters.Count)
        {
            combine[i].mesh = validsFilters[i].sharedMesh;
            combine[i].transform = validsFilters[i].transform.localToWorldMatrix;
            validsFilters[i].gameObject.SetActive(false);
            i++;
        }
        //renderer.materials = validsMaterials.ToArray();
        filter.mesh = new Mesh();
        filter.mesh.CombineMeshes(combine);
        collider.sharedMesh = filter.mesh;
        staticObj.SetActive(true);
        staticObj.transform.localScale = Vector3.one;
        staticObj.transform.rotation = Quaternion.identity;
        staticObj.transform.position = Vector3.zero;
    }

    Collider[] envColliders;
    Vector3[] physicsCheckCenters;
    public LayerMask physicMask;

    public PathMovingState MovingState { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public CancellationTokenSource PathCancellationSource { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }
    public int CurrentPathPriority { get => throw new NotImplementedException(); set => throw new NotImplementedException(); }

    private async Task PhysicUpdate(CancellationToken ct)
    {
        PulseDebug.Log("Physic loop started");
        while (!ct.IsCancellationRequested)
        {
            physicsCheckCenters[0] = transform.position;
            CheckPhysicSurround(physicMask, chkDist);
            await Task.Yield();
        }
        PulseDebug.Log("Physic loop ended");
    }

    private void CheckPhysicSurround(LayerMask mask, float checkDist = 1, JobHandle dependancy = default(JobHandle))
    {
        if (physicsCheckCenters == null || physicsCheckCenters.Length <= 0)
            return;
        Vector3[] castDirections = new Vector3[]
        {
            transform.right,
            transform.forward,
            -transform.forward,
            -transform.right,
        };
        //Create buffers
        NativeArray<RaycastCommand> commands = new NativeArray<RaycastCommand>(physicsCheckCenters.Length * castDirections.Length, Allocator.TempJob);
        NativeArray<RaycastHit> resultsRaycasts = new NativeArray<RaycastHit>(commands.Length, Allocator.TempJob);
        RaycastHit[] envHits;
        try
        {
            //Fill the command buffer
            for (int i = 0; i < commands.Length; i++)
            {
                int j = i % castDirections.Length;
                j = Mathf.Clamp(j, 0, castDirections.Length - 1);
                int k = (i / castDirections.Length);
                commands[i] = new RaycastCommand(physicsCheckCenters[k], castDirections[j], checkDist, mask);
            }
            //Schedule job
            JobHandle rayCastJob = RaycastCommand.ScheduleBatch(commands, resultsRaycasts, 1, dependancy);
            rayCastJob.Complete();
            envHits = resultsRaycasts.ToArray();
        }
        catch(Exception e)
        {
            PulseDebug.LogError("Physic Raycasts fail, cause : \n"+e);
            envHits = new RaycastHit[0];
        }
        finally
        {
            //dispose
            commands.Dispose();
            resultsRaycasts.Dispose();
        }
        //Debug usage
        for (int i = 0; i < envHits.Length; i++)
        {
            int j = i % castDirections.Length;
            j = Mathf.Clamp(j, 0, castDirections.Length - 1);
            int k = (i / castDirections.Length);
            var hit = envHits[i];

            Color c = hit.transform ? Color.green : Color.gray;
            float hitDist = hit.distance;
            float rayLenght = hit.transform ? hitDist : checkDist;
            PulseDebug.DrawRay(physicsCheckCenters[k], castDirections[j] * rayLenght, c);
            if (hit.transform)
                PulseDebug.Draw2dPolygon(hit.point, rayLenght * 0.2f, hit.normal, c);
        }
    }

    public Task FollowPath(Vector3[] _path, int priority, float weight)
    {
        throw new NotImplementedException();
    }

    public void CancelCurrentPath()
    {
        throw new NotImplementedException();
    }

    bool IMovable.CancelCurrentPath()
    {
        throw new NotImplementedException();
    }
}

public static class Extensions
{
    public static PathNode CalculateCost(this PathNode n, PathNode from, PathNode to, float nodeSize)
    {
        float HX = (Mathf.Abs(to.GridPosition.x - n.GridPosition.x) / nodeSize) * 100;
        float HY = (Mathf.Abs(to.GridPosition.y - n.GridPosition.y) / nodeSize) * 100;
        float GX = (Mathf.Abs(n.GridPosition.x - from.GridPosition.x));
        float GY = (Mathf.Abs(n.GridPosition.y - from.GridPosition.y));
        //n.HCost = Mathf.RoundToInt((Mathf.Abs(to.GridPosition.x - n.GridPosition.x) / nodeSize + Mathf.Abs(to.GridPosition.y - n.GridPosition.y) / nodeSize) * 1000);
        n.GCost = from.GCost + ((GX <= 0 ^ GY <= 0) ? 100 : 140);
        n.HCost = Mathf.FloorToInt(Mathf.Sqrt(HX * HX + HY * HY));
        //n.GCost = Mathf.RoundToInt((Mathf.Abs(from.GridPosition.x - n.GridPosition.x) / nodeSize + Mathf.Abs(from.GridPosition.y - n.GridPosition.y) / nodeSize) * 1000);
        return n;
    }

    public static PathNode[] GetNeithbors(this PathNode n, float nodeSize, PathNode from, PathNode to)
    {
        PathNode[] surroundings = new PathNode[] {
            new PathNode{GridPosition = new float2(n.GridPosition.x, n.GridPosition.y + nodeSize), WorldPosition = new float2(n.WorldPosition.x, n.WorldPosition.y + nodeSize)},
            new PathNode{GridPosition = new float2(n.GridPosition.x + nodeSize, n.GridPosition.y), WorldPosition = new float2(n.WorldPosition.x + nodeSize, n.WorldPosition.y)},
            new PathNode{GridPosition = new float2(n.GridPosition.x, n.GridPosition.y - nodeSize), WorldPosition = new float2(n.WorldPosition.x, n.WorldPosition.y - nodeSize)},
            new PathNode{GridPosition = new float2(n.GridPosition.x - nodeSize, n.GridPosition.y), WorldPosition = new float2(n.WorldPosition.x - nodeSize, n.WorldPosition.y)},
            new PathNode{GridPosition = new float2(n.GridPosition.x + nodeSize, n.GridPosition.y + nodeSize), WorldPosition = new float2(n.WorldPosition.x + nodeSize, n.WorldPosition.y + nodeSize)},
            new PathNode{GridPosition = new float2(n.GridPosition.x - nodeSize, n.GridPosition.y - nodeSize), WorldPosition = new float2(n.WorldPosition.x - nodeSize, n.WorldPosition.y - nodeSize)},
            new PathNode{GridPosition = new float2(n.GridPosition.x - nodeSize, n.GridPosition.y + nodeSize), WorldPosition = new float2(n.WorldPosition.x - nodeSize, n.WorldPosition.y + nodeSize)},
            new PathNode{GridPosition = new float2(n.GridPosition.x + nodeSize, n.GridPosition.y - nodeSize), WorldPosition = new float2(n.WorldPosition.x + nodeSize, n.WorldPosition.y - nodeSize)},
        };
        //for (int i = 0; i < surroundings.Length; i++)
        //    surroundings[i] = surroundings[i].CalculateCost(from, to, nodeSize);
        return surroundings;
    }

    public static NativeList<PathNode> GetNeithbors(this PathNode n, float nodeSize, NativeList<PathNode> neighborArray)
    {
        neighborArray.Clear();
        neighborArray.Add(new PathNode { GridPosition = new float2(n.GridPosition.x, n.GridPosition.y + nodeSize), WorldPosition = new float2(n.WorldPosition.x, n.WorldPosition.y + nodeSize) });
        neighborArray.Add(new PathNode { GridPosition = new float2(n.GridPosition.x + nodeSize, n.GridPosition.y), WorldPosition = new float2(n.WorldPosition.x + nodeSize, n.WorldPosition.y) });
        neighborArray.Add(new PathNode { GridPosition = new float2(n.GridPosition.x, n.GridPosition.y - nodeSize), WorldPosition = new float2(n.WorldPosition.x, n.WorldPosition.y - nodeSize) });
        neighborArray.Add(new PathNode { GridPosition = new float2(n.GridPosition.x - nodeSize, n.GridPosition.y), WorldPosition = new float2(n.WorldPosition.x - nodeSize, n.WorldPosition.y) });
        neighborArray.Add(new PathNode { GridPosition = new float2(n.GridPosition.x + nodeSize, n.GridPosition.y + nodeSize), WorldPosition = new float2(n.WorldPosition.x + nodeSize, n.WorldPosition.y + nodeSize) });
        neighborArray.Add(new PathNode { GridPosition = new float2(n.GridPosition.x - nodeSize, n.GridPosition.y - nodeSize), WorldPosition = new float2(n.WorldPosition.x - nodeSize, n.WorldPosition.y - nodeSize) });
        neighborArray.Add(new PathNode { GridPosition = new float2(n.GridPosition.x - nodeSize, n.GridPosition.y + nodeSize), WorldPosition = new float2(n.WorldPosition.x - nodeSize, n.WorldPosition.y + nodeSize) });
        neighborArray.Add(new PathNode { GridPosition = new float2(n.GridPosition.x + nodeSize, n.GridPosition.y - nodeSize), WorldPosition = new float2(n.WorldPosition.x + nodeSize, n.WorldPosition.y - nodeSize) });
        return neighborArray;
    }

    public static void GetNeithborsOnGrid(this PathNode n, float nodeSize, NativeArray<PathNode> neighborArray, NativeArray<PathNode> grid)
    {
        NativeArray<PathNode> surrounder = new NativeArray<PathNode>(new[]{
            new PathNode { GridPosition = new float2(n.GridPosition.x, n.GridPosition.y + nodeSize), WorldPosition = new float2(n.WorldPosition.x, n.WorldPosition.y + nodeSize) },
            new PathNode { GridPosition = new float2(n.GridPosition.x + nodeSize, n.GridPosition.y), WorldPosition = new float2(n.WorldPosition.x + nodeSize, n.WorldPosition.y) },
            new PathNode { GridPosition = new float2(n.GridPosition.x, n.GridPosition.y - nodeSize), WorldPosition = new float2(n.WorldPosition.x, n.WorldPosition.y - nodeSize) },
            new PathNode { GridPosition = new float2(n.GridPosition.x - nodeSize, n.GridPosition.y), WorldPosition = new float2(n.WorldPosition.x - nodeSize, n.WorldPosition.y) },
            new PathNode { GridPosition = new float2(n.GridPosition.x + nodeSize, n.GridPosition.y + nodeSize), WorldPosition = new float2(n.WorldPosition.x + nodeSize, n.WorldPosition.y + nodeSize) },
            new PathNode { GridPosition = new float2(n.GridPosition.x - nodeSize, n.GridPosition.y - nodeSize), WorldPosition = new float2(n.WorldPosition.x - nodeSize, n.WorldPosition.y - nodeSize) },
            new PathNode { GridPosition = new float2(n.GridPosition.x - nodeSize, n.GridPosition.y + nodeSize), WorldPosition = new float2(n.WorldPosition.x - nodeSize, n.WorldPosition.y + nodeSize) },
            new PathNode { GridPosition = new float2(n.GridPosition.x + nodeSize, n.GridPosition.y - nodeSize), WorldPosition = new float2(n.WorldPosition.x + nodeSize, n.WorldPosition.y - nodeSize) }
        }, Allocator.TempJob);
        NativeArray<int2> indexes = new NativeArray<int2>(grid.Length, Allocator.TempJob);
        NativeArray<int> chkList = new NativeArray<int>(grid.Length, Allocator.TempJob);
        try
        {
            grid.PathNodeIndexesOfNoAlloc(surrounder, indexes).Complete();
            for(int i = 0; i < indexes.Length; i++)
            {
                if (indexes[i].x >= 0 && indexes[i].x < neighborArray.Length)
                {
                    neighborArray[indexes[i].x] = grid[indexes[i].y];
                }
            }
        }
        finally
        {
            surrounder.Dispose();
            indexes.Dispose();
            chkList.Dispose();
        }

        //int index = -1;
        ////
        //index = grid.IndexOfNoAlloc<PathNode>(new PathNode { GridPosition = new float2(n.GridPosition.x, n.GridPosition.y + nodeSize), WorldPosition = new float2(n.WorldPosition.x, n.WorldPosition.y + nodeSize) });
        //if (index >= 0)
        //{
        //    neighborArray[0] = grid[index];
        //    PulseDebug.Log($"Added {grid[index]} at 0, from grid item {index}");
        //}
        //else
        //    neighborArray[0] = PathNode.NullNode;
        //index = -1;
        ////
        //index = grid.IndexOfNoAlloc<PathNode>(new PathNode { GridPosition = new float2(n.GridPosition.x + nodeSize, n.GridPosition.y), WorldPosition = new float2(n.WorldPosition.x + nodeSize, n.WorldPosition.y) });
        //if (index >= 0)
        //{
        //    neighborArray[1] = grid[index];
        //    PulseDebug.Log($"Added {grid[index]} at 1, from grid item {index}");
        //}
        //else
        //    neighborArray[1] = PathNode.NullNode;
        //index = -1;
        ////
        //index = grid.IndexOfNoAlloc<PathNode>(new PathNode { GridPosition = new float2(n.GridPosition.x, n.GridPosition.y - nodeSize), WorldPosition = new float2(n.WorldPosition.x, n.WorldPosition.y - nodeSize) });
        //if (index >= 0)
        //{
        //    neighborArray[2] = grid[index];
        //    PulseDebug.Log($"Added {grid[index]} at 2, from grid item {index}");
        //}
        //else
        //    neighborArray[2] = PathNode.NullNode;
        //index = -1;
        ////
        //index = grid.IndexOfNoAlloc<PathNode>(new PathNode { GridPosition = new float2(n.GridPosition.x - nodeSize, n.GridPosition.y), WorldPosition = new float2(n.WorldPosition.x - nodeSize, n.WorldPosition.y) });
        //if (index >= 0)
        //{
        //    neighborArray[3] = grid[index];
        //    PulseDebug.Log($"Added {grid[index]} at 3, from grid item {index}");
        //}
        //else
        //    neighborArray[3] = PathNode.NullNode;
        //index = -1;
        ////
        //index = grid.IndexOfNoAlloc<PathNode>(new PathNode { GridPosition = new float2(n.GridPosition.x + nodeSize, n.GridPosition.y + nodeSize), WorldPosition = new float2(n.WorldPosition.x + nodeSize, n.WorldPosition.y + nodeSize) });
        //if (index >= 0)
        //{
        //    neighborArray[4] = grid[index];
        //    PulseDebug.Log($"Added {grid[index]} at 4, from grid item {index}");
        //}
        //else
        //    neighborArray[4] = PathNode.NullNode;
        //index = -1;
        ////
        //index = grid.IndexOfNoAlloc<PathNode>(new PathNode { GridPosition = new float2(n.GridPosition.x - nodeSize, n.GridPosition.y - nodeSize), WorldPosition = new float2(n.WorldPosition.x - nodeSize, n.WorldPosition.y - nodeSize) });
        //if (index >= 0)
        //{
        //    neighborArray[5] = grid[index];
        //    PulseDebug.Log($"Added {grid[index]} at 5, from grid item {index}");
        //}
        //else
        //    neighborArray[5] = PathNode.NullNode;
        //index = -1;
        ////
        //index = grid.IndexOfNoAlloc<PathNode>(new PathNode { GridPosition = new float2(n.GridPosition.x - nodeSize, n.GridPosition.y + nodeSize), WorldPosition = new float2(n.WorldPosition.x - nodeSize, n.WorldPosition.y + nodeSize) });
        //if (index >= 0)
        //{
        //    neighborArray[6] = grid[index];
        //    PulseDebug.Log($"Added {grid[index]} at 6, from grid item {index}");
        //}
        //else
        //    neighborArray[6] = PathNode.NullNode;
        //index = -1;
        ////
        //index = grid.IndexOfNoAlloc<PathNode>(new PathNode { GridPosition = new float2(n.GridPosition.x + nodeSize, n.GridPosition.y - nodeSize), WorldPosition = new float2(n.WorldPosition.x + nodeSize, n.WorldPosition.y - nodeSize) });
        //if (index >= 0)
        //{
        //    neighborArray[7] = grid[index];
        //    PulseDebug.Log($"Added {grid[index]} at 7, from grid item {index}");
        //}
        //else
        //    neighborArray[7] = PathNode.NullNode;
    }

    public static PathNode[] CheckWalkability(this PathNode[] collection, float nodeSize, LayerMask mask, float checkDist = 1, JobHandle handle = default)
    {
        PathNode[] retCol = new PathNode[collection.Length];
        collection.CopyTo(retCol, 0);
        //Create buffers
        NativeArray<SpherecastCommand> obstacleCommands = new NativeArray<SpherecastCommand>(collection.Length, Allocator.TempJob);
        NativeArray<SpherecastCommand> groundCommands = new NativeArray<SpherecastCommand>(collection.Length, Allocator.TempJob);
        NativeArray<RaycastHit> obstacleResult = new NativeArray<RaycastHit>(obstacleCommands.Length, Allocator.TempJob);
        NativeArray<RaycastHit> groundResult = new NativeArray<RaycastHit>(groundCommands.Length, Allocator.TempJob);
        try
        {
            //Fill the command buffers
            for (int i = 0; i < collection.Length; i++)
            {
                int k = i;
                obstacleCommands[i] = new SpherecastCommand(collection[i].NodeWorldCenter(nodeSize) - Vector3.up * nodeSize, nodeSize * 0.5f, Vector3.up, checkDist, mask);
                groundCommands[i] = new SpherecastCommand(collection[i].NodeWorldCenter(nodeSize) + Vector3.up * nodeSize, nodeSize * 0.45f, -Vector3.up, checkDist, mask);
            }
            //Schedule jobs
            JobHandle groundJob = SpherecastCommand.ScheduleBatch(groundCommands, groundResult, 1, handle);
            JobHandle obstacleJob = SpherecastCommand.ScheduleBatch(obstacleCommands, obstacleResult, 1, groundJob);
            obstacleJob.Complete();
            for(int i = 0; i< collection.Length; i++)
            {
                var n = collection[i];
                n.Walkable = obstacleResult[i].collider == null && groundResult[i].collider != null;
                //Debug.DrawRay(collection[i].NodeWorldCenter(nodeSize) - Vector3.up * nodeSize, Vector3.up * checkDist, envHits[i].collider ? Color.black : Color.white, 10000);
                retCol[i] = n;
            }
        }
        catch (Exception e)
        {
            PulseDebug.LogError("CheckWalkability Raycasts fail, cause : \n" + e);
        }
        finally
        {
            //dispose
            obstacleCommands.Dispose();
            groundCommands.Dispose();
            obstacleResult.Dispose();
            groundResult.Dispose();
        }
        return retCol;
    }
    public static JobHandle CheckWalkability(this NativeArray<PathNode> collection, 
        NativeArray<SpherecastCommand> obstacleCommands, NativeArray<SpherecastCommand> groundCommands,
        NativeArray<RaycastHit> obstacleResult, NativeArray<RaycastHit> groundResult,
        float nodeSize, LayerMask mask, float checkDist = 1, JobHandle handle = default)
    {
        //Fill the command buffers
        for (int i = 0; i < collection.Length; i++)
        {
            int k = i;
            obstacleCommands[i] = new SpherecastCommand(collection[i].NodeWorldCenter(nodeSize) - Vector3.up * nodeSize, nodeSize * 0.5f, Vector3.up, checkDist, mask);
            groundCommands[i] = new SpherecastCommand(collection[i].NodeWorldCenter(nodeSize) + Vector3.up * nodeSize, nodeSize * 0.45f, -Vector3.up, checkDist, mask);
        }
        //Schedule jobs
        JobHandle groundJob = SpherecastCommand.ScheduleBatch(groundCommands, groundResult, 1, handle);
        JobHandle obstacleJob = SpherecastCommand.ScheduleBatch(obstacleCommands, obstacleResult, 1, groundJob);
        return obstacleJob;
    }


    public static NativeList<PathNode> CheckWalkability(this NativeList<PathNode> collection, float nodeSize, LayerMask mask, float checkDist = 1, JobHandle handle = default)
    {
        //Create buffers
        NativeArray<SpherecastCommand> obstacleCommands = new NativeArray<SpherecastCommand>(collection.Length, Allocator.TempJob);
        NativeArray<SpherecastCommand> groundCommands = new NativeArray<SpherecastCommand>(collection.Length, Allocator.TempJob);
        NativeArray<RaycastHit> obstacleResult = new NativeArray<RaycastHit>(obstacleCommands.Length, Allocator.TempJob);
        NativeArray<RaycastHit> groundResult = new NativeArray<RaycastHit>(groundCommands.Length, Allocator.TempJob);
        try
        {
            //Fill the command buffers
            for (int i = 0; i < collection.Length; i++)
            {
                int k = i;
                obstacleCommands[i] = new SpherecastCommand(collection[i].NodeWorldCenter(nodeSize) - Vector3.up * nodeSize, nodeSize * 0.5f, Vector3.up, checkDist, mask);
                groundCommands[i] = new SpherecastCommand(collection[i].NodeWorldCenter(nodeSize) + Vector3.up * nodeSize, nodeSize * 0.45f, -Vector3.up, checkDist, mask);
            }
            //Schedule jobs
            JobHandle groundJob = SpherecastCommand.ScheduleBatch(groundCommands, groundResult, 1, handle);
            JobHandle obstacleJob = SpherecastCommand.ScheduleBatch(obstacleCommands, obstacleResult, 1, groundJob);
            obstacleJob.Complete();
            for(int i = 0; i< collection.Length; i++)
            {
                var n = collection[i];
                n.Walkable = obstacleResult[i].collider == null && groundResult[i].collider != null;
                //Debug.DrawRay(collection[i].NodeWorldCenter(nodeSize) - Vector3.up * nodeSize, Vector3.up * checkDist, envHits[i].collider ? Color.black : Color.white, 10000);
                collection[i] = n;
            }
        }
        catch (Exception e)
        {
            PulseDebug.LogError("CheckWalkability Raycasts fail, cause : \n" + e);
        }
        finally
        {
            //dispose
            obstacleCommands.Dispose();
            groundCommands.Dispose();
            obstacleResult.Dispose();
            groundResult.Dispose();
        }
        return collection;
    }

    public static PathNode[] TracePath(this List<PathNode> closeList)
    {
        if (closeList.Count <= 0)
            throw new Exception("Cannot trace path from empty Node list");
        PathNode currentNode = closeList[closeList.Count - 1];
        List<PathNode> path = new List<PathNode>();
        path.Add(currentNode);
        int index = 0;
        int iterations = 0;
        //for(int i = 0; i < closeList.Count; i++)
        //{
        //    if (currentNode.GCost > closeList[i].GCost)
        //        continue;
        //    currentNode = closeList[i];
        //    path.Add(currentNode);
        //}
        do
        {
            if (iterations >= closeList.Count)
                break;
            index = closeList.FindIndex(n => { return n.GridPosition.Equals(currentNode.Parent); });
            if (index >= 0)
            {
                currentNode = closeList[index];
                path.Add(currentNode);
            }
            if (float.IsNaN(currentNode.Parent.x))
                break;
            iterations++;
        }
        while (index >= 0);
        path.Reverse();
        return path.ToArray();
    }



    /// <summary>
    /// Return an items's indexes in native array.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="item"></param>
    /// <param name="alloc"></param>
    /// <returns></returns>
    public static JobHandle PathNodeIndexesOfNoAlloc(this NativeArray<PathNode> collection, NativeArray<PathNode> items, NativeArray<int2> results, JobHandle dependance = default(JobHandle))
    {
        JobHandle indexJob = new Core.FindIndexJob
        {
            Collection = collection,
            Items = items,
            Results = results,
        }.Schedule(collection.Length, 8, dependance);
        return indexJob;
    }
}


public struct PathNode : IEquatable<PathNode>, IComparable<PathNode>
{
    float2 worldPosition;
    float2 gridPosition;
    float2 parent;
    bool walkable;
    int gCost;
    int hCost;

    public float2 WorldPosition { get => worldPosition; set => worldPosition = value; }
    public float2 GridPosition { get => gridPosition; set => gridPosition = value; }
    public float2 Parent { get => parent; set => parent = value; }
    public int GCost { get => gCost; set => gCost = value; }
    public int HCost { get => hCost; set => hCost = value; }
    public int FCost { get => hCost + gCost; }
    public bool Walkable { get => walkable; set => walkable = value; }

    public float2 NodeCenter(float nodeSize)
    {
        return worldPosition + new float2(1, 1) * 0.5f * nodeSize;
    }
    public Vector3 NodeWorldCenter(float nodeSize)
    {
        var grPos = worldPosition + new float2(1, 1) * 0.5f * nodeSize;
        return new Vector3(grPos.x, 0, grPos.y);
    }

    public bool CheckPositionOverlap(float nodeSize, Vector3 point)
    {
        bool rangeX = point.x >= worldPosition.x && point.x < worldPosition.x + nodeSize;
        bool rangeY = point.z >= worldPosition.y && point.z < worldPosition.y + nodeSize;
        return rangeX && rangeY;
    }

    public int CompareTo(PathNode other)
    {
        bool fcostChk = FCost.CompareTo(other.FCost) == 0;
        if (fcostChk)
            return HCost.CompareTo(other.HCost);
        return FCost.CompareTo(other.FCost);
    }

    public bool Equals(PathNode other)
    {
        return gridPosition.Equals(other.gridPosition);
    }

    public override bool Equals(object obj)
    {
        return base.Equals(obj);
    }

    public override int GetHashCode()
    {
        return base.GetHashCode();
    }

    public override string ToString()
    {
        return base.ToString();
    }

    public static bool operator==(PathNode a, PathNode b)
    {
        return a.Equals(b);
    }
    public static bool operator!=(PathNode a, PathNode b)
    {
        return !a.Equals(b);
    }

    public static PathNode NullNode
    {
        get => new PathNode { gridPosition = new int2(-1, -1) };
    }

    public static void NeighborNodesWork(PathNode node, PathNode start, PathNode end,
        NativeArray<PathNode> neighbors, NativeList<PathNode> closedList, NativeArray<PathNode> grid, NativeList<PathNode> openlist,
        float cellSize, JobHandle dependence = default(JobHandle))
    {
        NativeArray<PathNode> retList = new NativeArray<PathNode>(neighbors.Length, Allocator.TempJob);
        NativeArray<PathNode> ret = new NativeArray<PathNode>(0,Allocator.Temp);

        try
        {
            //PulseDebug.Log("Job schedule");
            JobHandle handle = new NeighborNodesJob
            {
                startNode = start,
                currentNode = node,
                endNode = end,
                cellSize = cellSize,
                GridNodes = grid,
                OpenList = openlist.AsArray(),
                CloseList = closedList.AsArray(),
                neigborList = neighbors,
                ReturnList = retList
            }.Schedule(neighbors.Length, 4, dependence);
            handle.Complete();
            for (int i = 0; i < retList.Length; i++)
            {
                if (retList[i] != PathNode.NullNode)
                    openlist.Add(retList[i]);
            }
            PulseDebug.Log($"Job finnished with {openlist.Length} in result OpenList , and {retList.Length} in results");
        }
        finally
        {
            ret.Dispose();
            retList.Dispose();
        }
    }

    [BurstCompile]
    private struct NeighborNodesJob : IJobParallelFor
    {
        [ReadOnly]
        public PathNode startNode;
        [ReadOnly]
        public PathNode endNode;
        [ReadOnly]
        public float cellSize;
        [ReadOnly]
        public PathNode currentNode;
        [ReadOnly]
        public NativeArray<PathNode> GridNodes;
        [ReadOnly]
        public NativeArray<PathNode> CloseList;

        public NativeArray<PathNode> OpenList;

        public NativeArray<PathNode> neigborList;
        [WriteOnly]
        public NativeArray<PathNode> ReturnList;

        private PathNode node;

        public void Execute(int index)
        {
            node = neigborList[index];
            NativeArray<PathNode> openlistBuffer = new NativeArray<PathNode>(OpenList, Allocator.Temp);
            ReturnList[index] = PathNode.NullNode;
            if (!GridNodes.Contains(node))
            {
                //PulseDebug.Log($"job{index} stop cause node is not in the grid");
                return;
            }
            if (CloseList.Contains(node))
            {
                //PulseDebug.Log($"job{index} stop cause node is already in the close list");
                return;
            }
            if (!node.Walkable)
            {
                //PulseDebug.Log($"job{index} stop cause node is not walkable");
                return;
            }
            var newNode = node.CalculateCost(currentNode, endNode, cellSize);
            //var newNode = node.CalculateCost(startNode, endNode, cellSize);
            newNode.Parent = currentNode.GridPosition;
            if (OpenList.Contains(newNode))
            {
                float2 nodeGridPlace = node.GridPosition;
                int nodeIndex = openlistBuffer.IndexOfNoAlloc(newNode);
                if (newNode.FCost < node.FCost)
                {
                    OpenList[nodeIndex] = newNode;
                }
            }
            else
            {
                ReturnList[index] = newNode;
            }
            openlistBuffer.Dispose();
        }
    }

    [BurstCompile]
    public struct GridPhysicRefresh: IJobParallelFor
    {
        public NativeArray<PathNode> GridList;
        [ReadOnly]
        public NativeArray<RaycastHit> GroundChks;
        [ReadOnly]
        public NativeArray<RaycastHit> ObstaclesChks;

        private PathNode n;

        public void Execute(int index)
        {
            n = GridList[index];
            n.Walkable = ObstaclesChks[index].point == Vector3.zero && GroundChks[index].point != Vector3.zero;
            GridList[index] = n;
        }
    }
}

public class PathGrid
{
    private Vector2 gridSize;
    private Vector2 cellSize;
    private List<PathNode> nodeList = new List<PathNode>();

    public Vector2 GridSize { get => gridSize; set => gridSize = value; }
    public Vector2 CellSize { get => cellSize; set => cellSize = value; }
    public List<PathNode> NodeList { get => nodeList; set => nodeList = value; }
    public void CreateQuad(float _cellSize, Vector2 _maxSize)
    {
        cellSize = new float2(1, 1) * _cellSize;
        gridSize.x = _maxSize.x / cellSize.x;
        gridSize.y = _maxSize.y / cellSize.y;
        for (float i = 0; i < gridSize.x; i += cellSize.x)
        {
            for (float j = 0; j < gridSize.y; j += cellSize.y)
            {
                nodeList.Add(new PathNode { GridPosition = new float2(i, j), WorldPosition = new float2(i - (gridSize.x * 0.5f), j - (gridSize.y * 0.5f)) });
            }
        }
    }

    public PathNode WorldToNode(Vector3 _point)
    {
        float fromX = Mathf.RoundToInt(((_point.x - nodeList[0].WorldPosition.x) / cellSize.x) - cellSize.x * 0.5f);
        float fromY = Mathf.RoundToInt(((_point.z - nodeList[0].WorldPosition.y) / cellSize.y) - cellSize.y * 0.5f);
        float2 gridPos = new float2(fromX * cellSize.x, fromY * cellSize.y);
        int index = nodeList.FindIndex(n => { return n.GridPosition.Equals(gridPos); });
        if (index >= 0)
        {
            return nodeList[index];
        }
        return PathNode.NullNode;
    }

    public PathNode ClosestNode(Vector3 _point, bool checkWalkability = true, int physicmask = 0, float chkDist = 1)
    {
        PathNode n = WorldToNode(_point);
        if (n != PathNode.NullNode)
        {
            if (!checkWalkability)
                return n;
            var nChk = new[] { n }.CheckWalkability(cellSize.x, physicmask, chkDist)[0];
            if (nChk.Walkable)
                return nChk;
        }
        List<PathNode> cpygrid;
        if(checkWalkability)
            cpygrid = new List<PathNode>(nodeList.ToArray().CheckWalkability(cellSize.x, physicmask, chkDist).ToList().Where(node=> { return node.Walkable; }));
        else
            cpygrid = new List<PathNode>(nodeList);
        cpygrid.Sort((n1, n2) =>
        {
            return (_point - n1.NodeWorldCenter(cellSize.x)).sqrMagnitude.CompareTo((_point - n2.NodeWorldCenter(cellSize.x)).sqrMagnitude);
        });
        return cpygrid[0];
    }
}

[Serializable]
public struct Serial
{
    public int ID;
    public List<string> childrens;

    public Serial(int _id)
    {
        ID = _id;
        childrens = new List<string>();
    }
}






