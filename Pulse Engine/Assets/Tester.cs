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
using System.Globalization;
using UnityEditor.Animations;
using UnityEngine.Assertions;
using UnityEngine.Rendering;

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

//[CustomEditor(typeof(PulseEngine.Module.Anima.AnimaStateMachine))]
//public class InspectorTest : Editor
public class InspectorTest : Editor
{
    private PulseEngine.Module.Anima.AnimaStateMachine stateMachine;
    private UnityEditor.Animations.AnimatorState state;

    private void OnEnable()
    {
        //stateMachine = target as PulseEngine.Module.Anima.AnimaStateMachine;
        //state = Selection.activeObject as UnityEditor.Animations.AnimatorState;
        //if (state)
        //{
        //    if (state.motion)
        //    {
        //        //Debug.Log("Motion name is " + state.motion.name);
        //        if (stateMachine)
        //            stateMachine.currentClipName = state.motion.name;
        //    }
        //}
    }

    public override bool HasPreviewGUI()
    {
        return true;
    }

    public override void OnPreviewGUI(Rect r, GUIStyle background)
    {
        base.OnPreviewGUI(r, background);

        //GUI.Label(r, target.name + " is being previewed");
    }

    //public override void OnInspectorGUI()
    //{
    //    base.OnInspectorGUI();

    //}
    public static Vector3 GetRenderableCenterRecurse(GameObject go, int minDepth, int maxDepth)
    {
        Vector3 center = Vector3.zero;
        float renderableCenterRecurse = GetRenderableCenterRecurse(ref center, go, 0, minDepth, maxDepth);
        if (renderableCenterRecurse > 0f)
        {
            return center / renderableCenterRecurse;
        }
        return go.transform.position;
    }
    private static float GetRenderableCenterRecurse(ref Vector3 center, GameObject go, int depth, int minDepth, int maxDepth)
    {
        if (depth > maxDepth)
        {
            return 0f;
        }
        float num = 0f;
        if (depth > minDepth)
        {
            MeshRenderer meshRenderer = go.GetComponent(typeof(MeshRenderer)) as MeshRenderer;
            MeshFilter x = go.GetComponent(typeof(MeshFilter)) as MeshFilter;
            SkinnedMeshRenderer skinnedMeshRenderer = go.GetComponent(typeof(SkinnedMeshRenderer)) as SkinnedMeshRenderer;
            SpriteRenderer spriteRenderer = go.GetComponent(typeof(SpriteRenderer)) as SpriteRenderer;
            if (meshRenderer == null && x == null && skinnedMeshRenderer == null && spriteRenderer == null)
            {
                num = 1f;
                center += go.transform.position;
            }
            else if (meshRenderer != null && x != null)
            {
                if (Vector3.Distance(meshRenderer.bounds.center, go.transform.position) < 0.01f)
                {
                    num = 1f;
                    center += go.transform.position;
                }
            }
            else if (skinnedMeshRenderer != null)
            {
                if (Vector3.Distance(skinnedMeshRenderer.bounds.center, go.transform.position) < 0.01f)
                {
                    num = 1f;
                    center += go.transform.position;
                }
            }
            else if (spriteRenderer != null && Vector3.Distance(spriteRenderer.bounds.center, go.transform.position) < 0.01f)
            {
                num = 1f;
                center += go.transform.position;
            }
        }
        depth++;
        IEnumerator enumerator = go.transform.GetEnumerator();
        try
        {
            while (enumerator.MoveNext())
            {
                Transform transform = (Transform)enumerator.Current;
                num += GetRenderableCenterRecurse(ref center, transform.gameObject, depth, minDepth, maxDepth);
            }
        }
        finally
        {
            IDisposable disposable;
            if ((disposable = (enumerator as IDisposable)) != null)
            {
                disposable.Dispose();
            }
        }
        return num;
    }
    internal static void SetEnabledRecursive(GameObject go, bool enabled)
    {
        Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
        foreach (Renderer renderer in componentsInChildren)
        {
            renderer.enabled = enabled;
        }
    }
}


public class EditorTest : EditorWindow
{
    private UnityEditor.Animations.AnimatorController controller;
    private UnityEditor.Animations.AnimatorState treeState;
    private UnityEditor.Animations.BlendTree tree;
    private Motion motion;
    private InspectorTest inps = new InspectorTest();
    private GameObject go;

    [MenuItem("Test editor/test")]
    public static void ShowWindow()
    {
        var win = GetWindow<EditorTest>();
        win.Show();
    }

    private void OnEnable()
    {
        //controller = AssetDatabase.LoadAssetAtPath<UnityEditor.Animations.AnimatorController>("Assets/control.controller");
        //if (!controller)
        //{
        //    controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/control.controller");
        //}
        //controller.name = "controller1";
    }

    private void OnGUI()
    {
        go = eWindow.ObjectField(go, typeof(GameObject), false) as GameObject;
        if (inps == null)
        {
            inps = new InspectorTest();
        }
        if (go)
        {
            try
            {
                if (!inps.target)
                    inps.Initialize(new[] { go });
                else
                    inps.OnPreviewGUI(GUILayoutUtility.GetAspectRect(16 / 9), null);
            }
            catch
            {
                inps.Initialize(new[] { go });
            }
        }
        //if (!controller) return;
        //eWindow.LabelField(controller.name);
        //motion = (Motion)eWindow.ObjectField(motion,typeof(Motion), false);
        //if (Window.Button("Add Layer"))
        //    controller.AddLayer("New Layer");
        //if (Window.Button("Add State"))
        //    controller.AddMotion(motion);
        //if (Window.Button("Add BlendTree"))
        //    treeState = controller.CreateBlendTreeInController("new tree", out tree);
        //if (tree && Window.Button("Add BlendTree child"))
        //    tree.AddChild(motion);
        //if (Window.Button("Add Parameter"))
        //    controller.AddParameter("New Param", AnimatorControllerParameterType.Int);
        //if (Window.Button("Save"))
        //    AssetDatabase.SaveAssets();
    }
}



