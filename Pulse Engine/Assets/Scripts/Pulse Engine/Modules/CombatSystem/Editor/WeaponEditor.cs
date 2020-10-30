using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Modules.CombatSystem;
using System;
using UnityEditor;
using System.Linq;
using PulseEngine;
using PulseEngine.Datas;

namespace PulseEditor.Modules
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
        /// The current previsualised animation clip.
        /// </summary>
        private AnimationClip previewClip;

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
                    for (int i = 0; i < data.ComponentParts.Count; i++)
                    {
                        GroupGUI(() =>
                        {
                            var obj = EditorGUILayout.ObjectField(data.ComponentParts[i], typeof(GameObject), false) as GameObject;
                            if (obj != data.ComponentParts[i])
                                OnListChange();
                            data.ComponentParts[i] = obj;
                            if (data.ComponentParts[i] == null)
                            {
                                // if (weaponPartsEditors.ContainsKey(null))
                                // weaponPartsEditors.Remove(null);
                                GUILayout.BeginArea(GUILayoutUtility.GetRect(100, 100));
                                GUILayout.EndArea();
                            }
                            else
                            {
                                if (!weaponPartsEditors.ContainsKey(data.ComponentParts[i]))
                                    weaponPartsEditors.Add(data.ComponentParts[i], Editor.CreateEditor(data.ComponentParts[i]));
                                if (weaponPartsEditors[data.ComponentParts[i]] == null || weaponPartsEditors[data.ComponentParts[i]].target == null)
                                    weaponPartsEditors[data.ComponentParts[i]] = Editor.CreateEditor(data.ComponentParts[i]);
                                weaponPartsEditors[data.ComponentParts[i]].OnInteractivePreviewGUI(GUILayoutUtility.GetRect(100, 100), null);
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
            DataLocation previewCliploc = default(DataLocation);
            GroupGUI(() =>
            {
                //ID
                EditorGUILayout.LabelField("ID: " + data.Location.id, style_label);
                //ID trad
                EditorGUILayout.LabelField("ID Trad: " + data.TradLocation.id, style_label);
                var locData = (Localisationdata)GetCachedData(data.TradLocation);
                string name = locData != null ? locData.Title.textField : "";
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name: ", style_label);
                EditorGUILayout.LabelField(name);
                if (GUILayout.Button("S", new[] { GUILayout.Width(25) }))
                {
                    ModuleSelector(ModulesEditors.LocalisationEditor, (obj, arg) =>
                    {
                        if (arg != null)
                            data.TradLocation = arg.dataObjectLocation;
                    }, TradDataTypes.Weapon);
                }
                if (GUILayout.Button("E", new[] { GUILayout.Width(25) }))
                {
                    ModuleModifier(ModulesEditors.LocalisationEditor, data.TradLocation);
                }
                GUILayout.EndHorizontal();

                //Degats
                data.TypeDegats = (TypeDegatArme)EditorGUILayout.EnumPopup("Damage type: ", data.TypeDegats);
                data.Degats = EditorGUILayout.FloatField("Damage Value: ", data.Degats);
                data.Degats = Mathf.Clamp(data.Degats, 1, data.Degats);
                //Valeur
                data.Cost = EditorGUILayout.FloatField("Valeur: ", data.Cost);
                //Stat Arme
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Weapon's Stats:");
                if (GUILayout.Button("Edit " + name + " Stats"))
                {
                    StatWinEditor.OpenEditor(data.PhysicProperties, obj =>
                    {
                        if (obj != null)
                            data.PhysicProperties = (PhysicStats)obj;
                    });
                }
                GUILayout.EndHorizontal();
                //animation au repos
                EditorGUILayout.LabelField("Arm and Disasm animations:");
                GUILayout.BeginHorizontal();
                if (GUILayout.Button("Draw Animation"))
                {
                    ModuleSelector(ModulesEditors.AnimaEditor, (obj, arg) =>
                    {
                        if (arg != null)
                        {
                            data.DrawMove = arg.dataObjectLocation;
                            previewCliploc = arg.dataObjectLocation;
                        }
                    });
                }
                if (GUILayout.Button("Idle Animation"))
                {
                    ModuleSelector(ModulesEditors.AnimaEditor, (obj, arg) =>
                    {
                        if (arg != null)
                        {
                            data.IdleMove = arg.dataObjectLocation;
                            previewCliploc = arg.dataObjectLocation;
                        }
                    });
                }
                if (GUILayout.Button("Sheath Animation"))
                {
                    ModuleSelector(ModulesEditors.AnimaEditor, (obj, arg) =>
                    {
                        if (arg != null)
                        {
                            data.SheathMove = arg.dataObjectLocation;
                            previewCliploc = arg.dataObjectLocation;
                        }
                    });
                }
                GUILayout.EndHorizontal();
                //Weapon game objects
                EditorGUILayout.LabelField("Weapon Parts:");
                GUILayout.BeginHorizontal();
                if (data.ComponentParts.Count > 0 && GUILayout.Button("-"))
                {
                    data.ComponentParts.RemoveAt(data.ComponentParts.Count - 1);
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
                GUILayout.Label("Count: " + data.ComponentParts.Count);
                if (data.ComponentParts.Count < 4 && GUILayout.Button("+"))
                {
                    data.ComponentParts.Add(new GameObject());
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
            if (previewClip == null)
            {
                previewCliploc = data.IdleMove;
            }
            if (previewCliploc != default(DataLocation))
            {
                AnimaData dataAnim = GetCachedData(previewCliploc) as AnimaData;
                if (dataAnim != null)
                    previewClip = dataAnim.Motion;
            }

            if (data.IdleMove != null && previewClip)
            {
                GroupGUI(() =>
                {
                    GUILayout.BeginVertical();
                    showRestPosition = EditorGUILayout.Toggle("At rest places", showRestPosition);
                    List<(GameObject, HumanBodyBones, Vector3, Vector3)> weaponInfos = new List<(GameObject, HumanBodyBones, Vector3, Vector3)>();
                    for (int i = 0; i < data.ComponentParts.Count; i++)
                    {
                        if (showRestPosition)
                            weaponInfos.Add((data.ComponentParts[i], data.RestPlaces[i].ParentBone, data.RestPlaces[i].positionOffset, data.RestPlaces[i].rotationOffset));
                        else
                            weaponInfos.Add((data.ComponentParts[i], data.CarryPlaces[i].ParentBone, data.CarryPlaces[i].positionOffset, data.CarryPlaces[i].rotationOffset));
                    }
                    if (preview != null)
                        preview.Previsualize(previewClip, 18 / 9, null, weaponInfos.ToArray());
                    GUILayout.EndVertical();
                }, "Preview");
            }
            GUILayout.EndVertical();
            //Weapon parts
            GroupGUInoStyle(() =>
            {
                weaponPartsScroll = GUILayout.BeginScrollView(weaponPartsScroll);
                GUILayout.BeginHorizontal();
                for (int i = 0; i < data.ComponentParts.Count; i++)
                {
                    GroupGUI(() =>
                    {
                        var obj = EditorGUILayout.ObjectField(data.ComponentParts[i], typeof(GameObject), false) as GameObject;
                        if (obj != data.ComponentParts[i])
                            OnListChange();
                        data.ComponentParts[i] = obj;
                        if (data.ComponentParts[i] == null)
                        {
                            // if (weaponPartsEditors.ContainsKey(null))
                            // weaponPartsEditors.Remove(null);
                            GUILayout.BeginArea(GUILayoutUtility.GetRect(100, 100));
                            GUILayout.EndArea();
                        }
                        else
                        {
                            if (!weaponPartsEditors.ContainsKey(data.ComponentParts[i]))
                                weaponPartsEditors.Add(data.ComponentParts[i], Editor.CreateEditor(data.ComponentParts[i]));
                            if (weaponPartsEditors[data.ComponentParts[i]] == null || weaponPartsEditors[data.ComponentParts[i]].target == null)
                                weaponPartsEditors[data.ComponentParts[i]] = Editor.CreateEditor(data.ComponentParts[i]);
                            weaponPartsEditors[data.ComponentParts[i]].OnInteractivePreviewGUI(GUILayoutUtility.GetRect(100, 100), null);
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
                        restPlace.positionOffset = EditorGUILayout.Vector3Field("position offset: ", data.RestPlaces[i].positionOffset);
                        restPlace.rotationOffset = EditorGUILayout.Vector3Field("rotation offset: ", data.RestPlaces[i].rotationOffset);
                        data.RestPlaces[i] = restPlace;
                        //Emplacement armee
                        EditorGUILayout.LabelField("Carry Place", EditorStyles.boldLabel);
                        var carryPlace = data.CarryPlaces[i];
                        carryPlace.ParentBone = (HumanBodyBones)EditorGUILayout.EnumPopup("Parent Bone", data.CarryPlaces[i].ParentBone);
                        carryPlace.positionOffset = EditorGUILayout.Vector3Field("position offset: ", data.CarryPlaces[i].positionOffset);
                        carryPlace.rotationOffset = EditorGUILayout.Vector3Field("rotation offset: ", data.CarryPlaces[i].rotationOffset);
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
            var all = LibraryFiller(allAssets.ConvertAll<WeaponLibrary>(new Converter<ScriptableObject, WeaponLibrary>(target => { return (WeaponLibrary)target; })), (Scopes)assetMainFilter);
            allAssets = all.ConvertAll<CoreLibrary>(new Converter<WeaponLibrary, CoreLibrary>(target => { return target; }));
            allAssets.ForEach(a => { if (((WeaponLibrary)a).Scope == (Scopes)assetMainFilter) originalAsset = a; });
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
                IData idata = data as IData;
                onSelectionEvent.Invoke(data, new EditorEventArgs { dataObjectLocation = idata != null ? idata.Location : default });
            }
        }


        #endregion

        /// <Summary>
        /// Implement here overrides methods.
        /// </Summary>
        #region Program FLow Methods ################################################################################################################################################################################################

        protected override void OnHeaderRedraw()
        {
            Header();
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

        protected override void OnBodyRedraw()
        {
            WeaponDetails((WeaponData)data);
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
            if (CoreLibrary.Exist<WeaponLibrary>(AssetsPath, _scope))
                inputLibrary.Add(CoreLibrary.Load<WeaponLibrary>(AssetsPath, _scope));
            else if (CoreLibrary.Save<WeaponLibrary>(AssetsPath, _scope))
                inputLibrary.Add(CoreLibrary.Load<WeaponLibrary>(AssetsPath, _scope));
            return inputLibrary;
        }

        #endregion
    }
}