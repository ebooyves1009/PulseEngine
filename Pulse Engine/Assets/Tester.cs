using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Module.Localisator;
using PulseEngine.Core;
using UnityEditor;
using eWindow = UnityEditor.EditorGUILayout;
using Window = UnityEngine.GUILayout;
using System;
using PulseEngine.Module.CharacterCreator;
using UnityEngine.AddressableAssets;

public class Tester : MonoBehaviour
{
    [Range(0,1)]
    public float time;

    public Transform here;

    private Animator animator;

    public CharactersLibrary datas;


    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponentInChildren<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        if (animator)
        {
            animator.SetFloat("time", time);
        }
    }

    private void OnGUI()
    {
        //if (animator)
        //{
        //    if (GUILayout.Button("kick"))
        //    {
        //        animator.CrossFadeInFixedTime("kick", time);
        //        animator.SetTarget(AvatarTarget.RightFoot, 0.3f);
        //    }
        //    if (here)
        //        here.position = animator.targetPosition;
        //}

        if (datas && GUILayout.Button("Create Character"))
        {
            if(datas.Characterlist.Count > 0)
            {
                var go = Instantiate(datas.Characterlist[0].Character);
                var animator = go.GetComponent<Animator>();
                if (!animator)
                    animator = go.AddComponent<Animator>();
                if (animator)
                {
                    animator.runtimeAnimatorController = datas.Characterlist[0].AnimatorController;
                    animator.avatar = datas.Characterlist[0].AnimatorAvatar;
                }
            }
        }
        //if (!string.IsNullOrEmpty(gotName))
        //    GUILayout.Label(gotName);
        //if(GUILayout.Button("Get details on 1"))
        //{
        //    gotName = await LocalisationManager.TextData(1, LocalisationManager.DatalocationField.title, (int)PulseCore_GlobalValue_Manager.DataType.CharacterInfos, (int)PulseCore_GlobalValue_Manager.Languages.Francais)+"; " +
        //        await LocalisationManager.TextData(1, LocalisationManager.DatalocationField.infos , (int)PulseCore_GlobalValue_Manager.DataType.CharacterInfos, (int)PulseCore_GlobalValue_Manager.Languages.Francais);
        //}
    }

}

[CustomEditor(typeof(PulseEngine.Module.Anima.AnimaStateMachine))]
public class InspectorTest : Editor
{
    private PulseEngine.Module.Anima.AnimaStateMachine stateMachine;
    private UnityEditor.Animations.AnimatorState state;

    private void OnEnable()
    {
        stateMachine = target as PulseEngine.Module.Anima.AnimaStateMachine;
        state = Selection.activeObject as UnityEditor.Animations.AnimatorState;
        if (state)
        {
            if (state.motion)
            {
                //Debug.Log("Motion name is " + state.motion.name);
                if (stateMachine)
                    stateMachine.currentClipName = state.motion.name;
            }
        }
    }

    public override void OnInspectorGUI()
    {
        base.OnInspectorGUI();
        
    }
}


public class EditorTest : EditorWindow
{
    private UnityEditor.Animations.AnimatorController controller;
    private UnityEditor.Animations.AnimatorState treeState;
    private UnityEditor.Animations.BlendTree tree;
    private Motion motion;

    [MenuItem("Test editor/test")]
    public static void ShowWindow()
    {
        var win = GetWindow<EditorTest>();
        win.Show();
    }

    private void OnEnable()
    {
        controller = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>("Assets/control.controller");
        if (!controller)
        {
            controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/control.controller");
        }
        controller.name = "controller1";
    }

    private void OnGUI()
    {
        if (!controller) return;
        eWindow.LabelField(controller.name);
        motion = (Motion)eWindow.ObjectField(motion,typeof(Motion), false);
        if (Window.Button("Add Layer"))
            controller.AddLayer("New Layer");
        if (Window.Button("Add State"))
            controller.AddMotion(motion);
        if (Window.Button("Add BlendTree"))
            treeState = controller.CreateBlendTreeInController("new tree", out tree);
        if (tree && Window.Button("Add BlendTree child"))
            tree.AddChild(motion);
        if (Window.Button("Add Parameter"))
            controller.AddParameter("New Param", AnimatorControllerParameterType.Int);
        if (Window.Button("Save"))
            AssetDatabase.SaveAssets();
    }
}
