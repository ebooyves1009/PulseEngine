using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEditor;
using PulseEngine.Core;
using PulseEditor.Module.Localisator;
using PulseEngine.Module.CharacterCreator;
using UnityEditor;
using System;
using PulseEngine.Module.CombatSystem;
using PulseEditor.Module.StatManager;
using PulseEngine.Module.StatHandler;

namespace PulseEditor.Module.CharacterCreator
{
    /// <summary>
    /// L'editeur de characters.
    /// </summary>
    public class CharacterEditor : PulseEngine_Core_BaseEditor
    {
        #region Fonctionnal Attributes ################################################################

        /// <summary>
        /// La previsualisation du charactere.
        /// </summary>
        private Editor objEditor;

        /// <summary>
        /// L'asset permanent
        /// </summary>
        private CharactersLibrary asset;

        /// <summary>
        /// L'asset en cours de modification.
        /// </summary>
        private CharactersLibrary editedAsset;

        /// <summary>
        /// La data en cours de modification.
        /// </summary>
        private CharacterData editedData;

        /// <summary>
        /// l'index de la data choisie dans le tableau.
        /// </summary>
        private int selectedDataIndex;

        /// <summary>
        /// Le type de character choisi.
        /// </summary>
        private CharacterManager.CharacterType typeSelected;
        private int indexSelectedWeapon;

        #endregion
        #region Visual Attributes ################################################################

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
                dataType = (int)editedAsset.LibraryDataType
            };
            if (onSelectionEvent != null)
                onSelectionEvent.Invoke(this, eventArgs);
        }

        #endregion
        #region Visual Methods ################################################################

        /// <summary>
        /// Open the editor.
        /// </summary>
        [MenuItem(PulseCore_GlobalValue_Manager.Menu_EDITOR_MENU + "Character Editor")]
        public static void OpenEditor()
        {
            var window = GetWindow<CharacterEditor>();
            window.windowOpenMode = EditorMode.Normal;
            window.Show();
        }

        /// <summary>
        /// Open the selector.
        /// </summary>
        public static void OpenSelector()
        {
            var window = GetWindow<CharacterEditor>();
            window.windowOpenMode = EditorMode.Selector;
            window.Show();
        }

        /// <summary>
        /// Initialise la fenetre.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            if (!asset)
            {
                asset = CharactersLibrary.Load(typeSelected);
                if(!asset && CharactersLibrary.Create(typeSelected))
                    asset = CharactersLibrary.Load(typeSelected);
                if (asset)
                    editedAsset = asset;
            }
        }

        protected override void OnRedraw()
        {
            base.OnRedraw();
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            if (!Header())
                return;
            CharacterList(editedAsset);
            SaveAndCancel();
            GUILayout.EndVertical();
            GUILayout.Space(5);
            GUILayout.BeginVertical();
            Details(editedData);
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
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
                int selectedTypeInd = GUILayout.Toolbar((int)typeSelected, Enum.GetNames(typeof(CharacterManager.CharacterType)));
                typeSelected = (CharacterManager.CharacterType)selectedTypeInd;
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
                ScrollablePanel(() =>
                {
                    GroupGUI(() =>
                    {
                        GUILayout.BeginVertical();
                        List<GUIContent> listContent = new List<GUIContent>();
                        int maxId = 0;
                        for (int i = 0; i < library.Characterlist.Count; i++)
                        {
                            var data = library.Characterlist[i];
                            var nameList = LocalisationEditor.GetTexts(data.IdTrad, PulseCore_GlobalValue_Manager.DataType.CharacterInfos);
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
                        selectedDataIndex = ListItems(selectedDataIndex, listContent.ToArray());
                        GUILayout.Space(5);
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("+"))
                        {
                            library.Characterlist.Add(new CharacterData { ID = maxId + 1 });
                        }
                        if(selectedDataIndex >= 0 && selectedDataIndex < editedAsset.Characterlist.Count)
                        {
                            if (GUILayout.Button("-"))
                            {
                                library.Characterlist.RemoveAt(selectedDataIndex);
                            }
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                    }, "Characters Datas List");
                });
                if (selectedDataIndex >= 0 && selectedDataIndex < editedAsset.Characterlist.Count)
                    editedData = editedAsset.Characterlist[selectedDataIndex];
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
            ScrollablePanel(() =>
            {
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
                    EditorGUILayout.LabelField("ID: "+data.ID.ToString(), style_label);
                    GUILayout.Space(5);
                    //Trad
                    EditorGUILayout.LabelField("Trad Id: "+data.IdTrad, style_label);
                    GUILayout.Space(5);
                    data.TradDataType = PulseCore_GlobalValue_Manager.DataType.CharacterInfos;
                    var texts = LocalisationEditor.GetTexts(data.IdTrad, PulseCore_GlobalValue_Manager.DataType.CharacterInfos);
                    string name = texts.Length > 0 ? texts[0] : string.Empty;
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Name:", style_label);
                    EditorGUILayout.LabelField(name);
                    if (GUILayout.Button("S", new[] { GUILayout.Width(25)}))
                    {
                        LocalisationEditor.OpenSelector((obj, arg) =>
                        {
                            var a = arg as EditorEventArgs;
                            if (a == null)
                                return;
                            data.IdTrad = a.ID;
                        }, data.TradDataType);
                    }
                    if (GUILayout.Button("E", new[] { GUILayout.Width(25)}))
                    {
                        LocalisationEditor.OpenModifier(data.IdTrad, data.TradDataType);
                    }
                    GUILayout.EndHorizontal();
                    EditorGUILayout.LabelField("Character:", style_label);
                    GUILayout.Space(5);
                    data.Character = EditorGUILayout.ObjectField(data.Character, typeof(GameObject), false) as GameObject;
                    GUILayout.EndVertical();
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);
                    //Stats --------------------------------------------------------------------------------------------------------------------------------
                    GUILayout.BeginHorizontal();
                    EditorGUILayout.LabelField("Stats:", style_label);
                    if(GUILayout.Button("Edit "+name+" Stats"))
                    {
                        StatEditor.OpenStatWindow(data.Stats, (obj)=> {
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
                        //TODO: Open the Weapon mini editor Here.
                    }
                    GUILayout.EndHorizontal();

                    GUILayout.Space(5);
                    // Animator Controller ------------------------------------------------------------------------------------------------------------------
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("Edit " + name + " Runtime Controller"))
                    {
                        //TODO: Open the Animator controller editor Here.
                    }
                    GUILayout.EndHorizontal();


                    GUILayout.EndVertical();
                }, data.ID + " Edition");
            });
        }

        /// <summary>
        /// la panel de sauvegarde.
        /// </summary>
        private void SaveAndCancel()
        {
            if (editedAsset == null)
                return;
            switch (windowOpenMode)
            {
                case EditorMode.Normal:
                    SaveCancelPanel(new[] {
                        new KeyValuePair<string, Action>("Save & Close", ()=> { Save();}),
                        new KeyValuePair<string, Action>("Close", ()=> { Cancel("Les Modifications ne seront pas sauvegardees.\nContinuer?"); })
                    });
                    break;
                case EditorMode.Selector:
                    SaveCancelPanel(new[] {
                        new KeyValuePair<string, Action>("Select", ()=> { Select(editedData);}),
                        new KeyValuePair<string, Action>("Cancel", ()=> { Cancel("La selection ne sera pas prise en compte.\nContinuer?");})
                    });
                    break;
                case EditorMode.ItemEdition:
                    break;
                case EditorMode.Preview:
                    break;
                case EditorMode.Node:
                    break;
                case EditorMode.Group:
                    break;
            }
        }


        #endregion
        #region Helpers & Tools ################################################################

        #endregion
    }
}