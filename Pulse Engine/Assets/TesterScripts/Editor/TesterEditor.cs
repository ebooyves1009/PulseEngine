using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Modules.Anima;
using UnityEditor;
using System.Reflection;
using System;

public class TesterEditor
{
    private UnityEditor.Animations.AnimatorController controller;
    private UnityEditor.Animations.AnimatorState treeState;
    private UnityEditor.Animations.BlendTree tree;
    private Motion motion;
    private GameObject go;
    private Tester data;

    [MenuItem("Test editor/test")]
    public static void ShowWindow()
    {
        //var win = GetWindow<TesterEditor>();
        //win.Show();
    }

    private void OnEnable()
    {
    }

    private void Update()
    {
        //Repaint();
    }

    public static void Showing()
    {
        Debug.Log("Content");
    }

    private void OnGUI()
    {
        if (data == null)
        {
            data = EditorGUILayout.ObjectField(data, typeof(Tester), false) as Tester;
        }
        else
        {
            if (GUILayout.Button("Add 10000 test"))
            {
                for (int i = 0; i < 10001; i++)
                {
                    data.linker.Add(typeof(TesterEditor).Name + ">" + "Showing");
                }
            }
            if (GUILayout.Button("Test All reflection"))
            {
                DateTime now = DateTime.Now;
                foreach (var link in data.linker)
                {
                    string[] compounds = link.Split('>');
                    string className = compounds[0];
                    string methodname = compounds[1];
                    var Class = TypeDelegator.GetType(className);
                    if(Class != null)
                    {
                        var method = Class.GetMethod(methodname);
                        if(method != null)
                        {
                            method.Invoke(null, null);
                        }
                    }
                }
                Debug.Log("<<< done Reflection in " + (DateTime.Now - now).Milliseconds + "ms >>>");
            }
            if (GUILayout.Button("Test All direct"))
            {
                DateTime now = DateTime.Now;
                foreach (var link in data.linker)
                {
                    Showing();
                }
                Debug.Log("<<< done direct in " + (DateTime.Now - now).Milliseconds + "ms >>>");
            }
        }
    }
}
