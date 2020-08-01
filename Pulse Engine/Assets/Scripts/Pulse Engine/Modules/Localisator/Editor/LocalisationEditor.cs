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

        /// <summary>
        /// le type data du hash tag selectionne.
        /// </summary>
        private int hashtag_dataTypeIndex;

        /// <summary>
        /// le type data du hash tag selectionne.
        /// </summary>
        private int hashtag_id;

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
        [MenuItem(PulseCore_GlobalValue_Manager.Menu_EDITOR_MENU + "Localisator Selector")]
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
            PulseCore_GlobalValue_Manager.Languages langue = PulseCore_GlobalValue_Manager.Languages.Francais;
            GroupGUInoStyle(() =>
            {
                selectedLangage = GUILayout.Toolbar(selectedLangage, Enum.GetNames(typeof(PulseCore_GlobalValue_Manager.Languages)));
                switch ((PulseCore_GlobalValue_Manager.Languages)selectedLangage)
                {
                    case PulseCore_GlobalValue_Manager.Languages.English:
                        langue = PulseCore_GlobalValue_Manager.Languages.English;
                        break;
                    default:
                        langue = PulseCore_GlobalValue_Manager.Languages.Francais;
                        break;
                }
            }, "Language",50);
            PulseCore_GlobalValue_Manager.DataType dataType = PulseCore_GlobalValue_Manager.DataType.None;
            GroupGUInoStyle(() =>
            {
                List<string> typeNames = Enum.GetNames(typeof(PulseCore_GlobalValue_Manager.DataType)).ToList();
                typeNames.RemoveAt(0);
                selectedDataType = GUILayout.Toolbar(selectedDataType - 1, typeNames.ToArray());
                selectedDataType = Mathf.Clamp(selectedDataType, 1, selectedDataType);
                switch ((PulseCore_GlobalValue_Manager.DataType)selectedDataType)
                {
                    case PulseCore_GlobalValue_Manager.DataType.CharacterInfos:
                        dataType = PulseCore_GlobalValue_Manager.DataType.CharacterInfos;
                        break;
                    default:
                        dataType = PulseCore_GlobalValue_Manager.DataType.None;
                        break;
                }
            }, "Type",50);
            if (dataType == PulseCore_GlobalValue_Manager.DataType.None)
                return false;
            if (asset == null)
            {
                if (allAsset.Count > 0)
                {
                    int index = allAsset.FindIndex(library => { return library.LibraryLanguage == langue && library.LibraryDataType == dataType; });
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
            GUILayout.BeginVertical();
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
                VerticalScrollablePanel(() =>
                {
                    GroupGUI(() =>
                    {
                        GUILayout.BeginVertical();
                        List<GUIContent> listContent = new List<GUIContent>();
                        int maxId = 0;
                        for (int i = 0; i < editedAsset.LocalizedDatas.Count; i++) {
                            var data = editedAsset.LocalizedDatas[i];
                            char[] titleChars = new char[LIST_MAX_CHARACTERS];
                            string pointDeSuspension = string.Empty;
                            try
                            {
                                if (data.Trad_ID > maxId) maxId = data.Trad_ID;
                                for (int j = 0; j < titleChars.Length; j++)
                                    if (j < data.Title.Length)
                                        titleChars[j] = data.Title[j];
                                if (data.Title.Length >= titleChars.Length)
                                    pointDeSuspension = "...";
                            }
                            catch { }
                            string title = new string(titleChars) + pointDeSuspension;
                            listContent.Add(new GUIContent { text = data != null ? data.Trad_ID + "-" + title : "null data" });
                        }
                        selectedDataIndex = ListItems(selectedDataIndex, listContent.ToArray());
                        GUILayout.Space(5);
                        if (GUILayout.Button("+"))
                        {
                            editedAsset.LocalizedDatas.Add(new Localisationdata { Trad_ID = maxId + 1 });
                        }
                        GUILayout.EndVertical();
                    }, "Localisation Datas List");
                });
            }
            GUILayout.Space(5);
            HashTagGenerator();
            GUILayout.Space(5);
            FootPage();
            GUILayout.EndVertical();
            GUILayout.Space(5);
            //column two
            try
            {
                if (selectedDataIndex < editedAsset.LocalizedDatas.Count && selectedDataIndex >= 0)
                {
                    editedData = editedAsset.LocalizedDatas[selectedDataIndex];
                    if (windowOpenMode == EditorMode.Normal)
                        PageDetailsEdition(editedData);
                    else if (windowOpenMode == EditorMode.Selector)
                        PageDetailsPreview(editedData);
                }
            }
            catch { }

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
                VerticalScrollablePanel(() =>
                {
                    GroupGUI(() =>
                    {
                        GUILayout.BeginVertical();
                        //Trad ID
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Traduction ID: ", style_label);
                        GUILayout.Label(data.Trad_ID.ToString());
                        GUILayout.EndHorizontal();
                        //Title
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Title :", style_label);
                        data.Title = EditorGUILayout.TextArea(data.Title);
                        GUILayout.EndHorizontal();
                        //Banner
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Banner :", style_label);
                        data.Banner = EditorGUILayout.TextArea(data.Banner);
                        GUILayout.EndHorizontal();
                        //GroupName
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("GroupName :", style_label);
                        data.GroupName = EditorGUILayout.TextArea(data.GroupName);
                        GUILayout.EndHorizontal();
                        //Header
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Header :", style_label);
                        data.Header = EditorGUILayout.TextArea(data.Header);
                        GUILayout.EndHorizontal();
                        //Infos
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Infos :", style_label);
                        data.Infos = EditorGUILayout.TextArea(data.Infos, style_txtArea);
                        GUILayout.EndHorizontal();
                        //Description
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Description :", style_label);
                        data.Description = EditorGUILayout.TextArea(data.Description);
                        GUILayout.EndHorizontal();
                        //Details
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Details :", style_label);
                        data.Details = EditorGUILayout.TextArea(data.Details);
                        GUILayout.EndHorizontal();
                        //ToolTip
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ToolTip :", style_label);
                        data.ToolTip = EditorGUILayout.TextArea(data.Details);
                        GUILayout.EndHorizontal();
                        //Child1
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Child1 :", style_label);
                        data.Child1 = EditorGUILayout.TextArea(data.Child1);
                        GUILayout.EndHorizontal();
                        //Child2
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Child2 :", style_label);
                        data.Child2 = EditorGUILayout.TextArea(data.Child2);
                        GUILayout.EndHorizontal();
                        //Child3
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Child3 :", style_label);
                        data.Child3 = EditorGUILayout.TextArea(data.Child3);
                        GUILayout.EndHorizontal();
                        //Child4
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Child4 :", style_label);
                        data.Child4 = EditorGUILayout.TextArea(data.Child4);
                        GUILayout.EndHorizontal();
                        //Child5
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Child5 :", style_label);
                        data.Child5 = EditorGUILayout.TextArea(data.Child5);
                        GUILayout.EndHorizontal();
                        //Child6
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Child6 :", style_label);
                        data.Child6 = EditorGUILayout.TextArea(data.Child6);
                        GUILayout.EndHorizontal();
                        //FootPage
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("FootPage :", style_label);
                        data.FootPage = EditorGUILayout.TextArea(data.FootPage);
                        GUILayout.EndHorizontal();
                        //Conclusion
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Conclusion :", style_label);
                        data.Conclusion = EditorGUILayout.TextArea(data.Conclusion);
                        GUILayout.EndHorizontal();
                        //End
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("End :", style_label);
                        data.End = EditorGUILayout.TextArea(data.End);
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
                VerticalScrollablePanel(() =>
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
        /// le genersteur de hashTag.
        /// </summary>
        private void HashTagGenerator()
        {
            if (allAsset == null || allAsset.Count <= 0)
                return;
            VerticalScrollablePanel(() =>
            {
                GroupGUInoStyle(() =>
                {
                    GUILayout.BeginVertical();
                    var IEdataTypes = from a in allAsset
                                      where a.LibraryLanguage == (PulseCore_GlobalValue_Manager.Languages)selectedLangage
                                      select a.LibraryDataType.ToString();
                    string[] dataTypes = IEdataTypes.ToArray();
                    hashtag_dataTypeIndex = EditorGUILayout.Popup(hashtag_dataTypeIndex, dataTypes);
                    var dataType = allAsset[hashtag_dataTypeIndex].LibraryDataType;
                    var filteredList = from a in allAsset
                                       where a.LibraryDataType == dataType && a.LibraryLanguage == (PulseCore_GlobalValue_Manager.Languages)selectedLangage
                                       select a;
                    var target = filteredList.FirstOrDefault();
                    int index = 0;
                    if (target)
                    {
                        index = target.LocalizedDatas.FindIndex(item => { return item.Trad_ID == hashtag_id; });
                        var ieListNames = from i in target.LocalizedDatas
                                          select i.Trad_ID + "-" + i.Title;
                        index = Mathf.Clamp(index, 0, index);
                        var listNames = ieListNames.ToArray();
                        index = EditorGUILayout.Popup(index, listNames);
                        hashtag_id = target.LocalizedDatas[index].Trad_ID;
                        if(hashtag_id > 0)
                            EditorGUILayout.DelayedTextField(MakeTag(hashtag_id, dataType));
                    }
                    GUILayout.EndVertical();

                },"HashTag Maker", 50);
            });
        }

        /// <summary>
        /// Window refresh.
        /// </summary>
        protected override void OnRedraw()
        {
            if (!Header())
                return;
            GUILayout.Space(20);
            PageBody();
        }

        #endregion

        #region Methods #######################################################################################################

        /// <summary>
        /// A l'ouverture de la fenetre.
        /// </summary>
        protected override void OnInitialize()
        {
            Initialise();
            switch (windowOpenMode)
            {
                case EditorMode.Normal:
                    //onSelectionEvent = delegate { };
                    break;
                case EditorMode.Selector:
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
                for (int j = 0, len2 = editedAsset.LocalizedDatas.Count; j < len2; j++)
                {
                    var data = editedAsset.LocalizedDatas[j];
                    if (otherAsset.LibraryDataType == editedAsset.LibraryDataType)
                    {
                        if (otherAsset.LocalizedDatas.FindIndex(d => { return d.Trad_ID == data.Trad_ID; }) < 0)
                        {
                            otherAsset.LocalizedDatas.Add(new Localisationdata { Trad_ID = data.Trad_ID });
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
            if(onSelectionEvent != null)
                onSelectionEvent.Invoke(this, eventArgs);
            if (close)
                Close();
        }

        /// <summary>
        /// Cree un Hashtag d'un element de type T.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string MakeTag(int id, PulseCore_GlobalValue_Manager.DataType T)
        {
            string hashTag = "#" + id + "_" + (int)T + "#";
            return hashTag;
        }

        #endregion
    }
}