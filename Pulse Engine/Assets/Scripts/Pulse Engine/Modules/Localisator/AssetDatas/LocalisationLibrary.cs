using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using PulseEngine.Globals;

namespace PulseEngine.Modules.Localisator
{
    /// <summary>
    /// L'asset des datas de localisation par Langue.
    /// </summary>
    [System.Serializable]
    public class LocalisationLibrary : ScriptableObject, IModuleAsset
    {
        #region Attributs #########################################################

        [SerializeField]
        private List<Localisationdata> libraryDatasList = new List<Localisationdata>();

        [SerializeField]
        private int libraryLanguage;

        [SerializeField]
        private int libraryScope;

        [SerializeField]
        private int libraryDataType;

        [SerializeField]
        private int libraryTradType;

        #endregion

        #region Proprietes ##########################################################


        /// <summary>
        /// La liste des datas localisees pour ce type de data, en cette langue.
        /// </summary>
        public List<Localisationdata> DatasList { get { return libraryDatasList; } set { libraryDatasList = value; } }

        /// <summary>
        /// La langue des datas de l'asset.
        /// </summary>
        public Languages Langage { get { return (Languages)libraryLanguage; } }

        /// <summary>
        /// Le type des datas de l'asset.
        /// </summary>
        public DataTypes DataType { get { return (DataTypes)libraryDataType; } }

        /// <summary>
        /// Le type de TradDatas de l'asset.
        /// </summary>
        public TradDataTypes TradType { get { return (TradDataTypes)libraryTradType; } }

        /// <summary>
        /// Le scope de l'asset.
        /// </summary>
        public Scopes Scope { get => (Scopes)libraryScope; set => libraryScope = (int)value; }


        #endregion

#if UNITY_EDITOR //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Methodes #############################################################

        /// <summary>
        /// Cree l'asset de Localisation.
        /// </summary>
        /// <param name="language">La langue cible</param>
        /// <param name="tradDataType">Le type cible.</param>
        /// <returns></returns>
        public static bool Save(Languages language, TradDataTypes tradDataType)
        {
            string fileName = "Localisator_" + LocalisationManager.LanguageConverter(language) + "_" + tradDataType.ToString() + ".Asset";
            string path = ModuleConstants.AssetsPath;
            string folderPath = string.Join("/", Globals.PulseEngineMgr.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", Globals.PulseEngineMgr.Path_GAMERESSOURCES, path, fileName);
            if (!AssetDatabase.IsValidFolder(Globals.PulseEngineMgr.Path_GAMERESSOURCES))
                return false;
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(Globals.PulseEngineMgr.Path_GAMERESSOURCES, path);
                AssetDatabase.SaveAssets();
            }
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                LocalisationLibrary asset = ScriptableObject.CreateInstance<LocalisationLibrary>();
                asset.libraryTradType = (int)tradDataType;
                asset.libraryDataType = (int)DataTypes.tradData;
                asset.libraryLanguage = (int)language;
                asset.Scope = Scopes.tous;
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
        /// <param name="tradDataType">Le type cible.</param>
        /// <returns></returns>
        public static bool Exist(Languages language, TradDataTypes tradDataType)
        {
            string fileName = "Localisator_" + LocalisationManager.LanguageConverter(language) + "_" + tradDataType.ToString() + ".Asset";
            string path = ModuleConstants.AssetsPath;
            string folderPath = string.Join("/", Globals.PulseEngineMgr.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", Globals.PulseEngineMgr.Path_GAMERESSOURCES, path, fileName);
            if (!AssetDatabase.IsValidFolder(Globals.PulseEngineMgr.Path_GAMERESSOURCES))
                return false;
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(Globals.PulseEngineMgr.Path_GAMERESSOURCES, path);
                AssetDatabase.SaveAssets();
            }
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                if (AssetDatabase.LoadAssetAtPath<LocalisationLibrary>(fullPath) == null)
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
        /// <param name="tradDataType">Le type cible.</param>
        /// <returns></returns>
        public static LocalisationLibrary Load(Languages language, TradDataTypes tradDataType)
        {
            string fileName = "Localisator_" + LocalisationManager.LanguageConverter(language) + "_" + tradDataType.ToString() + ".Asset";
            string path = ModuleConstants.AssetsPath;
            string folderPath = string.Join("/", Globals.PulseEngineMgr.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", Globals.PulseEngineMgr.Path_GAMERESSOURCES, path, fileName);
            if (Exist(language, tradDataType))
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