using System.Collections.Generic;
using UnityEngine;

namespace PulseEngine.Modules.Commander
{

    /// <summary>
    /// The Commands library asset.
    /// </summary>
    public class CommandsLibrary: CoreLibrary
    {
        #region Attributs #########################################################

        [SerializeField]
        private List<CommandSequence> dataList;

        #endregion
        #region Proprietes ##########################################################

        /// <summary>
        /// The library datas list.
        /// </summary>
        public override List<IData> DataList
        {
            get
            {
                if (dataList == null)
                {
                    dataList = new List<CommandSequence>();
                }
                return dataList.ConvertAll<IData>(new System.Converter<CommandSequence, IData>(item => { return item; }));
            }
            set
            {
                if (dataList == null)
                {
                    dataList = new List<CommandSequence>();
                }
                dataList = value.ConvertAll<CommandSequence>(new System.Converter<IData, CommandSequence>(item => { return (CommandSequence)item; })); ;
            }
        }

        /// <summary>
        /// The library's datalist zone
        /// </summary>
        public Zones Zone { get => (Zones)librarySecLocation; set => librarySecLocation = (int)value; }

        /// <summary>
        /// The scope where the library was created.
        /// </summary>
        public Scopes Scope { get => (Scopes)libraryMainLocation; set => libraryMainLocation = (int)value; }

        #endregion
    }
}
