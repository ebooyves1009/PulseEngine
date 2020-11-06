using PulseEngine.Datas;
using PulseEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PulseEngine.Modules.Localisator;
using PulseEngine.Modules.CharacterCreator;
using System.Threading.Tasks;

public class Tester : MonoBehaviour
{
    public TestAsset asset;
    string text = "it is the text <color=red>color1, and <color=blue>color2</color>...</color>";
    int spawnCount = 0;
    List<(Vector3 position, Vector3 parentPos)> representation = new List<(Vector3 position, Vector3 parentPos)>();

    private void OnGUI()
    {
        if (GUILayout.Button("Spawn"))
        {
            //LocData();
            spawnCount++;
            Spawn();
        }
        if (asset != null) {
            if (GUILayout.Button("Test inDepht creation"))
            {
                Command t = asset.cmd;
                //t.Parent = Command.NullCmd;

                for(int i = 0; i < 3; i++)
                {
                    var tmp = new CommandList();
                    tmp.targets = new List<Command>();
                    //
                    Command n = new Command();
                    n.PrimaryParameters = Vector4.one * (i + 1);
                    //
                    //tmp.targets.Add(n);
                    //t.Children = tmp;
                    for(int j = 0; j < 2; j++)
                    {
                        n.NodePosition = new Vector2((i + 1), j);
                        t.AddChild(n);
                    }
                    Debug.Log("Created command: " + t.PrimaryParameters.x);
                    for(int j = 0; j < t.ChildrenCount; j++)
                    {
                        //Debug.Log("\tchild command #" + (j + 1) + ": " + t.GetChild(j).NodePosition + " parented to " + t.GetChild(j).Parent.NodePosition);
                    }
                    t = t.GetChild();
                    //Command ancestor = Command.NullCmd;
                    //if (i > 0)
                    //{
                    //    ancestor = t;
                    //}
                    //t.PrimaryParameters = Vector4.one * i;
                    //t.Parent = ancestor;
                    //if (!ancestor.Equals(Command.NullCmd))
                    //{
                    //    Command f = t.Parent;
                    //    var v = f.Children;
                    //    v.targets = new List<Command>();
                    //    v.targets.Add(t);
                    //    f.Children = v;
                    //    t.Parent = f;
                    //    historic.Enqueue(t.Parent);
                    //}
                }

                //while (!t.Parent.Equals(Command.NullCmd))
                //{
                //    var f = t.Parent;
                //    var tmp = new CommandList();
                //    tmp.targets = new List<Command>();
                //    tmp.targets.Add(t);
                //    f.Children = tmp;
                //    t = f;
                //}

                asset.cmd = t.MasterCommand();
            }
            if (GUILayout.Button("Test inDepht march"))
            {
                representation.Clear();
                Command t = asset.cmd;
                Debug.Log("asset of cmd parameter x " + t.PrimaryParameters.x);
                FindRecursive(t);
                //for (int i = 0; i < 10; i++)
                //{
                //    if (t.Children == CommandList.NullCmdList)
                //        break;
                //    Command f = t.GetChild();
                //    Debug.Log("found child at " + i + " of parameter x " + f.PrimaryParameters.x);
                //    t = f;
                //}
            }
        }
        if (GUILayout.Button("Count: "+spawnCount))
        {
        }
    }

    private void OnDrawGizmos()
    {
        Gizmos.DrawCube(Vector3.zero, Vector3.one * 0.25f);
        for (int i = 0; i < representation.Count; i++)
        {
            Gizmos.color = Color.blue;
            Gizmos.DrawCube(representation[i].position, Vector3.one * 0.25f);
            Gizmos.color = Color.yellow;
            Gizmos.DrawLine(representation[i].position, representation[i].parentPos);
        }
    }


    private void FindRecursive(Command c)
    {
        int lenght = c.ChildrenCount;
        for(int i = 0; i < lenght; i++)
        {
            var ch = c.GetChild(i);
            Debug.Log("found child at: " + ch.NodePosition + " parented to: " + ch.Parent.NodePosition);
            representation.Add((ch.NodePosition, ch.Parent.NodePosition));
            FindRecursive(ch);
        }
    }


    private async void Spawn()
    {
        Vector2 randPos = UnityEngine.Random.insideUnitCircle * 5;
        Vector3 position = new Vector3(randPos.x, 0, randPos.y);
        await CharacterCreator.SpawnCharacter(new DataLocation { id = spawnCount, globalLocation = 0, localLocation = 0 }, position);
    }

    private async void LocData()
    {
        string t = await Localisator.TextData(new DataLocation { id = 1, globalLocation = 0, localLocation = 0 }, DatalocationField.title, true);
        string d = await Localisator.TextData(new DataLocation { id = 1, globalLocation = 0, localLocation = 0 }, DatalocationField.description, true);
        text = t.Italic() + " \n " + d;
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






