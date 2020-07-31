using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Core;


namespace PulseEngine.Module.Localisator
{
    /// <summary>
    /// La Data de localisation contenu dans un asset de localisation, dans une langue precise.
    /// </summary>
    [System.Serializable]
    public class Localisationdata
    {
        #region Attributes ###############################################################

        private int trad_ID;
        private string title;
        private string header;
        private string banner;
        private string groupName;
        private string toolTip;
        private string description;
        private string details;
        private string infos;
        private string child1;
        private string child2;
        private string child3;
        private string child4;
        private string child5;
        private string child6;
        private string footPage;
        private string conclusion;
        private string end;

        #endregion

        #region Proprietes ##################################################################

        /// <summary>
        /// L'id de traduction.
        /// </summary>
        public int Trad_ID { get { return trad_ID; } set { trad_ID = value; }  }

        /// <summary>
        /// Le titre.
        /// </summary>
        public string Title { get { return title; } set { title = value; } }

        /// <summary>
        /// L'entete.
        /// </summary>
        public string Header { get { return header; } set { header = value; } }

        /// <summary>
        /// La banniere.
        /// </summary>
        public string Banner { get { return banner; } set { banner = value; } }

        /// <summary>
        /// Le nom de groupe.
        /// </summary>
        public string GroupName { get { return groupName; } set { groupName = value; } }

        /// <summary>
        /// Le texte au survol.
        /// </summary>
        public string ToolTip { get { return toolTip; } set { toolTip = value; } }

        /// <summary>
        /// La description.
        /// </summary>
        public string Description { get { return description; } set { description = value; } }

        /// <summary>
        /// Les details.
        /// </summary>
        public string Details { get { return details; } set { details = value; } }

        /// <summary>
        /// Les details avances.
        /// </summary>
        public string Infos { get { return infos; } set { infos = value; } }

        /// <summary>
        /// Le sous texte 1
        /// </summary>
        public string Child1 { get { return child1; } set { child1 = value; } }

        /// <summary>
        /// Le sous texte 2
        /// </summary>
        public string Child2 { get { return child2; } set { child2 = value; } }

        /// <summary>
        /// Le sous texte 3
        /// </summary>
        public string Child3 { get { return child3; } set { child3 = value; } }

        /// <summary>
        /// Le sous texte 4
        /// </summary>
        public string Child4 { get { return child4; } set { child4 = value; } }

        /// <summary>
        /// Le sous texte 5
        /// </summary>
        public string Child5 { get { return child5; } set { child5 = value; } }

        /// <summary>
        /// Le sous texte 6
        /// </summary>
        public string Child6 { get { return child6; } set { child6 = value; } }

        /// <summary>
        /// Le pied de page.
        /// </summary>
        public string FootPage { get { return footPage; } set { footPage = value; } }

        /// <summary>
        /// La conclusion.
        /// </summary>
        public string Conclusion { get { return conclusion; } set { conclusion = value; } }

        /// <summary>
        /// Le ending / les credits
        /// </summary>
        public string End { get { return end; } set { end = value; } }

        #endregion

        #region methodes ############################################################

        #endregion
    }
}