using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PulseEngine.Core;
using PulseEditor;
using System;
using System.Linq;


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
        private int selectedDataIndex;

        /// <summary>
        /// Tous les assets de localisation.
        /// </summary>
        private List<LocalisationLibrary> allAsset = new List<LocalisationLibrary>();

        /// <summary>
        /// L'asset chargee.
        /// </summary>
        private LocalisationLibrary asset;

        /// <summary>
        /// L'asset temporaire du meme type de data mais pas de la meme langue.
        /// </summary>
        private LocalisationLibrary auXasset;

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
        [MenuItem(PulseCore_GlobalValue_Manager.Menu_EDITOR_MENU + "Localisator Editor")]
        public static void OpenEditor()
        {
            var window = GetWindow<LocalisationEditor>();
            window.windowOpenMode = EditorMode.Normal;
            window.Show();
        }

        /// <summary>
        /// Open the editor
        /// </summary>
        [MenuItem(PulseCore_GlobalValue_Manager.Menu_EDITOR_MENU+"Localisator Selector")]
        public static void OpenSelector()
        {
            var window = GetWindow<LocalisationEditor>();
            window.windowOpenMode = EditorMode.Selector;
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
            List<string> typeNames = Enum.GetNames(typeof(PulseCore_GlobalValue_Manager.DataType)).ToList();
            typeNames.RemoveAt(0);
            selectedDataType = GUILayout.Toolbar(selectedDataType - 1, typeNames.ToArray());
            selectedDataType = Mathf.Clamp(selectedDataType, 1, selectedDataType);
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
                    if (editedAsset != null && asset != null)
                        SaveAsset(editedAsset, asset);
                    editedAsset = null;
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
                        new KeyValuePair<string, Action>("Close", ()=> { if(EditorUtility.DisplayDialog("Warning", "The Changes you made won't be saved.\n Proceed?","Yes","No")) Close();})
                    });
                    break;
                case EditorMode.Selector:
                    SaveCancelPanel(new[] {
                        new KeyValuePair<string, Action>("Select", ()=> { Select(editedData, true);}),
                        new KeyValuePair<string, Action>("Cancel", ()=> { if(EditorUtility.DisplayDialog("Warning", "The Selection you made won't be saved.\n Proceed?","Yes","No")) Close();})
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
                        for (int i = 0; i < editedAsset.localizedDatas.Count; i++) {
                            var data = editedAsset.localizedDatas[i];
                            if (data.Trad_ID > maxId) maxId = data.Trad_ID;
                            char[] title = new char[LIST_MAX_CHARACTERS];
                            for (int j = 0; j < data.Title.Length; j++)
                                title[j] = data.Title[j];
                            listContent.Add(new GUIContent { text = data.Trad_ID+"-"+title});
                        }
                        selectedDataIndex = ListItems(0, selectedDataIndex, listContent.ToArray());
                        GUILayout.Space(5);
                        if (GUILayout.Button("+"))
                        {
                            editedAsset.localizedDatas.Add(new Localisationdata { Trad_ID = maxId + 1 });
                        }
                        GUILayout.EndVertical();
                    }, "Localisation Datas List");
                });
            }
            //column two
            if(selectedDataIndex < editedAsset.localizedDatas.Count && selectedDataIndex >= 0)
            {
                editedData = editedAsset.localizedDatas[selectedDataIndex];
                if (windowOpenMode == EditorMode.Normal)
                    PageDetailsEdition(editedData);
                else if (windowOpenMode == EditorMode.Selector)
                    PageDetailsPreview(editedData);
            }

            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// Les details de la data selectionnee.
        /// </summary>
        private void PageDetailsEdition(Localisationdata data)
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
                VerticalScrollablePanel(1, () =>
                {
                    GroupGUI(() =>
                    {
                        GUILayout.BeginVertical();
                        //Trad ID
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Traduction ID: ", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Trad_ID.ToString());
                        GUILayout.EndHorizontal();
                        //Title
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Title :", EditorStyles.boldLabel);
                        data.Title = EditorGUILayout.TextField(data.Title);
                        GUILayout.EndHorizontal();
                        //Banner
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Banner :", EditorStyles.boldLabel);
                        data.Banner = EditorGUILayout.TextField(data.Banner);
                        GUILayout.EndHorizontal();
                        //GroupName
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("GroupName :", EditorStyles.boldLabel);
                        data.GroupName = EditorGUILayout.TextField(data.GroupName);
                        GUILayout.EndHorizontal();
                        //Header
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Header :", EditorStyles.boldLabel);
                        data.Header = EditorGUILayout.TextField(data.Header);
                        GUILayout.EndHorizontal();
                        //Infos
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Infos :", EditorStyles.boldLabel);
                        data.Infos = EditorGUILayout.TextArea(data.Infos);
                        GUILayout.EndHorizontal();
                        //Description
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Description :", EditorStyles.boldLabel);
                        data.Description = EditorGUILayout.TextArea(data.Description);
                        GUILayout.EndHorizontal();
                        //Details
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Details :", EditorStyles.boldLabel);
                        data.Details = EditorGUILayout.TextArea(data.Details);
                        GUILayout.EndHorizontal();
                        //ToolTip
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("ToolTip :", EditorStyles.boldLabel);
                        data.ToolTip = EditorGUILayout.TextArea(data.Details);
                        GUILayout.EndHorizontal();
                        //Child1
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child1 :", EditorStyles.boldLabel);
                        data.Child1 = EditorGUILayout.TextField(data.Child1);
                        GUILayout.EndHorizontal();
                        //Child2
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child2 :", EditorStyles.boldLabel);
                        data.Child2 = EditorGUILayout.TextField(data.Child2);
                        GUILayout.EndHorizontal();
                        //Child3
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child3 :", EditorStyles.boldLabel);
                        data.Child3 = EditorGUILayout.TextField(data.Child3);
                        GUILayout.EndHorizontal();
                        //Child4
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child4 :", EditorStyles.boldLabel);
                        data.Child4 = EditorGUILayout.TextField(data.Child4);
                        GUILayout.EndHorizontal();
                        //Child5
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child5 :", EditorStyles.boldLabel);
                        data.Child5 = EditorGUILayout.TextField(data.Child5);
                        GUILayout.EndHorizontal();
                        //Child6
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child6 :", EditorStyles.boldLabel);
                        data.Child6 = EditorGUILayout.TextField(data.Child6);
                        GUILayout.EndHorizontal();
                        //FootPage
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("FootPage :", EditorStyles.boldLabel);
                        data.FootPage = EditorGUILayout.TextField(data.FootPage);
                        GUILayout.EndHorizontal();
                        //Conclusion
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Conclusion :", EditorStyles.boldLabel);
                        data.Conclusion = EditorGUILayout.TextArea(data.Conclusion);
                        GUILayout.EndHorizontal();
                        //End
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("End :", EditorStyles.boldLabel);
                        data.End = EditorGUILayout.TextField(data.End);
                        GUILayout.EndHorizontal();

                        GUILayout.EndVertical();
                    }, data.Trad_ID + " Edition");
                });
            }
        }

        /// <summary>
        /// Les details de la data selectionnee.
        /// </summary>
        private void PageDetailsPreview(Localisationdata data)
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
                VerticalScrollablePanel(1, () =>
                {
                    GroupGUI(() =>
                    {
                        GUILayout.BeginVertical();
                        //Trad ID
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Traduction ID: ", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Trad_ID.ToString());
                        GUILayout.EndHorizontal();
                        //Title
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Title :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Title);
                        GUILayout.EndHorizontal();
                        //Banner
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Banner :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Banner);
                        GUILayout.EndHorizontal();
                        //GroupName
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("GroupName :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.GroupName);
                        GUILayout.EndHorizontal();
                        //Header
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Header :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Header);
                        GUILayout.EndHorizontal();
                        //Infos
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Infos :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Infos);
                        GUILayout.EndHorizontal();
                        //Description
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Description :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Description);
                        GUILayout.EndHorizontal();
                        //Details
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Details :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Details);
                        GUILayout.EndHorizontal();
                        //ToolTip
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("ToolTip :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Details);
                        GUILayout.EndHorizontal();
                        //Child1
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child1 :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Child1);
                        GUILayout.EndHorizontal();
                        //Child2
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child2 :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Child2);
                        GUILayout.EndHorizontal();
                        //Child3
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child3 :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Child3);
                        GUILayout.EndHorizontal();
                        //Child4
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child4 :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Child4);
                        GUILayout.EndHorizontal();
                        //Child5
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child5 :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Child5);
                        GUILayout.EndHorizontal();
                        //Child6
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child6 :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Child6);
                        GUILayout.EndHorizontal();
                        //FootPage
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("FootPage :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.FootPage);
                        GUILayout.EndHorizontal();
                        //Conclusion
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Conclusion :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Conclusion);
                        GUILayout.EndHorizontal();
                        //End
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("End :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.End);
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
            if (!Header())
                return;
            GUILayout.Space(20);
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
            editedAsset = null;
            asset = null;
            auXasset = null;
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
                    if (type == PulseCore_GlobalValue_Manager.DataType.None)
                        continue;
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
        }

        /// <summary>
        /// Sauvegarde les changements et ferme la fenetre si besoin.
        /// </summary>
        /// <param name="close"></param>
        private void Save(bool close = false)
        {
            for (int i = 0, len = allAsset.Count; i < len; i++)
            {
                auXasset = allAsset[i];
                var otherAsset = auXasset;
                for (int j = 0, len2 = editedAsset.localizedDatas.Count; j < len2; j++)
                {
                    var data = editedAsset.localizedDatas[j];
                    if (otherAsset.LibraryDataType == editedAsset.LibraryDataType)
                    {
                        if (otherAsset.localizedDatas.FindIndex(d => { return d.Trad_ID == data.Trad_ID; }) < 0)
                        {
                            otherAsset.localizedDatas.Add(new Localisationdata { Trad_ID = data.Trad_ID });
                            EditorUtility.CopySerialized(otherAsset, auXasset);
                        }
                    }
                }
            }
            SaveAsset(editedAsset, asset);
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