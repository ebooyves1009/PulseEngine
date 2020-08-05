using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Core;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;



namespace PulseEngine.Module.CharacterCreator
{
    /// <summary>
    /// L'asset des characters.
    /// </summary>
    [System.Serializable]
    public class CharactersLibrary : ScriptableObject
    {
        #region Attributs #########################################################

        [SerializeField]
        private List<CharacterData> characterlist = new List<CharacterData>();

        [SerializeField]
        private int libraryDataType;

        #endregion
        #region Proprietes ##########################################################

        /// <summary>
        /// La liste des characters.
        /// </summary>
        public List<CharacterData> Characterlist { get => characterlist; set => characterlist = value; }

        /// <summary>
        /// Le type des datas de l'asset.
        /// </summary>
        public PulseCore_GlobalValue_Manager.DataType LibraryDataType { get { return (PulseCore_GlobalValue_Manager.DataType)libraryDataType; } }

        #endregion
#if UNITY_EDITOR //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Methodes #############################################################

        /// <summary>
        /// Cree l'asset.
        /// </summary>
        /// <returns></returns>
        public static bool Create(CharacterManager.CharacterType type)
        {
            string fileName = "Characters_"+type+".Asset";
            string path = CharacterManager.AssetsPath;
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
                CharactersLibrary asset = ScriptableObject.CreateInstance<CharactersLibrary>();
                asset.libraryDataType = (int)PulseCore_GlobalValue_Manager.DataType.CharacterInfos;
                Debug.Log("Created with : " + asset.LibraryDataType + " at " + fullPath);
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
        public static bool Exist(CharacterManager.CharacterType type)
        {
            string fileName = "Characters_" + type + ".Asset";
            string path = CharacterManager.AssetsPath;
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
        public static CharactersLibrary Load(CharacterManager.CharacterType type)
        {
            string fileName = "Characters_" + type + ".Asset";
            string path = CharacterManager.AssetsPath;
            string folderPath = string.Join("/", PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path, fileName);
            if (Exist(type))
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