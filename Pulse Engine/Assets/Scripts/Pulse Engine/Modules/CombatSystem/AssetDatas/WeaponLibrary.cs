using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Modules;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using PulseEngine.Datas;

namespace PulseEngine.Modules.CombatSystem
{
    /// <summary>
    /// L'asset des armes par type.
    /// </summary>
    [System.Serializable]
    public class WeaponLibrary : CoreLibrary
    {
        #region Attributs #########################################################

        [SerializeField]
        private List<WeaponData> dataList = new List<WeaponData>();

        #endregion

        #region Proprietes ##########################################################

        /// <summary>
        /// La liste des datas
        /// </summary>
        public override List<IData> DataList
        {
            get
            {
                if (dataList == null)
                {
                    dataList = new List<WeaponData>();
                }
                return dataList.ConvertAll<IData>(new System.Converter<WeaponData, IData>(item => { return item; }));
            }
            set
            {
                if (dataList == null)
                {
                    dataList = new List<WeaponData>();
                }
                dataList = value.ConvertAll<WeaponData>(new System.Converter<IData, WeaponData>(item => { return (WeaponData)item; })); ;
            }
        }

        /// <summary>
        /// Le scope.
        /// </summary>
        public Scopes Scope { get => (Scopes)libraryMainLocation; set => libraryMainLocation = (int)value; }

        #endregion
    }
}