using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using PulseEngine.Globals;
using UILayout = UnityEngine.GUILayout;


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
            protected List<ScriptableObject> allAssets; 

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
                var options = new[] { GUILayout.MinWidth(100), GUILayout.MaxWidth(300) };
                scroolPos = GUILayout.BeginScrollView(scroolPos, fixedSize ? options : null);
                GUILayout.BeginVertical();
                if (guiFunctions != null)
                    guiFunctions.Invoke();
                GUILayout.EndVertical();
                GUILayout.EndScrollView();
                PanelsScrools[scrollPanCount] = scroolPos;
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
        public static class Previewer 
        {

            #region Attributes ###################################################################################

            /// <summary>
            /// The playback time.
            /// </summary>
            private static float playBackTime;

            /// <summary>
            /// The playback motion.
            /// </summary>
            private static Motion playBackMotion;

            /// <summary>
            /// the target to render.
            /// </summary>
            private static GameObject target;

            /// <summary>
            /// The default rendered avatar.
            /// </summary>
            private static GameObject defaultAvatar;

            /// <summary>
            /// The grond plane mesh.
            /// </summary>
            private static Mesh planeMesh;

            /// <summary>
            /// The target's animator.
            /// </summary>
            private static Animator targetAnimator;

            /// <summary>
            /// Active when previwe is playing anim.
            /// </summary>
            private static bool isPlaying;

            /// <summary>
            /// the preview renderer.
            /// </summary>
            private static PreviewRenderUtility previewRenderer;

            #endregion

            #region Static Methods #################################################################################################

            /// <summary>
            /// Render the preview.
            /// </summary>
            /// <param name="_motion"></param>
            /// <param name="_target"></param>
            public static float Previsualize(Motion _motion, GameObject _target = null)
            {
                if(!_motion || _motion != playBackMotion || _target != target)
                {
                    Reset();
                    return 0;
                }
                if (Initialize(_motion, _target))
                {
                    RenderPreview();
                }
                else
                    RenderNull();

                return playBackTime;
            }

            #endregion

            #region Methods #################################################################################################

            /// <summary>
            /// Render the preview.
            /// </summary>
            private static void RenderPreview()
            {
                //TODO: Render the preview here.
            }

            /// <summary>
            /// Render empty preview.
            /// </summary>
            private static void RenderNull()
            {
                GUILayout.BeginVertical();
                var r = GUILayoutUtility.GetAspectRect(16f / 9);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("<< Nothing to render >>");
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
            }

            /// <summary>
            /// Initialise the avatar.
            /// </summary>
            /// <param name="_avatar"></param>
            private static bool Initialize(Motion _motion, GameObject _avatar = null)
            {
                if (!defaultAvatar)
                {
                    //TODO: Load the default avatar here.
                }
                if (!target)
                {
                    target = GameObject.Instantiate(_avatar ? _avatar : defaultAvatar, Vector3.zero, Quaternion.identity, true);
                    if (target) {
                        targetAnimator = target.GetComponentInChildren<Animator>();
                        if (!targetAnimator)
                        {
                            Reset();
                            return false;
                        }
                    }
                    else {
                        Reset();
                        return false;
                    }
                }
                if (!planeMesh)
                {
                    //TODO: Create or load plane mesh here
                }

                if (previewRenderer == null)
                {
                    previewRenderer = new PreviewRenderUtility();
                    //TODO: Set Lights, draw plane...
                }

                return true;
            }

            /// <summary>
            /// Reset the preview.
            /// </summary>
            private static void Reset()
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
            }

            /// <summary>
            /// Update the animation.
            /// </summary>
            private static IEnumerator UpdateAnimation()
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

        #endregion
    }

}