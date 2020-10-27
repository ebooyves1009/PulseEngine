using PulseEngine.Datas;
using PulseEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using PulseEngine.Modules.Localisator;


public class Tester : MonoBehaviour
{
    public TestAsset asset;
    string text = "";

    private void OnGUI()
    {
        if (GUILayout.Button("Get the first"))
        {
            LocData();
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
        if (GUILayout.Button(text))
        {
        }
    }

    private async void LocData()
    {
        string t = await LocalisationManager.TextData(new DataLocation { id = 1, globalLocation = 0, localLocation = 0 }, DatalocationField.title);
        string d = await LocalisationManager.TextData(new DataLocation { id = 1, globalLocation = 0, localLocation = 0 }, DatalocationField.description);
        text = t + " || " + d;
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





