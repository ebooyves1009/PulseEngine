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
        protected const int LIST_MAX_CHARACTERS = 20;

        /// <summary>
        /// Le nombre de charactere maximal d'une d'un champ texte.
        /// </summary>
        protected const int FIELD_MAX_CHARACTERS = 50;

        /// <summary>
        /// le nombre de liste a chaque refresh.
        /// </summary>
        private int listViewCount = 0;

        /// <summary>
        /// le nombre de panel scrollables a chaque refresh.
        /// </summary>
        private int scrollPanCount = 0;

        /// <summary>
        /// empeche de set les styles plusieur fois.
        /// </summary>
        private bool multipleStyleSetLock;

        #endregion

        #region Styles ##########################################################################

        /// <summary>
        /// le style des items d'une liste.
        /// </summary>
        protected GUIStyle style_listItem;

        /// <summary>
        /// le style des editeurs multi ligne.
        /// </summary>
        protected GUIStyle style_txtArea;

        /// <summary>
        /// le style des groupes.
        /// </summary>
        protected GUIStyle style_group;

        /// <summary>
        /// le style des labels.
        /// </summary>
        protected GUIStyle style_label;

        /// <summary>
        /// le style des selecteur d'objet.
        /// </summary>
        protected GUILayoutOption[] style_objSelect;

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

        private void OnEnable()
        {
            minSize = new Vector2(600, 600);
            Focus();
            OnInitialize();
        }

        private void OnGUI()
        {
            StyleSetter();
            listViewCount = 0;
            scrollPanCount = 0;
            OnRedraw();
        }

        /// <summary>
        /// Appellee au demarrage de la fenetre, a utiliser a la place de OnEnable dans les fenetres heritantes
        /// </summary>
        protected virtual void OnInitialize()
        {

        }

        /// <summary>
        /// Appellee a chaque rafraichissement de la fenetre, a utiliser a la place de onGUI dans les fenetres heritantes
        /// </summary>
        protected virtual void OnRedraw()
        {

        }

        /// <summary>
        /// Pour initialiser les styles.
        /// </summary>
        private void StyleSetter()
        {
            if (multipleStyleSetLock)
                return;
            //Text Area
            style_txtArea = new GUIStyle(GUI.skin.textArea);
            //list field
            style_listItem = new GUIStyle(GUI.skin.textField);
            style_listItem.onNormal.textColor = Color.blue;
            style_listItem.onHover.textColor = Color.blue;
            style_listItem.onActive.textColor = Color.blue;
            style_listItem.hover.textColor = Color.black;
            style_listItem.normal.textColor = Color.gray;
            style_listItem.clipping = TextClipping.Clip;
            //groupes
            style_group = new GUIStyle(GUI.skin.window);
            style_group.stretchWidth = false;
            style_group.fontStyle = FontStyle.Bold;
            style_group.margin = new RectOffset(8, 8, 5, 8);
            //labels
            style_label = new GUIStyle(GUI.skin.label);
            style_label.stretchWidth = false;
            style_label.fixedWidth = 120;
            style_label.fontStyle = FontStyle.Bold;
            //obj select
            style_objSelect = new[] { GUILayout.Width(150), GUILayout.Height(150) };

            multipleStyleSetLock = true;
        }

        /// <summary>
        /// Un champs texte a caracteres limites.
        /// </summary>
        /// <param name="input"></param>
        /// <param name="style"></param>
        /// <returns></returns>
        protected string LimitText(string input)
        {
            string str = EditorGUILayout.TextArea(input, style_txtArea);
            char[] cutted = new char[FIELD_MAX_CHARACTERS];
            for (int i = 0; i < FIELD_MAX_CHARACTERS; i++)
                if(str.Length > i)
                    cutted[i] = str[i];
            string ret = new string(cutted);
            return ret;
        }

        /// <summary>
        /// Faire un group d'items
        /// </summary>
        /// <param name="guiFunctions"></param>
        /// <param name="groupTitle"></param>
        protected void GroupGUI(Action guiFunctions, string groupTitle = "", int height = 0)
        {
            GUILayout.BeginVertical(groupTitle, style_group, height > 0 ? new[] { GUILayout.Height(height) } : null);
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical("GroupBox");
            if (guiFunctions != null)
                guiFunctions.Invoke();
            GUILayout.EndVertical();
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Faire un group d'items sans le style d'interieur
        /// </summary>
        /// <param name="guiFunctions"></param>
        /// <param name="groupTitle"></param>
        protected void GroupGUInoStyle(Action guiFunctions, string groupTitle = "", int height = 0)
        {
            GUILayout.BeginVertical(groupTitle, style_group, height > 0? new[] { GUILayout.Height(height)}: null);
            if (guiFunctions != null)
                guiFunctions.Invoke();
            GUILayout.EndVertical();
        }

        /// <summary>
        /// Panel, generalement en bas de fenetre conteneant le plus souvent les bouttons 'save' et 'cancel'
        /// </summary>
        /// <param name="actionButtons"></param>
        protected void SaveCancelPanel(params KeyValuePair<string,Action>[] actionButtons)
        {
            GroupGUInoStyle(() =>
            {
                GUILayout.BeginHorizontal();
                for (int i = 0; i < actionButtons.Length; i++)
                {
                    if (GUILayout.Button(actionButtons[i].Key)) { if (actionButtons[i].Value != null) actionButtons[i].Value.Invoke(); }
                }
                GUILayout.EndHorizontal();
            },"",50);
        }

        /// <summary>
        /// Faire une liste d'elements, et renvoyer l'element selectionne.
        /// </summary>
        /// <param name="listID"></param>
        /// <param name="content"></param>
        protected int ListItems(int selected = -1, params GUIContent[] content)
        {
            listViewCount++;
            Vector2 scroolPos = Vector2.zero;
            if (ListsScrolls.ContainsKey(listViewCount))
                scroolPos = ListsScrolls[listViewCount];
            else
                ListsScrolls.Add(listViewCount, scroolPos);
            scroolPos = GUILayout.BeginScrollView(scroolPos);
            GUILayout.BeginVertical();
            int sel = GUILayout.SelectionGrid(selected, content, 1, style_listItem);
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            ListsScrolls[listViewCount] = scroolPos;
            return sel;
        }

        /// <summary>
        /// Faire une Grille d'elements, et renvoyer l'element selectionne.
        /// </summary>
        /// <param name="listID"></param>
        /// <param name="content"></param>
        protected int GridItems(int selected = -1, int xSize = 2, params GUIContent[] content)
        {
            listViewCount++;
            Vector2 scroolPos = Vector2.zero;
            if (ListsScrolls.ContainsKey(listViewCount))
                scroolPos = ListsScrolls[listViewCount];
            else
                ListsScrolls.Add(listViewCount, scroolPos);
            scroolPos = GUILayout.BeginScrollView(scroolPos);
            GUILayout.BeginVertical();
            int sel = GUILayout.SelectionGrid(selected, content, xSize);
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            ListsScrolls[listViewCount] = scroolPos;
            return sel;
        }

        /// <summary>
        /// faire un panel scroolable verticalement.
        /// </summary>
        /// <param name="guiFunctions"></param>
        /// <param name="groupTitle"></param>
        protected void VerticalScrollablePanel(Action guiFunctions = null)
        {
            scrollPanCount++;
            Vector2 scroolPos = Vector2.zero;
            if (PanelsScrools.ContainsKey(scrollPanCount))
                scroolPos = PanelsScrools[scrollPanCount];
            else
                PanelsScrools.Add(scrollPanCount, scroolPos);
            scroolPos = GUILayout.BeginScrollView(scroolPos);
            GUILayout.BeginVertical();
            if (guiFunctions != null)
                guiFunctions.Invoke();
            GUILayout.EndVertical();
            GUILayout.EndScrollView();
            PanelsScrools[scrollPanCount] = scroolPos;
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