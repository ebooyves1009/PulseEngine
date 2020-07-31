using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PulseEngine.Core;
using PulseEditor;
using System;


//TODO: implementer les details de la data.
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
        /// L'index de la data choisie.
        /// </summary>
        private int selectedData;

        /// <summary>
        /// Tous les assets de localisation.
        /// </summary>
        private List<LocalisationLibrary> allAsset = new List<LocalisationLibrary>();

        /// <summary>
        /// L'asset chargee.
        /// </summary>
        private LocalisationLibrary asset;

        /// <summary>
        /// L'asset en cours de modification.
        /// </summary>
        private LocalisationLibrary editedAsset;

        /// <summary>
        /// La data de localisation en cours d'edition.
        /// </summary>
        private Localisationdata editedData;

        #endregion

        #region GUIFunctions ###############################################################################

        /// <summary>
        /// Open the editor
        /// </summary>
        [MenuItem(PulseCore_GlobalValue_Manager.Menu_EDITOR_MENU+"Localisator Editor")]
        public static void OpenEditor()
        {
            var window = GetWindow<LocalisationEditor>();
            window.windowOpenMode = EditorMode.Normal;
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
                if (allAsset.Count > 0)
                {
                    int index = allAsset.FindIndex(library => { return library.LibraryLanguage == langue && library.LibraryDataType == dataType; });
                    Debug.Log("Selected asset index is " + index);
                    if (index >= 0)
                        asset = allAsset[index];
                    else
                        return false;
                }
                else
                    return false;
            }
            else if (asset.LibraryDataType != dataType || asset.LibraryLanguage != langue)
            {
                int index = allAsset.FindIndex(library => { return library.LibraryLanguage == langue && library.LibraryDataType == dataType; });
                Debug.Log("different asset index is " + index);
                if (index >= 0)
                    asset = allAsset[index];
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
        /// Le pied de fenetre.
        /// </summary>
        private void FootPage()
        {
            if (editedAsset == null)
                return;
            switch (windowOpenMode)
            {
                case EditorMode.Normal:
                    SaveCancelPanel(new[] {
                        new KeyValuePair<string, Action>("Save & Close", ()=> { Save(true);}),
                        new KeyValuePair<string, Action>("Close", ()=> { Close();})
                    });
                    break;
                case EditorMode.Selector:
                    SaveCancelPanel(new[] {
                        new KeyValuePair<string, Action>("Select", ()=> { Select(editedData, true);}),
                        new KeyValuePair<string, Action>("Cancel", ()=> { Close();})
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

        /// <summary>
        /// Le corps de page.
        /// </summary>
        private void PageBody()
        {
            if (editedAsset == null)
                return;

            GUILayout.BeginHorizontal();
            //Column one
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
                VerticalScrollablePanel(0, () =>
                {
                    GroupGUI(() =>
                    {
                        GUILayout.BeginVertical();
                        List<GUIContent> listContent = new List<GUIContent>();
                        int maxId = 0;
                        for (int i = 0; i < editedAsset.LocalizedDatas.Count; i++) {
                            var data = editedAsset.LocalizedDatas[i];
                            Debug.Log(editedAsset.name + " , item " + i + " is " + data);
                            if (data.Trad_ID > maxId) maxId = data.Trad_ID;
                            listContent.Add(new GUIContent { text = data.Trad_ID+"-"+data.Title});
                        }
                        selectedData = ListItems(0, selectedData, listContent.ToArray());
                        GUILayout.Space(5);
                        if (GUILayout.Button("+"))
                        {
                            editedAsset.LocalizedDatas.Add(new Localisationdata { Trad_ID = maxId + 1 });
                        }
                        GUILayout.EndVertical();
                    }, "Localisation Datas List");
                });
            }
            //column two
            if(selectedData < editedAsset.LocalizedDatas.Count && selectedData >= 0)
            {
                editedData = editedAsset.LocalizedDatas[selectedData];
                PageDetails(editedData);
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Les details de la data selectionnee.
        /// </summary>
        private void PageDetails(Localisationdata data)
        {
            if (data == null)
                return;
            Func<bool> detailsCompatiblesmode = () =>
            {
                switch (windowOpenMode)
                {
                    case EditorMode.Normal:
                        return true;
                    case EditorMode.Selector:
                        return false;
                    case EditorMode.ItemEdition:
                        return true;
                    case EditorMode.Preview:
                        return true;
                    default:
                        return true;
                }
            };
            if (detailsCompatiblesmode())
            {
                VerticalScrollablePanel(0, () =>
                {
                    GroupGUI(() =>
                    {
                        GUILayout.BeginVertical();
                    //Trad ID
                    GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Traduction ID: ", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Trad_ID.ToString());
                        GUILayout.EndHorizontal();

                        GUILayout.EndVertical();
                    }, data.Trad_ID + " Edition");
                });
            }
        }

        /// <summary>
        /// Window refresh.
        /// </summary>
        private void OnGUI()
        {
            if (windowOpenMode == EditorMode.Normal)
            {
                if (!Header())
                    return;
            }
            PageBody();
            FootPage();
        }

        #endregion

        #region Methods #######################################################################################################

        /// <summary>
        /// A l'ouverture de la fenetre.
        /// </summary>
        private void OnEnable()
        {
            Initialise();
            Focus();
        }

        /// <summary>
        /// A la fermeture.
        /// </summary>
        private void OnDisable()
        {
            allAsset.Clear();
            CloseWindow();
        }

        /// <summary>
        /// Initialise la fenetre.
        /// </summary>
        /// <param name="close"></param>
        private void Initialise()
        {
            foreach(PulseCore_GlobalValue_Manager.Languages langue in Enum.GetValues(typeof(PulseCore_GlobalValue_Manager.Languages)))
            {
                foreach(PulseCore_GlobalValue_Manager.DataType type in Enum.GetValues(typeof(PulseCore_GlobalValue_Manager.DataType)))
                {
                    if (LocalisationLibrary.Exist(langue, type))
                    {
                        var load = LocalisationLibrary.Load(langue, type);
                        if (load != null)
                            allAsset.Add(load);
                    }
                    else if (LocalisationLibrary.Create(langue, type))
                    {
                        var load = LocalisationLibrary.Load(langue, type);
                        if (load != null)
                            allAsset.Add(load);
                    }
                }
            }
            Debug.Log("Found " + allAsset.Count + " assets");
        }

        /// <summary>
        /// Sauvegarde les changements et ferme la fenetre si besoin.
        /// </summary>
        /// <param name="close"></param>
        private void Save(bool close = false)
        {
            SaveAsset(editedAsset, asset);
            foreach(var otherAsset in allAsset)
            {
                if(otherAsset.LibraryDataType == asset.LibraryDataType)
                {
                    foreach(var data in asset.LocalizedDatas)
                    {
                        if (otherAsset.LocalizedDatas.FindIndex(d => { return d.Trad_ID == data.Trad_ID; }) < 0)
                        {
                            otherAsset.LocalizedDatas.Add(new Localisationdata { Trad_ID = data.Trad_ID });
                            Debug.Log("other asset modded");
                        }
                    }
                }
            }
            AssetDatabase.SaveAssets();
            if (close)
                Close();
        }

        /// <summary>
        /// Sauvegarde les changements et ferme la fenetre si besoin.
        /// </summary>
        /// <param name="close"></param>
        private void Select(Localisationdata data, bool close = false)
        {
            if (data == null || editedAsset == null)
                return;
            EditorEventArgs eventArgs = new EditorEventArgs
            {
                ID = data.Trad_ID,
                dataType = (int)editedAsset.LibraryDataType
            };
            onSelectionEvent.Invoke(this, eventArgs);
            if (close)
                Close();
        }

            #endregion
    }
}