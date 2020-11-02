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
            if (GUILayout.Button("Test serialisation"))
            {
                var s = asset.item;
                Serial holder = default;
                var tchindren = s.childrens;
                tchindren.Add(JsonUtility.ToJson(new Serial()));
                s.childrens = tchindren;
                holder = JsonUtility.FromJson<Serial>(tchindren[0]);
                for (int i = 0; i < 10; i++)
                {
                    var stChildern = holder.childrens;
                    stChildern.Add(JsonUtility.ToJson(new Serial()));
                    holder.childrens = stChildern;
                    var t = JsonUtility.FromJson<Serial>(stChildern[0]);
                    holder = t;
                    Debug.Log("Serial N+" + (i + 1));
                }
                asset.item = s;
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


[Serializable]
[CreateAssetMenu(fileName = "ScriptableTest", menuName = "Asset/SR test", order = 1)]
public class TestAsset: ScriptableObject
{
    public Serial item;
}





