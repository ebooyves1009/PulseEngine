using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Core;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;

namespace PulseEngine.Module.Anima
{
    /// <summary>
    /// la library d'animation par type et catagorie.
    /// </summary>
    [System.Serializable]
    public class AnimaLibrary : ScriptableObject
    {
        #region Attributs #########################################################

        [SerializeField]
        private int animCategory;
        [SerializeField]
        private int animType;
        [SerializeField]
        private int dataType;

        #endregion
        #region Proprietes ##########################################################

        /// <summary>
        /// La categorie de l'animation; representant souvent le type d'anatomie de la cible.
        /// </summary>
        public AnimaManager.AnimaCategory AnimCategory { get => (AnimaManager.AnimaCategory)animCategory; set => animCategory = (int)value; }

        /// <summary>
        /// La type d'animation, idle, locomotion etc...
        /// </summary>
        public AnimaManager.AnimationType AnimType { get => (AnimaManager.AnimationType)animType; set => animType = (int)value; }

        /// <summary>
        /// Le type de data.
        /// </summary>
        public PulseCore_GlobalValue_Manager.DataType DataType { get => (PulseCore_GlobalValue_Manager.DataType)dataType; set => dataType = (int)value; }

        #endregion
#if UNITY_EDITOR //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Methodes #############################################################

        /// <summary>
        /// Cree l'asset.
        /// </summary>
        /// <returns></returns>
        public static bool Create(AnimaManager.AnimaCategory _cat, AnimaManager.AnimationType _typ)
        {
            string fileName = "AnimaLibrary_" + _cat +"_"+_typ+ ".Asset";
            string path = AnimaManager.AssetsPath;
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
                AnimaLibrary asset = ScriptableObject.CreateInstance<AnimaLibrary>();
                asset.dataType = (int)PulseCore_GlobalValue_Manager.DataType.Animations;
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
        public static bool Exist(AnimaManager.AnimaCategory _cat, AnimaManager.AnimationType _typ)
        {
            string fileName = "AnimaLibrary_" + _cat + "_" + _typ + ".Asset";
            string path = AnimaManager.AssetsPath;
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
        public static AnimaLibrary Load(AnimaManager.AnimaCategory _cat, AnimaManager.AnimationType _typ)
        {
            string fileName = "AnimaLibrary_" + _cat + "_" + _typ + ".Asset";
            string path = AnimaManager.AssetsPath;
            string folderPath = string.Join("/", PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", PulseCore_GlobalValue_Manager.Path_GAMERESSOURCES, path, fileName);
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
