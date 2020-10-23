using PulseEngine.Datas;
using PulseEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Modules.Localisator;

public class Tester : MonoBehaviour
{
    string text = "";

    private void OnGUI()
    {
        if(GUILayout.Button("Get the first"))
        {
            LocData();
        }
        if(GUILayout.Button(text))
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






