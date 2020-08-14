using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Globals;

namespace PulseEngine.Modules.Localisator
{
    /// <summary>
    /// La Data de localisation contenu dans un asset de localisation, dans une langue precise.
    /// </summary>
    [System.Serializable]
    public class Localisationdata : IModuleData
    {
        #region Attributes ###############################################################

        [SerializeField]
        private int trad_ID;

        [SerializeField]
        private TradField title;

        [SerializeField]
        private TradField header;

        [SerializeField]
        private TradField banner;

        [SerializeField]
        private TradField groupName;

        [SerializeField]
        private TradField toolTip;

        [SerializeField]
        private TradField description;

        [SerializeField]
        private TradField details;

        [SerializeField]
        private TradField infos;

        [SerializeField]
        private TradField child1;

        [SerializeField]
        private TradField child2;

        [SerializeField]
        private TradField child3;

        [SerializeField]
        private TradField child4;

        [SerializeField]
        private TradField child5;

        [SerializeField]
        private TradField child6;

        [SerializeField]
        private TradField footPage;

        [SerializeField]
        private TradField conclusion;

        [SerializeField]
        private TradField end;

        #endregion

        #region Proprietes ##################################################################

        /// <summary>
        /// L'id de traduction.
        /// </summary>
        public int ID { get { return trad_ID; } set { trad_ID = value; } }

        /// <summary>
        /// Le titre.
        /// </summary>
        public TradField Title { get {return title;} set { title = value; } }

        /// <summary>
        /// L'entete.
        /// </summary>
        public TradField Header { get { return header; } set { header = value; } }

        /// <summary>
        /// La banniere.
        /// </summary>
        public TradField Banner { get { return banner; } set { banner = value; } }

        /// <summary>
        /// Le nom de groupe.
        /// </summary>
        public TradField GroupName { get { return groupName; } set { groupName = value; } }

        /// <summary>
        /// Le texte au survol.
        /// </summary>
        public TradField ToolTip { get { return toolTip; } set { toolTip = value; } }

        /// <summary>
        /// La description.
        /// </summary>
        public TradField Description { get { return description; } set { description = value; } }

        /// <summary>
        /// Les details.
        /// </summary>
        public TradField Details { get { return details; } set { details = value; } }

        /// <summary>
        /// Les details avances.
        /// </summary>
        public TradField Infos { get { return infos; } set { infos = value; } }

        /// <summary>
        /// Le sous texte 1
        /// </summary>
        public TradField Child1 { get { return child1; } set { child1 = value; } }

        /// <summary>
        /// Le sous texte 2
        /// </summary>
        public TradField Child2 { get { return child2; } set { child2 = value; } }

        /// <summary>
        /// Le sous texte 3
        /// </summary>
        public TradField Child3 { get { return child3; } set { child3 = value; } }

        /// <summary>
        /// Le sous texte 4
        /// </summary>
        public TradField Child4 { get { return child4; } set { child4 = value; } }

        /// <summary>
        /// Le sous texte 5
        /// </summary>
        public TradField Child5 { get { return child5; } set { child5 = value; } }

        /// <summary>
        /// Le sous texte 6
        /// </summary>
        public TradField Child6 { get { return child6; } set { child6 = value; } }

        /// <summary>
        /// Le pied de page.
        /// </summary>
        public TradField FootPage { get { return footPage; } set { footPage = value; } }

        /// <summary>
        /// La conclusion.
        /// </summary>
        public TradField Conclusion { get { return conclusion; } set { conclusion = value; } }

        /// <summary>
        /// Le ending / les credits
        /// </summary>
        public TradField End { get { return end; } set { end = value; } }

        #endregion

        #region methodes ############################################################

        #endregion
    }
}