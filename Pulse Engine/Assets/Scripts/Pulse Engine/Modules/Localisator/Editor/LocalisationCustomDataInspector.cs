using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using PulseEngine.Core;


namespace PulseEngine.Module.Localisator.AssetEditor
{

    /// <summary>
    /// Le custom inspector de l'asset de localisation.
    /// </summary>
    [CustomEditor(typeof(LocalisationLibrary))]
    [CanEditMultipleObjects]
    public class LocalisationCustomDataInspector : Editor
    {
        LocalisationLibrary localisationAsset;

        void OnEnable()
        {
            localisationAsset = (LocalisationLibrary)target;
        }

        public override void OnInspectorGUI()
        {
            EditorGUILayout.LabelField("Language", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(localisationAsset.LibraryLanguage.ToString());
            EditorGUILayout.LabelField("Data Type", EditorStyles.boldLabel);
            EditorGUILayout.LabelField(localisationAsset.LibraryDataType.ToString());
            EditorGUILayout.LabelField("Data List : ", EditorStyles.boldLabel);
            if (localisationAsset.LocalizedDatas.Count > 0)
            {
                for (int i = 0; i < localisationAsset.LocalizedDatas.Count; i++)
                    EditorGUILayout.LabelField(localisationAsset.LocalizedDatas[i].Trad_ID.ToString());
            }
            else
                EditorGUILayout.LabelField("''None''");
        }
    }
}