using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using PulseEngine.Datas;

namespace PulseEngine.Modules.Anima
{
    /// <summary>
    /// la library d'animation par type et catagorie.
    /// </summary>
    [System.Serializable]
    public class AnimaLibrary : ScriptableObject
    {
        #region Attributs #########################################################

        [SerializeField]
        private List<AnimaData> datasList;
        [SerializeField]
        private int scope;
        [SerializeField]
        private int avatarType;
        [SerializeField]
        private int animType;

        #endregion

        #region Proprietes ##########################################################

        /// <summary>
        /// La liste des animations.
        /// </summary>
        public List<AnimaData> DataList { get { if (datasList == null) { datasList = new List<AnimaData>(); } return datasList; } set { if (datasList == null) { datasList = new List<AnimaData>(); } datasList = value; } }

        /// <summary>
        /// Le scope de celui qui a cree.
        /// </summary>
        public Scopes Scope { get => (Scopes)scope; set => scope = (int)value; }

        /// <summary>
        /// Le type d'avatar.
        /// </summary>
        public AvatarType AvatarType { get => (AvatarType)avatarType; set => avatarType = (int)value; }

        /// <summary>
        /// La type d'animation, idle, locomotion etc...
        /// </summary>
        public AnimaType AnimType { get => (AnimaType)animType; set => animType = (int)value; }



        #endregion

#if UNITY_EDITOR //++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++++

        #region Methodes #############################################################

        /// <summary>
        /// Cree l'asset.
        /// </summary>
        /// <returns></returns>
        public static bool Save(AvatarType _avatarType, AnimaType _animType)
        {
            string fileName = "AnimaLibrary_" + _avatarType +"_"+_animType+ ".Asset";
            string path = AnimaManager.AssetsPath;
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
                AnimaLibrary asset = ScriptableObject.CreateInstance<AnimaLibrary>();
                asset.AnimType = _animType;
                asset.AvatarType = _avatarType;
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
        public static bool Exist(AvatarType _avatarType, AnimaType _animaType)
        {
            string fileName = "AnimaLibrary_" + _avatarType + "_" + _animaType + ".Asset";
            string path = AnimaManager.AssetsPath;
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
        public static AnimaLibrary Load(AvatarType _avatarType, AnimaType _animaType)
        {
            string fileName = "AnimaLibrary_" + _avatarType + "_" + _animaType + ".Asset";
            string path = AnimaManager.AssetsPath;
            string folderPath = string.Join("/", Core.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", Core.Path_GAMERESSOURCES, path, fileName);
            if (Exist(_avatarType, _animaType))
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
