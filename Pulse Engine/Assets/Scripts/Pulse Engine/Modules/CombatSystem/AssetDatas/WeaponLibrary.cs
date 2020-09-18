using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Modules;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using PulseEngine.Datas;

namespace PulseEngine.Modules.CombatSystem
{
    /// <summary>
    /// L'asset des armes par type.
    /// </summary>
    [System.Serializable]
    public class WeaponLibrary : ScriptableObject
    {
        #region Attributs #########################################################

        [SerializeField]
        private List<WeaponData> dataList = new List<WeaponData>();

        [SerializeField]
        private int scope;

        #endregion

        #region Proprietes ##########################################################

        /// <summary>
        /// La liste des datas
        /// </summary>
        public List<WeaponData> DataList { get => dataList; set => dataList = value; }
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
        public static bool Save(Scopes scope)
        {
            string fileName = "Weapons_" + scope + ".Asset";
            string path = CombatSystemManager.AssetsPath;
            string folderPath = string.Join("/", Core.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", Core.Path_GAMERESSOURCES, path, fileName);
            if (!AssetDatabase.IsValidFolder(Core.Path_GAMERESSOURCES))
                return false;
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(Core.Path_GAMERESSOURCES, path);
                AssetDatabase.SaveAssets();
            }
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                WeaponLibrary asset = ScriptableObject.CreateInstance<WeaponLibrary>();
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
        public static bool Exist(Scopes scope)
        {
            string fileName = "Weapons_" + scope + ".Asset";
            string path = CombatSystemManager.AssetsPath;
            string folderPath = string.Join("/", Core.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", Core.Path_GAMERESSOURCES, path, fileName);
            if (!AssetDatabase.IsValidFolder(Core.Path_GAMERESSOURCES))
                return false;
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(Core.Path_GAMERESSOURCES, path);
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
        public static WeaponLibrary Load(Scopes _scope)
        {
            string fileName = "Weapons_" + _scope + ".Asset";
            string path = CombatSystemManager.AssetsPath;
            string folderPath = string.Join("/", Core.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", Core.Path_GAMERESSOURCES, path, fileName);
            if (Exist(_scope))
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