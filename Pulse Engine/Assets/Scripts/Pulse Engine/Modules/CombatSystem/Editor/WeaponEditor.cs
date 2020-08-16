using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEditor.Globals;
using PulseEngine.Globals;
using PulseEngine.Modules;
using PulseEditor.Modules.Localisator;
using PulseEngine.Module.CharacterCreator;
using UnityEditor;
using System;
using PulseEngine.Modules.CombatSystem;
using PulseEditor.Modules.StatHandler;
using PulseEngine.Modules.StatHandler;
using System.Linq;
using PulseEngine.Modules.PhysicSpace;

namespace PulseEditor.Modules.CombatSystem
{
    /// <summary>
    /// L'editeur d'arsenal.
    /// </summary>
    public class WeaponEditor : PulseEditorMgr
    {
        #region Fonctionnal Attributes ################################################################

        /// <summary>
        /// La liste des gameobject et leurs previsualisation.
        /// </summary>
        private Dictionary<GameObject, Editor> weaponPartsEditors = new Dictionary<GameObject, Editor>();


        #endregion

        #region Visual Attributes ################################################################

        /// <summary>
        /// Le type d'arme selectionne.
        /// </summary>
        private WeaponType weaponTypeSelected;


        #endregion

        #region Fonctionnal Methods ################################################################


        /// <summary>
        /// Initialisation.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            var all = LibraryFiller(allAssets.ConvertAll<WeaponLibrary>(new Converter<ScriptableObject, WeaponLibrary>(target => { return (WeaponLibrary)target; })), currentScope);
            allAssets = all.ConvertAll<ScriptableObject>(new Converter<WeaponLibrary, ScriptableObject>(target => { return (ScriptableObject)target; }));
            allAssets.ForEach(a => { if (((WeaponLibrary)a).LibraryWeaponType == weaponTypeSelected) asset = a; });
            if (asset)
                editedAsset = asset;
        }

        #endregion

        #region Static Methods ################################################################

        /// <summary>
        /// open weapon editor.
        /// </summary>
        [MenuItem(Menu_EDITOR_MENU + "Weapon Editor")]
        public static void OpenEditor()
        {
            var window = GetWindow<WeaponEditor>();
            window.windowOpenMode = EditorMode.Normal;
            window.Show();
        }

        /// <summary>
        /// open weapon selector.
        /// </summary>
        public static void OpenSelector(Action<object, EventArgs> onSelect)
        {
            var window = GetWindow<WeaponEditor>();
            window.windowOpenMode = EditorMode.Selector;
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
        public static void OpenModifier(int _id, WeaponType type, Scopes _scope)
        {
            var window = GetWindow<WeaponEditor>(true);
            window.windowOpenMode = EditorMode.ItemEdition;
            window.dataID = _id;
            window.currentScope = _scope;
            window.weaponTypeSelected = type;
            window.Show();
        }

        /// <summary>
        /// Affiche l'arsenal correspondant aux id qu'il recoit.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="winName"></param>
        public static void ShowArmury(List<int> weaponsId, Action<object> returneWeaponry, string winName = "")
        {

        }

        #endregion

        #region Visual Methods ################################################################


        /// <summary>
        /// Refresh.
        /// </summary>
        protected override void OnRedraw()
        {
            base.OnRedraw();

            GUILayout.BeginHorizontal();
            ScrollablePanel(() =>
            {
                Header();
                WeaponList((WeaponLibrary)editedAsset);
                Foot();
            },true);
            ScrollablePanel(() =>
            {
                WeaponDetails((WeaponData)editedData);
            });
            GUILayout.EndHorizontal();
        }

        #endregion

        #region Common Windows ################################################################



        /// <summary>
        /// The header.
        /// </summary>
        protected bool Header()
        {
            var bkpType = weaponTypeSelected;
            GroupGUInoStyle(() =>
            {
                int selected = (int)weaponTypeSelected;
                selected = GUILayout.Toolbar((int)weaponTypeSelected , Enum.GetNames(typeof(WeaponType)));
                weaponTypeSelected = (WeaponType)selected;
            },"Weapon Type",50);
            if (bkpType != weaponTypeSelected)
            {
                allAssets.Clear();
                asset = null;
                editedAsset = null;
                editedData = null;
                OnInitialize();
            }
            return asset != null;
        }

        /// <summary>
        /// The footer.
        /// </summary>
        protected void Foot()
        {
            SaveBarPanel(editedAsset, asset);
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
                        for (int i = 0; i < library.DataList.Count; i++)
                        {
                            var data = library.DataList[i];
                            var nameList = LocalisationEditor.GetTexts(data.IdTrad, data.TradType);
                            string name = nameList.Length > 0 ? nameList[0] : string.Empty;
                            char[] titleChars = new char[LIST_MAX_CHARACTERS];
                            string pointDeSuspension = string.Empty;
                            try
                            {
                                if (data.ID > maxId) maxId = data.ID;
                                for (int j = 0; j < titleChars.Length; j++)
                                    if (j < name.Length)
                                        titleChars[j] = name[j];
                                if (name.Length >= titleChars.Length)
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
                            library.DataList.Add(new WeaponData { ID = maxId + 1, TradType = TradDataTypes.Weapon, TypeArme = weaponTypeSelected }) ;
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
                    editedData = library.DataList[selectDataIndex];
                else
                    editedData = null;
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
            Func<bool> listCompatiblesmode = () =>
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
                        return false;
                    default:
                        return false;
                }
            };
            if (!listCompatiblesmode())
                return;
            ScrollablePanel(() =>
            {
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
                    data.TypeDegats = (TypeDegatArme)EditorGUILayout.EnumPopup("Damage type: ",data.TypeDegats);
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
                        StatEditor.OpenStatWindow(data.StatWeapon, (obj) => {
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
                        StatEditor.OpenStatWindow(data.StatOwner, (obj) => {
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
                    if (GUILayout.Button("Edit Arm Animation"))
                    {
                        //TODO: Anima Editor.
                    }
                    if (GUILayout.Button("Edit Disarm Animation"))
                    {
                        //TODO: Anima Editor.
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
                    GUILayout.Label("Count: "+data.Weapons.Count);
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
            });
            ScrollablePanel(() =>
            {
                GroupGUInoStyle(() =>
                {
                    GUILayout.BeginHorizontal();
                    for (int i = 0; i < data.Weapons.Count; i++)
                    {
                        GroupGUI(() =>
                        {
                            data.Weapons[i] = EditorGUILayout.ObjectField(data.Weapons[i], typeof(GameObject), false) as GameObject;
                            if (data.Weapons[i] == null)
                            {
                                // if (weaponPartsEditors.ContainsKey(null))
                                // weaponPartsEditors.Remove(null);
                                GUILayout.BeginArea(GUILayoutUtility.GetRect(150, 150));
                                GUILayout.EndArea();
                            }
                            else
                            {
                                if (!weaponPartsEditors.ContainsKey(data.Weapons[i]))
                                    weaponPartsEditors.Add(data.Weapons[i], Editor.CreateEditor(data.Weapons[i]));
                                if (weaponPartsEditors[data.Weapons[i]] == null || weaponPartsEditors[data.Weapons[i]].target == null)
                                    weaponPartsEditors[data.Weapons[i]] = Editor.CreateEditor(data.Weapons[i]);
                                weaponPartsEditors[data.Weapons[i]].OnInteractivePreviewGUI(GUILayoutUtility.GetRect(150, 150), null);
                            }
                            //Materiau
                            var d = data.Materiaux;
                            var mat = d[i];
                            mat = (PhysicMaterials)EditorGUILayout.EnumPopup("Materiau: ",mat);
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
                            restPlace.RotationOffset.eulerAngles = EditorGUILayout.Vector3Field("rotation offset: ", data.RestPlaces[i].RotationOffset.eulerAngles);
                            data.RestPlaces[i] = restPlace;
                            //Emplacement armee
                            EditorGUILayout.LabelField("Carry Place", EditorStyles.boldLabel);
                            var carryPlace = data.CarryPlaces[i];
                            carryPlace.ParentBone = (HumanBodyBones)EditorGUILayout.EnumPopup("Parent Bone",data.CarryPlaces[i].ParentBone);
                            carryPlace.PositionOffset = EditorGUILayout.Vector3Field("position offset: ", data.CarryPlaces[i].PositionOffset);
                            carryPlace.RotationOffset.eulerAngles = EditorGUILayout.Vector3Field("rotation offset: ", data.CarryPlaces[i].RotationOffset.eulerAngles);
                            data.CarryPlaces[i] = carryPlace;
                            //point sortie projectile
                            EditorGUILayout.LabelField("Projectile", EditorStyles.boldLabel);
                            data.ProjectilesOutPoints[i] = EditorGUILayout.Vector3Field("Projectile out Pt: ", data.ProjectilesOutPoints[i]);

                        }, 180);
                    }
                    GUILayout.EndHorizontal();
                });
            });
            ScrollablePanel(() =>
            {
            });
        }

        #endregion

        #region Helpers & Tools ################################################################

        /// <summary>
        /// Charge tous les assets d'arme.
        /// </summary>
        /// <param name="inputLibrary"></param>
        /// <returns></returns>
        private static List<WeaponLibrary> LibraryFiller(List<WeaponLibrary> inputLibrary, Scopes _scope)
        {
            if (inputLibrary == null)
                inputLibrary = new List<WeaponLibrary>();
            foreach(WeaponType type in Enum.GetValues(typeof(WeaponType)))
            {
                if (type == WeaponType.aucun)
                    continue;
                if (WeaponLibrary.Exist(type, _scope))
                    inputLibrary.Add(WeaponLibrary.Load(type, _scope));
                else if(WeaponLibrary.Save(type, _scope))
                    inputLibrary.Add(WeaponLibrary.Load(type, _scope));
            }
            return inputLibrary;
        }

        #endregion
    }
}