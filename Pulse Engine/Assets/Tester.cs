using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Module.Localisator;
using PulseEngine.Core;
using UnityEditor;

public class Tester : MonoBehaviour
{
    [Range(0,1)]
    public float time;

    public Transform here;

    private Animator animator;

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

    string gotName;

    private void OnGUI()
    {
        if (animator)
        {
            if (GUILayout.Button("kick"))
            {
                animator.CrossFadeInFixedTime("kick", time);
                animator.SetTarget(AvatarTarget.RightFoot, 0.3f);
            }
            if (here)
                here.position = animator.targetPosition;
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


public class EditorTest : EditorWindow
{
    Editor preview;
    GameObject go;
    AnimationClip clip;
    private float time = 0;
    private float speed;

    [MenuItem("Test/Win")]
    public static void ShowWindow()
    {
        var window = GetWindow<EditorTest>();
        window.Show();
    }

    private void OnEnable()
    {
        //previewRender = new PreviewRenderUtility();
        var scene = GetWindow<SceneView>();
        scene.Show();
    }

    private void OnDisable()
    {
        
    }

    private void OnGUI()
    {
        go = EditorGUILayout.ObjectField(go, typeof(GameObject), false) as GameObject;
        clip = EditorGUILayout.ObjectField(clip, typeof(AnimationClip), false) as AnimationClip;

        GUIStyle prevstyle = new GUIStyle();

        //if (go && clip)
        //{
        //    if (!preview)
        //        preview = Editor.CreateEditor(go);
        //    speed = EditorGUILayout.FloatField("speed", speed);
        //    EditorGUILayout.LabelField(time.ToString());
        //    time += Time.deltaTime * speed;
        //    time = time % clip.length;
        //    AnimationMode.StartAnimationMode();
        //    AnimationMode.BeginSampling();
        //    AnimationMode.SampleAnimationClip(go, clip, time);
        //    preview.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(300, 300), prevstyle);
        //    AnimationMode.EndSampling();
        //    AnimationMode.StopAnimationMode();
        //    Repaint();
        //}

        //if (go && clip)
        //{
        //    speed = EditorGUILayout.FloatField("speed", speed);
        //    Editor.DestroyImmediate(preview);
        //    preview = Editor.CreateEditor(go);
        //    //if (!preview || preview.target == null || preview.target != go)
        //    //    preview = Editor.CreateEditor(go);
        //    AnimationMode.StartAnimationMode();
        //    AnimationMode.BeginSampling();
        //    AnimationMode.SampleAnimationClip(go, clip, time);
        //    //preview.DrawPreview(new Rect(10, 100, 300, 300));
        //    preview.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(300, 300), prevstyle);
        //    AnimationMode.EndSampling();
        //    AnimationMode.StopAnimationMode();
        //    preview.ReloadPreviewInstances();
        //    time += Time.deltaTime * speed;
        //    time = time % clip.length;
        //    Repaint();
        //}
    }
}
