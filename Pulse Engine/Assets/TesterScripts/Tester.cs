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

public class Tester : MonoBehaviour, IMovable
{
    bool busy = false;
    CancellationTokenSource source = new CancellationTokenSource();
    string title;
    string description;
    public Animator characterAnim;
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
        for(int i = 0; i < path.Count - 1; i++)
        {
            Color c = Color.Lerp(Color.red, Color.green, Mathf.InverseLerp(0, path.Count - 1, i));
            Handles.color = c;
            Handles.DrawLine(path[i], path[i + 1]);
        }
        openList.ForEach(node =>
        {
            Vector3 pos = new Vector3(node.NodeCenter(cell_Size).x, 0, node.NodeCenter(cell_Size).y);
            Handles.Label(new Vector3(node.WorldPosition.x, 0, pos.z), $"G:{node.GCost}; H:{node.HCost}; F:{node.FCost}");
        });
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
        if (!characterAnim)
            return;
        var data = await CoreLibrary.GetData<AnimaData>(new DataLocation { id = 1, dType = DataTypes.Anima, globalLocation = 0, localLocation = 0 }, source.Token);
        if (data == null)
            return;
        if (data.Motion == null)
            return;
        //characterAnim.Play("State", 0);
        var component = characterAnim.GetBehaviour<AnimaStateMachine>();
        if (!component)
            return;
        component.AnimationData = data;
        component.OverrideAnimation(data.Motion, characterAnim);
        PulseDebug.Log(component.name);
        characterAnim.Play("State", 0);
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


    private void Update()
    {
        if (currentCam)
        {
            var ray = currentCam.ScreenPointToRay(new Vector3(Mouse.current.position.x.ReadValue(), Mouse.current.position.y.ReadValue()));
            RaycastHit hit;
            if(Physics.Raycast(ray, out hit))
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
                    if(characterAnim)
                        SearchPath(characterAnim.transform.position, worldPoint);
                }
            }
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
            if (!characterAnim)
                return;
            NativeArray<PathNode> _grid = new NativeArray<PathNode>(grid.NodeList.ToArray(), Allocator.TempJob);
            NativeArray<int> chkTrue = new NativeArray<int>(grid.NodeList.Count, Allocator.TempJob);
            NativeArray<float3> posArray = new NativeArray<float3>(new[] { (float3)worldPoint, (float3)characterAnim.transform.position }, Allocator.TempJob);
            try
            {
                var job = new CheckNodeJob { grid = _grid, cellSize = cell_Size, checkedTrue = chkTrue, posittions = posArray }.Schedule(_grid.Length, 4);
                grid.NodeList = grid.NodeList.ToArray().CheckWalkability(cell_Size, physicMask, 2, job).ToList();
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
                        PulseDebug.Draw2dPolygon(pos, cell_Size * 0.5f, Vector3.up, Color.cyan, 60);
                    }
                    else if (closedList.Contains(node))
                        PulseDebug.Draw2dPolygon(pos, cell_Size * 0.5f, Vector3.up, Color.red, 60);
                    else if (openList.Contains(node))
                        PulseDebug.Draw2dPolygon(pos, cell_Size * 0.5f, Vector3.up, Color.green, 60);
                    else
                    {
                        PulseDebug.Draw2dPolygon(pos, cell_Size * 0.5f, Vector3.up, ckhArray[i] >= 0 ? Color.blue : (node.Walkable ? Color.yellow : Color.black), 60);
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
    List<PathNode> openList = new List<PathNode>();
    List<PathNode> closedList = new List<PathNode>();
    List<Vector3> path = new List<Vector3>();
    PathNode currentNode = PathNode.NullNode;
    PathNode startNode = PathNode.NullNode;
    PathNode endNode = PathNode.NullNode;
    bool searching = false;

    public async Task SearchPath(Vector3 start, Vector3 end)
    {
        if (searching)
            return;
        searching = true;
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
                characterAnim.Play("Search", 0);
                path.Clear();
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
                    if(currentNode == endNode)
                    {
                        PulseDebug.Log($"Reached end...");
                        closedList.Add(currentNode);
                        break;
                    }
                    var ngb = currentNode.GetNeithbors(cell_Size, startNode, endNode)
                        .CheckWalkability(cell_Size, physicMask, 2);
                    bool breakMade = false;
                    for(int j = 0; j < ngb.Length; j++)
                    {
                        var node = ngb[j];
                        if (!grid.NodeList.Contains(node))
                            continue;
                        if (closedList.Contains(node))
                            continue;
                        if (!node.Walkable)
                            continue;
                        var newNode = node.CalculateCost(startNode, endNode, cell_Size);
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
                            if (newNode.FCost < node.FCost) {
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
            }
        }
        finally
        {
            var Ppath = closedList.TracePath();
            for(int i = 0; i < Ppath.Length; i++)
            {
                path.Add(Ppath[i].NodeWorldCenter(cell_Size));
            }
            searching = false;
            TestAsset target = null;
            if (characterAnim && characterAnim.TryGetComponent<TestAsset>(out target))
            {
                //await Task.Delay(500);
                await Task.Yield();
                var cpyPath = new List<PathNode>(Ppath);
                var mover = ((IMovable)target);
                characterAnim.Play("Walk", 0);
                do
                {
                    var pos = new Vector3(cpyPath[0].NodeCenter(cell_Size).x, 0, cpyPath[0].NodeCenter(cell_Size).y);
                    mover.MoveTo(pos);
                    characterAnim.transform.rotation = Quaternion.LookRotation(pos - characterAnim.transform.position);
                    await Core.WaitPredicate(() => { return cpyPath[0].CheckPositionOverlap(cell_Size, characterAnim.transform.position); }, source.Token);
                    cpyPath.RemoveAt(0);
                } while (cpyPath.Count > 0);
                characterAnim.Play("Idle", 0);
                PulseDebug.Log($"Walked end of path...");
            }
        }
    }

    private void Start()
    {
        {
            int gridSize = 10;
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


}

public static class Extensions
{
    public static PathNode CalculateCost(this PathNode n, PathNode from, PathNode to, float nodeSize)
    {
        n.HCost = Mathf.RoundToInt(Mathf.Abs(to.GridPosition.x - n.GridPosition.x) / nodeSize + Mathf.Abs(to.GridPosition.y - n.GridPosition.y) / nodeSize) * 10;
        //n.GCost = from.GCost + Mathf.RoundToInt((new Vector2(Mathf.Abs(from.GridPosition.x - n.GridPosition.x), Mathf.Abs(from.GridPosition.y - n.GridPosition.y)).magnitude / nodeSize) * 10);
        //n.HCost = -n.GCost + Mathf.RoundToInt((new Vector2(Mathf.Abs(to.GridPosition.x - n.GridPosition.x), Mathf.Abs(to.GridPosition.y - n.GridPosition.y)).magnitude / nodeSize) * 10);
        n.GCost = Mathf.RoundToInt(Mathf.Abs(from.GridPosition.x - n.GridPosition.x) / nodeSize + Mathf.Abs(from.GridPosition.y - n.GridPosition.y) / nodeSize) * 10;
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






