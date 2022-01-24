using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Modules.CharacterCreator;
using UnityEditor;
using System;
using PulseEngine;
using System.Reflection;
using System.Linq;

namespace PulseEditor.Modules
{
    /// <summary>
    /// L'editeur de characters.
    /// </summary>
    public class CharacterEditor : PulseEditorMgr
    { 
       /// <Summary>
      /// Implement here
      /// 1- static Dictionnary<Vector3Int,object> StaticCache; for fast retrieval of assets datas, where Vector3 is the unique path of the data in assets.
      /// 2- Static object GetData(Vector3int dataLocation); to retrieve data from outside of the module by reflection. it returns null when nothing found and mark the entry on the dictionnary or trigger refresh.
      /// 3- Static void RefreshCache(); to refresh the static cache dictionnary.
      /// 4- Static bool RefreshingCache; to Prevent from launching several refreshes
      /// </Summary>
        #region Static Accessors ################################################################################################################################################################################################
#if UNITY_EDITOR //********************************************************************************************************************************************

        ///<summary>
        /// Active when the editor is already registered to OnCacheRefresh event.
        ///</summary>
        public static bool registeredToRefresh;


        /// <summary>
        /// to refresh the static cache dictionnary
        /// </summary>
        public static void RefreshCache(object _dictionnary, DataTypes _dtype)
        {
            if (_dtype != DataTypes.Character)
                return;
            var dictionnary = _dictionnary as Dictionary<DataLocation, IData>;
            if (dictionnary == null)
                return;

            var allAsset = new List<CharactersLibrary>();
            //get all assets
            foreach(Scopes scp in Enum.GetValues(typeof(Scopes)))
            {
                if (CoreLibrary.Exist<CharacterData,CharactersLibrary>(AssetsPath, scp)) {
                    var load = CoreLibrary.Load<CharacterData, CharactersLibrary>(AssetsPath, scp);
                    if (load != null)
                        allAsset.Add(load);
                }
                else if (CoreLibrary.Save<CharacterData, CharactersLibrary>(AssetsPath, scp))
                {
                    var load = CoreLibrary.Load<CharacterData, CharactersLibrary>(AssetsPath, scp);
                    if (load != null)
                        allAsset.Add(load);
                }
            }
            //fill the missings
            foreach (var entry in dictionnary)
            {
                if (entry.Value == null)
                {
                    var library = allAsset.Find(lib => { return (int)lib.Scope == entry.Key.globalLocation; });
                    if (library != null)
                    {
                        var data = library.DataList.Find(d =>
                        {
                            return d.Location.id == entry.Key.id;
                        }) as CharacterData;
                        if (data != null)
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

        /// <summary>
        /// Active when adding new character
        /// </summary>
        private bool addindNewCharacter;

        /// <summary>
        /// the new character category
        /// </summary>
        private AnimaAvatar newCharAvatarType;

        /// <summary>
        /// the index of the currently selected weapon
        /// </summary>
        private int characterWeaponsIndex;

        #endregion

        /// <Summary>
        /// Declare here every attribute used for deep behaviour ot the editor window.
        /// </Summary>
        #region Fonctionnal Attributes ################################################################################################################################################################################################


        /// <summary>
        /// Le chemin d'access des datas.
        /// </summary>
        public const string AssetsPath = "CharactersDatas"; 

        /// <summary>
        /// Les armes dans l'armurerie du character.
        /// </summary>
        private List<WeaponData> characterWeapons = new List<WeaponData>();

        /// <summary>
        /// l'emplacement des armes dans la previsualiation
        /// </summary>
        List<(GameObject go, HumanBodyBones bone, Vector3 offset, Vector3 rot)> weaponLocationTab = new List<(GameObject go, HumanBodyBones bone, Vector3 offset, Vector3 rot)>();


        //TODO: clothes list of this character
        //TODO: gadgets/wearables inventory list of this character

        #endregion

        /// <Summary>
        /// Implement here Methods To Open the window.
        /// </Summary>
        #region Door Methods ################################################################################################################################################################################################

        /// <summary>
        /// Open the editor.
        /// </summary>
        [MenuItem(Menu_EDITOR_MENU + "Character Editor")]
        public static void OpenEditor()
        {
            if (!registeredToRefresh)
                OnCacheRefresh += RefreshCache;
            var window = GetWindow<CharacterEditor>();
            window.currentEditorMode = EditorMode.Edition;
            window.editorDataType = DataTypes.Character;
            window.Show();
        }

        /// <summary>
        /// Open the selector.
        /// </summary>
        public static void OpenSelector(Action<object, EventArgs> onSelect)
        {
            if (!registeredToRefresh)
            {
                OnCacheRefresh += RefreshCache;
                registeredToRefresh = true;
            }
            var window = GetWindow<CharacterEditor>(true);
            window.currentEditorMode = EditorMode.Selection;
            window.editorDataType = DataTypes.Character;
            window.onSelectionEvent += (obj, arg) =>
            {
                if (onSelect != null)
                {
                    IData idata = window.data as IData;
                    onSelect.Invoke(null, new EditorEventArgs
                    {
                        dataObjectLocation = idata != null ? idata.Location : default
                    }); ;
                }
            };
            window.ShowUtility();
        }

        /// <summary>
        /// Open the Modifier.
        /// </summary>
        public static void OpenModifier(DataLocation location)
        {
            if (!registeredToRefresh)
            {
                OnCacheRefresh += RefreshCache;
                registeredToRefresh = true;
            }
            var window = GetWindow<CharacterEditor>(true);
            window.currentEditorMode = EditorMode.DataEdition;
            window.editorDataType = DataTypes.Character;
            window.assetLocalFilter = location.localLocation;
            window.dataID = location.id;
            window.assetMainFilter = location.globalLocation;
            window.OnInitialize();
            window.ShowUtility();
        }


        #endregion

        /// <Summary>
        /// Implement here Methods related to GUI.
        /// </Summary>
        #region GUI Methods ################################################################################################################################################################################################

        /// <summary>
        /// L'entete.
        /// </summary>
        /// <returns></returns>
        private void Header()
        {
            GroupGUInoStyle(() =>
            {
                ScopeSelector();
                MakeHeader(assetLocalFilter, Enum.GetNames(typeof(CharacterType)), index => { assetLocalFilter = index; });
            }, "Character Type", 50);
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
                //ID
                GUILayout.BeginVertical();
                EditorGUILayout.LabelField("ID: " + data.Location.id.ToString(), style_label);
                GUILayout.Space(5);
                //Trad
                EditorGUILayout.LabelField("Trad Id: " + data.TradLocation.id, style_label);
                GUILayout.Space(5);
                var locData = GetCachedData(data.TradLocation) as Localisationdata;
                string name = locData != null ? locData.Title.textField : string.Empty;
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name:", style_label);
                EditorGUILayout.LabelField(name);
                if (GUILayout.Button("S", new[] { GUILayout.Width(25) }))
                {
                    var locEditorType = TypeInfo.GetType("LocalisationEditor");
                    LocalisationEditor.OpenSelector((object obj, EditorEventArgs arg) =>
                    {
                        var a = arg as EditorEventArgs;
                        if (a == null)
                            return;
                        data.TradLocation = a.dataObjectLocation;
                    }, TradDataTypes.Person);
                }
                if (GUILayout.Button("E", new[] { GUILayout.Width(25) }))
                {
                    LocalisationEditor.OpenModifier(data.TradLocation);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                GUILayout.Space(5);
                //Stats --------------------------------------------------------------------------------------------------------------------------------
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Stats:", style_label);
                if (GUILayout.Button("Edit " + name + " Stats"))
                {
                    StatWinEditor.OpenEditor(data.Stats, obj =>
                    {
                        try
                        {
                            MindStat rSt = (MindStat)obj;
                            data.Stats = rSt;
                        }
                        catch (Exception e) { PulseEngine.PulseDebug.Log(e.Message); }
                    });
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5);

                GUILayout.EndVertical();
            }, data.Location.id + " Edition");
            GUILayout.Space(10);
        }
               
        #endregion

        /// <Summary>
        /// Implement here behaviours methods.
        /// </Summary>
        #region Fontionnal Methods ################################################################################################################################################################################################


        /// <summary>
        /// la selection d'un item.
        /// </summary>
        /// <param name="data"></param>
        private void Select(CharacterData data)
        {
            if (data == null || asset == null)
                return;
            EditorEventArgs eventArgs = new EditorEventArgs
            {
                dataObjectLocation = data.Location
            };
            if (onSelectionEvent != null)
                onSelectionEvent.Invoke(this, eventArgs);
        }
        
        /// <summary>
        /// Reinitialise les listes d'equipement propre a 1 character.
        /// </summary>
        private void ResetPreviewAdditives()
        {
            //Weapons
            characterWeapons.Clear();
        }

        #endregion

        /// <Summary>
        /// Implement here overrides methods.
        /// </Summary>
        #region Program FLow Methods ################################################################################################################################################################################################

        /// <summary>
        /// Initialise la fenetre.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            SelectAction = () => { Select((CharacterData)data); };
            if (!originalAsset)
            {
                originalAsset = CoreLibrary.Load<CharacterData,CharactersLibrary>(AssetsPath, new object[] { assetMainFilter, assetLocalFilter });
                if (!originalAsset && CoreLibrary.Save<CharacterData, CharactersLibrary>(AssetsPath, new object[] { assetMainFilter, assetLocalFilter }))
                    originalAsset = CoreLibrary.Load<CharacterData, CharactersLibrary>(AssetsPath, new object[] { assetMainFilter, assetLocalFilter });
                if (originalAsset)
                    asset = Core.LibraryClone(originalAsset);
                if (asset != null)
                    dataList = asset.DataList.Cast<object>().ToList();
            }
            if (currentEditorMode == EditorMode.DataEdition && dataList != null)
            {
                data = dataList.Find(d => {
                    CharacterData c = d as CharacterData;
                    return c != null && c.Location.id == dataID; });
            }
            RefreshPreview();
        }

        protected override void OnBodyRedraw()
        {
            Details((CharacterData)data);
        }

        protected override void OnHeaderRedraw()
        {
            Header();
        }

        protected override void OnQuit()
        {
            if (previewer != null)
                previewer.Destroy();
            ResetPreviewAdditives();
            try
            {
                OnCacheRefresh -= RefreshCache;
            }
            catch { }
        }

        private void RefreshPreview()
        {
            if (previewer != null)
                previewer.Destroy();
            previewer = new Previewer();
        }

        protected override void OnHeaderChange()
        {
            RefreshPreview();
            ResetPreviewAdditives();
        }

        protected override void OnListChange()
        {
            RefreshPreview();
            ResetPreviewAdditives();
        }

        #endregion

        /// <Summary>
        /// Implement here miscelaneous methods relative to the module in editor mode.
        /// </Summary>
        #region Helpers & Tools ################################################################################################################################################################################################

        #endregion

    }
}