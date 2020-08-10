using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEditor;
using PulseEngine.Core;
using PulseEditor.Module.Localisator;
using PulseEngine.Module.CharacterCreator;
using UnityEditor;
using System;
using PulseEngine.Module.CombatSystem;
using PulseEditor.Module.StatManager;
using PulseEngine.Module.StatHandler;
using System.Linq;
using PulseEngine.Module.PhysicSpace;

namespace PulseEditor.Module.CombatSystem
{
    /// <summary>
    /// L'editeur d'arsenal.
    /// </summary>
    public class WeaponEditor : PulseEngine_Core_BaseEditor
    {
        #region Fonctionnal Attributes ################################################################

        /// <summary>
        /// toutes les armes de tous les types.
        /// </summary>
        private List<WeaponLibrary> allAssets = new List<WeaponLibrary>();

        /// <summary>
        /// L'asset permanent.
        /// </summary>
        private WeaponLibrary asset = null;

        /// <summary>
        /// L'asset temporaire en cours de modification.
        /// </summary>
        private WeaponLibrary editedAsset = null;

        /// <summary>
        /// La data en cours de modification.
        /// </summary>
        private WeaponData editedData = null;

        /// <summary>
        /// La liste des gameobject et leurs previsualisation.
        /// </summary>
        private Dictionary<GameObject, Editor> weaponPartsEditors = new Dictionary<GameObject, Editor>();


        #endregion
        #region Visual Attributes ################################################################

        /// <summary>
        /// Le type d'arme selectionne.
        /// </summary>
        private CombatSystemManager.WeaponType weaponTypeSelected;

        /// <summary>
        /// l'index de la data choisie dans la liste des datas.
        /// </summary>
        private int indexDataSelected;


        #endregion
        #region Fonctionnal Methods ################################################################

        #endregion
        #region Visual Methods ################################################################

        /// <summary>
        /// open weapon editor.
        /// </summary>
        [MenuItem(PulseCore_GlobalValue_Manager.Menu_EDITOR_MENU + "Weapon Editor")]
        public static void OpenEditor()
        {
            var window = GetWindow<WeaponEditor>();
            window.windowOpenMode = EditorMode.Normal;
            window.Show();
        }

        /// <summary>
        /// open weapon selector.
        /// </summary>
        public static void OpenSelector(Action<object,EventArgs> onSelect)
        {
            var window = GetWindow<WeaponEditor>();
            window.windowOpenMode = EditorMode.Selector;
            if(onSelect != null)
            {
                window.onSelectionEvent += (obj, arg) => {
                    onSelect.Invoke(obj, arg);
                };
            }
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
        #region Common Windows ################################################################

        /// <summary>
        /// Initialisation.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            allAssets = LibraryFiller(allAssets);
            allAssets.ForEach(a => { if (a.LibraryWeaponType == weaponTypeSelected) asset = a; });
            if (asset)
                editedAsset = asset;
        }


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
                WeaponList(editedAsset);
                Foot();
            });
            ScrollablePanel(() =>
            {
                WeaponDetails(editedData);
            });
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// The header.
        /// </summary>
        protected bool Header()
        {
            var bkpType = weaponTypeSelected;
            GroupGUInoStyle(() =>
            {
                int selected = GUILayout.Toolbar((int)weaponTypeSelected , Enum.GetNames(typeof(CombatSystemManager.WeaponType)));
                weaponTypeSelected = (CombatSystemManager.WeaponType)selected;
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
            SaveCancelPanel(new[] {
                        new KeyValuePair<string, System.Action> ( "Save", () => {
                            SaveAsset(editedAsset,asset);
                            Close(); } ),
                        new KeyValuePair<string, System.Action>("Cancel", () => { Close(); })
                    });
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
                        for (int i = 0; i < library.WeaponList.Count; i++)
                        {
                            var data = library.WeaponList[i];
                            var nameList = LocalisationEditor.GetTexts(data.IdTrad, data.TradDataType);
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
                        indexDataSelected = ListItems(indexDataSelected, listContent.ToArray());
                        GUILayout.Space(5);
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("+"))
                        {
                            library.WeaponList.Add(new WeaponData { ID = maxId + 1, TradDataType = PulseCore_GlobalValue_Manager.DataType.Weapon, TypeArme = weaponTypeSelected }) ;
                        }
                        if (indexDataSelected >= 0 && indexDataSelected < editedAsset.WeaponList.Count)
                        {
                            if (GUILayout.Button("-"))
                            {
                                library.WeaponList.RemoveAt(indexDataSelected);
                            }
                        }
                        GUILayout.EndHorizontal();
                        GUILayout.EndVertical();
                    }, "Weapon Datas List");
                });
                if (indexDataSelected >= 0 && indexDataSelected < editedAsset.WeaponList.Count)
                    editedData = editedAsset.WeaponList[indexDataSelected];
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
                    string[] names = LocalisationEditor.GetTexts(data.IdTrad, data.TradDataType);
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
                        }, data.TradDataType);
                    }
                    if (GUILayout.Button("E", new[] { GUILayout.Width(25) }))
                    {
                        LocalisationEditor.OpenModifier(data.IdTrad, data.TradDataType);
                    }
                    GUILayout.EndHorizontal();

                    //Degats
                    data.TypeDegats = (CombatSystemManager.TypeDegatArme)EditorGUILayout.EnumPopup("Damage type: ",data.TypeDegats);
                    data.Degats = EditorGUILayout.FloatField("Damage Value: ", data.Degats);
                    data.Degats = Mathf.Clamp(data.Degats, 1, data.Degats);
                    //portee
                    data.Portee = EditorGUILayout.FloatField("Max Range: ", data.Portee);
                    Vector2 ranges = Vector2.zero;
                    switch (data.TypeArme)
                    {
                        case CombatSystemManager.WeaponType.shortRange:
                            ranges = new Vector2(1, 4);
                            break;
                        case CombatSystemManager.WeaponType.LongRange:
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
                        d.Add(PhysicManager.PhysicMaterials.none);
                        data.Materiaux = d;
                        data.RestPlaces.Add(new CombatSystemManager.WeaponPlace());
                        data.CarryPlaces.Add(new CombatSystemManager.WeaponPlace());
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
                            mat = (PhysicManager.PhysicMaterials)EditorGUILayout.EnumPopup("Materiau: ",mat);
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
        private static List<WeaponLibrary> LibraryFiller(List<WeaponLibrary> inputLibrary)
        {
            if (inputLibrary == null)
                inputLibrary = new List<WeaponLibrary>();
            foreach(CombatSystemManager.WeaponType type in Enum.GetValues(typeof(CombatSystemManager.WeaponType)))
            {
                if (type == CombatSystemManager.WeaponType.aucun)
                    continue;
                if (WeaponLibrary.Exist(type))
                    inputLibrary.Add(WeaponLibrary.Load(type));
                else if(WeaponLibrary.Create(type))
                    inputLibrary.Add(WeaponLibrary.Load(type));
            }
            return inputLibrary;
        }

        #endregion
    }
}