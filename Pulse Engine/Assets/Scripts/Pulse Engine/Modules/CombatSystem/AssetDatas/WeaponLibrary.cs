using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Globals;
using PulseEngine.Modules;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace PulseEngine.Modules.CombatSystem
{
    /// <summary>
    /// L'asset des armes par type.
    /// </summary>
    [System.Serializable]
    public class WeaponLibrary : ScriptableObject, IModuleAsset
    {
        #region Attributs #########################################################

        [SerializeField]
        private List<WeaponData> dataList = new List<WeaponData>();

        [SerializeField]
        private int libraryWeaponType;

        [SerializeField]
        private int libraryDataType;

        [SerializeField]
        private int scope;

        #endregion

        #region Proprietes ##########################################################

        /// <summary>
        /// La liste des armes
        /// </summary>
        public List<WeaponData> DataList { get => dataList; set => dataList = value; }

        /// <summary>
        /// Le type d'arme contenus dans cet asset.
        /// </summary>
        public WeaponType LibraryWeaponType { get => (WeaponType)libraryWeaponType; set => libraryWeaponType = (int)value; }

        /// <summary>
        /// Le type de data.
        /// </summary>
        public DataTypes DataType  => (DataTypes)libraryDataType;

        /// <summary>
        /// Le scope.
        /// </summary>
        public Scopes Scope { get => (Scopes)scope; set => scope = (int)value; }

        #endregion

#if UNITY_EDITOR //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Methodes #############################################################

        /// <summary>
        /// Cree l'asset d'armes.
        /// </summary>
        /// <returns></returns>
        public static bool Save(WeaponType _weaponType, Scopes scope)
        {
            string fileName = "Weapon_" + _weaponType + "_" + scope + ".Asset";
            string path = ModuleConstants.AssetsPath;
            string folderPath = string.Join("/", PulseEngineMgr.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", PulseEngineMgr.Path_GAMERESSOURCES, path, fileName);
            if (!AssetDatabase.IsValidFolder(PulseEngineMgr.Path_GAMERESSOURCES))
                return false;
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(PulseEngineMgr.Path_GAMERESSOURCES, path);
                AssetDatabase.SaveAssets();
            }
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                WeaponLibrary asset = ScriptableObject.CreateInstance<WeaponLibrary>();
                asset.libraryDataType = (int)DataTypes.WeaponData;
                asset.LibraryWeaponType = _weaponType;
                asset.Scope = scope;
                AssetDatabase.CreateAsset(asset, fullPath);
                AssetDatabase.SaveAssets();
                //Make a gameobject an addressable
                var settings = AddressableAssetSettingsDefaultObject.Settings;
                if (settings != null)
                {
                    AddressableAssetGroup g = settings.DefaultGroup;
                    if (g != null)
                    {
                        var guid = AssetDatabase.AssetPathToGUID(fullPath);
                        //This is the function that actually makes the object addressable
                        var entry = settings.CreateOrMoveEntry(guid, g);
                        //You'll need these to run to save the changes!
                        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
                        AssetDatabase.SaveAssets();
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifie si l'asset existe.
        /// </summary>
        /// <returns></returns>
        public static bool Exist(WeaponType _weaponType, Scopes scope)
        {
            string fileName = "Weapon_" + _weaponType + "_" + scope + ".Asset";
            string path = ModuleConstants.AssetsPath;
            string folderPath = string.Join("/", PulseEngineMgr.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", PulseEngineMgr.Path_GAMERESSOURCES, path, fileName);
            if (!AssetDatabase.IsValidFolder(PulseEngineMgr.Path_GAMERESSOURCES))
                return false;
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(PulseEngineMgr.Path_GAMERESSOURCES, path);
                AssetDatabase.SaveAssets();
            }
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                if (AssetDatabase.LoadAssetAtPath<WeaponLibrary>(fullPath) == null)
                    return false;
                else
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Charge l'asset.
        /// </summary>
        /// <returns></returns>
        public static WeaponLibrary Load(WeaponType _weaponType, Scopes scope)
        {
            string fileName = "Weapon_" + _weaponType + "_" + scope + ".Asset";
            string path = ModuleConstants.AssetsPath;
            string folderPath = string.Join("/", PulseEngineMgr.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", PulseEngineMgr.Path_GAMERESSOURCES, path, fileName);
            if (Exist(_weaponType, scope))
            {
                return AssetDatabase.LoadAssetAtPath(fullPath, typeof(WeaponLibrary)) as WeaponLibrary;
            }
            else
                return null;
        }

        #endregion

#endif
    }
}