using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Modules;
using System.Threading.Tasks;
using PulseEngine.Datas;
using UnityEngine.AddressableAssets;

namespace PulseEngine.Modules.CombatSystem
{
    public static class CombatSystemManager
    {
        #region Attributes ####################################################################

        /// <summary>
        /// Le chemin d'access des datas.
        /// </summary>
        public static string AssetsPath { get => "CombatDatas"; }

        #endregion

        #region Methods ####################################################################

        /// <summary>
        /// Get all module datas with specified parameters
        /// </summary>
        /// <returns></returns>
        public static async Task<List<WeaponData>> GetDatas(Scopes _scope = Scopes.tous)
        {
            var library = await Addressables.LoadAssetAsync<WeaponLibrary>("Weapons_" + _scope).Task;
            return Core.DeepCopy(library).DataList;
        }

        /// <summary>
        /// Get module data with ID
        /// </summary>
        /// <returns></returns>
        public static async Task<WeaponData> GetData(int _id, Scopes _scope = Scopes.tous)
        {
            var list = await GetDatas(_scope);
            return list.Find(data => { return data.ID == _id; });
        }

        #endregion

        #region Extension&Helpers ####################################################################

        #endregion
    }
}