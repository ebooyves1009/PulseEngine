using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Module.Localisator;
using PulseEngine.Core;

public class Tester : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    string gotName;

    private async void OnGUI()
    {
        if (!string.IsNullOrEmpty(gotName))
            GUILayout.Label(gotName);
        if(GUILayout.Button("Get details on 1"))
        {
            gotName = await LocalisationManager.TextData(1, LocalisationManager.DatalocationField.title, (int)PulseCore_GlobalValue_Manager.DataType.CharacterInfos, (int)PulseCore_GlobalValue_Manager.Languages.Francais)+"; " +
                await LocalisationManager.TextData(1, LocalisationManager.DatalocationField.infos , (int)PulseCore_GlobalValue_Manager.DataType.CharacterInfos, (int)PulseCore_GlobalValue_Manager.Languages.Francais);
        }
    }
}
