using PulseEngine.Datas;
using PulseEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PulseEngine.Modules.Localisator;
using PulseEngine.Modules.CharacterCreator;
using System.Threading.Tasks;
using PulseEngine.Modules.Commander;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using System.Diagnostics;
using System.Collections.Concurrent;
using System.Linq;

public class Tester : MonoBehaviour, IMovable
{
    bool busy = false;
    CancellationTokenSource source = new CancellationTokenSource();

    private void OnGUI()
    {
        if (busy)
            return;
        if (GUILayout.Button("Execute first sequence"))
        {
            if (Commander.virtualEmitter != this)
                Commander.virtualEmitter = this;
            PulseDebug.Log("virtual emitter is " + Commander.virtualEmitter);
            var ct = source.Token;
            WaitSequenceExecution(ct);
            busy = true;
        }
        //if (GUILayout.Button("Test sorting"))
        //{
        //    List<string> database = new List<string>{ "John", "Michael", "Ambassa", "Mola incur", "Mola excur", "Zamina" };
        //    Stopwatch sw = new Stopwatch();
        //    sw.Start();
        //    var dt = database.AsParallel().OrderBy(s => s);
        //    database = dt.ToList();
        //    //database.Sort((x,y) => { return x.CompareTo(y); });
        //    database.ForEach(s => { PulseDebug.Log(s); });
        //    PulseDebug.Log($"Sorting took {sw.ElapsedMilliseconds} to complete");
        //    sw.Stop();
        //}
    }

    private async Task WaitSequenceExecution(CancellationToken ct)
    {
        await Core.ManagerAsyncMethod(ModulesManagers.Commander, "PlayCommandSequence", new object[] { ct, new DataLocation { id = 1 }, false });
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
        //Gizmos.DrawCube(representation[0].position, Vector3.one * 0.5f);
        //for (int i = 1; i < representation.Count; i++)
        //{
        //    var color = palette[i % palette.Length];
        //    Gizmos.color = color;
        //    Gizmos.DrawCube(representation[i].position, Vector3.one * 0.5f);
        //    Gizmos.color = color;
        //    Gizmos.DrawLine(representation[i].position, representation[i].parentPos);
        //}
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

    private async void LocData()
    {
        using (var source = new CancellationTokenSource())
        {
            CancellationToken ct = source.Token;
            string t = await Localisator.TextData(new DataLocation { id = 1, globalLocation = 0, localLocation = 0 }, DatalocationField.title, ct, true);
            string d = await Localisator.TextData(new DataLocation { id = 1, globalLocation = 0, localLocation = 0 }, DatalocationField.description, ct, true);
        }
        //text = t.Italic() + " \n " + d;
        //string w = await CoreData.ManagerAsyncMethod<string>(ModulesManagers.Localisator, "TextData", new object[] { (object)new DataLocation { id = 1, globalLocation = 0, localLocation = 0 }
        //    , (object)DatalocationField.title,
        //    (object)false });
        //Debug.Log("C");
        //text = w;
    }

    private static dynamic PlayGround()
    {
        return 1 + 1;
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


    public Vector3 movingPlace;
    public float speed = 5;
    public float reachDist = 1;
    public float chkDist = 2;

    private void Update()
    {
        if (movingPlace != Vector3.zero)
        {
            transform.Translate((movingPlace - transform.position).normalized * speed * Time.deltaTime, Space.Self);
            if((movingPlace - transform.position).sqrMagnitude <= Mathf.Pow(reachDist, 2))
            {
                movingPlace = Vector3.zero;
                IMovable mover = this;
                mover.ArrivedAt();
            }
        }
    }

    private void Start()
    {
        //var rootGameO = gameObject.scene.GetRootGameObjects();
        //for(int i = 0; i < rootGameO.Length; i++)
        //{
        //    //CombineMeshes(rootGameO[i]);
        //}
        physicsCheckCenters = new[]
        {
            transform.position,
        };
        Task physicLoop = PhysicUpdate(source.Token);
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






