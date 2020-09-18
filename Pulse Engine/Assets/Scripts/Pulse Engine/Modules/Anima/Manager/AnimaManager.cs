using PulseEngine.Datas;
using System.Collections.Generic;
using UnityEngine.AddressableAssets;
using System.Threading.Tasks;

namespace PulseEngine.Modules.Anima
{
    /// <summary>
    /// Le manager du module Anima
    /// </summary>
    public static class AnimaManager
    {
        #region Attributes ####################################################################

        /// <summary>
        /// Le chemin d'access des datas.
        /// </summary>
        public static string AssetsPath { get => "AnimaDatas"; }

        #endregion

        #region Methods ####################################################################

        /// <summary>
        /// Get all module datas with specified parameters
        /// </summary>
        /// <param name="_avatarType"></param>
        /// <param name="_animType"></param>
        /// <returns></returns>
        public static async Task<List<AnimaData>> GetDatas(AvatarType _avatarType, AnimaType _animType)
        {
            var library = await Addressables.LoadAssetAsync<AnimaLibrary>("AnimaLibrary_" + _avatarType + "_" + _animType).Task;
            return Core.DeepCopy(library).DataList;
        }

        /// <summary>
        /// Get module data with ID
        /// </summary>
        /// <param name="_avatarType"></param>
        /// <param name="_animType"></param>
        /// <param name="_id"></param>
        /// <returns></returns>
        public static async Task<AnimaData> GetData(AvatarType _avatarType, AnimaType _animType, int _id)
        {
            var list = await GetDatas(_avatarType, _animType);
            return list.Find(data => { return data.ID == _id; });
        }

        #endregion

        #region Extension&Helpers ####################################################################

        #endregion
    }
}