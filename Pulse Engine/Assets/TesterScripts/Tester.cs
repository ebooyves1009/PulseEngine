using PulseEngine.Datas;
using PulseEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Modules.Localisator;

public class Tester : MonoBehaviour
{
    string text = "";

    private async void OnGUI()
    {
        if(GUILayout.Button("Get the first"))
        {
            var locData = await CoreData.GetData<Localisationdata,LocalisationLibrary>(new DataLocation { id = 1, globalLocation = 0, localLocation = 0 });
            if(locData != null)
            {
                text = locData.Title.textField;
            }
        }
        if(GUILayout.Button(text))
        {
        }
    }
}






