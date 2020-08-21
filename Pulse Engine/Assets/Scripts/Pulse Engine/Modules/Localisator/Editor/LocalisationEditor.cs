using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using PulseEditor.Globals;
using PulseEngine.Globals;
using PulseEngine.Modules;
using PulseEngine.Modules.Localisator;
using UILayout = UnityEngine.GUILayout;
using EILayout = UnityEditor.EditorGUILayout;


//TODO: implementer les details de la data.
namespace PulseEditor.Modules.Localisator
{
    /// <summary>
    /// L'editeur de localisation.
    /// </summary>
    public class LocalisationEditor : PulseEditorMgr
    {
        #region Attributs ##################################################################################

        /// <summary>
        /// La langue choisie.
        /// </summary>
        private Languages selectedLangage;

        /// <summary>
        /// Le type de data choisie.
        /// </summary>
        private TradDataTypes selectedDataType;

        /// <summary>
        /// L'asset temporaire du meme type de data mais pas de la meme langue.
        /// </summary>
        private LocalisationLibrary auXasset;

        /// <summary>
        /// le type data du hash tag selectionne.
        /// </summary>
        private int hashtag_dataTypeIndex;

        /// <summary>
        /// le type data du hash tag selectionne.
        /// </summary>
        private int hashtag_id;

        #endregion

        #region Static Functions ###########################################################################

        /// <summary>
        /// Open the editor
        /// </summary>
        [MenuItem(Menu_EDITOR_MENU + "Localisator Editor")]
        public static void OpenEditor()
        {
            var window = GetWindow<LocalisationEditor>();
            window.windowOpenMode = EditorMode.Normal;
            window.Show();
        }

        /// <summary>
        /// Open the selector
        /// </summary>
        public static void OpenSelector(Action<object, EventArgs> onSelect, TradDataTypes dtype)
        {
            var window = GetWindow<LocalisationEditor>(true, "Localisator Selector");
            window.windowOpenMode = EditorMode.Selector;
            window.selectedDataType = dtype;
            if (onSelect != null)
            {
                window.onSelectionEvent += (obj, arg) =>
                {
                    onSelect.Invoke(obj, arg);
                };
            }
            window.ShowAuxWindow();
        }

        /// <summary>
        /// Open the Item selector
        /// </summary>
        public static void OpenModifier(int _id, TradDataTypes dType)
        {
            var window = GetWindow<LocalisationEditor>(true, "Localisator Modifier");
            window.windowOpenMode = EditorMode.ItemEdition;
            window.selectedDataType = dType;
            window.dataID = _id;
            window.ShowAuxWindow();
        }

        /// <summary>
        /// Request a text on editor mode
        /// </summary>
        public static string[] GetTexts(int _id, TradDataTypes dType)
        {
            List<string> retList = new List<string>();
            var allAsset = new List<LocalisationLibrary>();
            LocalisationLibrary asset = null;

            foreach (Languages langue in Enum.GetValues(typeof(Languages)))
            {
                foreach (TradDataTypes type in Enum.GetValues(typeof(TradDataTypes)))
                {
                    if (LocalisationLibrary.Exist(langue, type))
                    {
                        var load = LocalisationLibrary.Load(langue, type);
                        if (load != null)
                            allAsset.Add(load);
                    }
                    else if (LocalisationLibrary.Save(langue, type))
                    {
                        var load = LocalisationLibrary.Load(langue, type);
                        if (load != null)
                            allAsset.Add(load);
                    }
                }
            }

            if (allAsset != null && allAsset.Count > 0 && _id > 0)
            {
                int index = allAsset.FindIndex(library => { return library.Langage == PulseEngineMgr.currentLanguage && library.TradType == dType; });
                if (index >= 0)
                    asset = allAsset[index];
                var data = asset.DatasList.Find(dt => { return dt.ID == _id; });
                if (data != null)
                {
                    retList.Add(data.Title.s_textField);
                    retList.Add(data.Description.s_textField);
                    retList.Add(data.Details.s_textField);
                }
            }
            return retList.ToArray();
        }

        #endregion

        #region GUIFunctions ###############################################################################


        /// <summary>
        /// La tete de fenetre.
        /// </summary>
        /// <returns></returns>
        private bool Header(Languages langue = Languages.Francais , TradDataTypes tradDataType = TradDataTypes.Person)
        {
            if (windowOpenMode == EditorMode.Normal)
            {
                langue = Languages.Francais;
                GroupGUInoStyle(() =>
                {
                    selectedLangage = (Languages)GUILayout.Toolbar((int)selectedLangage, Enum.GetNames(typeof(Languages)));
                    switch (selectedLangage)
                    {
                        case Languages.English:
                            langue = Languages.English;
                            break;
                        default:
                            langue = Languages.Francais;
                            break;
                    }
                }, "Language", 50);
                tradDataType = TradDataTypes.Person;
                GroupGUInoStyle(() =>
                {
                    selectedDataType = (TradDataTypes)EILayout.EnumPopup(selectedDataType);
                    //selectedDataType = Mathf.Clamp(selectedDataType, 1, selectedDataType);
                    tradDataType = selectedDataType;
                }, "Type", 50);
            }
            if (asset == null)
            {
                if (allAssets.Count > 0)
                {
                    int index = allAssets.FindIndex(library => { return ((LocalisationLibrary)library).Langage == langue && ((LocalisationLibrary)library).TradType == tradDataType; });
                    if (editedAsset != null && asset != null)
                        SaveAsset(editedAsset, asset);
                    editedAsset = null;
                    if (index >= 0)
                        asset = allAssets[index];
                    else
                        return false;
                }
                else
                    return false;
            }
            else if (((LocalisationLibrary)asset).TradType != tradDataType || ((LocalisationLibrary)asset).Langage != langue)
            {
                editedData = null;
                selectDataIndex = -1;
                int index = allAssets.FindIndex(library => { return ((LocalisationLibrary)library).Langage == langue && ((LocalisationLibrary)library).TradType == tradDataType; });
                if (index >= 0)
                    asset = allAssets[index];
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
                        new KeyValuePair<string, Action>("Save", ()=> { Save(false);}),
                        new KeyValuePair<string, Action>("Close", ()=> { if(EditorUtility.DisplayDialog("Warning", "The Changes you made won't be saved.\n Proceed?","Yes","No")) Close();})
                    });
                    break;
                case EditorMode.Selector:
                    SaveCancelPanel(new[] {
                        new KeyValuePair<string, Action>("Select", ()=> { Select((Localisationdata)editedData, true);}),
                        new KeyValuePair<string, Action>("Cancel", ()=> { if(EditorUtility.DisplayDialog("Warning", "The Selection you made won't be saved.\n Proceed?","Yes","No")) Close();})
                    });
                    break;
                case EditorMode.ItemEdition:
                    SaveCancelPanel(new[] {
                        new KeyValuePair<string, Action>("Save & Close", ()=> { Save(true);}),
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
        /// Le corps de page.
        /// </summary>
        private void PageBody()
        {
            var _editedAsset = editedAsset as LocalisationLibrary;
            if (_editedAsset == null)
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
                ScrollablePanel(() =>
                {
                    GroupGUI(() =>
                    {
                        GUILayout.BeginVertical();
                        List<GUIContent> listContent = new List<GUIContent>();
                        int maxId = 0;
                        for (int i = 0; i < _editedAsset.DatasList.Count; i++)
                        {
                            var data = _editedAsset.DatasList[i];
                            char[] titleChars = new char[LIST_MAX_CHARACTERS];
                            string pointDeSuspension = string.Empty;
                            try
                            {
                                if (data.ID > maxId) maxId = data.ID;
                                for (int j = 0; j < titleChars.Length; j++)
                                    if (j < data.Title.s_textField.Length)
                                        titleChars[j] = data.Title.s_textField[j];
                                if (data.Title.s_textField.Length >= titleChars.Length)
                                    pointDeSuspension = "...";
                            }
                            catch { }
                            string title = new string(titleChars) + pointDeSuspension;
                            listContent.Add(new GUIContent { text = data != null ? data.ID + "-" + title : "null data" });
                        }
                        selectDataIndex = ListItems(selectDataIndex, listContent.ToArray());
                        GUILayout.Space(5);
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("+"))
                        {
                            _editedAsset.DatasList.Add(new Localisationdata { ID = maxId + 1 });
                        }
                        if (selectDataIndex >= 0)
                        {
                            if (GUILayout.Button("-"))
                            {
                                _editedAsset.DatasList.RemoveAt(selectDataIndex);
                            }
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                    }, "Localisation Datas List");
                    GUILayout.Space(5);
                    HashTagGenerator();
                    FootPage();
                }, true);
            }else if(windowOpenMode == EditorMode.ItemEdition)
            {
                selectDataIndex = ((LocalisationLibrary)asset).DatasList.FindIndex(data => { return data.ID == dataID; });
                FootPage();
            }
            GUILayout.Space(5);
            //column two
            try
            {
                if (selectDataIndex < _editedAsset.DatasList.Count && selectDataIndex >= 0)
                {
                    editedData = _editedAsset.DatasList[selectDataIndex];
                    if (windowOpenMode == EditorMode.Normal || windowOpenMode == EditorMode.ItemEdition)
                        PageDetailsEdition((Localisationdata)editedData);
                    else if (windowOpenMode == EditorMode.Selector)
                        PageDetailsPreview((Localisationdata)editedData);
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
            Func<TradField, TradField> MakeField = field =>
              {
                  var fieldCpy = field;
                  fieldCpy.s_textField = EditorGUILayout.TextArea(fieldCpy.s_textField, style_txtArea);
                  fieldCpy.s_audioField = (AudioClip)EditorGUILayout.ObjectField(fieldCpy.s_audioField, typeof(AudioClip), false, new[] { UILayout.Width(50) });
                  fieldCpy.s_imageField = (Sprite)EditorGUILayout.ObjectField(fieldCpy.s_imageField, typeof(Sprite), false, new[] { UILayout.Width(50) });
                  return fieldCpy;
              };
            if (detailsCompatiblesmode())
            {
                ScrollablePanel(() =>
                {
                    GroupGUI(() =>
                    {
                        GUILayout.BeginVertical();
                        //Trad ID
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Traduction ID: ", style_label);
                        GUILayout.Label(data.ID.ToString());
                        GUILayout.EndHorizontal();
                        //Title
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Title :", style_label);
                        data.Title = MakeField(data.Title);
                        GUILayout.EndHorizontal();
                        //Banner
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Banner :", style_label);
                        data.Banner = MakeField(data.Banner);
                        GUILayout.EndHorizontal();
                        //GroupName
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("GroupName :", style_label);
                        data.GroupName = MakeField(data.GroupName);
                        GUILayout.EndHorizontal();
                        //Header
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Header :", style_label);
                        data.Header = MakeField(data.Header);
                        GUILayout.EndHorizontal();
                        //Infos
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Infos :", style_label);
                        data.Infos = MakeField(data.Infos);
                        GUILayout.EndHorizontal();
                        //Description
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Description :", style_label);
                        data.Description = MakeField(data.Description);
                        GUILayout.EndHorizontal();
                        //Details
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Details :", style_label);
                        data.Details = MakeField(data.Details);
                        GUILayout.EndHorizontal();
                        //ToolTip
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("ToolTip :", style_label);
                        data.ToolTip = MakeField(data.ToolTip);
                        GUILayout.EndHorizontal();
                        //Child1
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Child1 :", style_label);
                        data.Child1 = MakeField(data.Child1);
                        GUILayout.EndHorizontal();
                        //Child2
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Child2 :", style_label);
                        data.Child2 = MakeField(data.Child2);
                        GUILayout.EndHorizontal();
                        //Child3
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Child3 :", style_label);
                        data.Child3 = MakeField(data.Child3);
                        GUILayout.EndHorizontal();
                        //Child4
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Child4 :", style_label);
                        data.Child4 = MakeField(data.Child4);
                        GUILayout.EndHorizontal();
                        //Child5
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Child5 :", style_label);
                        data.Child5 = MakeField(data.Child5);
                        GUILayout.EndHorizontal();
                        //Child6
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Child6 :", style_label);
                        data.Child6 = MakeField(data.Child6);
                        GUILayout.EndHorizontal();
                        //FootPage
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("FootPage :", style_label);
                        data.FootPage = MakeField(data.FootPage);
                        GUILayout.EndHorizontal();
                        //Conclusion
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("Conclusion :", style_label);
                        data.Conclusion = MakeField(data.Conclusion);
                        GUILayout.EndHorizontal();
                        //End
                        GUILayout.BeginHorizontal();
                        GUILayout.Label("End :", style_label);
                        data.End = MakeField(data.End);
                        GUILayout.EndHorizontal();

                        GUILayout.EndVertical();
                    }, data.ID + " Edition");
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
                ScrollablePanel(() =>
                {
                    GroupGUI(() =>
                    {
                        GUILayout.BeginVertical();
                        //Trad ID
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Traduction ID: ", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.ID.ToString());
                        GUILayout.EndHorizontal();
                        //Title
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Title :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Title.s_textField);
                        GUILayout.EndHorizontal();
                        //Banner
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Banner :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Banner.s_textField);
                        GUILayout.EndHorizontal();
                        //GroupName
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("GroupName :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.GroupName.s_textField);
                        GUILayout.EndHorizontal();
                        //Header
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Header :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Header.s_textField);
                        GUILayout.EndHorizontal();
                        //Infos
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Infos :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Infos.s_textField);
                        GUILayout.EndHorizontal();
                        //Description
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Description :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Description.s_textField);
                        GUILayout.EndHorizontal();
                        //Details
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Details :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Details.s_textField);
                        GUILayout.EndHorizontal();
                        //ToolTip
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("ToolTip :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Details.s_textField);
                        GUILayout.EndHorizontal();
                        //Child1
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child1 :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Child1.s_textField);
                        GUILayout.EndHorizontal();
                        //Child2
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child2 :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Child2.s_textField);
                        GUILayout.EndHorizontal();
                        //Child3
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child3 :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Child3.s_textField);
                        GUILayout.EndHorizontal();
                        //Child4
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child4 :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Child4.s_textField);
                        GUILayout.EndHorizontal();
                        //Child5
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child5 :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Child5.s_textField);
                        GUILayout.EndHorizontal();
                        //Child6
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Child6 :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Child6.s_textField);
                        GUILayout.EndHorizontal();
                        //FootPage
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("FootPage :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.FootPage.s_textField);
                        GUILayout.EndHorizontal();
                        //Conclusion
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("Conclusion :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.Conclusion.s_textField);
                        GUILayout.EndHorizontal();
                        //End
                        GUILayout.BeginHorizontal();
                        EditorGUILayout.LabelField("End :", EditorStyles.boldLabel);
                        EditorGUILayout.LabelField(data.End.s_textField);
                        GUILayout.EndHorizontal();

                        GUILayout.EndVertical();
                    }, data.ID + " Edition");
                });
            }
        }

        /// <summary>
        /// le genersteur de hashTag.
        /// </summary>
        private void HashTagGenerator()
        {
            if (allAssets == null || allAssets.Count <= 0)
                return;
            if (windowOpenMode != EditorMode.Normal)
                return;
            ScrollablePanel(() =>
            {
                GroupGUInoStyle(() =>
                {
                    GUILayout.BeginVertical();
                    var IEdataTypes = from a in allAssets.ConvertAll(new Converter<ScriptableObject, LocalisationLibrary>(obj => { return (LocalisationLibrary)obj; }))
                                      where a.Langage == selectedLangage
                                      select a.TradType.ToString();
                    string[] dataTypes = IEdataTypes.ToArray();
                    hashtag_dataTypeIndex = EditorGUILayout.Popup(hashtag_dataTypeIndex, dataTypes);
                    var dataType = ((LocalisationLibrary)allAssets[hashtag_dataTypeIndex]).DataType;
                    var filteredList = from a in allAssets.ConvertAll(new Converter<ScriptableObject, LocalisationLibrary>(obj => { return (LocalisationLibrary)obj; }))
                                       where a.DataType == dataType && a.Langage == selectedLangage
                                       select a;
                    var target = filteredList.FirstOrDefault();
                    int index = 0;
                    if (target)
                    {
                        index = target.DatasList.FindIndex(item => { return item.ID == hashtag_id; });
                        var ieListNames = from i in target.DatasList
                                          select i.ID + "-" + i.Title.s_textField;
                        index = Mathf.Clamp(index, 0, index);
                        var listNames = ieListNames.ToArray();
                        index = EditorGUILayout.Popup(index, listNames);
                        if (index >= 0 && target.DatasList.Count > index)
                        {
                            hashtag_id = target.DatasList[index].ID;
                            if (hashtag_id > 0)
                                EditorGUILayout.DelayedTextField(MakeTag(hashtag_id, dataType));
                        }
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
            switch (windowOpenMode)
            {
                case EditorMode.Normal:
                    if (!Header())
                        return;
                    break;
                case EditorMode.Selector:
                    if (!Header(PulseEngineMgr.currentLanguage, selectedDataType))
                        return;
                    break;
                case EditorMode.ItemEdition:
                    if (!Header(PulseEngineMgr.currentLanguage, selectedDataType))
                        return;
                    break;
                case EditorMode.Preview:
                    break;
                case EditorMode.Node:
                    break;
                case EditorMode.Group:
                    break;
            }
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
            allAssets.Clear();
            editedAsset = null;
            asset = null;
            auXasset = null;
            onSelectionEvent = delegate { };
        }

        /// <summary>
        /// Initialise la fenetre.
        /// </summary>
        /// <param name="close"></param>
        private void Initialise()
        {
            allAssets.Clear();
            foreach(Languages langue in Enum.GetValues(typeof(Languages)))
            {
                foreach(TradDataTypes type in Enum.GetValues(typeof(TradDataTypes)))
                {
                    if (LocalisationLibrary.Exist(langue, type))
                    {
                        var load = LocalisationLibrary.Load(langue, type);
                        if (load != null)
                            allAssets.Add(load);
                    }
                    else if (LocalisationLibrary.Save(langue, type))
                    {
                        var load = LocalisationLibrary.Load(langue, type);
                        if (load != null)
                            allAssets.Add(load);
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
            for (int i = 0, len = allAssets.Count; i < len; i++)
            {
                auXasset = allAssets[i] as LocalisationLibrary;
                var otherAsset = auXasset;
                for (int j = 0, len2 = ((LocalisationLibrary)editedAsset).DatasList.Count; j < len2; j++)
                {
                    var data = ((LocalisationLibrary)editedAsset).DatasList[j];
                    if (otherAsset.TradType == ((LocalisationLibrary)editedAsset).TradType)
                    {
                        if (otherAsset.DatasList.FindIndex(d => { return d.ID == data.ID; }) < 0)
                        {
                            Localisationdata newOne = new Localisationdata { ID = data.ID };
                            if (((LocalisationLibrary)editedAsset).TradType == TradDataTypes.Person)
                                newOne.Title = data.Title;
                            otherAsset.DatasList.Add(newOne);
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
                ID = data.ID,
                dataType = (int)(((LocalisationLibrary)editedAsset).DataType)
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
        public static string MakeTag(int id, DataTypes T)
        {
            string hashTag = "#" + id + "_" + (int)T + "#";
            return hashTag;
        }

        #endregion
    }
}