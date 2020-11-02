using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using PulseEngine.Datas;


namespace PulseEngine.Modules.MessageSystem
{

    /// <summary>
    /// The message system data library.
    /// </summary>
    public class MessageLibrary: CoreLibrary
    {
        #region Attributs #########################################################

        [SerializeField]
        private List<MessageData> dataList;
        [SerializeField]
        private Vector2 onEditorLocation;

        #endregion
        #region Proprietes ##########################################################

        /// <summary>
        /// La liste des messages.
        /// </summary>
        public override List<IData> DataList
        {
            get
            {
                if (dataList == null)
                {
                    dataList = new List<MessageData>();
                }
                return dataList.ConvertAll<IData>(new System.Converter<MessageData, IData>(item => { return item; }));
            }
            set
            {
                if (dataList == null)
                {
                    dataList = new List<MessageData>();
                }
                dataList = value.ConvertAll<MessageData>(new System.Converter<IData, MessageData>(item => { return (MessageData)item; })); ;
            }
        }

        /// <summary>
        /// Le scope du message
        /// </summary>
        public Scopes Scope { get => (Scopes)libraryMainLocation; set => libraryMainLocation = (int)value; }

        /// <summary>
        /// la zone du message.
        /// </summary>
        public DataLocation Zone { get => new DataLocation { id = librarySecLocation, globalLocation = libraryMainLocation} ; set => librarySecLocation = value.id; }

#if UNITY_EDITOR

        /// <summary>
        /// the node position on the grid in the editor.
        /// </summary>
        public Vector2 GridLocation { get => onEditorLocation; set => onEditorLocation = value; }

#endif

#endregion
    }
}
