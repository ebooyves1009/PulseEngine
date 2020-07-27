using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PulseEngine.Core;
using PulseEditor;
using System;


//TODO: Implementer le fonctionnel de la fenetre avec la liste les data et leur edition.
namespace PulseEngine.Module.Localisator.AssetEditor
{
    /// <summary>
    /// L'editeur de localisation.
    /// </summary>
    public class LocalisationEditor : PulseEngine_Core_BaseEditor
    {
        #region Attributs ##################################################################################

        /// <summary>
        /// La langue choisie.
        /// </summary>
        private int selectedLangage;

        /// <summary>
        /// Le type de data choisie.
        /// </summary>
        private int selectedDataType;

        /// <summary>
        /// L'asset chargee.
        /// </summary>
        private LocalisationLibrary asset;

        /// <summary>
        /// L'asset en cours de modification.
        /// </summary>
        private LocalisationLibrary editedAsset;

        #endregion

        #region GUIFunctions ###############################################################################
        /// <summary>
        /// Open the editor
        /// </summary>
        [MenuItem(PulseCore_GlobalValue_Manager.Menu_EDITOR_MENU+"Localisator Editor")]
        public static void OpenEditor()
        {
            var window = GetWindow<LocalisationEditor>();
            window.Show();
        }

        /// <summary>
        /// La tete de fenetre.
        /// </summary>
        /// <returns></returns>
        private bool Header()
        {
            EditorGUILayout.LabelField("Select Language", EditorStyles.boldLabel);
            selectedLangage = GUILayout.Toolbar(selectedLangage, Enum.GetNames(typeof(PulseCore_GlobalValue_Manager.Languages)));
            PulseCore_GlobalValue_Manager.Languages langue = PulseCore_GlobalValue_Manager.Languages.Francais;
            switch ((PulseCore_GlobalValue_Manager.Languages)selectedLangage)
            {
                case PulseCore_GlobalValue_Manager.Languages.English:
                    langue = PulseCore_GlobalValue_Manager.Languages.English;
                    break;
                default:
                    langue = PulseCore_GlobalValue_Manager.Languages.Francais;
                    break;
            }
            EditorGUILayout.LabelField("Select Type", EditorStyles.boldLabel);
            selectedDataType = GUILayout.Toolbar(selectedDataType, Enum.GetNames(typeof(PulseCore_GlobalValue_Manager.DataType)));
            PulseCore_GlobalValue_Manager.DataType dataType = PulseCore_GlobalValue_Manager.DataType.None;
            switch ((PulseCore_GlobalValue_Manager.DataType)selectedDataType)
            {
                case PulseCore_GlobalValue_Manager.DataType.CharacterInfos:
                    dataType = PulseCore_GlobalValue_Manager.DataType.CharacterInfos;
                    break;
                default:
                    dataType = PulseCore_GlobalValue_Manager.DataType.None;
                    break;
            }
            if (dataType == PulseCore_GlobalValue_Manager.DataType.None)
                return false;
            if (asset == null)
            {
                if (LocalisationLibrary.Exist(langue, dataType))
                    asset = LocalisationLibrary.Load(langue, dataType);
                else
                    return false;
            }
            else if (asset.LibraryDataType != dataType || asset.LibraryLanguage != langue)
            {
                if (LocalisationLibrary.Exist(langue, dataType))
                    asset = LocalisationLibrary.Load(langue, dataType);
                else
                    return false;
            }
            if (editedAsset == null || editedAsset.name != asset.name)
                editedAsset = asset;
            if (editedAsset == null)
                return false;
            return true;
        }

        /// <summary>
        /// Window refresh.
        /// </summary>
        private void OnGUI()
        {
            if (!Header())
                return;
            VerticalScrollablePanel(0, () =>
            {
                GroupGUI(() =>
                {
                    if (GUILayout.Button("Check Asset"))
                    {
                        if (LocalisationLibrary.Create(PulseCore_GlobalValue_Manager.Languages.Francais, PulseCore_GlobalValue_Manager.DataType.CharacterInfos))
                            Debug.Log("Created");
                        else
                            Debug.Log("Unfortunatly...");
                    }
                }, "Test group Asset");
            });

        }

        #endregion
    }
}
