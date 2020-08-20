using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEditor.Globals;
using PulseEditor.Modules.Anima;
using PulseEngine.Modules.Anima;
using UnityEditor;

public class TesterEditor : PulseEditorMgr
{
    private UnityEditor.Animations.AnimatorController controller;
    private UnityEditor.Animations.AnimatorState treeState;
    private UnityEditor.Animations.BlendTree tree;
    private Motion motion;
    private GameObject go;

    [MenuItem("Test editor/test")]
    public static void ShowWindow()
    {
        var win = GetWindow<TesterEditor>();
        win.Show();
    }

    private void OnEnable()
    {
    }

    private void Update()
    {
        Repaint();
    }

    private void OnGUI()
    {
        if (GUILayout.Button("Anima Modifier"))
        {
            AnimaEditor.OpenModifier(1, PulseEngine.Modules.AnimaCategory.humanoid, PulseEngine.Modules.AnimaType.Locamotion);
        }
    }
}
