using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;


//TODO: Continuer d'implementer en fonction des besoins recurents des fenetre qui en dependent
namespace PulseEditor
{
    /// <summary>
    /// La classe de base pour tous les editeurs du Moteur.
    /// </summary>
    public class PulseEngine_Core_BaseEditor : EditorWindow
    {
        #region Propietes ##########################################################################

        /// <summary>
        /// La taille par defaut des fenetre de l'editeur.
        /// </summary>
        protected Vector2 DefaultWindowSize { get { return new Vector2(500, 900); } }

        /// <summary>
        /// Les panels crees accompagne de leur vector de position de scroll
        /// </summary>
        private Dictionary<int, Vector2> PanelsScrools = new Dictionary<int, Vector2>();

        #endregion

        #region GuiMethods ##########################################################################

        /// <summary>
        /// Faire un group d'items
        /// </summary>
        /// <param name="guiFunctions"></param>
        /// <param name="groupTitle"></param>
        protected void GroupGUI(Action guiFunctions, string groupTitle = "")
        {
            GUILayout.BeginVertical("HelpBox");
            GUILayout.Label(groupTitle, EditorStyles.boldLabel);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("GroupBox");
            if (guiFunctions != null)
                guiFunctions.Invoke();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// faire un panel scroolable verticalement.
        /// </summary>
        /// <param name="guiFunctions"></param>
        /// <param name="groupTitle"></param>
        protected void VerticalScrollablePanel(int panelID = -1, Action guiFunctions = null)
        {
            if (panelID < 0)
                return;
            Vector2 scroolPos = Vector2.zero;
            if (PanelsScrools.ContainsKey(panelID))
                scroolPos = PanelsScrools[panelID];
            else
                PanelsScrools.Add(panelID, scroolPos);
            GUILayout.BeginVertical();
            scroolPos = GUILayout.BeginScrollView(scroolPos);
            if (guiFunctions != null)
                guiFunctions.Invoke();
            GUILayout.EndScrollView();
            GUILayout.EndVertical();
            PanelsScrools[panelID] = scroolPos;
        }

        #endregion

        #region Methods #############################################################################

        #endregion
    }
}