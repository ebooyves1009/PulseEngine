using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEditor;
using PulseEngine.Core;
using PulseEngine.Module.Anima;
using System;
using UnityEditor;

namespace PulseEditor.Module.Anima
{
    /// <summary>
    /// l'editeur d'animation.
    /// </summary>
    public class AnimaEditor : PulseEngine_Core_BaseEditor
    {
        #region Fonctionnal Attributes ################################################################

        /// <summary>
        /// Toutes les assets confondues
        /// </summary>
        private List<AnimaLibrary> allAsset = new List<AnimaLibrary>();

        /// <summary>
        /// l'asset correspondante au choix de category et type
        /// </summary>
        private AnimaLibrary asset;

        /// <summary>
        /// L'asset en cours d'edition.
        /// </summary>
        private AnimaLibrary editedAsset;

        /// <summary>
        /// La data en cors d'edition.
        /// </summary>
        private AnimaData dataEdited;

        #endregion
        #region Visual Attributes ################################################################


        /// <summary>
        /// l'index de la data selectionne.
        /// </summary>
        private int selectedDataIdx;

        /// <summary>
        /// La categoie selectionne
        /// </summary>
        private AnimaManager.AnimaCategory selectedCategory;

        /// <summary>
        /// Le ypes selectionne.
        /// </summary>
        private AnimaManager.AnimationType selectedType;

        #endregion
        #region Fonctionnal Methods ################################################################

        /// <summary>
        /// initialise toutes les assets.
        /// </summary>
        private void Initialisation()
        {
            allAsset.Clear();
            editedAsset = null;
            dataEdited = null;
            foreach (AnimaManager.AnimaCategory category in Enum.GetValues(typeof(AnimaManager.AnimaCategory)))
            {
                foreach (AnimaManager.AnimationType type in Enum.GetValues(typeof(AnimaManager.AnimationType)))
                {
                    if (AnimaLibrary.Exist(category, type))
                        allAsset.Add(AnimaLibrary.Load(category, type));
                    else if (AnimaLibrary.Create(category, type))
                        allAsset.Add(AnimaLibrary.Load(category, type));
                }
            }
            asset = allAsset.Find(ass => { return ass.AnimCategory == selectedCategory && ass.AnimType == selectedType; });
            if (asset)
            {
                editedAsset = asset;
            }
        }

        /// <summary>
        /// A la selection d'un item.
        /// </summary>
        private void onSelect()
        {
            throw new NotImplementedException();
        }

        #endregion
        #region Visual Methods ################################################################

        /// <summary>
        /// open Anima editor.
        /// </summary>
        [MenuItem(PulseCore_GlobalValue_Manager.Menu_EDITOR_MENU + "Anima Editor")]
        public static void OpenEditor()
        {
            var window = GetWindow<AnimaEditor>();
            window.windowOpenMode = EditorMode.Normal;
            window.Show();
        }

        /// <summary>
        /// open Anima selector.
        /// </summary>
        public static void OpenSelector(Action<object, EventArgs> onSelect)
        {
            var window = GetWindow<AnimaEditor>();
            window.windowOpenMode = EditorMode.Selector;
            if (onSelect != null)
            {
                window.onSelectionEvent += (obj, arg) => {
                    onSelect.Invoke(obj, arg);
                };
            }
            window.Show();
        }

        /// <summary>
        /// refraichie.
        /// </summary>
        protected override void OnRedraw()
        {
            base.OnRedraw();
            if (Header())
            {
                GUILayout.BeginHorizontal();
                ScrollablePanel(() =>
                {
                    ListAnimations(editedAsset);
                    Foot();
                });
                ScrollablePanel(() =>
                {
                    AnimDetails(dataEdited);
                });
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// initialise.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            Initialisation();
        }


        #endregion
        #region Common Windows ################################################################

        /// <summary>
        /// l'entete.
        /// </summary>
        /// <returns></returns>
        public bool Header()
        {
            int categorySelect = (int)selectedCategory;
            int typeSelect = (int)selectedType;
            GroupGUInoStyle(() =>
            {
                selectedCategory = (AnimaManager.AnimaCategory)GUILayout.Toolbar((int)selectedCategory, Enum.GetNames(typeof(AnimaManager.AnimaCategory)));
            }, "Category", 50);
            GroupGUInoStyle(() =>
            {
                selectedType = (AnimaManager.AnimationType)GUILayout.Toolbar((int)selectedType, Enum.GetNames(typeof(AnimaManager.AnimationType)));
            }, "Type", 50);

            if (selectedCategory != (AnimaManager.AnimaCategory)categorySelect || selectedType != (AnimaManager.AnimationType)typeSelect)
            {
                Initialisation();
            }
            if (editedAsset)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Le peid de page.
        /// </summary>
        public void Foot()
        {
            SaveBarPanel(editedAsset, asset, onSelect);
        }

        /// <summary>
        /// Liste les animations.
        /// </summary>
        /// <param name="library"></param>
        public void ListAnimations(AnimaLibrary library)
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
                    for (int i = 0; i < library.AnimList.Count; i++)
                    {
                        var data = library.AnimList[i];
                        var name = data.Motion ? data.Motion.name : "null";
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
                    selectedDataIdx = ListItems(selectedDataIdx, listContent.ToArray());
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("+"))
                    {
                        library.AnimList.Add(new AnimaData { ID = maxId + 1, AnimLayer = (AnimaManager.AnimationLayer)selectedType, IsHumanMotion = selectedCategory == AnimaManager.AnimaCategory.humanoid });
                    }
                    if (selectedDataIdx >= 0 && selectedDataIdx < editedAsset.AnimList.Count)
                    {
                        if (GUILayout.Button("-"))
                        {
                            library.AnimList.RemoveAt(selectedDataIdx);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }, "Weapon Datas List");
                if (selectedDataIdx >= 0 && selectedDataIdx < editedAsset.AnimList.Count)
                    dataEdited = editedAsset.AnimList[selectedDataIdx];
                else
                    dataEdited = null;
            }
        }

        /// <summary>
        /// details.
        /// </summary>
        /// <param name="data"></param>
        public void AnimDetails(AnimaData data)
        {
            if (data == null)
                return;
            GroupGUI(() =>
            {
                EditorGUILayout.LabelField("ID: " + data.ID, EditorStyles.boldLabel);
            }, "Details");
        }

        #endregion
        #region Helpers & Tools ################################################################

        #endregion
    }
}