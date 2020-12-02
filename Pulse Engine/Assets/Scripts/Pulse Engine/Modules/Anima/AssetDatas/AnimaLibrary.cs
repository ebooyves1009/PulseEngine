using System.Collections.Generic;
using UnityEngine;

namespace PulseEngine.Modules.Anima
{
    /// <summary>
    /// la library d'animation par type et catagorie.
    /// </summary>
    [System.Serializable]
    public class AnimaLibrary : CoreLibrary
    {
        #region Attributs #########################################################

        [SerializeField]
        private List<AnimaData> dataList;

        #endregion

        #region Proprietes ##########################################################

        /// <summary>
        /// La liste des animations.
        /// </summary>
        public override List<IData> DataList {
            get
            {
                if (dataList == null)
                {
                    dataList = new List<AnimaData>();
                }
                return dataList.ConvertAll<IData>(new System.Converter<AnimaData, IData>(item => { return item; }));
            }
            set
            {
                if (dataList == null)
                {
                    dataList = new List<AnimaData>();
                }
                dataList = value.ConvertAll<AnimaData>(new System.Converter<IData, AnimaData>(item => { return (AnimaData)item; })); ;
            }
        }

        /// <summary>
        /// Le scope de celui qui a cree.
        /// </summary>
        public Scopes Scope { get => (Scopes)libraryMainLocation; set => libraryMainLocation = (int)value; }

        /// <summary>
        /// La type d'animation, idle, locomotion etc...
        /// </summary>
        public AnimaType AnimType { get => (AnimaType)librarySecLocation; set => librarySecLocation = (int)value; }



        #endregion
    }
}
