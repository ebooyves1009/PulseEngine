using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Core;
using UnityEditor;
using System.IO;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;



namespace PulseEngine.Module.Localisator
{
    /// <summary>
    /// L'asset des datas de localisation par Langue.
    /// </summary>
    public class LocalisationLibrary : ScriptableObject
    {
        #region Attributs #########################################################

        private List<Localisationdata> localizedDatas;
        private int libraryLanguage;
        private int libraryDataType;

        #endregion

        #region Proprietes ##########################################################


        /// <summary>
        /// La liste des datas localisees pour ce type de data, en cette langue.
        /// </summary>
        public List<Localisationdata> LocalizedDatas { get { return localizedDatas; }set { localizedDatas = value; } }

        /// <summary>
        /// La langue des datas de l'asset.
        /// </summary>
        public PulseCore_GlobalValue_Manager.Languages LibraryLanguage { get { return (PulseCore_GlobalValue_Manager.Languages)libraryLanguage; } }

        /// <summary>
        /// Le type des datas de l'asset.
        /// </summary>
        public PulseCore_GlobalValue_Manager.DataType LibraryDataType { get { return (PulseCore_GlobalValue_Manager.DataType)libraryDataType; } }

        #endregion

#if UNITY_EDITOR //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Methodes #############################################################

        /// <summary>
        /// Cree l'asset de Localisation.
        /// </summary>
        /// <param name="language">La langue cible</param>
        /// <param name="dataType">Le type cible.</param>
        /// <returns></returns>
        public static bool Create(PulseCore_GlobalValue_Manager.Languages language, PulseCore_GlobalValue_Manager.DataType dataType)
        {
            string fileName = "Localisator_" + LocalisationManager.LanguageConverter(language) + "_" + dataType.ToString() + ".Asset";
            string path = LocalisationManager.AssetsPath;
            string folderPath = Path.Combine(PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path);
            string fullPath = Path.Combine(PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path, fileName);
            if (!AssetDatabase.IsValidFolder(PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES))
                return false;
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path);
                AssetDatabase.SaveAssets();
            }
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                LocalisationLibrary asset = new LocalisationLibrary();
                asset.libraryDataType = (int)dataType;
                asset.libraryLanguage = (int)language;
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
        public static bool Exist(PulseCore_GlobalValue_Manager.Languages language, PulseCore_GlobalValue_Manager.DataType dataType)
        {
            string fileName = "Localisator_" + LocalisationManager.LanguageConverter(language) + "_" + dataType.ToString() + ".Asset";
            string path = LocalisationManager.AssetsPath;
            string folderPath = Path.Combine(PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path);
            string fullPath = Path.Combine(PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path, fileName);
            Debug.Log("Full Path : " + fullPath);
            if (!AssetDatabase.IsValidFolder(PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES))
                return false;
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path);
                AssetDatabase.SaveAssets();
            }
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                if (AssetDatabase.AssetPathToGUID(fullPath) == "")
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
        public static LocalisationLibrary Load(PulseCore_GlobalValue_Manager.Languages language, PulseCore_GlobalValue_Manager.DataType dataType)
        {
            string fileName = "Localisator_" + LocalisationManager.LanguageConverter(language) + "_" + dataType.ToString() + ".Asset";
            string path = LocalisationManager.AssetsPath;
            string folderPath = Path.Combine(PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path);
            string fullPath = Path.Combine(PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path, fileName);
            if (Exist(language, dataType))
            {
                return AssetDatabase.LoadAssetAtPath(fullPath, typeof(LocalisationLibrary)) as LocalisationLibrary;
            }
            else
                return null;
        }

        #endregion

#endif
    }
}