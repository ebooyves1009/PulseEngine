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
        #region EditorEnums #########################################################################

        /// <summary>
        /// Les differents modes dans lesquels une fenetre d'editeur peut etre ouverte.
        /// </summary>
        protected enum EditorMode
        {
            Normal, Selector, ItemEdition, Preview, Node, Group
        }

        #endregion

        #region Editor Events & Arguments ######################################################################

        /// <summary>
        /// Les argeuments d'un evenement d'editeur.
        /// </summary>
        protected class EditorEventArgs: EventArgs
        {
            public int Scope;
            public int dataType;
            public int ID;
            public int Zone;
            public int Language;
        }

        /// <summary>
        /// L'evenement emit a la selection et validation d'un element en mode Selection.
        /// </summary>
        public EventHandler onSelectionEvent;

        #endregion


        #region Atrributs ##########################################################################

        /// <summary>
        /// Le mode dans lequel la fenetre a ete ouverte.
        /// </summary>
        protected EditorMode windowOpenMode;

        /// <summary>
        /// Le nombre de charactere maximal d'une liste.
        /// </summary>
        protected const int LIST_MAX_CHARACTERS = 10;

        #endregion

        #region Proprietes ##########################################################################

        /// <summary>
        /// La taille par defaut des fenetre de l'editeur.
        /// </summary>
        protected Vector2 DefaultWindowSize { get { return new Vector2(500, 900); } }

        /// <summary>
        /// Les panels crees accompagne de leur vector de position de scroll
        /// </summary>
        private Dictionary<int, Vector2> PanelsScrools = new Dictionary<int, Vector2>();

        /// <summary>
        /// Les Listes crees accompagne de leur vector de position de scroll
        /// </summary>
        private Dictionary<int, Vector2> ListsScrolls = new Dictionary<int, Vector2>();

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
        /// Panel, generalement en bas de fenetre conteneant le plus souvent les bouttons 'save' et 'cancel'
        /// </summary>
        /// <param name="actionButtons"></param>
        protected void SaveCancelPanel(params KeyValuePair<string,Action>[] actionButtons)
        {
            GroupGUI(() =>
            {
                GUILayout.BeginHorizontal();
                for (int i = 0; i < actionButtons.Length; i++)
                {
                    if (GUILayout.Button(actionButtons[i].Key)) { if (actionButtons[i].Value != null) actionButtons[i].Value.Invoke(); }
                }
                GUILayout.EndHorizontal();
            });
        }

        /// <summary>
        /// Faire une liste d'elements, et renvoyer l'element selectionne.
        /// </summary>
        /// <param name="listID"></param>
        /// <param name="content"></param>
        protected int ListItems(int listID = -1, int selected = -1, params GUIContent[] content)
        {
            if (listID < 0)
                return -1;
            Vector2 scroolPos = Vector2.zero;
            if (ListsScrolls.ContainsKey(listID))
                scroolPos = ListsScrolls[listID];
            else
                ListsScrolls.Add(listID, scroolPos);
            scroolPos = GUILayout.BeginScrollView(scroolPos);
            GUILayout.BeginVertical();
            int sel = GUILayout.SelectionGrid(selected, content, 1);
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            ListsScrolls[listID] = scroolPos;
            return sel;
        }

        /// <summary>
        /// Faire une Grille d'elements, et renvoyer l'element selectionne.
        /// </summary>
        /// <param name="listID"></param>
        /// <param name="content"></param>
        protected int GridItems(int listID = -1, int selected = -1, int xSize = 2, params GUIContent[] content)
        {
            if (listID < 0)
                return -1;
            Vector2 scroolPos = Vector2.zero;
            if (ListsScrolls.ContainsKey(listID))
                scroolPos = ListsScrolls[listID];
            else
                ListsScrolls.Add(listID, scroolPos);
            scroolPos = GUILayout.BeginScrollView(scroolPos);
            GUILayout.BeginVertical();
            int sel = GUILayout.SelectionGrid(selected, content, xSize);
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            ListsScrolls[listID] = scroolPos;
            return sel;
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
            scroolPos = GUILayout.BeginScrollView(scroolPos);
            GUILayout.BeginVertical();
            if (guiFunctions != null)
                guiFunctions.Invoke();
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            PanelsScrools[panelID] = scroolPos;
        }

        #endregion

        #region Methods #############################################################################

        /// <summary>
        /// Pour sauvegarder des changements effectues sur un asset clone.
        /// </summary>
        /// <param name="edited"></param>
        /// <param name="loaded"></param>
        protected void SaveAsset(UnityEngine.Object edited, UnityEngine.Object loaded)
        {
            EditorUtility.CopySerialized(edited, loaded);
            AssetDatabase.SaveAssets();
        }

        protected virtual void CloseWindow()
        {
            onSelectionEvent = delegate { };
            EditorUtility.UnloadUnusedAssetsImmediate();
        }

        #endregion
    }
}