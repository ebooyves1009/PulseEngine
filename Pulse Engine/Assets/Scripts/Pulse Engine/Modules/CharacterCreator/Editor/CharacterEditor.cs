using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEditor.Modules.Localisator;
using PulseEngine.Modules;
using PulseEngine.Modules.CharacterCreator;
using UnityEditor;
using System;
using PulseEngine;
using System.Threading.Tasks;
using PulseEngine.Datas;

namespace PulseEditor.Modules.CharacterCreator
{
    /// <summary>
    /// L'editeur de characters.
    /// </summary>
    public class CharacterEditor : PulseEditorMgr
    { /// <Summary>
      /// Implement here
      /// 1- static Dictionnary<Vector3Int,object> StaticCache; for fast retrieval of assets datas, where Vector3 is the unique path of the data in assets.
      /// 2- Static object GetData(Vector3int dataLocation); to retrieve data from outside of the module by reflection. it returns null when nothing found and mark the entry on the dictionnary or trigger refresh.
      /// 3- Static void RefreshCache(); to refresh the static cache dictionnary.
      /// 4- Static bool RefreshingCache; to Prevent from launching several refreshes
      /// </Summary>
        #region Static Accessors ################################################################################################################################################################################################
#if UNITY_EDITOR //********************************************************************************************************************************************

        ///<summary>
        /// for fast retrieval of assets datas, where Vector3 is the unique path of the data in assets
        ///</summary>
        private static Dictionary<DataLocation, IData> StaticCache;

        /// <summary>
        /// Prevent from launching several refreshes.
        /// </summary>
        private static bool RefreshingCache;

        /// <summary>
        /// to retrieve data from outside of the module by reflection
        /// </summary>
        /// <param name="_location"></param>
        /// <returns></returns>
        public static object GetData(DataLocation _location)
        {
            if (StaticCache.ContainsKey(_location))
            {
                if (StaticCache[_location] == null && !RefreshingCache)
                    RefreshCache();
                return StaticCache[_location];
            }
            return null;
        }

        /// <summary>
        /// to refresh the static cache dictionnary
        /// </summary>
        public static async Task RefreshCache()
        {
            RefreshingCache = true;

            RefreshingCache = false;
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
        /// Le type de character choisi.
        /// </summary>
        private CharacterType typeSelected;

        /// <summary>
        /// Les armes dans l'armurerie du character.
        /// </summary>
        private List<WeaponData> characterWeapons = new List<WeaponData>();

        /// <summary>
        /// l'emplacement des armes dans la previsualiation
        /// </summary>
        List<(GameObject go, HumanBodyBones bone, Vector3 offset, Quaternion rot)> weaponLocationTab = new List<(GameObject go, HumanBodyBones bone, Vector3 offset, Quaternion rot)>();


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
            var window = GetWindow<CharacterEditor>();
            window.currentEditorMode = EditorMode.Edition;
            window.Show();
        }

        /// <summary>
        /// Open the selector.
        /// </summary>
        public static void OpenSelector(Action<object, EventArgs> onSelect)
        {
            var window = GetWindow<CharacterEditor>(true);
            window.currentEditorMode = EditorMode.Selection;
            window.onSelectionEvent += (obj, arg) =>
            {
                if (onSelect != null)
                {
                    onSelect.Invoke(null, new EditorEventArgs
                    {
                        dataObjectLocation = window.data.Location
                    });
                }
            };
            window.ShowUtility();
        }

        /// <summary>
        /// Open the Modifier.
        /// </summary>
        public static void OpenModifier(DataLocation location)
        {
            var window = GetWindow<CharacterEditor>(true);
            window.currentEditorMode = EditorMode.DataEdition;
            window.typeSelected = (CharacterType)location.localLocation;
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
                MakeHeader((int)typeSelected, Enum.GetNames(typeof(CharacterType)), index => { typeSelected = (CharacterType)index; });
            }, "Character Type", 50);
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
                switch (currentEditorMode)
                {
                    case EditorMode.Edition:
                        return true;
                    case EditorMode.Selection:
                        return true;
                    case EditorMode.DataEdition:
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
                    List<string> listContent = new List<string>();
                    int maxId = 0;
                    for (int i = 0; i < library.DataList.Count; i++)
                    {
                        var data = library.DataList[i];
                        var nameList = LocalisationEditor.GetTexts(data.IdTrad, TradDataTypes.Person);
                        string name = nameList.Length > 0 ? nameList[0] : string.Empty;
                        listContent.Add(name);
                    }
                    selectDataIndex = MakeList(selectDataIndex, listContent.ToArray(), library.DataList);
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    if (addindNewCharacter)
                    {
                        newCharAvatarType = (AnimaCategory)EditorGUILayout.EnumPopup(newCharAvatarType);
                        if (GUILayout.Button("Ok"))
                        {
                            addindNewCharacter = false;
                            library.DataList.Add(new CharacterData
                            {
                                ID = maxId + 1,
                                TradType = TradDataTypes.Person,
                                AnimCat = newCharAvatarType
                            });
                        }
                    }
                    else
                    {
                        if (GUILayout.Button("+"))
                        {
                            addindNewCharacter = true;
                        }
                        if (data != null)
                        {
                            if (GUILayout.Button("-"))
                            {
                                library.DataList.RemoveAt(selectDataIndex);
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }, "Characters Datas List");
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
                EditorGUILayout.LabelField("ID: " + data.ID.ToString(), style_label);
                GUILayout.Space(5);
                //Trad
                EditorGUILayout.LabelField("Trad Id: " + data.IdTrad, style_label);
                GUILayout.Space(5);
                data.TradType = TradDataTypes.Person;
                var texts = LocalisationEditor.GetTexts(data.IdTrad, TradDataTypes.Person);
                string name = texts.Length > 0 ? texts[0] : string.Empty;
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name:", style_label);
                EditorGUILayout.LabelField(name);
                if (GUILayout.Button("S", new[] { GUILayout.Width(25) }))
                {
                    LocalisationEditor.OpenSelector((obj, arg) =>
                    {
                        var a = arg as EditorEventArgs;
                        if (a == null)
                            return;
                        data.IdTrad = a.ID;
                    }, data.TradType);
                }
                if (GUILayout.Button("E", new[] { GUILayout.Width(25) }))
                {
                    LocalisationEditor.OpenModifier(data.IdTrad, data.TradType);
                }
                GUILayout.EndHorizontal();
                EditorGUILayout.LabelField("Character:", style_label);
                GUILayout.Space(5);
                var Char = EditorGUILayout.ObjectField(data.Character, typeof(GameObject), false) as GameObject;
                if (Char != data.Character)
                    RefreshPreview();
                data.Character = Char;
                GUILayout.EndVertical();
                GUILayout.EndHorizontal();

                GUILayout.Space(5);
                //Stats --------------------------------------------------------------------------------------------------------------------------------
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Stats:", style_label);
                if (GUILayout.Button("Edit " + name + " Stats"))
                {
                    StatEditor.OpenStatWindow(data.Stats, (obj) => {
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
                    List<(int, WeaponType, Scopes)> ps = new List<(int, WeaponType, Scopes)>();
                    for (int i = 0; i < data.Armurie.Count; i++)
                    {
                        var weap = data.Armurie[i];
                        ps.Add((weap.x, (WeaponType)weap.y, (Scopes)weap.z));
                    }
                    CombatSystem.WeaponEditor.WeaponryEditor.Open(ps, (obj, arg) =>
                    {
                        var result = obj as List<(PulseEngine.Modules.CombatSystem.WeaponData, Scopes)>;
                        if (result != null)
                        {
                            List<Vector3Int> sp = new List<Vector3Int>();
                            for (int i = 0; i < result.Count; i++)
                            {
                                var weap = result[i];
                                sp.Add(new Vector3Int(weap.Item1.ID, (int)weap.Item1.TypeArme, (int)weap.Item2));
                            }
                            data.Armurie = sp;
                        }
                    });
                }
                GUILayout.EndHorizontal();

                GUILayout.Space(5);
                // Animator Controller ------------------------------------------------------------------------------------------------------------------
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Edit " + name + " Runtime Controller"))
                {
                    Anima.AnimaEditor.AnimaMachineEditor.Open(data.AnimatorController, data.AnimCat, name, (obj, arg) =>
                    {
                        var rt = obj as RuntimeAnimatorController;
                        if (rt != null)
                            data.AnimatorController = rt;
                        RefreshPreview();
                    });
                }
                GUILayout.EndHorizontal();


                GUILayout.EndVertical();
            }, data.ID + " Edition");
        }

        /// <summary>
        /// Affiche une previsualisation.
        /// </summary>
        /// <param name="data"></param>
        private void AnimationsPreview(CharacterData data)
        {
            if (data == null)
                return;
            GroupGUI(() =>
            {
                if (data.AnimatorController == null)
                    return;
                var controller = (UnityEditor.Animations.AnimatorController)data.AnimatorController;
                var layerlist = controller.layers;
                string[] layerNames = new string[layerlist.Length];
                for (int i = 0; i < layerlist.Length; i++)
                    layerNames[i] = layerlist[i].name;
                var newLayer = EditorGUILayout.Popup("Layers", layerAnimIndex, layerNames);
                if (newLayer != layerAnimIndex)
                {
                    RefreshPreview();
                    StateAnimIndex = 0;
                }
                layerAnimIndex = newLayer;
                UnityEditor.Animations.ChildAnimatorState[] stateList = null;
                if (layerAnimIndex < controller.layers.Length && layerAnimIndex >= 0)
                {
                    stateList = controller.layers[layerAnimIndex].stateMachine.states;
                }
                if (stateList != null)
                {
                    string[] stateNames = new string[stateList.Length];
                    for (int i = 0; i < stateList.Length; i++)
                        stateNames[i] = stateList[i].state.name;
                    var newAnimindex = EditorGUILayout.Popup("States", StateAnimIndex, stateNames);
                    if (StateAnimIndex != newAnimindex)
                        RefreshPreview();
                    StateAnimIndex = newAnimindex;
                    if (StateAnimIndex < stateList.Length && StateAnimIndex >= 0)
                        selectedmotion = stateList[StateAnimIndex].state.motion;
                }
                if (characterWeapons != null && characterWeapons.Count > 0)
                {
                    string[] weaponNames = new string[characterWeapons.Count];
                    for (int i = 0; i < characterWeapons.Count; i++)
                    {
                        weaponNames[i] = LocalisationEditor.GetTexts(characterWeapons[i].IdTrad, characterWeapons[i].TradType)[0];
                    }
                    var newCharWeaponIDx = EditorGUILayout.Popup("Armurie", characterWeaponsIndex, weaponNames);
                    if (characterWeaponsIndex != newCharWeaponIDx)
                    {
                        if (newCharWeaponIDx >= 0 && newCharWeaponIDx < characterWeapons.Count)
                        {
                            weaponLocationTab.Clear();
                            var select_weapon = characterWeapons[newCharWeaponIDx];
                            for (int i = 0, len = select_weapon.Weapons.Count; i < len; i++)
                            {
                                var part = select_weapon.Weapons[i];
                                var place = select_weapon.CarryPlaces[i];
                                weaponLocationTab.Add((part, place.ParentBone, place.PositionOffset, place.RotationOffset));
                            }
                        }
                    }
                    characterWeaponsIndex = newCharWeaponIDx;
                }
                else
                    SetPreviewAdditives(data);
                if (previewer != null)
                    previewer.Previsualize(selectedmotion, 18 / 9, data.Character, weaponLocationTab.ToArray());

            }, "Animation preview");
        }

        /// <summary>
        /// la panel de sauvegarde.
        /// </summary>
        private void SaveAndCancel()
        {
            if (asset == null)
                return;
            SaveBarPanel(() => { Select((CharacterData)data); });
        }


        #endregion

        /// <Summary>
        /// Implement here behaviours methods.
        /// </Summary>
        #region Fontionnal Methods ################################################################################################################################################################################################

        /// <summary>
        /// Sauvegarde las modifications.
        /// </summary>
        private void Save()
        {
            SaveAsset(asset, originalAsset);
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
            if (data == null || asset == null)
                return;
            EditorEventArgs eventArgs = new EditorEventArgs
            {
                ID = data.ID,
                dataType = (int)((CharactersLibrary)asset).DataType
            };
            if (onSelectionEvent != null)
                onSelectionEvent.Invoke(this, eventArgs);
        }

        /// <summary>
        /// Reinitialise les listes d'equipement propre a 1 character.
        /// </summary>
        private void SetPreviewAdditives(CharacterData _data)
        {
            if (_data == null)
                return;
            //Weapons
            var W_collection = new List<(int, WeaponType, Scopes)>();
            for (int i = 0, len = _data.Armurie.Count; i < len; i++)
            {
                var item = _data.Armurie[i];
                W_collection.Add((item.x, (WeaponType)item.y, (Scopes)item.z));
            }
            characterWeapons = CombatSystem.WeaponEditor.GetWeapons(W_collection);
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
            if (!originalAsset)
            {
                originalAsset = CharactersLibrary.Load(typeSelected, assetMainFilter);
                if (!originalAsset && CharactersLibrary.Save(typeSelected, assetMainFilter))
                    originalAsset = CharactersLibrary.Load(typeSelected, assetMainFilter);
                if (originalAsset)
                    asset = originalAsset;
            }
            if (currentEditorMode == EditorMode.DataEdition)
            {
                data = ((CharactersLibrary)asset).DataList.Find(d => { return d.ID == dataID; });
                if (((CharacterData)data) != null)
                    SetPreviewAdditives((CharacterData)data);
            }
            RefreshPreview();
        }

        protected override void OnRedraw()
        {
            base.OnRedraw();
            if (!Header())
                return;
            GUILayout.BeginHorizontal();
            ScrollablePanel(() =>
            {
                CharacterList((CharactersLibrary)asset);
                SaveAndCancel();
            }, true);
            GUILayout.Space(5);
            ScrollablePanel(() =>
            {
                Details((CharacterData)data);
                GUILayout.Space(5);
                AnimationsPreview((CharacterData)data);
            });
            GUILayout.EndHorizontal();
        }

        protected override void OnQuit()
        {
            if (previewer != null)
                previewer.Destroy();
            ResetPreviewAdditives();
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
            SetPreviewAdditives((CharacterData)data);
        }

        #endregion

        /// <Summary>
        /// Implement here miscelaneous methods relative to the module in editor mode.
        /// </Summary>
        #region Helpers & Tools ################################################################################################################################################################################################

        #endregion

    }
}