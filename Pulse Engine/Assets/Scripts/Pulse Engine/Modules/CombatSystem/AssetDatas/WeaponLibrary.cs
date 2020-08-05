using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Core;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace PulseEngine.Module.CombatSystem
{
    /// <summary>
    /// L'asset des armes par type.
    /// </summary>
    [System.Serializable]
    public class WeaponLibrary : ScriptableObject
    {
        #region Attributs #########################################################

        [SerializeField]
        private List<WeaponData> weaponList = new List<WeaponData>();

        [SerializeField]
        private int libraryWeaponType;

        [SerializeField]
        private int libraryDataType;

        #endregion

        #region Proprietes ##########################################################

        /// <summary>
        /// La liste des armes
        /// </summary>
        public List<WeaponData> WeaponList { get => weaponList; set => weaponList = value; }

        /// <summary>
        /// Le type d'arme contenus dans cet asset.
        /// </summary>
        public CombatSystemManager.WeaponType LibraryWeaponType { get => (CombatSystemManager.WeaponType)libraryWeaponType; set => libraryWeaponType = (int)value; }

        /// <summary>
        /// Le type de data.
        /// </summary>
        public PulseCore_GlobalValue_Manager.DataType LibraryDataType { get => (PulseCore_GlobalValue_Manager.DataType)libraryDataType; set => libraryDataType = (int)value; }

        #endregion

#if UNITY_EDITOR //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Methodes #############################################################

        /// <summary>
        /// Cree l'asset d'armes.
        /// </summary>
        /// <returns></returns>
        public static bool Create(CombatSystemManager.WeaponType _weaponType)
        {
            string fileName = "Weapon_"+ _weaponType.ToString() + ".Asset";
            string path = CombatSystemManager.AssetsPath;
            string folderPath = string.Join("/", PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path, fileName);
            if (!AssetDatabase.IsValidFolder(PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES))
                return false;
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path);
                AssetDatabase.SaveAssets();
            }
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                WeaponLibrary asset = ScriptableObject.CreateInstance<WeaponLibrary>();
                asset.libraryDataType = (int)PulseCore_GlobalValue_Manager.DataType.Weapon;
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
        /// <param name="language">La langue cible</param>
        /// <param name="dataType">Le type cible.</param>
        /// <returns></returns>
        public static bool Exist(CombatSystemManager.WeaponType _weaponType)
        {
            string fileName = "Weapon_" + _weaponType.ToString() + ".Asset";
            string path = CombatSystemManager.AssetsPath;
            string folderPath = string.Join("/", PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path, fileName);
            if (!AssetDatabase.IsValidFolder(PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES))
                return false;
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path);
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
        /// <param name="language">La langue cible</param>
        /// <param name="dataType">Le type cible.</param>
        /// <returns></returns>
        public static WeaponLibrary Load(CombatSystemManager.WeaponType _weaponType)
        {
            string fileName = "Weapon_" + _weaponType.ToString() + ".Asset";
            string path = CombatSystemManager.AssetsPath;
            string folderPath = string.Join("/", PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path, fileName);
            if (Exist(_weaponType))
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