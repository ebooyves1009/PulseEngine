using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEditor.Globals;
using PulseEditor.Modules.Localisator;
using PulseEditor.Modules.StatHandler;
using PulseEngine.Globals;
using PulseEngine.Modules;
using PulseEngine.Modules.CharacterCreator;
using PulseEngine.Modules.StatHandler;
using UnityEditor;
using System;

namespace PulseEditor.Modules.CharacterCreator
{
    /// <summary>
    /// L'editeur de characters.
    /// </summary>
    public class CharacterEditor : PulseEditorMgr
    {
        #region Fonctionnal Attributes ################################################################

        /// <summary>
        /// La previsualisation du charactere.
        /// </summary>
        private Editor objEditor;

        /// <summary>
        /// Le type de character choisi.
        /// </summary>
        private CharacterType typeSelected;

        #endregion
        #region Visual Attributes ################################################################

        /// <summary>
        /// the preview panel.
        /// </summary>
       private Previewer previewer;
        
        /// <summary>
        /// the currently playing motion.
        /// </summary>
       private Motion selectedmotion;
        
        /// <summary>
        /// the index of selected layer.
        /// </summary>
       private int layerAnimIndex;
        
        /// <summary>
        /// the index of selected State.
        /// </summary>
       private int StateAnimIndex;

        #endregion
        #region Fonctionnal Methods ################################################################

        /// <summary>
        /// Sauvegarde las modifications.
        /// </summary>
        private void Save()
        {
            SaveAsset(editedAsset, asset);
            Close();
        }

        /// <summary>
        /// Annule les modifications.
        /// </summary>
        /// <param name="msg"></param>
        private void Cancel(string msg)
        {
            if (EditorUtility.DisplayDialog("Warning", msg, "Yes", "No"))
                Close();
        }

        /// <summary>
        /// la selection d'un item.
        /// </summary>
        /// <param name="data"></param>
        private void Select(CharacterData data)
        {
            if (data == null || editedAsset == null)
                return;
            EditorEventArgs eventArgs = new EditorEventArgs
            {
                ID = data.ID,
                dataType = (int)((CharactersLibrary)editedAsset).DataType
            };
            if (onSelectionEvent != null)
                onSelectionEvent.Invoke(this, eventArgs);
        }

        #endregion

        #region Static Methods ################################################################

        /// <summary>
        /// Open the editor.
        /// </summary>
        [MenuItem(Menu_EDITOR_MENU + "Character Editor")]
        public static void OpenEditor()
        {
            var window = GetWindow<CharacterEditor>();
            window.windowOpenMode = EditorMode.Normal;
            window.Show();
        }

        /// <summary>
        /// Open the selector.
        /// </summary>
        public static void OpenSelector(Action<object,EventArgs> onSelect)
        {
            var window = GetWindow<CharacterEditor>(true);
            window.windowOpenMode = EditorMode.Selector;
            window.onSelectionEvent += (obj,arg)=>
            {
                if(onSelect != null)
                {
                    onSelect.Invoke(null, new EditorEventArgs
                    {
                        ID = ((CharacterData)window.editedData).ID,
                        dataType = (int)((CharactersLibrary)window.editedAsset).DataType,
                        Scope = (int)((CharactersLibrary)window.editedAsset).Scope
                    });
                }
            };
            window.ShowUtility();
        }

        /// <summary>
        /// Open the Modifier.
        /// </summary>
        public static void OpenModifier(int _id, Scopes scope, CharacterType _type)
        {
            var window = GetWindow<CharacterEditor>(true);
            window.windowOpenMode = EditorMode.ItemEdition;
            window.typeSelected = _type;
            window.dataID = _id;
            window.currentScope = scope;
            window.ShowUtility();
        }


        #endregion

        #region Visual Methods ################################################################

        /// <summary>
        /// Initialise la fenetre.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            if (!asset)
            {
                asset = CharactersLibrary.Load(typeSelected, currentScope);
                if (!asset && CharactersLibrary.Save(typeSelected, currentScope))
                    asset = CharactersLibrary.Load(typeSelected, currentScope);
                if (asset)
                    editedAsset = asset;
            }
            RefreshPreview();
        }

        protected override void OnRedraw()
        {
            base.OnRedraw();
            if (!Header())
                return;
            GUILayout.BeginHorizontal();
            ScrollablePanel(() =>
            {
                CharacterList((CharactersLibrary)editedAsset);
                SaveAndCancel();
            },true);
            GUILayout.Space(5);
            ScrollablePanel(() =>
            {
                Details((CharacterData)editedData);
                GUILayout.Space(5);
                AnimationsPreview((CharacterData)editedData);
            });
            GUILayout.EndHorizontal();
        }

        protected override void OnQuit()
        {
            if (previewer != null)
                previewer.Destroy();
        }

        private void RefreshPreview()
        {
            if (previewer != null)
                previewer.Destroy();
            previewer = new Previewer();
        }

        #endregion
        #region Common Windows ################################################################

        /// <summary>
        /// L'entete.
        /// </summary>
        /// <returns></returns>
        private bool Header()
        {
            var bkpType = typeSelected;
            GroupGUInoStyle(() =>
            {
                int selectedTypeInd = GUILayout.Toolbar((int)typeSelected, Enum.GetNames(typeof(CharacterType)));
                typeSelected = (CharacterType)selectedTypeInd;
            }, "Character Type", 50);
            if(bkpType != typeSelected)
            {
                asset = null;
                editedAsset = null;
                OnInitialize();
            }
            if (editedAsset)
                return true;
            else
                return false;
        }

        /// <summary>
        /// La liste des characters.
        /// </summary>
        /// <param name="library"></param>
        private void CharacterList(CharactersLibrary library)
        {
            if (!library)
                return;
            Func<bool> listCompatiblesmode = () =>
            {
                switch (windowOpenMode)
                {
                    case EditorMode.Normal:
                        return true;
                    case EditorMode.Selector:
                        return true;
                    case EditorMode.ItemEdition:
                        return false;
                    case EditorMode.Preview:
                        return false;
                    default:
                        return false;
                }
            };
            if (listCompatiblesmode())
            {
                GroupGUI(() =>
                {
                    GUILayout.BeginVertical();
                    List<GUIContent> listContent = new List<GUIContent>();
                    int maxId = 0;
                    for (int i = 0; i < library.DataList.Count; i++)
                    {
                        var data = library.DataList[i];
                        var nameList = LocalisationEditor.GetTexts(data.IdTrad, TradDataTypes.Person);
                        string name = nameList.Length > 0 ? nameList[0] : string.Empty;
                        char[] titleChars = new char[LIST_MAX_CHARACTERS];
                        string pointDeSuspension = string.Empty;
                        try
                        {
                            if (data.ID > maxId) maxId = data.ID;
                            for (int j = 0; j < titleChars.Length; j++)
                                if (j < name.Length)
                                    titleChars[j] = name[j];
                            if (name.Length >= titleChars.Length)
                                pointDeSuspension = "...";
                        }
                        catch { }
                        string title = new string(titleChars) + pointDeSuspension;
                        listContent.Add(new GUIContent { text = data != null ? data.ID + "-" + title : "null data" });
                    }
                    int newOne = ListItems(selectDataIndex, listContent.ToArray());
                    if (newOne != selectDataIndex)
                        RefreshPreview();
                    selectDataIndex = newOne;
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("+"))
                    {
                        library.DataList.Add(new CharacterData
                        {
                            ID = maxId + 1,
                            TradType = TradDataTypes.Person
                        });
                    }
                    if (selectDataIndex >= 0 && selectDataIndex < library.DataList.Count)
                    {
                        if (GUILayout.Button("-"))
                        {
                            library.DataList.RemoveAt(selectDataIndex);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }, "Characters Datas List");
                if (selectDataIndex >= 0 && selectDataIndex < library.DataList.Count)
                    editedData = library.DataList[selectDataIndex];
                else
                    editedData = null;
            }
        }

        /// <summary>
        /// les details d'une data.
        /// </summary>
        /// <param name="data"></param>
        private void Details(CharacterData data)
        {
            if (data == null)
                return;
            GroupGUI(() =>
            {
                GUILayout.BeginVertical();
                //Game object ---------------------------------------------------------------------------------------------------
                GUILayout.BeginHorizontal();
                if (data.Character)
                {
                    if (objEditor == null || data.Character != objEditor.target)
                        objEditor = Editor.CreateEditor(data.Character);
                    objEditor.OnInteractivePreviewGUI(GUILayoutUtility.GetRect(150, 150), null);
                }
                else
                {
                    GUILayout.BeginArea(GUILayoutUtility.GetRect(150, 150));
                    GUILayout.EndArea();
                }
                //ID
                GUILayout.BeginVertical();
                EditorGUILayout.LabelField("ID: " + data.ID.ToString(), style_label);
                GUILayout.Space(5);
                //Trad
                EditorGUILayout.LabelField("Trad Id: " + data.IdTrad, style_label);
                GUILayout.Space(5);
                data.TradType = TradDataTypes.Person;
                var texts = LocalisationEditor.GetTexts(data.IdTrad, TradDataTypes.Person);
                string name = texts.Length > 0 ? texts[0] : string.Empty;
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name:", style_label);
                EditorGUILayout.LabelField(name);
                if (GUILayout.Button("S", new[] { GUILayout.Width(25) }))
                {
                    LocalisationEditor.OpenSelector((obj, arg) =>
                    {
                        var a = arg as EditorEventArgs;
                        if (a == null)
                            return;
                        data.IdTrad = a.ID;
                    }, data.TradType);
                }
                if (GUILayout.Button("E", new[] { GUILayout.Width(25) }))
                {
                    LocalisationEditor.OpenModifier(data.IdTrad, data.TradType);
                }
                GUILayout.EndHorizontal();
                EditorGUILayout.LabelField("Character:", style_label);
                GUILayout.Space(5);
                var Char = EditorGUILayout.ObjectField(data.Character, typeof(GameObject), false) as GameObject;
                if (Char != data.Character)
                    RefreshPreview();
                data.Character = Char;
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                GUILayout.Space(5);
                //Stats --------------------------------------------------------------------------------------------------------------------------------
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Stats:", style_label);
                if (GUILayout.Button("Edit " + name + " Stats"))
                {
                    StatEditor.OpenStatWindow(data.Stats, (obj) => {
                        var st = obj as StatData;
                        if (st != null)
                            data.Stats = st;
                    }, name + "'s Stats");
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5);
                // Animator Avatar ---------------------------------------------------------------------------------------------------------------------
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Avatar:", style_label);
                data.AnimatorAvatar = EditorGUILayout.ObjectField(data.AnimatorAvatar, typeof(Avatar), false) as Avatar;
                GUILayout.EndHorizontal();

                GUILayout.Space(5);
                // Weapons -------------------------------------------------------------------------------------------------------------------------------
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Weaponry:", style_label);
                if (GUILayout.Button("Edit " + name + "'s Weaponry"))
                {
                    List<(int, WeaponType, Scopes)> ps = new List<(int, WeaponType, Scopes)>();
                    for(int i = 0; i < data.Armurie.Count; i++)
                    {
                        var weap = data.Armurie[i];
                        ps.Add((weap.x, (WeaponType)weap.y, (Scopes)weap.z));
                    }
                    CombatSystem.WeaponEditor.WeaponryEditor.Open(ps, (obj,arg)=>
                    {
                        var result = obj as List<(int, WeaponType, Scopes)>;
                        if(result != null)
                        {
                            List<Vector3Int> sp = new List<Vector3Int>();
                            for (int i = 0; i < result.Count; i++)
                            {
                                var weap = result[i];
                                sp.Add(new Vector3Int(weap.Item1, (int)weap.Item2, (int)weap.Item3));
                            }
                            data.Armurie = sp;
                        }
                    });
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5);
                // Animator Controller ------------------------------------------------------------------------------------------------------------------
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Edit " + name + " Runtime Controller"))
                {
                    Anima.AnimaEditor.AnimaMachineEditor.Open(data.AnimatorController, name, (obj, arg) =>
                    {
                        var rt = obj as RuntimeAnimatorController;
                        if (rt != null)
                            data.AnimatorController = rt;
                        RefreshPreview();
                    });
                }
                GUILayout.EndHorizontal();


                GUILayout.EndVertical();
            }, data.ID + " Edition");
        }

        /// <summary>
        /// Affiche une previsualisation.
        /// </summary>
        /// <param name="data"></param>
        private void AnimationsPreview(CharacterData data)
        {
            if (data == null)
                return;
            GroupGUI(() =>
            {
                if (data.AnimatorController == null)
                    return;
                var controller = (UnityEditor.Animations.AnimatorController)data.AnimatorController;
                var layerlist = controller.layers;
                string[] layerNames = new string[layerlist.Length];
                for (int i = 0; i < layerlist.Length; i++)
                    layerNames[i] = layerlist[i].name;
                layerAnimIndex = EditorGUILayout.Popup("Layers", layerAnimIndex, layerNames);
                UnityEditor.Animations.ChildAnimatorState[] stateList = null;
                if (layerAnimIndex < controller.layers.Length && layerAnimIndex >= 0)
                {
                    stateList = controller.layers[layerAnimIndex].stateMachine.states;
                }
                if(stateList != null)
                {
                    string[] stateNames = new string[layerlist.Length];
                    for (int i = 0; i < stateList.Length; i++)
                        stateNames[i] = stateList[i].state.name;
                    StateAnimIndex = EditorGUILayout.Popup("States", StateAnimIndex, stateNames);
                    if (StateAnimIndex < stateList.Length && StateAnimIndex >= 0)
                        selectedmotion = stateList[StateAnimIndex].state.motion;
                }
                if (previewer != null)
                    previewer.Previsualize(selectedmotion, 18 / 9, data.Character);

            }, "Animation preview");
        }

        /// <summary>
        /// la panel de sauvegarde.
        /// </summary>
        private void SaveAndCancel()
        {
            if (editedAsset == null)
                return;
            SaveBarPanel(editedAsset, asset, () => { Select((CharacterData)editedData); });
        }


        #endregion
        #region Helpers & Tools ################################################################


        #endregion
    }
}