using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using PulseEngine.Globals;
using UILayout = UnityEngine.GUILayout;
using UnityEngine.Assertions;
using UnityEngine.Rendering;


//TODO: Continuer d'implementer en fonction des besoins recurents des fenetre qui en dependent
namespace PulseEditor
{

    namespace Globals
    {
        #region Enums #################################################################################

        /// <summary>
        /// Les differents modes dans lesquels une fenetre d'editeur peut etre ouverte.
        /// </summary>
        public enum EditorMode
        {
            Normal, Selector, ItemEdition, Preview, Node, Group
        }

        #endregion
        #region Class #################################################################################

        /// <summary>
        /// La classe de base pour tous les editeurs du Moteur.
        /// </summary>
        public class PulseEditorMgr : EditorWindow
        {
            #region Constants #################################################################

            /// <summary>
            /// Le nombre de charactere maximal d'une liste.
            /// </summary>
            protected const int LIST_MAX_CHARACTERS = 20;

            /// <summary>
            /// Le nombre de charactere maximal d'une d'un champ texte.
            /// </summary>
            protected const int FIELD_MAX_CHARACTERS = 50;

            /// <summary>
            /// Le chimin d'acces de l'avatrar par defaut.
            /// </summary>
            public const string previewAvatarPath = "Assets/Scripts/Pulse Engine/Core/Res/defaultAvatar.fbx";

            /// <summary>
            /// Le menu dans lequel seront crees les menus des editeurs.
            /// </summary>
            public const string Menu_EDITOR_MENU = "PulseEngine/Module/";

            #endregion

            #region Protected Atrributes ##########################################################################

            /// <summary>
            /// Le mode dans lequel la fenetre a ete ouverte.
            /// </summary>
            protected EditorMode windowOpenMode;

            /// <summary>
            /// Toutes les assets manipulees par le module.
            /// </summary>
            protected List<ScriptableObject> allAssets = new List<ScriptableObject>(); 

            /// <summary>
            /// l'asset selectionne.
            /// </summary>
            protected ScriptableObject asset; 

            /// <summary>
            /// L'asset en cours de modification.
            /// </summary>
            protected ScriptableObject editedAsset; 

            /// <summary>
            /// La data en cours de modification.
            /// </summary>
            protected object editedData;

            /// <summary>
            /// l'index de l'asset dans all assets.
            /// </summary>
            protected int selectAssetIndex;

            /// <summary>
            /// l'index de la data dans la liste des datas.
            /// </summary>
            protected int selectDataIndex = -1;

            /// <summary>
            /// L'id de la data a modifier en mode modification
            /// </summary>
            protected int dataID;

            /// <summary>
            /// Le scope actuel.
            /// </summary>
            protected Scopes currentScope;

            #endregion

            #region Private Atrributes ##########################################################################

            /// <summary>
            /// le nombre de liste a chaque refresh.
            /// </summary>
            private int listViewCount = 0;

            /// <summary>
            /// le nombre de panel scrollables a chaque refresh.
            /// </summary>
            private int scrollPanCount = 0;

            /// <summary>
            /// empeche de set les styles plusieur fois.
            /// </summary>
            private bool multipleStyleSetLock;

            /// <summary>
            /// Les panels crees accompagne de leur vector de position de scroll
            /// </summary>
            private Dictionary<int, Vector2> PanelsScrools = new Dictionary<int, Vector2>();

            /// <summary>
            /// Les Listes crees accompagne de leur vector de position de scroll
            /// </summary>
            private Dictionary<int, Vector2> ListsScrolls = new Dictionary<int, Vector2>();

            #endregion

            #region Proprietes ##########################################################################

            /// <summary>
            /// La taille par defaut des fenetre de l'editeur.
            /// </summary>
            protected Vector2 DefaultWindowSize { get { return new Vector2(500, 900); } }

            #endregion

            #region Styles ##########################################################################

            /// <summary>
            /// le style des items d'une liste.
            /// </summary>
            protected GUIStyle style_listItem;

            /// <summary>
            /// le style des editeurs multi ligne.
            /// </summary>
            protected GUIStyle style_txtArea;

            /// <summary>
            /// le style des groupes.
            /// </summary>
            protected GUIStyle style_group;

            /// <summary>
            /// le style des labels.
            /// </summary>
            protected GUIStyle style_label;

            /// <summary>
            /// le style des selecteur d'objet.
            /// </summary>
            protected GUILayoutOption[] style_objSelect;

            #endregion

            #region Events ######################################################################

            /// <summary>
            /// L'evenement emit a la selection et validation d'un element en mode Selection.
            /// </summary>
            public EventHandler onSelectionEvent;

            #endregion

            #region GuiMethods ##########################################################################

            private void OnEnable()
            {
                minSize = new Vector2(600, 600);
                Focus();
                OnInitialize();
            }

            private void OnGUI()
            {
                StyleSetter();
                listViewCount = 0;
                scrollPanCount = 0;
                OnRedraw();
            }

            private void OnDisable()
            {
                CloseWindow();
            }

            /// <summary>
            /// Appellee au demarrage de la fenetre, a utiliser a la place de OnEnable dans les fenetres heritantes
            /// </summary>
            protected virtual void OnInitialize()
            {

            }

            /// <summary>
            /// Appellee a chaque rafraichissement de la fenetre, a utiliser a la place de onGUI dans les fenetres heritantes
            /// </summary>
            protected virtual void OnRedraw()
            {

            }

            /// <summary>
            /// Pour initialiser les styles.
            /// </summary>
            private void StyleSetter()
            {
                if (multipleStyleSetLock)
                    return;
                //Text Area
                style_txtArea = new GUIStyle(GUI.skin.textArea);
                //list field
                style_listItem = new GUIStyle(GUI.skin.textField);
                style_listItem.onNormal.textColor = Color.blue;
                style_listItem.onHover.textColor = Color.blue;
                style_listItem.onActive.textColor = Color.blue;
                style_listItem.hover.textColor = Color.black;
                style_listItem.normal.textColor = Color.gray;
                style_listItem.clipping = TextClipping.Clip;
                //groupes
                style_group = new GUIStyle(GUI.skin.window);
                style_group.stretchWidth = false;
                style_group.fontStyle = FontStyle.Bold;
                style_group.margin = new RectOffset(8, 8, 5, 8);
                //labels
                style_label = new GUIStyle(GUI.skin.label);
                style_label.stretchWidth = false;
                style_label.fixedWidth = 120;
                style_label.fontStyle = FontStyle.Bold;
                //obj select
                style_objSelect = new[] { GUILayout.Width(150), GUILayout.Height(150) };

                multipleStyleSetLock = true;
            }

            /// <summary>
            /// Un champs texte a caracteres limites.
            /// </summary>
            /// <param name="input"></param>
            /// <param name="style"></param>
            /// <returns></returns>
            protected string LimitText(string input)
            {
                string str = EditorGUILayout.TextArea(input, style_txtArea);
                char[] cutted = new char[FIELD_MAX_CHARACTERS];
                for (int i = 0; i < FIELD_MAX_CHARACTERS; i++)
                    if (str.Length > i)
                        cutted[i] = str[i];
                string ret = new string(cutted);
                return ret;
            }

            /// <summary>
            /// Faire un group d'items
            /// </summary>
            /// <param name="guiFunctions"></param>
            /// <param name="groupTitle"></param>
            protected void GroupGUI(Action guiFunctions, int widht)
            {
                GUILayout.BeginVertical("", style_group, new[] { GUILayout.Width(widht) });
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical("GroupBox");
                if (guiFunctions != null)
                    guiFunctions.Invoke();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            /// <summary>
            /// Faire un group d'items sans le style d'interieur
            /// </summary>
            /// <param name="guiFunctions"></param>
            /// <param name="groupTitle"></param>
            protected void GroupGUInoStyle(Action guiFunctions, int width)
            {
                GUILayout.BeginVertical("", style_group, new[] { GUILayout.Width(width) });
                if (guiFunctions != null)
                    guiFunctions.Invoke();
                GUILayout.EndVertical();
            }

            /// <summary>
            /// Faire un group d'items
            /// </summary>
            /// <param name="guiFunctions"></param>
            /// <param name="groupTitle"></param>
            protected void GroupGUI(Action guiFunctions, string groupTitle = "", int height = 0)
            {
                List<GUILayoutOption> options = new List<GUILayoutOption>();
                if (height > 0)
                    options.Add(GUILayout.Height(height));
                GUILayout.BeginVertical(groupTitle, style_group, options.ToArray());
                GUILayout.BeginHorizontal();
                GUILayout.BeginVertical("GroupBox");
                if (guiFunctions != null)
                    guiFunctions.Invoke();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            /// <summary>
            /// Faire un group d'items sans le style d'interieur
            /// </summary>
            /// <param name="guiFunctions"></param>
            /// <param name="groupTitle"></param>
            protected void GroupGUInoStyle(Action guiFunctions, string groupTitle = "", int height = 0)
            {
                List<GUILayoutOption> options = new List<GUILayoutOption>();
                if (height > 0)
                    options.Add(GUILayout.Height(height));
                GUILayout.BeginVertical(groupTitle, style_group, options.ToArray());
                if (guiFunctions != null)
                    guiFunctions.Invoke();
                GUILayout.EndVertical();
            }

            /// <summary>
            /// Panel, generalement en bas de fenetre conteneant le plus souvent les bouttons 'save' et 'cancel'
            /// </summary>
            /// <param name="actionButtons"></param>
            protected void SaveCancelPanel(params KeyValuePair<string, Action>[] actionButtons)
            {
                GroupGUInoStyle(() =>
                {
                    GUILayout.BeginHorizontal();
                    for (int i = 0; i < actionButtons.Length; i++)
                    {
                        if (GUILayout.Button(actionButtons[i].Key)) { if (actionButtons[i].Value != null) actionButtons[i].Value.Invoke(); }
                    }
                    GUILayout.EndHorizontal();
                }, "", 50);
            }

            /// <summary>
            /// Un panel classique de sauvegarde.
            /// </summary>
            /// <param name="_toSave"></param>
            /// <param name="_whereSave"></param>
            protected void SaveBarPanel(UnityEngine.Object _toSave, UnityEngine.Object _whereSave, Action SelectAction = null)
            {
                switch (windowOpenMode)
                {
                    case EditorMode.Normal:
                        SaveCancelPanel(new[] {
                        new KeyValuePair<string, Action>("Save & Close", ()=> { SaveAsset(_toSave, _whereSave); Close(); }),
                        new KeyValuePair<string, Action>("Close", ()=> { if(EditorUtility.DisplayDialog("Warning", "The Changes you made won't be saved.\n Proceed?","Yes","No")) Close();})
                    });
                        break;
                    case EditorMode.Selector:
                        SaveCancelPanel(new[] {
                        new KeyValuePair<string, Action>("Select", ()=> { if(SelectAction != null) SelectAction.Invoke(); Close(); }),
                        new KeyValuePair<string, Action>("Cancel", ()=> { if(EditorUtility.DisplayDialog("Warning", "The Selection you made won't be saved.\n Proceed?","Yes","No")) Close();})
                    });
                        break;
                    case EditorMode.ItemEdition:
                        SaveCancelPanel(new[] {
                        new KeyValuePair<string, Action>("Save", ()=> { SaveAsset(_toSave, _whereSave);}),
                        new KeyValuePair<string, Action>("Close", ()=> { if(EditorUtility.DisplayDialog("Warning", "The Changes you made won't be saved.\n Proceed?","Yes","No")) Close();})
                    });
                        break;
                    case EditorMode.Preview:
                        break;
                    case EditorMode.Node:
                        break;
                    case EditorMode.Group:
                        break;
                }
            }

            /// <summary>
            /// Faire une liste d'elements, et renvoyer l'element selectionne.
            /// </summary>
            /// <param name="listID"></param>
            /// <param name="content"></param>
            protected int ListItems(int selected = -1, params GUIContent[] content)
            {
                listViewCount++;
                Vector2 scroolPos = Vector2.zero;
                if (ListsScrolls == null)
                    ListsScrolls = new Dictionary<int, Vector2>();
                if (ListsScrolls.ContainsKey(listViewCount))
                    scroolPos = ListsScrolls[listViewCount];
                else
                    ListsScrolls.Add(listViewCount, scroolPos);
                scroolPos = GUILayout.BeginScrollView(scroolPos);
                GUILayout.BeginVertical();
                int sel = GUILayout.SelectionGrid(selected, content, 1, style_listItem);
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
                ListsScrolls[listViewCount] = scroolPos;
                return sel;
            }

            /// <summary>
            /// Faire une Grille d'elements, et renvoyer l'element selectionne.
            /// </summary>
            /// <param name="listID"></param>
            /// <param name="content"></param>
            protected int GridItems(int selected = -1, int xSize = 2, params GUIContent[] content)
            {
                listViewCount++;
                Vector2 scroolPos = Vector2.zero;
                if (ListsScrolls.ContainsKey(listViewCount))
                    scroolPos = ListsScrolls[listViewCount];
                else
                    ListsScrolls.Add(listViewCount, scroolPos);
                scroolPos = GUILayout.BeginScrollView(scroolPos);
                GUILayout.BeginVertical();
                int sel = GUILayout.SelectionGrid(selected, content, xSize);
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
                ListsScrolls[listViewCount] = scroolPos;
                return sel;
            }

            /// <summary>
            /// faire un panel scroolable verticalement.
            /// </summary>
            /// <param name="guiFunctions"></param>
            /// <param name="groupTitle"></param>
            protected void ScrollablePanel(Action guiFunctions = null, bool fixedSize = false)
            {
                scrollPanCount++;
                Vector2 scroolPos = Vector2.zero;
                if (PanelsScrools == null)
                    PanelsScrools = new Dictionary<int, Vector2>();
                if (PanelsScrools.ContainsKey(scrollPanCount))
                    scroolPos = PanelsScrools[scrollPanCount];
                else
                    PanelsScrools.Add(scrollPanCount, scroolPos);
                var options = new[] { GUILayout.MinWidth(100), GUILayout.Width(300) };
                scroolPos = GUILayout.BeginScrollView(scroolPos, fixedSize ? options : null);
                GUILayout.BeginVertical();
                if (guiFunctions != null)
                    guiFunctions.Invoke();
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
                PanelsScrools[scrollPanCount] = scroolPos;
            }

            /// <summary>
            /// Le panel de selection de scope.
            /// </summary>
            protected void ScopeSelector()
            {
                GroupGUInoStyle(() =>
                {
                    currentScope = (Scopes)EditorGUILayout.EnumPopup(currentScope);
                }, "Scope Select", 35);
            }

            #endregion

            #region Methods #############################################################################

            /// <summary>
            /// Pour sauvegarder des changements effectues sur un asset clone.
            /// </summary>
            /// <param name="edited"></param>
            /// <param name="loaded"></param>
            protected void SaveAsset(UnityEngine.Object edited, UnityEngine.Object loaded)
            {
                EditorUtility.CopySerialized(edited, loaded);
                AssetDatabase.SaveAssets();
            }

            /// <summary>
            /// ferme la fenetre
            /// </summary>
            protected virtual void CloseWindow()
            {
                onSelectionEvent = delegate { };
                EditorUtility.UnloadUnusedAssetsImmediate();
            }

            #endregion

            #region utils #########################################################################

            /// <summary>
            /// Return a texture colored.
            /// </summary>
            /// <param name="col"></param>
            /// <returns></returns>
            public static Texture2D ColorToTexture(Color col)
            {
                return default;
            }

            #endregion
        }

        /// <summary>
        /// Fait une previsualisation.
        /// </summary>
        public class Previewer 
        {

            #region Attributes ###################################################################################

            /// <summary>
            /// The playback time.
            /// </summary>
            private float playBackTime;

            /// <summary>
            /// The playback motion.
            /// </summary>
            private Motion playBackMotion;

            /// <summary>
            /// the target to render.
            /// </summary>
            private GameObject target;

            /// <summary>
            /// the target's accessories to render.
            /// </summary>
            private Dictionary<GameObject,(HumanBodyBones bone, Vector3 offset)> accesories;

            /// <summary>
            /// arrow indicator
            /// </summary>
            private GameObject directionArrow;

            /// <summary>
            /// root indicator
            /// </summary>
            private GameObject rootGameObject;

            /// <summary>
            /// The grond plane mesh.
            /// </summary>
            private GameObject floorPlane;

            /// <summary>
            /// The target's animator.
            /// </summary>
            private Animator targetAnimator;

            /// <summary>
            /// The target's animator runtimeController.
            /// </summary>
            private UnityEditor.Animations.AnimatorController rtController;

            /// <summary>
            /// The target's animator runtimeController's state.
            /// </summary>
            private UnityEditor.Animations.AnimatorState animState;

            /// <summary>
            /// Active when previwe is playing anim.
            /// </summary>
            private bool isPlaying;

            /// <summary>
            /// the preview renderer.
            /// </summary>
            private PreviewRenderUtility previewRenderer;

            /// <summary>
            /// the floor texture.
            /// </summary>
            private Texture2D floorTexture;

            /// <summary>
            /// the floor material.
            /// </summary>
            private Material floorMaterial;

            /// <summary>
            /// the shadow mask material.
            /// </summary>
            private Material shadowMaskMaterial;

            /// <summary>
            /// the shadowPlane material.
            /// </summary>
            private Material shadowPlaneMaterial;

            /// <summary>
            /// the zooming factor
            /// </summary>
            private float zoomFactor = 3;

            /// <summary>
            /// the pan angle around target.
            /// </summary>
            private float panAngle = 50;

            /// <summary>
            /// the tilt angle around target.
            /// </summary>
            private float tiltAngle = 50;

            /// <summary>
            /// usefull for positionning Go in scene.
            /// </summary>
            private bool nextTargetIsForward;

            /// <summary>
            /// the target's scale.
            /// </summary>
            private float targetScale = 1;

            /// <summary>
            /// the preview cam pivot offset.
            /// </summary>
            private Vector3 pivotOffset;

            /// <summary>
            /// 
            /// </summary>
            private GameObject cameraPivot;

            /// <summary>
            /// the view tool used right now
            /// </summary>
            private ViewTool viewTool = ViewTool.None;

            #endregion

            #region public Methods #################################################################################################

            /// <summary>
            /// Render the preview.
            /// </summary>
            /// <param name="_motion"></param>
            /// <param name="_target"></param>
            public float Previsualize(Motion _motion, GameObject _target = null, params (GameObject go, HumanBodyBones bone, Vector3 offset)[] accessories)
            {
                if(!_motion)
                {
                    Reset();
                    RenderNull();
                    return 0;
                }
                if (Initialize(_motion, _target, accessories))
                {
                    //pivotOffset = EditorGUILayout.Vector3Field("Cam pos", pivotOffset);
                    targetScale = EditorGUILayout.FloatField("Scale", targetScale);
                    zoomFactor = EditorGUILayout.FloatField("distance", zoomFactor);
                    panAngle = EditorGUILayout.FloatField("Pan", panAngle);
                    tiltAngle = EditorGUILayout.FloatField("tilt", tiltAngle);
                    pivotOffset.y = EditorGUILayout.FloatField("Height", pivotOffset.y);
                    playBackTime = EditorGUILayout.FloatField("PlayBackTime", playBackTime);
                    if (rtController && animState)
                    {
                        if (GUILayout.Button("Play"))
                        {
                            targetAnimator.Play(animState.name);
                        }
                    }
                    try { EditorGUILayout.LabelField(targetAnimator.avatar.name); } catch { EditorGUILayout.LabelField("No Avatar"); }
                    try { EditorGUILayout.LabelField(targetAnimator.runtimeAnimatorController.name); } catch { EditorGUILayout.LabelField("No RT controller"); }
                    var r = GUILayoutUtility.GetAspectRect(16f / 9);
                    Rect rect2 = r;
                    rect2.yMin += 20f;
                    rect2.height = Mathf.Max(rect2.height, 64f);
                    int controlID = GUIUtility.GetControlID("Preview".GetHashCode(), FocusType.Passive, rect2);
                    Event current = Event.current;
                    EventType typeForControl = current.GetTypeForControl(controlID);
                    if (typeForControl == EventType.Repaint)
                    {
                        RenderPreview(r);
                        //previewRenderer.EndAndDrawPreview(rect2);
                    }
                    //AvatarTimeControlGUI(rect);
                    int controlID2 = GUIUtility.GetControlID("Preview".GetHashCode(), FocusType.Passive);
                    typeForControl = current.GetTypeForControl(controlID2);
                    HandleViewTool(Event.current, typeForControl, 0, r);
                    //DoAvatarPreviewDrag(current, typeForControl);
                    //HandleViewTool(current, typeForControl, controlID2, rect2);
                    //DoAvatarPreviewFrame(current, typeForControl, rect2);

                }
                else
                    RenderNull();

                return playBackTime;
            }

            /// <summary>
            /// Close the rendrepreview
            /// </summary>
            public void Destroy()
            {
                Reset();
            }

            #endregion

            #region private Methods #################################################################################################


            #region Camera movements #####################################################


            /// <summary>
            /// Handle the mouse up event
            /// </summary>
            /// <param name="evt"></param>
            /// <param name="id"></param>
            private void HandleMouseUp(Event evt, int id)
            {
                if (GUIUtility.hotControl == id)
                {
                    viewTool = ViewTool.None;
                    GUIUtility.hotControl = 0;
                    EditorGUIUtility.SetWantsMouseJumping(0);
                    viewTool = ViewTool.None;
                    evt.Use();
                }
            }

            /// <summary>
            /// Handle the mouse down event.
            /// </summary>
            /// <param name="evt"></param>
            /// <param name="id"></param>
            /// <param name="previewRect"></param>
            private void HandleMouseDown(Event evt, int id, Rect previewRect)
            {
                if (viewTool != 0 && previewRect.Contains(evt.mousePosition))
                {
                    EditorGUIUtility.SetWantsMouseJumping(1);
                    if(evt.button == 0)
                    {
                        viewTool = ViewTool.Orbit;
                    }else if(evt.button == 2)
                    {
                        viewTool = ViewTool.Pan;
                    }
                    evt.Use();
                    GUIUtility.hotControl = id;
                }
            }

            /// <summary>
            /// handle the mouse drag
            /// </summary>
            /// <param name="evt"></param>
            /// <param name="id"></param>
            /// <param name="previewRect"></param>
            private void HandleMouseDrag(Event evt, int id, Rect previewRect)
            {
                if (!(previewRenderer == null) && GUIUtility.hotControl == id)
                {
                    switch (viewTool)
                    {
                        case ViewTool.Orbit:
                            DoAvatarPreviewOrbit(evt, previewRect);
                            break;
                        case ViewTool.Pan:
                            DoAvatarPreviewPan(evt, previewRect);
                            break;
                        case ViewTool.Zoom:
                            DoAvatarPreviewZoom(evt, (0f - HandleUtility.niceMouseDeltaZoom) * ((!evt.shift) ? 0.5f : 2f));
                            break;
                        default:
                            break;
                    }
                }
            }

            /// <summary>
            /// Pan
            /// </summary>
            /// <param name="evt"></param>
            private void DoAvatarPreviewPan(Event evt, Rect previewRect)
            {
                Vector2 camPivot = new Vector2(0, -pivotOffset.y);
                camPivot -= evt.delta * ((!evt.shift) ? 1 : 3) / Mathf.Min(previewRect.width, previewRect.height) * 12f;
                //camPivot.y = Mathf.Clamp(camPivot.y, 0, 2);
                pivotOffset.y = -camPivot.y;
                pivotOffset.y = Mathf.Clamp(pivotOffset.y, 0, 2);
                evt.Use();
            }


            /// <summary>
            /// Handle the view tool
            /// </summary>
            /// <param name="evt"></param>
            /// <param name="eventType"></param>
            /// <param name="id"></param>
            /// <param name="previewRect"></param>
            protected void HandleViewTool(Event evt, EventType eventType, int id, Rect previewRect)
            {
                switch (eventType)
                {
                    case EventType.MouseMove:
                    case EventType.KeyDown:
                    case EventType.KeyUp:
                        break;
                    case EventType.ScrollWheel:
                        DoAvatarPreviewZoom(evt, HandleUtility.niceMouseDeltaZoom * ((!evt.shift) ? 0.5f : 2f));
                        break;
                    case EventType.MouseDown:
                        HandleMouseDown(evt, id, previewRect);
                        break;
                    case EventType.MouseUp:
                        HandleMouseUp(evt, id);
                        break;
                    case EventType.MouseDrag:
                        HandleMouseDrag(evt, id, previewRect);
                        break;
                }
            }

            /// <summary>
            /// Orbit around
            /// </summary>
            /// <param name="evt"></param>
            /// <param name="previewRect"></param>
            public void DoAvatarPreviewOrbit(Event evt, Rect previewRect)
            {
                Vector2 camAxis = new Vector2(panAngle, -tiltAngle);
                camAxis -= evt.delta * ((!evt.shift) ? 1 : 3) / Mathf.Min(previewRect.width, previewRect.height) * 140f;
                camAxis.y = Mathf.Clamp(camAxis.y, -90f, 90f);
                panAngle = camAxis.x;
                tiltAngle = -camAxis.y;
                evt.Use();
            }

            /// <summary>
            /// zoom
            /// </summary>
            /// <param name="evt"></param>
            /// <param name="delta"></param>
            public void DoAvatarPreviewZoom(Event evt, float delta)
            {
                float num = (0f - delta) * 0.05f;
                zoomFactor += zoomFactor * num;
                zoomFactor = Mathf.Max(zoomFactor, targetScale / 10f);
                evt.Use();
            }

            #endregion

            /// <summary>
            /// Render the preview.
            /// </summary>
            private void RenderPreview(Rect r)
            {
                //TODO: Render the preview here.
                #region approach 1
                //SphericalHarmonicsL2 ambientProbe = RenderSettings.ambientProbe;
                //previewRenderer.BeginPreview(r, GUIStyle.none);
                //Vector3 rootPos = Vector3.zero;
                //Quaternion targetRot;
                //Vector3 targetPosition;
                //Quaternion arrowRot;
                //if (targetAnimator.isHuman)
                //{
                //    targetRot = targetAnimator.rootRotation;
                //    targetPosition = targetAnimator.rootPosition;
                //    arrowRot = targetAnimator.bodyRotation;
                //}else if (targetAnimator.hasRootMotion)
                //{
                //    targetRot = targetAnimator.rootRotation;
                //    targetPosition = targetAnimator.rootPosition;
                //    arrowRot = Quaternion.identity;
                //}
                //else
                //{
                //    targetRot = Quaternion.identity;
                //    targetPosition = Vector3.zero;
                //    arrowRot = Quaternion.identity;
                //}
                ////SetupPreviewLightingAndFx(ambientProbe);
                //Vector3 forward = arrowRot * Vector3.forward;
                //forward[1] = 0;
                //Quaternion arrowRotation = Quaternion.LookRotation(forward);
                //Vector3 arrowPosition = targetPosition;
                //PositionPreviewObjects(targetRot, targetPosition, arrowRotation, targetRot, targetPosition, arrowPosition, targetScale);
                //Quaternion referenceRot = Quaternion.identity;
                //Vector3 referencePos = target.transform.position;
                //Matrix4x4 outterMatrix;
                //RenderTexture renderTex =
                //    RenderPreviewShadowmap(previewRenderer.lights[0], 128, targetAnimator.bodyPosition, Vector3.zero, out outterMatrix);

                //previewRenderer.camera.nearClipPlane = 0.5f * zoomFactor;
                //previewRenderer.camera.farClipPlane = 100f * targetScale;
                //Quaternion rotation = Quaternion.Euler(0f - previewDir.y, 0f - previewDir.x, 0f);
                //Vector3 position2 = rotation * (Vector3.forward * -5.5f * zoomFactor) 
                //    + rootPos + pivotOffset;
                //previewRenderer.camera.transform.position = -Vector3.forward * 10;//position2;
                //previewRenderer.camera.transform.rotation = Quaternion.LookRotation(target.transform.position -
                //    previewRenderer.camera.transform.position);//rotation;
                //SetpreviewCharEnabled(true);
                //previewRenderer.Render();
                //SetpreviewCharEnabled(false);
                //Vector3 position = target.transform.position;
                //Vector2 floorTexOffset = -new Vector2(position.x, position.z);
                //Material floorMat = floorMaterial;
                //Matrix4x4 matrix = Matrix4x4.TRS(position, referenceRot, Vector3.one * 5f * targetScale);
                //floorMat.mainTextureOffset = floorTexOffset * 5f * 0.08f * (1f / targetScale);
                //floorMat.SetTexture("_ShadowTexture", renderTex);
                //floorMat.SetMatrix("_ShadowTextureMatrix", outterMatrix);
                //floorMat.SetVector("_Alphas", new Vector4(0.5f , 0.3f , 0f, 0f));
                //floorMat.renderQueue = 1000;
                //Graphics.DrawMesh(planeMesh, matrix, floorMaterial, 0, previewRenderer.camera, 0);

                //CameraClearFlags clearFlags = previewRenderer.camera.clearFlags;
                //previewRenderer.camera.clearFlags = CameraClearFlags.Nothing;
                //previewRenderer.Render();
                //previewRenderer.camera.clearFlags = clearFlags;
                //RenderTexture.ReleaseTemporary(renderTex);
                #endregion
                #region Approach 2
                if(cameraPivot == null)
                {

                }

                //cameraPivot.transform.position = target.transform.position + Vector3.up * 1;

                //if (previewRenderer.camera.transform.parent != cameraPivot)
                //    previewRenderer.camera.transform.SetParent(cameraPivot.transform);
                if (!target)
                    return;
                if (previewRenderer == null)
                    return;

                Animate();
                target.transform.localToWorldMatrix.SetTRS(target.transform.position, target.transform.rotation, target.transform.localScale);
                PositionPreviewObjects();

                previewRenderer.BeginPreview(r, GUIStyle.none);
                //render floor plane
                if (floorPlane && floorMaterial)
                {
                    var floorMesh = floorPlane.GetComponent<SkinnedMeshRenderer>();
                    if (floorMesh)
                    {
                        floorMesh.receiveShadows = true;
                        previewRenderer.DrawMesh(floorMesh.sharedMesh, Vector3.zero, Quaternion.identity, floorMaterial, 0);
                    }
                }
                //render target avatar
                var mesh = target.GetComponentsInChildren<SkinnedMeshRenderer>();
                for (int i = 0; i < mesh.Length; i++)
                {
                    var m = mesh[i];
                    for (int j = 0; j < m.sharedMesh.subMeshCount; j++)
                    {
                        if(m.sharedMesh != null)
                        {
                            m.shadowCastingMode = ShadowCastingMode.On;
                            previewRenderer.DrawMesh(m.sharedMesh, m.transform.localToWorldMatrix * target.transform.localToWorldMatrix, m.sharedMaterials[j], j);
                        }
                    }
                }
                //Render Accesories
                foreach(var acc in accesories)
                {
                    var subMesh = acc.Key.GetComponentsInChildren<SkinnedMeshRenderer>();
                    acc.Key.transform.localToWorldMatrix.SetTRS(acc.Key.transform.position, acc.Key.transform.rotation, acc.Key.transform.localScale);
                    for (int i = 0; i < subMesh.Length; i++)
                    {
                        var m = subMesh[i];
                        for (int j = 0; j < m.sharedMesh.subMeshCount; j++)
                        {
                            if (m.sharedMesh != null)
                            {
                                m.shadowCastingMode = ShadowCastingMode.On;
                                previewRenderer.DrawMesh(m.sharedMesh, m.transform.localToWorldMatrix * acc.Key.transform.localToWorldMatrix, m.sharedMaterials[j], j);
                            }
                        }
                    }
                }

                bool fog = RenderSettings.fog;
                Unsupported.SetRenderSettingsUseFogNoDirty(false);
                previewRenderer.camera.Render();
                Unsupported.SetRenderSettingsUseFogNoDirty(fog);

                Texture texture = previewRenderer.EndPreview();
                GUI.DrawTexture(r, texture);

                #endregion
            }

            /// <summary>
            /// Animate the target
            /// </summary>
            private void Animate()
            {
                if (!target || !targetAnimator)
                    return;
                targetAnimator.SetFloat("PlaybackTime", playBackTime);
            }

            /// <summary>
            /// Render empty preview.
            /// </summary>
            private static void RenderNull()
            {
                GUILayout.BeginVertical();
                var r = GUILayoutUtility.GetAspectRect(16f / 9);
                EditorGUI.DropShadowLabel(r, "No Motion to preview.\nPlease select a valid motion to preview.");
                GUILayout.EndVertical();
            }


            /// <summary>
            /// Initialise the avatar.
            /// </summary>
            /// <param name="_avatar"></param>
            private bool Initialize(Motion _motion, GameObject _avatar = null, params (GameObject go, HumanBodyBones bone, Vector3 offset)[] accessories)
            {
                if (previewRenderer == null)
                {
                    previewRenderer = new PreviewRenderUtility();
                    pivotOffset = new Vector3(0, 1, 0);
                }
                else
                {
                    //Camera Transform
                    var cx = zoomFactor * Mathf.Cos(panAngle * Mathf.Deg2Rad);
                    var cy = zoomFactor * Mathf.Sin(tiltAngle * Mathf.Deg2Rad);
                    var cz = zoomFactor * Mathf.Sin(panAngle * Mathf.Deg2Rad);
                    previewRenderer.camera.transform.localPosition = pivotOffset + new Vector3(cx, cy, cz);
                    previewRenderer.camera.transform.LookAt(pivotOffset, Vector3.up);

                    //Camera config
                    previewRenderer.camera.fieldOfView = 60;
                    previewRenderer.camera.nearClipPlane = 0.01f;
                    previewRenderer.camera.farClipPlane = 100;

                    //Lights and FX
                    SphericalHarmonicsL2 ambientProbe = RenderSettings.ambientProbe;
                    SetupPreviewLightingAndFx(ambientProbe);

                    //Shadows and floor Texture offset
                    Matrix4x4 outterMatrix;
                    RenderTexture renderTex = null;
                    try
                    {
                        //renderTex = RenderPreviewShadowmap(previewRenderer.lights[0], 128, pivotOffset, Vector3.zero, out outterMatrix);
                        //Vector3 position = target.transform.position;
                        //Vector2 floorTexOffset = -new Vector2(position.x, position.z);
                        //if (floorMaterial)
                        //{
                        //    Material floorMat = floorMaterial;
                        //    Matrix4x4 matrix = Matrix4x4.TRS(position, cameraPivot.transform.rotation, Vector3.one * 5f * targetScale);
                        //    floorMat.mainTextureOffset = floorTexOffset * 5f * 0.08f * (1f / targetScale);
                        //    floorMat.SetTexture("_ShadowTexture", renderTex);
                        //    floorMat.SetMatrix("_ShadowTextureMatrix", outterMatrix);
                        //    floorMat.SetVector("_Alphas", new Vector4(0.5f, 0.3f, 0f, 0f));
                        //    floorMat.renderQueue = 1000;
                        //}
                        //RenderTexture.ReleaseTemporary(renderTex);
                    }
                    catch { }

                }
                if (floorTexture == null)
                {
                    floorTexture = (Texture2D)EditorGUIUtility.Load("Avatar/Textures/AvatarFloor.png");
                }
                if (floorMaterial == null)
                {
                    Shader shader = EditorGUIUtility.LoadRequired("Previews/PreviewPlaneWithShadow.shader") as Shader;
                    floorMaterial = new Material(shader);
                    floorMaterial.mainTexture = floorTexture;
                    floorMaterial.mainTextureScale = Vector2.one * 5f * 4f;
                    floorMaterial.SetVector("_Alphas", new Vector4(0.5f, 0.3f, 0f, 0f));
                    floorMaterial.hideFlags = HideFlags.HideAndDontSave;
                    floorMaterial = new Material(floorMaterial);
                }
                if (shadowMaskMaterial == null)
                {
                    Shader shader2 = EditorGUIUtility.LoadRequired("Previews/PreviewShadowMask.shader") as Shader;
                    shadowMaskMaterial = new Material(shader2);
                    shadowMaskMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                if (shadowPlaneMaterial == null)
                {
                    Shader shader3 = EditorGUIUtility.LoadRequired("Previews/PreviewShadowPlaneClip.shader") as Shader;
                    shadowPlaneMaterial = new Material(shader3);
                    shadowPlaneMaterial.hideFlags = HideFlags.HideAndDontSave;
                }
                if (!floorPlane)
                {
                    var originalMesh = Resources.GetBuiltinResource<Mesh>("New-Plane.fbx");
                    var plMesh = UnityEngine.Object.Instantiate(originalMesh, Vector3.zero, Quaternion.identity);
                    var renderer = floorPlane.AddComponent<SkinnedMeshRenderer>();
                    renderer.sharedMesh = plMesh;
                }
                if (!directionArrow)
                {
                    var original = (GameObject)EditorGUIUtility.Load("Avatar/dial_flat.prefab");
                    directionArrow = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity);
                    previewRenderer.AddSingleGO(directionArrow);
                    //SetEnabledRecursive(directionArrow, true);
                }
                if (!rootGameObject)
                {
                    var original = (GameObject)EditorGUIUtility.Load("Avatar/root.fbx");
                    rootGameObject = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity);
                    previewRenderer.AddSingleGO(rootGameObject);
                    //SetEnabledRecursive(rootGameObject, true);
                }
                if (!target)
                {
                    //TODO: Load the default avatar here.
                    var defaultAvatar = (GameObject)EditorGUIUtility.Load(PulseEditorMgr.previewAvatarPath);
                    target = _avatar ? _avatar : defaultAvatar;
                    if (target) {
                        //UnityEngine.Object.Destroy(defaultAvatar);
                        defaultAvatar = null;
                        targetAnimator = target.GetComponentInChildren<Animator>();
                        if (!targetAnimator)
                        {
                            Reset();
                            return false;
                        }

                        targetAnimator.enabled = false;
                        targetAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                        targetAnimator.logWarnings = false;
                        targetAnimator.fireEvents = false;
                        previewRenderer.AddSingleGO(target);
                        //SetEnabledRecursive(target, true);
                        //rtController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/control.controller");
                        if (rtController == null)
                        {
                            rtController = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath("Assets/control.controller");
                            animState = rtController.AddMotion(playBackMotion);
                            rtController.AddParameter("PlaybackTime", AnimatorControllerParameterType.Float);
                            animState.timeParameter = "PlaybackTime";
                            targetAnimator.runtimeAnimatorController = rtController;
                        }
                    }
                    else {
                        Reset();
                        return false;
                    }
                }
                foreach(var accessory in accessories)
                {
                    if (accesories == null)
                        accesories = new Dictionary<GameObject, (HumanBodyBones bone, Vector3 offset)>();
                    if (!accesories.ContainsKey(accessory.go))
                    {
                        accesories.Add(accessory.go, (accessory.bone, accessory.offset));
                    }
                }

                return true;
            }

            /// <summary>
            /// Reset the preview.
            /// </summary>
            private void Reset()
            {
                target = null;
                targetAnimator = null;
                if (previewRenderer != null)
                {
                    previewRenderer.Cleanup();
                    previewRenderer = null;
                }
                playBackMotion = null;
                playBackTime = 0;
                floorPlane = null;
                cameraPivot = null;
                rootGameObject = null;
            }

            /// <summary>
            /// Active go renderers
            /// </summary>
            /// <param name="go"></param>
            /// <param name="enabled"></param>
            private static void SetEnabledRecursive(GameObject go, bool enabled)
            {
                Renderer[] componentsInChildren = go.GetComponentsInChildren<Renderer>();
                foreach (Renderer renderer in componentsInChildren)
                {
                    renderer.enabled = enabled;
                }
            }

            /// <summary>
            /// enable or disable target renderers.
            /// </summary>
            /// <param name="enabled"></param>
            private void SetpreviewCharEnabled(bool enabled)
            {
                SetEnabledRecursive(target, enabled);
                SetEnabledRecursive(directionArrow, enabled);
                SetEnabledRecursive(rootGameObject, enabled);
            }

            /// <summary>
            /// Generate shadow.
            /// </summary>
            /// <param name="light"></param>
            /// <param name="scale"></param>
            /// <param name="center"></param>
            /// <param name="floorPos"></param>
            /// <param name="outShadowMatrix"></param>
            /// <returns></returns>
            private RenderTexture RenderPreviewShadowmap(Light light, float scale, Vector3 center, Vector3 floorPos, out Matrix4x4 outShadowMatrix)
            {
                Assert.IsTrue(Event.current.type == EventType.Repaint);
                Camera camera = previewRenderer.camera;
                camera.orthographic = true;
                camera.orthographicSize = scale * 2f;
                camera.nearClipPlane = 1f * scale;
                camera.farClipPlane = 25f * scale;
                camera.transform.rotation =  light.transform.rotation;
                camera.transform.position = center - light.transform.forward * (scale * 5.5f);
                CameraClearFlags clearFlags = camera.clearFlags;
                camera.clearFlags = CameraClearFlags.Color;
                Color backgroundColor = camera.backgroundColor;
                camera.backgroundColor = new Color(0f, 0f, 0f, 0f);
                RenderTexture targetTexture = camera.targetTexture;
                RenderTexture temporary = RenderTexture.GetTemporary(256, 256, 16);
                temporary.isPowerOfTwo = true;
                temporary.wrapMode = TextureWrapMode.Clamp;
                temporary.filterMode = FilterMode.Bilinear;
                camera.targetTexture = temporary;
                //SetPreviewCharacterEnabled(enabled: true, showReference: false);
                previewRenderer.camera.Render();
                RenderTexture.active = temporary;
                GL.PushMatrix();
                GL.LoadOrtho();
                shadowMaskMaterial.SetPass(0);
                GL.Begin(7);
                GL.Vertex3(0f, 0f, -99f);
                GL.Vertex3(1f, 0f, -99f);
                GL.Vertex3(1f, 1f, -99f);
                GL.Vertex3(0f, 1f, -99f);
                GL.End();
                GL.LoadProjectionMatrix(camera.projectionMatrix);
                GL.LoadIdentity();
                GL.MultMatrix(camera.worldToCameraMatrix);
                shadowPlaneMaterial.SetPass(0);
                GL.Begin(7);
                float num = 5f * scale;
                GL.Vertex(floorPos + new Vector3(0f - num, 0f, 0f - num));
                GL.Vertex(floorPos + new Vector3(num, 0f, 0f - num));
                GL.Vertex(floorPos + new Vector3(num, 0f, num));
                GL.Vertex(floorPos + new Vector3(0f - num, 0f, num));
                GL.End();
                GL.PopMatrix();
                Matrix4x4 lhs = Matrix4x4.TRS(new Vector3(0.5f, 0.5f, 0.5f), Quaternion.identity, new Vector3(0.5f, 0.5f, 0.5f));
                outShadowMatrix = lhs * camera.projectionMatrix * camera.worldToCameraMatrix;
                camera.orthographic = false;
                camera.clearFlags = clearFlags;
                camera.backgroundColor = backgroundColor;
                camera.targetTexture = targetTexture;
                return temporary;
            }

            /// <summary>
            /// Set up the lights and FX
            /// </summary>
            /// <param name="probe"></param>
            private void SetupPreviewLightingAndFx(SphericalHarmonicsL2 probe)
            {
                previewRenderer.lights[0].intensity = 1.4f;
                previewRenderer.lights[0].transform.rotation = Quaternion.Euler(40f, 40f, 0f);
                previewRenderer.lights[1].intensity = 1.4f;
                RenderSettings.ambientMode = AmbientMode.Custom;
                RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.1f, 1f);
                RenderSettings.ambientProbe = probe;
            }


            /// <summary>
            /// Position the gameobjects in scene.
            /// </summary>
            /// <param name="pivotRot"></param>
            /// <param name="pivotPos"></param>
            /// <param name="bodyRot"></param>
            /// <param name="bodyPos"></param>
            /// <param name="directionRot"></param>
            /// <param name="rootRot"></param>
            /// <param name="rootPos"></param>
            /// <param name="directionPos"></param>
            /// <param name="scale"></param>
            private void PositionPreviewObjects()
            {
                target.transform.position = targetAnimator.rootPosition;
                target.transform.rotation = targetAnimator.rootRotation;
                target.transform.localScale = Vector3.one * targetScale;
                directionArrow.transform.position = targetAnimator.rootPosition;
                directionArrow.transform.rotation = targetAnimator.bodyRotation;
                directionArrow.transform.localScale = Vector3.one * targetScale * 2f;
                rootGameObject.transform.position = pivotOffset;
                rootGameObject.transform.rotation = Quaternion.identity;
                rootGameObject.transform.localScale = Vector3.one * targetScale * 0.25f;

                foreach(var acc in accesories)
                {
                    if (targetAnimator)
                    {
                        var bone = targetAnimator.GetBoneTransform(acc.Value.bone);
                        acc.Key.transform.position = bone.position + acc.Value.offset;
                        acc.Key.transform.rotation = bone.rotation;
                    }
                }
                //float normalizedTime = timeControl.normalizedTime;
                //float num = timeControl.deltaTime / (timeControl.stopTime - timeControl.startTime);
                //if (normalizedTime - num < 0f || normalizedTime - num >= 1f)
                //{
                //    m_PrevFloorHeight = m_NextFloorHeight;
                //}
                //if (m_LastNormalizedTime != -1000f && timeControl.startTime == m_LastStartTime && timeControl.stopTime == m_LastStopTime)
                //{
                //    float num2 = normalizedTime - num - m_LastNormalizedTime;
                //    if (num2 > 0.5f)
                //    {
                //        num2 -= 1f;
                //    }
                //    else if (num2 < -0.5f)
                //    {
                //        num2 += 1f;
                //    }
                //}
                //m_LastNormalizedTime = normalizedTime;
                //m_LastStartTime = timeControl.startTime;
                //m_LastStopTime = timeControl.stopTime;
                //if (m_NextTargetIsForward)
                //{
                //    m_NextFloorHeight = Animator.targetPosition.y;
                //}
                //else
                //{
                //    m_PrevFloorHeight = Animator.targetPosition.y;
                //}
                nextTargetIsForward = !nextTargetIsForward;
                targetAnimator.SetTarget(AvatarTarget.Root, nextTargetIsForward ? 1 : 0);
            }

            /// <summary>
            /// Update the animation.
            /// </summary>
            private IEnumerator UpdateAnimation()
            {
                isPlaying = true;
                for (; ; )
                {
                    //TODO: Animate here/ update positions and rotations.

                    //--------------------------------------------------
                    yield return new WaitForEndOfFrame();
                    if (!playBackMotion || !target || !targetAnimator)
                    {
                        isPlaying = false;
                        yield break;
                    }
                }
            }

            #endregion
        }


        /// <summary>
        /// Les argeuments d'un evenement d'editeur.
        /// </summary>
        public class EditorEventArgs : EventArgs
        {
            public int Scope;
            public int dataType;
            public int ID;
            public int Zone;
            public int Language;
        }


        #endregion
    }

    namespace Modules
    {
        #region Children namespaces #####################################################################

        namespace Localisator
        {

        }

        namespace PhysicSpace
        {

        }

        namespace StatHandler
        {

        }

        namespace Commander
        {

        }

        namespace Anima
        {

        }

        namespace CharacterCreator
        {

        }

        namespace CombatSystem
        {

        }

        #endregion
    }

}