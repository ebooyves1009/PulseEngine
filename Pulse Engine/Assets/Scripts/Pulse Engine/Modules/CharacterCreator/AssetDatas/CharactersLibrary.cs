using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using PulseEngine.Modules;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using PulseEngine.Datas;

namespace PulseEngine.Modules.CharacterCreator
{
    /// <summary>
    /// L'asset des characters.
    /// </summary>
    [System.Serializable]
    public class CharactersLibrary : CoreLibrary
    {
        #region Attributs #########################################################

        [SerializeField]
        private List<CharacterData> dataList = new List<CharacterData>();

        #endregion

        #region Proprietes ##########################################################

        /// <summary>
        /// La liste des characters.
        /// </summary>
        public override List<IData> DataList
        {
            get
            {
                if (dataList == null)
                {
                    dataList = new List<CharacterData>();
                }
                return dataList.ConvertAll<IData>(new System.Converter<CharacterData, IData>(item => { return item; }));
            }
            set
            {
                if (dataList == null)
                {
                    dataList = new List<CharacterData>();
                }
                dataList = value.ConvertAll<CharacterData>(new System.Converter<IData, CharacterData>(item => { return (CharacterData)item; })); ;
            }
        }

        /// <summary>
        /// Le scope de l'asset.
        /// </summary>
        public Scopes Scope { get => (Scopes)libraryMainLocation; set => libraryMainLocation = (int)value; }

        #endregion
    }
}