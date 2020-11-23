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

public class Tester : MonoBehaviour
{
    bool busy = false;

    private async void OnGUI()
    {
        if (busy)
            return;
        if (GUILayout.Button("Execute first sequence"))
        {
            using (var source = new CancellationTokenSource())
            {
                var ct = source.Token;
                busy = true;
                await Core.ManagerAsyncMethod(ModulesManagers.Commander, "PlayCommandSequence", new object[] { ct, new DataLocation { id = 1 }, false });
            }
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






