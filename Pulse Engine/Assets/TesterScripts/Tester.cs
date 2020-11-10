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
                Command t = new Command();
                t.Path = new CommandPath
                {
                    label = "Master Command",
                };
                t.Parent = CommandPath.EmptyPath;
                CommandSequence sequence = asset.SQ;
                var sq = sequence.Sequence;
                if (sq == null)
                    sq = new List<Command>();
                sq.Add(t);
                sequence.Sequence = sq;

                for(int i = 0; i < 5; i++)
                {
                    for(int j = 0; j < 3; j++)
                    {
                        Command n = new Command();
                        n.PrimaryParameters = Vector4.one * (i + 1);
                        n.NodePosition = new Vector2((i + 1), j);
                        n.Path = new CommandPath
                        {
                            depth = i + 1,
                            id = j,
                            label = "Command at depth= " + (i + 1) + ", and id= " + j,
                        };
                        t.AddChild(sequence, n);
                    }
                    Debug.Log("Created command: " + t.PrimaryParameters.x);
                    for(int j = 0; j < t.ChildrenCount; j++)
                    {
                        //Debug.Log("\tchild command #" + (j + 1) + ": " + t.GetChild(j).NodePosition + " parented to " + t.GetChild(j).Parent.NodePosition);
                    }
                    t = t.GetChild(sequence);
                }

                asset.SQ = sequence;
            }
            if (GUILayout.Button("Test inDepht march"))
            {
                representation.Clear();
                CommandSequence sequence = asset.SQ;
                int len = sequence.Sequence.Count;
                for(int i = 0; i < len; i++)
                {
                    Debug.Log("Command Label : " + sequence.Sequence[i].Path.label);
                    representation.Add(((Vector3)(sequence.Sequence[i].NodePosition), (Vector3)sequence.Sequence[i].GetParent(sequence).NodePosition));
                }

                //Command t = asset.cmd;
                //Debug.Log("asset of cmd parameter x " + t.PrimaryParameters.x);
                //FindRecursive(t);
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
        Gizmos.DrawCube(representation[0].position, Vector3.one * 0.5f);
        for (int i = 1; i < representation.Count; i++)
        {
            var color = palette[i % palette.Length];
            Gizmos.color = color;
            Gizmos.DrawCube(representation[i].position, Vector3.one * 0.5f);
            Gizmos.color = color;
            Gizmos.DrawLine(representation[i].position, representation[i].parentPos);
        }
    }


    private void FindRecursive(Command c)
    {
        int lenght = c.ChildrenCount;
        //for(int i = 0; i < lenght; i++)
        //{
        //    var ch = c.GetChild(i);
        //    Debug.Log("found child at: " + ch.NodePosition + " parented to: " + ch.Parent.NodePosition);
        //    representation.Add((ch.NodePosition, ch.Parent.NodePosition));
        //    FindRecursive(ch);
        //}
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






