﻿using System.Collections.Generic;
using UnityEngine;

namespace PulseEngine.Modules.Localisator
{
    /// <summary>
    /// L'asset des datas de localisation par Langue.
    /// </summary>
    [System.Serializable]
    public class LocalisationLibrary : CoreLibrary
    {
        #region Attributs #########################################################

        [SerializeField]
        private List<Localisationdata> dataList = new List<Localisationdata>();

        #endregion

        #region Proprietes ##########################################################


        /// <summary>
        /// La liste des datas localisees pour ce type de data, en cette langue.
        /// </summary>
        public override List<IData> DataList
        {
            get
            {
                if (dataList == null)
                {
                    dataList = new List<Localisationdata>();
                }
                return dataList.ConvertAll<IData>(new System.Converter<Localisationdata, IData>(item => { return item; }));
            }
            set
            {
                if (dataList == null)
                {
                    dataList = new List<Localisationdata>();
                }
                dataList = value.ConvertAll<Localisationdata>(new System.Converter<IData, Localisationdata>(item => { return (Localisationdata)item; })); ;
            }
        }

        /// <summary>
        /// La langue des datas de l'asset.
        /// </summary>
        public Languages Langage { get { return (Languages)libraryMainLocation; } }

        /// <summary>
        /// Le type de TradDatas de l'asset.
        /// </summary>
        public TradDataTypes TradType { get { return (TradDataTypes)librarySecLocation; } }


        #endregion
    }
}