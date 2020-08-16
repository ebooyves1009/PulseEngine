using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PulseEngine.Globals;
using PulseEngine.Modules;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;



namespace PulseEngine.Modules.CharacterCreator
{
    /// <summary>
    /// L'asset des characters.
    /// </summary>
    [System.Serializable]
    public class CharactersLibrary : ScriptableObject, IModuleAsset
    {
        #region Attributs #########################################################

        [SerializeField]
        private List<CharacterData> dataList = new List<CharacterData>();

        [SerializeField]
        private int libraryDataType;

        [SerializeField]
        private int scope;

        #endregion
        #region Proprietes ##########################################################

        /// <summary>
        /// La liste des characters.
        /// </summary>
        public List<CharacterData> DataList { get => dataList; set => dataList = value; }

        /// <summary>
        /// Le type des datas de l'asset.
        /// </summary>
        public DataTypes DataType => (DataTypes)libraryDataType;

        /// <summary>
        /// Le scope de l'asset.
        /// </summary>
        public Scopes Scope { get => (Scopes)scope; set => scope = (int)value; }

        #endregion
#if UNITY_EDITOR //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Methodes #############################################################

        /// <summary>
        /// Cree l'asset.
        /// </summary>
        /// <returns></returns>
        public static bool Save(CharacterType type, Scopes _scope)
        {
            string fileName = "Characters_" + type + "_" + _scope + ".Asset";
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
                CharactersLibrary asset = ScriptableObject.CreateInstance<CharactersLibrary>();
                asset.libraryDataType = (int)DataTypes.CharacterData;
                asset.Scope = _scope;
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
        public static bool Exist(CharacterType type, Scopes _scope)
        {
            string fileName = "Characters_" + type + "_" + _scope + ".Asset";
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
                if (AssetDatabase.LoadAssetAtPath<CharactersLibrary>(fullPath) == null)
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
        public static CharactersLibrary Load(CharacterType type, Scopes _scope)
        {
            string fileName = "Characters_" + type + "_" + _scope + ".Asset";
            string path = ModuleConstants.AssetsPath;
            string folderPath = string.Join("/", PulseEngineMgr.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", PulseEngineMgr.Path_GAMERESSOURCES, path, fileName);
            if (Exist(type, _scope))
            {
                return AssetDatabase.LoadAssetAtPath(fullPath, typeof(CharactersLibrary)) as CharactersLibrary;
            }
            else
                return null;
        }

        #endregion

#endif
    }
}