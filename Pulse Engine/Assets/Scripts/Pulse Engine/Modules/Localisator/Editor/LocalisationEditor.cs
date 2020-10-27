using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using System.Linq;
using PulseEngine;
using PulseEngine.Datas;
using PulseEngine.Modules.Localisator;
using UILayout = UnityEngine.GUILayout;
using EILayout = UnityEditor.EditorGUILayout;
using System.Threading.Tasks;


//TODO: implementer les details de la data.
namespace PulseEditor.Modules
{

    /// <summary>
    /// L'editeur de localisation.
    /// </summary>
    public class LocalisationEditor : PulseEditorMgr
    {
        /// <Summary>
        /// Implement here
        /// 3- Static void RefreshCache(object _dictionnary, DataTypes _dtype); to refresh the static cache dictionnary.
        /// 4- Static bool registeredToRefresh; to Prevent from registering to OnCacheRefresh several times.
        /// </Summary>
        #region Static Accessors ################################################################################################################################################################################################
#if UNITY_EDITOR //**********************************************************************

        ///<summary>
        /// Active when the editor is already registered to OnCacheRefresh event.
        ///</summary>
        public static bool registeredToRefresh;


        /// <summary>
        /// to refresh the static cache dictionnary
        /// </summary>
        public static void RefreshCache(object _dictionnary, DataTypes _dtype)
        {
            if (_dtype != DataTypes.Localisation)
                return;
            var dictionnary = _dictionnary as Dictionary<DataLocation, IData>;
            if (dictionnary == null)
                return;

            var allAsset = new List<LocalisationLibrary>();

            foreach (Languages langue in Enum.GetValues(typeof(Languages)))
            {
                foreach (TradDataTypes type in Enum.GetValues(typeof(TradDataTypes)))
                {
                    if (CoreLibrary.Exist<LocalisationLibrary>(AssetsPath, langue, type))
                    {
                        var load = CoreLibrary.Load<LocalisationLibrary>(AssetsPath, langue, type);
                        if (load != null)
                            allAsset.Add(load);
                    }
                    else if (CoreLibrary.Save<LocalisationLibrary>(AssetsPath, langue, type))
                    {
                        var load = CoreLibrary.Load<LocalisationLibrary>(AssetsPath, langue, type);
                        if (load != null)
                            allAsset.Add(load);
                    }
                }
            }
            foreach(var entry in dictionnary)
            {
                if(entry.Value == null)
                {
                    var library = allAsset.Find(lib => { return lib.Langage == (Languages)entry.Key.globalLocation && lib.TradType == (TradDataTypes)entry.Key.localLocation; });
                    if (library != null)
                    {
                        var data = library.DataList.Find(d =>
                        {
                            return d.Location.id == entry.Key.id;
                        }) as Localisationdata;
                        if(data != null)
                        {
                            dictionnary[entry.Key] = data;
                        }
                    }
                }
            }

        }

#endif
        #endregion

        /// <Summary>
        /// Declare here every attribute used for visual behaviour of the editor window.
        /// </Summary>
        #region Visual Attributes ################################################################################################################################################################################################

        #endregion

        /// <Summary>
        /// Declare here every attribute used for deep behaviour ot the editor window.
        /// </Summary>
        #region Fonctionnal Attributes ################################################################################################################################################################################################

        /// <summary>
        /// Le chemin d'access des datas.
        /// </summary>
        const string AssetsPath = "LocalisationDatas";

        /// <summary>
        /// le type data du hash tag selectionne.
        /// </summary>
        private int hashtag_dataTypeIndex;

        /// <summary>
        /// le type data du hash tag selectionne.
        /// </summary>
        private int hashtag_id;

        #endregion


        /// <Summary>
        /// Implement here Methods To Open the window.
        /// </Summary>
        #region Door Methods ################################################################################################################################################################################################

        /// <summary>
        /// Open the editor
        /// </summary>
        [MenuItem(Menu_EDITOR_MENU + "Localisator Editor")]
        public static void OpenEditor()
        {
            if (!registeredToRefresh)
            {
                OnCacheRefresh += RefreshCache;
                registeredToRefresh = true;
            }
            var window = GetWindow<LocalisationEditor>();
            window.currentEditorMode = EditorMode.Edition;
            window.editorDataType = DataTypes.Localisation;
            window.Show();
        }

        /// <summary>
        /// Open the selector
        /// </summary>
        public static void OpenSelector(Action<object, EditorEventArgs> onSelect, TradDataTypes dType)
        {
            if (!registeredToRefresh)
            {
                OnCacheRefresh += RefreshCache;
                registeredToRefresh = true;
            }
            var window = GetWindow<LocalisationEditor>(true, "Localisator Selector");
            window.currentEditorMode = EditorMode.Selection;
            window.editorDataType = DataTypes.Localisation;
            window.assetLocalFilter = (int)dType;
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
        public static void OpenModifier(DataLocation _location)
        {
            if (!registeredToRefresh)
            {
                OnCacheRefresh += RefreshCache;
                registeredToRefresh = true;
            }
            var window = GetWindow<LocalisationEditor>(true, "Localisator Modifier");
            window.currentEditorMode = EditorMode.DataEdition;
            window.editorDataType = DataTypes.Localisation;
            window.assetLocalFilter = _location.localLocation;
            window.assetMainFilter = _location.globalLocation;
            window.dataID = _location.id;
            window.ShowAuxWindow();
        }

        #endregion



        /// <Summary>
        /// Implement here Methods related to GUI.
        /// </Summary>
        #region GUI Methods ################################################################################################################################################################################################


        /// <summary>
        /// La tete de fenetre.
        /// </summary>
        /// <returns></returns>
        private void Header()
        {
            GroupGUInoStyle(() =>
            {
                MakeHeader(assetMainFilter, Enum.GetNames(typeof(Languages)), index => assetMainFilter = index);
            }, "Language", 50);
            GroupGUInoStyle(() =>
            {
                MakeHeader(assetLocalFilter, Enum.GetNames(typeof(TradDataTypes)), index => assetLocalFilter = index);
            }, "Type", 50);
        }
        
        /// <summary>
        /// Les details de la data selectionnee.
        /// </summary>
        private void PageDetailsEdition(Localisationdata data)
        {
            if (data == null)
                return;

            Func<TradField, TradField> MakeField = field =>
            {
                var fieldCpy = field;
                fieldCpy.textField = EditorGUILayout.TextArea(fieldCpy.textField, style_txtArea);
                fieldCpy.audioField = (AudioClip)EditorGUILayout.ObjectField(fieldCpy.audioField, typeof(AudioClip), false, new[] { UILayout.Width(50) });
                fieldCpy.imageField = (Sprite)EditorGUILayout.ObjectField(fieldCpy.imageField, typeof(Sprite), false, new[] { UILayout.Width(50) });
                return fieldCpy;
            };

            ScrollablePanel(() =>
            {
                GroupGUI(() =>
                {
                    GUILayout.BeginVertical();
                    //Trad ID
                    GUILayout.BeginHorizontal();
                    GUILayout.Label("Traduction ID: ", style_label);
                    GUILayout.Label(data.Location.id.ToString());
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
                }, data.Location.id + " Edition");
            });
    }

        /// <summary>
        /// le generateur de hashTag.
        /// </summary>
        private void HashTagGenerator()
        {
            if (allAssets == null || allAssets.Count <= 0)
                return;
            if (currentEditorMode != EditorMode.Edition)
                return;
            ScrollablePanel(() =>
            {
                GroupGUInoStyle(() =>
                {
                    GUILayout.BeginVertical();
                    var IEdataTypes = from a in allAssets.ConvertAll(new Converter<ScriptableObject, LocalisationLibrary>(obj => { return (LocalisationLibrary)obj; }))
                                      where a.Langage == (Languages)assetMainFilter
                                      select a.TradType.ToString();
                    string[] dataTypes = IEdataTypes.ToArray();
                    hashtag_dataTypeIndex = EditorGUILayout.Popup(hashtag_dataTypeIndex, dataTypes);
                    var dataType = ((LocalisationLibrary)allAssets[hashtag_dataTypeIndex]).TradType;
                    var filteredList = from a in allAssets.ConvertAll(new Converter<ScriptableObject, LocalisationLibrary>(obj => { return (LocalisationLibrary)obj; }))
                                       where a.TradType == dataType && a.Langage == (Languages)assetMainFilter
                                       select a;
                    var target = filteredList.FirstOrDefault();
                    int index = 0;
                    if (target != null)
                    {
                        index = target.DataList.FindIndex(item => { return ((Localisationdata)item).Location.id == hashtag_id; });
                        var ieListNames = from i in target.DataList
                                          let j = i as Localisationdata
                                          where j != null
                                          select j.Location.id + "-" + j.Title.textField;

                        index = Mathf.Clamp(index, 0, index);
                        var listNames = ieListNames.ToArray();
                        index = EditorGUILayout.Popup(index, listNames);
                        if (index >= 0 && target.DataList.Count > index)
                        {
                            hashtag_id = ((Localisationdata)target.DataList[index]).Location.id;
                            if (hashtag_id > 0)
                                EditorGUILayout.DelayedTextField(MakeTag(hashtag_id, (Languages)assetMainFilter, dataType));
                        }
                    }
                    GUILayout.EndVertical();

                }, "HashTag Maker", 50);
            });
        }

        #endregion

        /// <Summary>
        /// Implement here behaviours methods.
        /// </Summary>
        #region Fontionnal Methods ################################################################################################################################################################################################


        /// <summary>
        /// Initialise la fenetre.
        /// </summary>
        /// <param name="close"></param>
        private void Initialise()
        {
            allAssets.Clear();
            SelectAction = () => { Select((Localisationdata)data); };
            customSave = Save;
            foreach (Languages langue in Enum.GetValues(typeof(Languages)))
            {
                foreach (TradDataTypes type in Enum.GetValues(typeof(TradDataTypes)))
                {
                    if (CoreLibrary.Exist<LocalisationLibrary>(AssetsPath, langue, type))
                    {
                        var load = CoreLibrary.Load<LocalisationLibrary>(AssetsPath, langue, type);
                        if (load != null)
                            allAssets.Add(load);
                    }
                    else if (CoreLibrary.Save<LocalisationLibrary>(AssetsPath, langue, type))
                    {
                        var load = CoreLibrary.Load<LocalisationLibrary>(AssetsPath, langue, type);
                        if (load != null)
                            allAssets.Add(load);
                    }
                }
            }
        }

        /// <summary>
        /// Rafraichi la selection d'asset.
        /// </summary>
        private bool RefreshAssetChange()
        {
            if (allAssets == null || allAssets.Count <= 0)
                return false;
            var matchingAsset = allAssets.Find(library =>
            {
                var lib = library as LocalisationLibrary;
                return lib.Langage == (Languages)assetMainFilter &&
                lib.TradType == (TradDataTypes)assetLocalFilter;
            }) as LocalisationLibrary;
            if(matchingAsset != null)
            {
                originalAsset = matchingAsset;
                asset = Core.LibraryClone(matchingAsset);
                return true;
            }
            return false;
        }


        /// <summary>
        /// Sauvegarde les changements et ferme la fenetre si besoin.
        /// </summary>
        /// <param name="close"></param>
        private void Save()
        {
            for (int i = 0, len = allAssets.Count; i < len; i++)
            {
                var otherAsset = allAssets[i] as LocalisationLibrary;
                if (otherAsset == null)
                    continue;
                if (otherAsset.Langage == ((LocalisationLibrary)asset).Langage)
                    continue;
                if (otherAsset.TradType == ((LocalisationLibrary)asset).TradType)
                {
                    for (int j = 0, len2 = ((LocalisationLibrary)asset).DataList.Count; j < len2; j++)
                    {
                        var locData = ((LocalisationLibrary)asset).DataList[j] as Localisationdata;
                        if (locData == null)
                            continue;
                        int foundIndex = otherAsset.DataList.FindIndex(d => {
                            var dt = d as Localisationdata;
                            return dt != null && dt.Location.id == locData.Location.id;
                        });
                        bool found = !(foundIndex < 0);

                        if (!found)
                        {
                            Localisationdata newOne = new Localisationdata { Location = new DataLocation { id = locData.Location.id, localLocation = (int)otherAsset.TradType , globalLocation = (int)otherAsset.Langage} };
                            if (((LocalisationLibrary)asset).TradType == TradDataTypes.Person)
                                newOne.Title = locData.Title;
                            var dList = otherAsset.DataList;
                            if (dList == null)
                                dList = new List<IData>();
                            dList.Add(newOne);
                            otherAsset.DataList = dList;
                            EditorUtility.SetDirty(otherAsset);
                        }else if(((LocalisationLibrary)asset).TradType == TradDataTypes.Person)
                        {
                            var dList = otherAsset.DataList;
                            var item = dList[foundIndex] as Localisationdata;
                            if (item == null)
                                continue;
                            item.Title = locData.Title;
                            dList[foundIndex] = item;
                            otherAsset.DataList = dList;
                        }
                    }
                }
            }
            SaveAsset(asset, originalAsset);
        }


        /// <summary>
        /// Sauvegarde les changements et ferme la fenetre si besoin.
        /// </summary>
        private void Select(Localisationdata data)
        {
            if (data == null || asset == null)
                return;
            EditorEventArgs eventArgs = new EditorEventArgs
            {
                dataObjectLocation = data.Location
            };
            if (onSelectionEvent != null)
                onSelectionEvent.Invoke(data, eventArgs);
        }

        #endregion

        /// <Summary>
        /// Implement here overrides methods.
        /// </Summary>
        #region Program FLow Methods ################################################################################################################################################################################################

        protected override void OnInitialize()
        {
            Initialise();
        }

        protected override void OnHeaderChange()
        {
            RefreshAssetChange();
        }

        protected override void OnBodyRedraw()
        {
            if (currentEditorMode == EditorMode.Edition || currentEditorMode == EditorMode.DataEdition)
            {
                PageDetailsEdition((Localisationdata)data);
            }
        }

        protected override void OnFootRedraw()
        {
            if (currentEditorMode == EditorMode.Edition || currentEditorMode == EditorMode.DataEdition)
            {
                HashTagGenerator();
            }
        }

        protected override void OnHeaderRedraw()
        {
            Header();
        }

        protected override void OnQuit()
        {

        }

        #endregion

        #region GUIFunctions ###############################################################################

        #endregion

        #region Utils ##############################################################################################################################################################################################################


        /// <summary>
        /// Cree un Hashtag d'un element de type T.
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public static string MakeTag(int id, Languages L, TradDataTypes T)
        {
            string hashTag = "#" + id + "_" + (int)L + "_"+ (int)T + "#";
            return hashTag;
        }

        #endregion
    }
}