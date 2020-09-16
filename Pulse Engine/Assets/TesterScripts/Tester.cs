using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using eWindow = UnityEditor.EditorGUILayout;
using Window = UnityEngine.GUILayout;
using System;
using PulseEngine.Globals;
using PulseEngine.Modules.Anima;
using UnityEngine.Events;
using System.Reflection;

[CreateAssetMenu(fileName = "Data", menuName = "ScriptableObjects/testerData", order = 1)]
public class Tester : ScriptableObject
{
    public List<string> linker = new List<string>();
}






