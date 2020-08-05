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



namespace PulseEditor.Module.CombatSystem
{
    /// <summary>
    /// L'editeur d'arsenal.
    /// </summary>
    public class WeaponEditor : PulseEngine_Core_BaseEditor
    {
        #region Fonctionnal Attributes ################################################################

        /// <summary>
        /// toutes les armes de tous les types.
        /// </summary>
        private List<WeaponLibrary> allAssets = new List<WeaponLibrary>();

        /// <summary>
        /// L'asset permanent.
        /// </summary>
        private WeaponLibrary asset = null;

        /// <summary>
        /// L'asset temporaire en cours de modification.
        /// </summary>
        private WeaponLibrary EditedAsset = null;

        /// <summary>
        /// La data en cours de modification.
        /// </summary>
        private WeaponData data = null;


        #endregion
        #region Visual Attributes ################################################################

        /// <summary>
        /// Le type d'arme selectionne.
        /// </summary>
        private CombatSystemManager.WeaponType weaponTypeSelected;

        /// <summary>
        /// l'index de la data choisie dans la liste des datas.
        /// </summary>
        private int indexDataSelected;


        #endregion
        #region Fonctionnal Methods ################################################################

        #endregion
        #region Visual Methods ################################################################

        /// <summary>
        /// open weapon editor.
        /// </summary>
        [MenuItem(PulseCore_GlobalValue_Manager.Menu_EDITOR_MENU + "Weapon Editor")]
        public static void OpenEditor()
        {
            var window = GetWindow<WeaponEditor>();
            window.windowOpenMode = EditorMode.Normal;
            window.Show();
        }

        /// <summary>
        /// open weapon selector.
        /// </summary>
        public static void OpenSelector(Action<object,EventArgs> onSelect)
        {
            var window = GetWindow<WeaponEditor>();
            window.windowOpenMode = EditorMode.Selector;
            if(onSelect != null)
            {
                window.onSelectionEvent += (obj, arg) => {
                    onSelect.Invoke(obj, arg);
                };
            }
            window.Show();
        }

        /// <summary>
        /// Affiche l'arsenal correspondant aux id qu'il recoit.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="winName"></param>
        public static void ShowArmury(List<int> weaponsId, Action<object> returneWeaponry, string winName = "")
        {

        }

        #endregion
        #region Common Windows ################################################################

        /// <summary>
        /// Initialisation.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            allAssets = LibraryFiller(allAssets);
            allAssets.ForEach(a => { if (a.LibraryWeaponType == weaponTypeSelected) asset = a; });
            if (asset)
                EditedAsset = asset;
        }


        /// <summary>
        /// Refresh.
        /// </summary>
        protected override void OnRedraw()
        {
            base.OnRedraw();

            VerticalScrollablePanel(() =>
            {
                GroupGUI(() =>
                {
                    GUILayout.BeginVertical();
                    Header();

                    GUILayout.EndVertical();
                }, "Stats");

                GroupGUInoStyle(() =>
                {
                    SaveCancelPanel(new[] {
                        new KeyValuePair<string, System.Action> ( "Save", () => {

                            Close(); } ),
                        new KeyValuePair<string, System.Action>("Cancel", () => { Close(); })
                    });
                });
            });
        }

        /// <summary>
        /// The header.
        /// </summary>
        protected bool Header()
        {
            var bkpType = weaponTypeSelected;
            GroupGUInoStyle(() =>
            {
                int selected = GUILayout.Toolbar((int)weaponTypeSelected , Enum.GetNames(typeof(CombatSystemManager.WeaponType)));
                weaponTypeSelected = (CombatSystemManager.WeaponType)selected;
            });
            if (bkpType != weaponTypeSelected)
            {
                asset = null;
                EditedAsset = null;
                data = null;
                OnInitialize();
            }
            return asset != null;
        }

        #endregion
        #region Helpers & Tools ################################################################

        /// <summary>
        /// Charge tous les assets d'arme.
        /// </summary>
        /// <param name="inputLibrary"></param>
        /// <returns></returns>
        private static List<WeaponLibrary> LibraryFiller(List<WeaponLibrary> inputLibrary)
        {
            if (inputLibrary == null)
                inputLibrary = new List<WeaponLibrary>();
            foreach(CombatSystemManager.WeaponType type in Enum.GetValues(typeof(CombatSystemManager.WeaponType)))
            {
                if (WeaponLibrary.Exist(type))
                    inputLibrary.Add(WeaponLibrary.Load(type));
                else if(WeaponLibrary.Create(type))
                    inputLibrary.Add(WeaponLibrary.Load(type));
            }
            return inputLibrary;
        }

        #endregion
    }
}