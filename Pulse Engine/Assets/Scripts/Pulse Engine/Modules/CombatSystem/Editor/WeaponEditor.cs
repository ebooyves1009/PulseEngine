using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Modules.CombatSystem;
using System;
using UnityEditor;
using System.Linq;
using PulseEngine;
using PulseEngine.Datas;

namespace PulseEditor.Modules.CombatSystem
{
    /// <summary>
    /// L'editeur d'arsenal.
    /// </summary>
    public class WeaponEditor : PulseEditorMgr
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
            if (_dtype != DataTypes.Weapon)
                return;
            var dictionnary = _dictionnary as Dictionary<DataLocation, IData>;
            if (dictionnary == null)
                return;

            var allAsset = new List<WeaponLibrary>();
            foreach(var scp in Enum.GetValues(typeof(Scopes))){
                if(CoreLibrary.Exist<WeaponLibrary>(AssetsPath, scp))
                {
                    var load = CoreLibrary.Load<WeaponLibrary>(AssetsPath, scp);
                    if (load != null)
                        allAsset.Add(load);
                }else if (CoreLibrary.Save<WeaponLibrary>(AssetsPath, scp))
                {
                    var load = CoreLibrary.Load<WeaponLibrary>(AssetsPath, scp);
                    if (load != null)
                        allAsset.Add(load);
                }
            }

            foreach (var entry in dictionnary)
            {
                if (entry.Value == null)
                {
                    var library = allAsset.Find(lib => { return lib.Scope == (Scopes)entry.Key.globalLocation; });
                    if (library != null)
                    {
                        var data = library.DataList.Find(d =>
                        {
                            return d.Location.id == entry.Key.id;
                        }) as WeaponData;
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
        /// the preview.
        /// </summary>
        private Previewer preview;

        /// <summary>
        /// Le scroll de la vue des parts de l'arme.
        /// </summary>
        private Vector2 weaponPartsScroll;

        /// <summary>
        /// either show weapon parts on rest position or not.
        /// </summary>
        private bool showRestPosition;

        #endregion

        /// <Summary>
        /// Declare here every attribute used for deep behaviour ot the editor window.
        /// </Summary>
        #region Fonctionnal Attributes ################################################################################################################################################################################################

        /// <summary>
        /// La liste des gameobject et leurs previsualisation.
        /// </summary>
        private Dictionary<GameObject, Editor> weaponPartsEditors = new Dictionary<GameObject, Editor>();


        /// <summary>
        /// Le chemin d'access des datas.
        /// </summary>
        public const string AssetsPath = "CombatDatas"; 


        #endregion

        /// <Summary>
        /// Implement here Methods To Open the window, and register to OnCacheRefresh
        /// </Summary>
        #region Door Methods ################################################################################################################################################################################################

        /// <summary>
        /// open weapon editor.
        /// </summary>
        [MenuItem(Menu_EDITOR_MENU + "Weapon Editor")]
        public static void OpenEditor()
        {
            if (!registeredToRefresh)
            {
                OnCacheRefresh += RefreshCache;
                registeredToRefresh = true;
            }
            var window = GetWindow<WeaponEditor>();
            window.currentEditorMode = EditorMode.Edition;
            window.editorDataType = DataTypes.Weapon;
            window.Show();
        }

        /// <summary>
        /// open weapon selector.
        /// </summary>
        public static void OpenSelector(Action<object, EventArgs> onSelect)
        {
            if (!registeredToRefresh)
            {
                OnCacheRefresh += RefreshCache;
                registeredToRefresh = true;
            }
            var window = GetWindow<WeaponEditor>();
            window.currentEditorMode = EditorMode.Selection;
            window.editorDataType = DataTypes.Weapon;
            if (onSelect != null)
            {
                window.onSelectionEvent += (obj, arg) => {
                    onSelect.Invoke(obj, arg);
                };
            }
            window.Show();
        }

        /// <summary>
        /// open weapon Modifier.
        /// </summary>
        public static void OpenModifier(DataLocation _location)
        {
            if (!registeredToRefresh)
            {
                OnCacheRefresh += RefreshCache;
                registeredToRefresh = true;
            }
            var window = GetWindow<WeaponEditor>(true);
            window.currentEditorMode = EditorMode.DataEdition;
            window.editorDataType = DataTypes.Weapon;
            window.dataID = _location.id;
            window.assetMainFilter = _location.globalLocation;
            window.OnInitialize();
            window.ShowModal();
        }

        #endregion

        /// <Summary>
        /// Implement here Methods related to GUI.
        /// </Summary>
        #region GUI Methods ################################################################################################################################################################################################


        /// <summary>
        /// Refresh the preview
        /// </summary>
        private void RefreshPreview()
        {
            if (preview != null)
                preview.Destroy();
            preview = new Previewer();
        }

        /// <summary>
        /// The header.
        /// </summary>
        protected void Header()
        {
            ScopeSelector();
        }

        /// <summary>
        /// The weapons list in this category.
        /// </summary>
        /// <param name="library"></param>
        /// <param name="wType"></param>
        protected void WeaponList(WeaponLibrary library)
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
                ScrollablePanel(() =>
                {
                    GroupGUI(() =>
                    {
                        GUILayout.BeginVertical();
                        List<string> listContent = new List<string>();
                        int maxId = 0;
                        for (int i = 0; i < library.DataList.Count; i++)
                        {
                            var data = library.DataList[i];
                            var nameList = LocalisationEditor.GetTexts(data.IdTrad, data.TradType);
                            string name = nameList.Length > 0 ? nameList[0] : string.Empty;
                            listContent.Add(name);
                        }
                        selectDataIndex = MakeList(selectDataIndex, listContent.ToArray());
                        GUILayout.Space(5);
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("+"))
                        {
                            library.DataList.Add(new WeaponData { ID = maxId + 1, TradType = TradDataTypes.Weapon, TypeArme = weaponTypeSelected });
                        }
                        if (selectDataIndex >= 0 && selectDataIndex < library.DataList.Count)
                        {
                            if (GUILayout.Button("-"))
                            {
                                library.DataList.RemoveAt(selectDataIndex);
                            }
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                    }, "Weapon Datas List");
                });
                if (selectDataIndex >= 0 && selectDataIndex < library.DataList.Count)
                    data = library.DataList[selectDataIndex];
                else
                    data = null;
            }
        }

        /// <summary>
        /// the selected weapon datas.
        /// </summary>
        /// <param name="data"></param>
        protected void WeaponDetails(WeaponData data)
        {
            if (data == null)
                return;
            if (currentEditorMode == EditorMode.Selection)
            {
                GroupGUInoStyle(() =>
                {
                    weaponPartsScroll = GUILayout.BeginScrollView(weaponPartsScroll);
                    GUILayout.BeginHorizontal();
                    for (int i = 0; i < data.Weapons.Count; i++)
                    {
                        GroupGUI(() =>
                        {
                            var obj = EditorGUILayout.ObjectField(data.Weapons[i], typeof(GameObject), false) as GameObject;
                            if (obj != data.Weapons[i])
                                OnListChange();
                            data.Weapons[i] = obj;
                            if (data.Weapons[i] == null)
                            {
                                // if (weaponPartsEditors.ContainsKey(null))
                                // weaponPartsEditors.Remove(null);
                                GUILayout.BeginArea(GUILayoutUtility.GetRect(100, 100));
                                GUILayout.EndArea();
                            }
                            else
                            {
                                if (!weaponPartsEditors.ContainsKey(data.Weapons[i]))
                                    weaponPartsEditors.Add(data.Weapons[i], Editor.CreateEditor(data.Weapons[i]));
                                if (weaponPartsEditors[data.Weapons[i]] == null || weaponPartsEditors[data.Weapons[i]].target == null)
                                    weaponPartsEditors[data.Weapons[i]] = Editor.CreateEditor(data.Weapons[i]);
                                weaponPartsEditors[data.Weapons[i]].OnInteractivePreviewGUI(GUILayoutUtility.GetRect(100, 100), null);
                            }

                        }, 180);
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndScrollView();
                });
                return;
            }
            Func<bool> listCompatiblesmode = () =>
            {
                switch (currentEditorMode)
                {
                    case EditorMode.Edition:
                        return true;
                    case EditorMode.Selection:
                        return false;
                    case EditorMode.DataEdition:
                        return true;
                    case EditorMode.Preview:
                        return false;
                    default:
                        return false;
                }
            };
            if (!listCompatiblesmode())
                return;
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            //Global params
            GroupGUI(() =>
            {
                //ID
                EditorGUILayout.LabelField("ID: " + data.ID, style_label);
                //ID trad
                EditorGUILayout.LabelField("ID Trad: " + data.IdTrad, style_label);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Weapon Type: ", style_label);
                EditorGUILayout.LabelField(data.TypeArme.ToString());
                GUILayout.EndHorizontal();
                string name = string.Empty;
                string[] names = LocalisationEditor.GetTexts(data.IdTrad, data.TradType);
                if (names.Length > 0)
                    name = names[0];
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name: ", style_label);
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

                //Degats
                data.TypeDegats = (TypeDegatArme)EditorGUILayout.EnumPopup("Damage type: ", data.TypeDegats);
                data.Degats = EditorGUILayout.FloatField("Damage Value: ", data.Degats);
                data.Degats = Mathf.Clamp(data.Degats, 1, data.Degats);
                //portee
                data.Portee = EditorGUILayout.FloatField("Max Range: ", data.Portee);
                Vector2 ranges = Vector2.zero;
                switch (data.TypeArme)
                {
                    case WeaponType.shortRange:
                        ranges = new Vector2(1, 4);
                        break;
                    case WeaponType.LongRange:
                        ranges = new Vector2(4, float.PositiveInfinity);
                        break;
                }
                data.Portee = Mathf.Clamp(data.Portee, ranges.x, ranges.y);
                //Valeur
                data.Valeur = EditorGUILayout.FloatField("Valeur: ", data.Valeur);
                //Stat Arme
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Weapon's Stats:");
                if (GUILayout.Button("Edit " + name + " Stats"))
                {
                    StatEditor.OpenStatWindow(data.StatWeapon, (obj) =>
                    {
                        var st = obj as StatData;
                        if (st != null)
                            data.StatWeapon = st;
                    }, name + "'s Stats");
                }
                GUILayout.EndHorizontal();
                //Stat Donnees
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Stat given to Owner:");
                if (GUILayout.Button("Edit given Stats"))
                {
                    StatEditor.OpenStatWindow(data.StatOwner, (obj) =>
                    {
                        var st = obj as StatData;
                        if (st != null)
                            data.StatOwner = st;
                    }, "guiven Stats");
                }
                GUILayout.EndHorizontal();
                //Portabilitee
                data.Portable = EditorGUILayout.Toggle("Is portable? ", data.Portable);
                //Peux parrer
                data.CanParry = EditorGUILayout.Toggle("Can Parry Attacks? ", data.CanParry);
                //animation au repos
                EditorGUILayout.LabelField("Arm and Disasm animations:");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Equip Animation"))
                {
                    Anima.AnimaEditor.OpenSelector((obj, arg) =>
                    {
                        var d = obj as PulseEngine.Modules.Anima.AnimaData;
                        if (d != null)
                        {
                            try
                            {
                                data.HandleMoves[0] = d;
                            }
                            catch
                            {
                                while (data.HandleMoves.Count < 2)
                                {
                                    data.HandleMoves.Add(null);
                                }
                                data.HandleMoves[0] = d;
                            }
                        }
                    });
                }
                if (GUILayout.Button("Idle Animation"))
                {
                    Anima.AnimaEditor.OpenSelector((obj, arg) =>
                    {
                        var d = obj as PulseEngine.Modules.Anima.AnimaData;
                        if (d != null)
                        {
                            data.IdleMove = d;
                            RefreshPreview();
                        }
                    });
                }
                if (GUILayout.Button("UnEquip Animation"))
                {
                    Anima.AnimaEditor.OpenSelector((obj, arg) =>
                    {
                        var d = obj as PulseEngine.Modules.Anima.AnimaData;
                        if (d != null)
                        {
                            try
                            {
                                data.HandleMoves[1] = d;
                            }
                            catch
                            {
                                while (data.HandleMoves.Count < 2)
                                {
                                    data.HandleMoves.Add(null);
                                }
                                data.HandleMoves[1] = d;
                            }
                        }
                    });
                }
                GUILayout.EndHorizontal();
                //Weapon game objects
                EditorGUILayout.LabelField("Weapon Parts:");
                GUILayout.BeginHorizontal();
                if (data.Weapons.Count > 0 && GUILayout.Button("-"))
                {
                    data.Weapons.RemoveAt(data.Weapons.Count - 1);
                    if (data.Materiaux.Count > 0)
                    {
                        var d = data.Materiaux;
                        d.RemoveAt(data.Materiaux.Count - 1);
                        data.Materiaux = d;
                    }
                    if (data.RestPlaces.Count > 0)
                        data.RestPlaces.RemoveAt(data.RestPlaces.Count - 1);
                    if (data.CarryPlaces.Count > 0)
                        data.CarryPlaces.RemoveAt(data.CarryPlaces.Count - 1);
                    if (data.ProjectilesOutPoints.Count > 0)
                        data.ProjectilesOutPoints.RemoveAt(data.ProjectilesOutPoints.Count - 1);
                }
                GUILayout.Label("Count: " + data.Weapons.Count);
                if (data.Weapons.Count < 4 && GUILayout.Button("+"))
                {
                    data.Weapons.Add(new GameObject());
                    var d = data.Materiaux;
                    d.Add(PhysicMaterials.none);
                    data.Materiaux = d;
                    data.RestPlaces.Add(new WeaponPlace());
                    data.CarryPlaces.Add(new WeaponPlace());
                    data.ProjectilesOutPoints.Add(new Vector3());
                }
                GUILayout.EndHorizontal();

            }, "Common Parameters");
            //preview.
            if (data.IdleMove != null && data.IdleMove.Motion)
            {
                GroupGUI(() =>
                {
                    GUILayout.BeginVertical();
                    showRestPosition = EditorGUILayout.Toggle("At rest places", showRestPosition);
                    List<(GameObject, HumanBodyBones, Vector3, Quaternion)> weaponInfos = new List<(GameObject, HumanBodyBones, Vector3, Quaternion)>();
                    for (int i = 0; i < data.Weapons.Count; i++)
                    {
                        if (showRestPosition)
                            weaponInfos.Add((data.Weapons[i], data.RestPlaces[i].ParentBone, data.RestPlaces[i].PositionOffset, data.RestPlaces[i].RotationOffset));
                        else
                            weaponInfos.Add((data.Weapons[i], data.CarryPlaces[i].ParentBone, data.CarryPlaces[i].PositionOffset, data.CarryPlaces[i].RotationOffset));
                    }
                    if (preview != null)
                        preview.Previsualize(data.IdleMove.Motion, 18 / 9, null, weaponInfos.ToArray());
                    GUILayout.EndVertical();
                }, "Preview");
            }
            GUILayout.EndVertical();
            //Weapon parts
            GroupGUInoStyle(() =>
            {
                weaponPartsScroll = GUILayout.BeginScrollView(weaponPartsScroll);
                GUILayout.BeginHorizontal();
                for (int i = 0; i < data.Weapons.Count; i++)
                {
                    GroupGUI(() =>
                    {
                        var obj = EditorGUILayout.ObjectField(data.Weapons[i], typeof(GameObject), false) as GameObject;
                        if (obj != data.Weapons[i])
                            OnListChange();
                        data.Weapons[i] = obj;
                        if (data.Weapons[i] == null)
                        {
                            // if (weaponPartsEditors.ContainsKey(null))
                            // weaponPartsEditors.Remove(null);
                            GUILayout.BeginArea(GUILayoutUtility.GetRect(100, 100));
                            GUILayout.EndArea();
                        }
                        else
                        {
                            if (!weaponPartsEditors.ContainsKey(data.Weapons[i]))
                                weaponPartsEditors.Add(data.Weapons[i], Editor.CreateEditor(data.Weapons[i]));
                            if (weaponPartsEditors[data.Weapons[i]] == null || weaponPartsEditors[data.Weapons[i]].target == null)
                                weaponPartsEditors[data.Weapons[i]] = Editor.CreateEditor(data.Weapons[i]);
                            weaponPartsEditors[data.Weapons[i]].OnInteractivePreviewGUI(GUILayoutUtility.GetRect(100, 100), null);
                        }
                        //Materiau
                        var d = data.Materiaux;
                        var mat = d[i];
                        mat = (PhysicMaterials)EditorGUILayout.EnumPopup("Materiau: ", mat);
                        d[i] = mat;
                        if (mat != data.Materiaux[i])
                        {
                            data.Materiaux = d;
                        }
                        //Emplacement repos
                        EditorGUILayout.LabelField("Rest Place", EditorStyles.boldLabel);
                        var restPlace = data.RestPlaces[i];
                        restPlace.ParentBone = (HumanBodyBones)EditorGUILayout.EnumPopup("Parent Bone", data.RestPlaces[i].ParentBone);
                        restPlace.PositionOffset = EditorGUILayout.Vector3Field("position offset: ", data.RestPlaces[i].PositionOffset);
                        restPlace.RotationOffset = Quaternion.Euler(EditorGUILayout.Vector3Field("rotation offset: ", data.RestPlaces[i].RotationOffset.eulerAngles));
                        data.RestPlaces[i] = restPlace;
                        //Emplacement armee
                        EditorGUILayout.LabelField("Carry Place", EditorStyles.boldLabel);
                        var carryPlace = data.CarryPlaces[i];
                        carryPlace.ParentBone = (HumanBodyBones)EditorGUILayout.EnumPopup("Parent Bone", data.CarryPlaces[i].ParentBone);
                        carryPlace.PositionOffset = EditorGUILayout.Vector3Field("position offset: ", data.CarryPlaces[i].PositionOffset);
                        carryPlace.RotationOffset = Quaternion.Euler(EditorGUILayout.Vector3Field("rotation offset: ", data.CarryPlaces[i].RotationOffset.eulerAngles));
                        data.CarryPlaces[i] = carryPlace;
                        //point sortie projectile
                        EditorGUILayout.LabelField("Projectile", EditorStyles.boldLabel);
                        data.ProjectilesOutPoints[i] = EditorGUILayout.Vector3Field("Projectile out Pt: ", data.ProjectilesOutPoints[i]);

                    }, 180);
                }
                GUILayout.EndHorizontal();
                GUILayout.EndScrollView();
            });
            GUILayout.EndHorizontal();
        }


        #endregion

        /// <Summary>
        /// Implement here behaviours methods.
        /// </Summary>
        #region Fontionnal Methods ################################################################################################################################################################################################


        /// <summary>
        /// Initialisation.
        /// </summary>
        protected override void OnInitialize()
        {
            RefreshAssetSelect();
        }

        /// <summary>
        /// To call whenever we need to refresh asset selection.
        /// </summary>
        protected void RefreshAssetSelect()
        {
            var all = LibraryFiller(allAssets.ConvertAll<WeaponLibrary>(new Converter<ScriptableObject, WeaponLibrary>(target => { return (WeaponLibrary)target; })), assetMainFilter);
            allAssets = all.ConvertAll<ScriptableObject>(new Converter<WeaponLibrary, ScriptableObject>(target => { return (ScriptableObject)target; }));
            allAssets.ForEach(a => { if (((WeaponLibrary)a).LibraryWeaponType == weaponTypeSelected) originalAsset = a; });
            if (originalAsset)
                asset = originalAsset;
            RefreshPreview();
        }

        /// <summary>
        /// Executed at item selection in selection mode
        /// </summary>
        protected void SelectItem()
        {
            if (onSelectionEvent != null)
            {
                onSelectionEvent.Invoke(data, new EditorEventArgs { Scope = asset != null ? (int)((WeaponLibrary)asset).Scope : 0 });
            }
        }


        #endregion

        /// <Summary>
        /// Implement here overrides methods.
        /// </Summary>
        #region Program FLow Methods ################################################################################################################################################################################################


        /// <summary>
        /// Refresh.
        /// </summary>
        protected override void OnRedraw()
        {
            base.OnRedraw();

            GUILayout.BeginHorizontal();
            if (currentEditorMode != EditorMode.DataEdition)
            {
                ScrollablePanel(() =>
                {
                    Header();
                    WeaponList((WeaponLibrary)asset);
                    Foot();
                }, true);
            }
            else if (asset != null && data == null)
            {
                data = ((WeaponLibrary)asset).DataList.Find(d => { return d.ID == dataID; });
            }
            ScrollablePanel(() =>
            {
                WeaponDetails((WeaponData)data);
            });
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// on item changed on a list.
        /// </summary>
        protected override void OnListChange()
        {
            RefreshPreview();
        }

        /// <summary>
        /// on header changed.
        /// </summary>
        protected override void OnHeaderChange()
        {
            RefreshPreview();
        }

        #endregion

        /// <Summary>
        /// Implement here miscelaneous methods relative to the module in editor mode.
        /// </Summary>
        #region Helpers & Tools ################################################################################################################################################################################################

        /// <summary>
        /// Charge tous les assets d'arme.
        /// </summary>
        /// <param name="inputLibrary"></param>
        /// <returns></returns>
        private static List<WeaponLibrary> LibraryFiller(List<WeaponLibrary> inputLibrary, Scopes _scope)
        {
            if (inputLibrary == null)
                inputLibrary = new List<WeaponLibrary>();
            foreach (WeaponType type in Enum.GetValues(typeof(WeaponType)))
            {
                if (type == WeaponType.aucun)
                    continue;
                if (WeaponLibrary.Exist(type, _scope))
                    inputLibrary.Add(WeaponLibrary.Load(type, _scope));
                else if (WeaponLibrary.Save(type, _scope))
                    inputLibrary.Add(WeaponLibrary.Load(type, _scope));
            }
            return inputLibrary;
        }

        /// <summary>
        /// Get an weapon from its id, type and scope
        /// </summary>
        /// <param name="_ids"></param>
        /// <returns></returns>
        public static List<WeaponData> GetWeapons(List<(int _id, WeaponType _type, Scopes _scope)> collection)
        {
            List<WeaponData> retList = null;
            var allAs = new Dictionary<Scopes, List<WeaponLibrary>>();
            foreach (Scopes scope in Enum.GetValues(typeof(Scopes)))
            {
                allAs.Add(scope, LibraryFiller(new List<WeaponLibrary>(), scope));
            }
            foreach (var item in collection)
            {
                var subCol = allAs[item._scope];
                if (subCol != null)
                {
                    var library = subCol.Find(lib => { return lib.LibraryWeaponType == item._type; });
                    if (library != null)
                    {
                        var weapon = library.DataList.Find(w => { return w.ID == item._id; });
                        if (weapon != null)
                        {
                            if (retList == null)
                                retList = new List<WeaponData>();
                            retList.Add(weapon);
                        }
                    }
                }
            }

            return retList;
        }

        /// <summary>
        /// The weaponry editor
        /// </summary>
        public class WeaponryEditor : PulseEditorMgr
        {
            #region Attributes >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

            /// <summary>
            /// the passed weapons IDs Data Base.
            /// </summary>
            List<(WeaponData, Scopes)> dataBase = new List<(WeaponData, Scopes)>();

            #endregion

            #region Methods >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

            public static void Open(List<(int _id, WeaponType type, Scopes _scope)> weaponRefs, Action<object, EventArgs> onSelect)
            {
                var window = GetWindow<WeaponryEditor>();
                window.Init(weaponRefs);
                if (onSelect != null)
                    window.onSelectionEvent += (obj, arg) =>
                    {
                        onSelect.Invoke(obj, arg);
                    };
                window.Show();
            }


            protected void Init(List<(int _id, WeaponType type, Scopes _scope)> Refs)
            {
                List<WeaponLibrary> all = new List<WeaponLibrary>();
                foreach (Scopes sc in Enum.GetValues(typeof(Scopes)))
                {
                    all.AddRange(LibraryFiller(allAssets.ConvertAll<WeaponLibrary>(new Converter<ScriptableObject, WeaponLibrary>(target => { return (WeaponLibrary)target; })), sc));
                }
                for (int i = 0; i < Refs.Count; i++)
                {
                    WeaponData data = null;
                    Scopes _scope = Scopes.tous;
                    for (int j = 0; j < all.Count; j++)
                    {
                        var Library = all[j];
                        if (Library.Scope == Refs[i]._scope)
                        {
                            var d = Library.DataList.Find(d2 => { return d2.ID == Refs[i]._id; });
                            if (d != null)
                            {
                                data = d;
                                _scope = Library.Scope;
                            }
                        }
                    }
                    if (data != null)
                        dataBase.Add((data, _scope));
                }
            }

            protected override void OnRedraw()
            {
                //List
                GroupGUI(() =>
                {
                    GUILayout.BeginVertical();
                    List<string> listContent = new List<string>();
                    for (int i = 0; i < dataBase.Count; i++)
                    {
                        var data = dataBase[i];
                        var nameList = LocalisationEditor.GetTexts(data.Item1.IdTrad, data.Item1.TradType);
                        string name = nameList.Length > 0 ? nameList[0] : string.Empty;
                        listContent.Add(name);
                    }
                    selectDataIndex = MakeList(selectDataIndex, listContent.ToArray());
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("+"))
                    {
                        OpenSelector((obj, arg) =>
                        {
                            var data = obj as WeaponData;
                            var scope = arg as EditorEventArgs;
                            if (data != null && scope != null)
                                dataBase.Add((data, (Scopes)scope.Scope));
                        });
                    }
                    if (selectDataIndex >= 0 && selectDataIndex < dataBase.Count)
                    {
                        if (GUILayout.Button("Edit"))
                        {
                            var infos = dataBase[selectDataIndex];
                            OpenModifier(infos.Item1.ID, infos.Item1.TypeArme, infos.Item2);
                        }
                        if (GUILayout.Button("-"))
                        {
                            dataBase.RemoveAt(selectDataIndex);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }, "Weaponry");
                //Foot
                GroupGUInoStyle(() =>
                {
                    if (GUILayout.Button("Close"))
                    {
                        if (onSelectionEvent != null)
                            onSelectionEvent.Invoke(dataBase, null);
                        Close();
                    }
                }, "", 50);
            }

            #endregion
        }

        #endregion
    }
}