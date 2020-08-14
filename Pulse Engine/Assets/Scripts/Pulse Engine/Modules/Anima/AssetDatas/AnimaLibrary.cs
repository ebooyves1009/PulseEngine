using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using PulseEngine.Globals;

namespace PulseEngine.Modules.Anima
{
    /// <summary>
    /// la library d'animation par type et catagorie.
    /// </summary>
    [System.Serializable]
    public class AnimaLibrary : ScriptableObject, IModuleAsset
    {
        #region Attributs #########################################################

        [SerializeField]
        private List<AnimaData> datasList = new List<AnimaData>();
        [SerializeField]
        private int animCategory;
        [SerializeField]
        private int animType;
        [SerializeField]
        private int dataType;
        [SerializeField]
        private int scope;

        #endregion
        #region Proprietes ##########################################################

        /// <summary>
        /// La liste des animations.
        /// </summary>
        public List<AnimaData> DataList { get => datasList; set => datasList = value; }

        /// <summary>
        /// La categorie de l'animation; representant souvent le type d'anatomie de la cible.
        /// </summary>
        public AnimaCategory AnimCategory { get => (AnimaCategory)animCategory; set => animCategory = (int)value; }

        /// <summary>
        /// La type d'animation, idle, locomotion etc...
        /// </summary>
        public AnimaType AnimType { get => (AnimaType)animType; set => animType = (int)value; }

        /// <summary>
        /// Le type de data.
        /// </summary>
        DataTypes IModuleAsset.DataType => (DataTypes)dataType;

        /// <summary>
        /// Le scope de celui qui a cree.
        /// </summary>
        public Scopes Scope { get => (Scopes)scope; set => scope = (int)value; }


        #endregion
#if UNITY_EDITOR //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Methodes #############################################################

        /// <summary>
        /// Cree l'asset.
        /// </summary>
        /// <returns></returns>
        public static bool Save(AnimaCategory _cat, AnimaType _typ)
        {
            string fileName = "AnimaLibrary_" + _cat +"_"+_typ+ ".Asset";
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
                AnimaLibrary asset = ScriptableObject.CreateInstance<AnimaLibrary>();
                asset.dataType = (int)DataTypes.AnimaData;
                asset.AnimType = _typ;
                asset.AnimCategory = _cat;
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
        /// <returns></returns>
        public static bool Exist(AnimaCategory _cat, AnimaType _typ)
        {
            string fileName = "AnimaLibrary_" + _cat + "_" + _typ + ".Asset";
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
                if (AssetDatabase.LoadAssetAtPath<AnimaLibrary>(fullPath) == null)
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
        public static AnimaLibrary Load(AnimaCategory _cat, AnimaType _typ)
        {
            string fileName = "AnimaLibrary_" + _cat + "_" + _typ + ".Asset";
            string path = ModuleConstants.AssetsPath;
            string folderPath = string.Join("/", PulseEngineMgr.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", PulseEngineMgr.Path_GAMERESSOURCES, path, fileName);
            if (Exist(_cat, _typ))
            {
                return AssetDatabase.LoadAssetAtPath(fullPath, typeof(AnimaLibrary)) as AnimaLibrary;
            }
            else
                return null;
        }

        #endregion

#endif
    }
}
