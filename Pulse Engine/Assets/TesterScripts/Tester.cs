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
                Queue<Command> historic = new Queue<Command>();

                for(int i = 0; i < 3; i++)
                {
                    var tmp = new CommandList();
                    tmp.targets = new List<Command>();
                    tmp.targets.Add(new Command());
                    t.Children = tmp;
                    Command ancestor = Command.NullCmd;
                    if (i > 0)
                    {
                        ancestor = t;
                    }
                    t = t.Children.targets[0];
                    t.PrimaryParameters = Vector4.one * i;
                    t.Parent = ancestor;
                    if (!ancestor.Equals(Command.NullCmd))
                    {
                        Command f = t.Parent;
                        var v = f.Children;
                        v.targets = new List<Command>();
                        v.targets.Add(t);
                        f.Children = v;
                        t.Parent = f;
                        historic.Enqueue(t.Parent);
                    }
                }

                //while(!t.Parent.Equals(Command.NullCmd))
                //{
                //    var f = t.Parent;
                //}

                t = historic.Dequeue();
                asset.cmd = t;
                historic.Clear();
            }
            if (GUILayout.Button("Test inDepht march"))
            {
                Command t = asset.cmd;
                Debug.Log("asset of cmd parameter x " + t.PrimaryParameters.x);
                for (int i = 0; i < 10; i++)
                {
                    try
                    {
                        Command f = t.Children.targets[0];
                        Debug.Log("found child at " + i + " of parameter x " + f.PrimaryParameters.x);
                        t = f;
                    }
                    catch { break; }
                }
            }
        }
        if (GUILayout.Button("Count: "+spawnCount))
        {
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






