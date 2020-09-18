using PulseEngine.Datas;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

namespace PulseEngine.Modules.CharacterCreator
{
    /// <summary>
    /// Le manager du mondule de creation de charactere.
    /// </summary>
    public static class CharacterManager
    {
        #region Attributes ####################################################################

        /// <summary>
        /// Le chemin d'access des datas.
        /// </summary>
        public static string AssetsPath { get => "CharactersDatas"; }

        #endregion

        #region Methods ####################################################################

        /// <summary>
        /// Get all module datas with specified parameters
        /// </summary>
        /// <returns></returns>
        public static async Task<List<CharacterData>> GetDatas(CharacterType type, Scopes _scope = Scopes.tous)
        {
            var library = await Addressables.LoadAssetAsync<CharactersLibrary>("Characters_" + _scope + "_" + type).Task;
            return Core.DeepCopy(library).DataList;
        }

        /// <summary>
        /// Get module data with ID
        /// </summary>
        /// <returns></returns>
        public static async Task<CharacterData> GetData(CharacterType _type, int _id, Scopes _scope = Scopes.tous)
        {
            var list = await GetDatas(_type, _scope);
            return list.Find(data => { return data.ID == _id; });
        }

        #endregion

        #region Extension&Helpers ####################################################################

        #endregion
    }
}