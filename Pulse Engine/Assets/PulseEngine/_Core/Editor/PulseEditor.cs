using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System;
using UnityEngine.Rendering;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using System.IO;


//TODO: Continuer d'implementer en fonction des besoins recurents des fenetre qui en dependent


#region Enums #################################################################################

/// <summary>
/// Les differents modes dans lesquels une fenetre d'editeur peut etre ouverte.
/// </summary>
public enum EditorMode
{
    Edition, Selection, DataEdition, Preview, NodeGraph
}

#endregion

#region Delegates #################################################################################

#endregion

#region Class #################################################################################

/// <summary>
/// La classe de base pour tous les editeurs du Moteur.
/// </summary>

[InitializeOnLoad]
public abstract class PulseEditor<T> : EditorWindow where T : BaseSerializedData
{
    #region Constants #################################################################

    /// <summary>
    /// Le nombre de charactere maximal d'une liste.
    /// </summary>
    protected const int LIST_MAX_CHARACTERS = 20;

    /// <summary>
    /// Le nombre de charactere maximal d'une d'un champ texte.
    /// </summary>
    protected const int FIELD_MAX_CHARACTERS = 50;

    /// <summary>
    /// Le menu dans lequel seront crees les menus des editeurs.
    /// </summary>
    public const string Menu_EDITOR_MENU = "PulseEngine/";

    /// <summary>
    /// Le Repertoire de base ou chercher les assets
    /// </summary>
    public const string GAME_RES_PATH = "Assets/GameResources/";

    #endregion

    #region Protected Atrributes ##########################################################################

    /// <summary>
    /// Le mode dans lequel la fenetre a ete ouverte.
    /// </summary>
    protected EditorMode currentEditorMode;

    /// <summary>
    /// l'asset original.
    /// </summary>
    protected T originalAsset;

    /// <summary>
    /// La liste des datas en cours de modification.
    /// </summary>
    protected List<T> dataList = new List<T>();

    /// <summary>
    /// La data en cours de modification.
    /// </summary>
    protected T data;

    /// <summary>
    /// l'index de l'asset dans all assets.
    /// </summary>
    protected int selectAssetIndex;

    /// <summary>
    /// l'index de la data dans la liste des datas.
    /// </summary>
    protected int selectDataIndex = -1;

    /// <summary>
    /// L'id de la data a modifier en mode modification
    /// </summary>
    protected int dataID;

    /// <summary>
    /// Le parametre pour retrouver un asset, souvent le scope auquel il appartient
    /// </summary>
    protected int assetMainFilter;

    /// <summary>
    /// Le parametre pour retrouver un asset, souvent la zone a laquelle il appartient.
    /// </summary>
    protected int assetLocalFilter;

    /// <summary>
    /// The copied object on the clipboard.
    /// </summary>
    protected dynamic clipBoard;

    #endregion

    #region Private Atrributes ##########################################################################

    /// <summary>
    /// La liste des pths des datas a supprimer a la sauvegarde.
    /// </summary>
    protected List<string> delAssetPath = new List<string>();

    /// <summary>
    /// le nombre de liste a chaque refresh.
    /// </summary>
    private int listViewCount = 0;

    /// <summary>
    /// le nombre de panel scrollables a chaque refresh.
    /// </summary>
    private int scrollPanCount = 0;

    /// <summary>
    /// le nombre de panel Preview animation a chaque refresh.
    /// </summary>
    private int animPreviewCount = 0;

    /// <summary>
    /// empeche de set les styles plusieur fois.
    /// </summary>
    private bool multipleStyleSetLock;

    /// <summary>
    /// Les panels crees accompagne de leur vector de position de scroll
    /// </summary>
    private Dictionary<int, Vector2> PanelsScrools = new Dictionary<int, Vector2>();

    /// <summary>
    /// Les Listes crees accompagne de leur vector de position de scroll
    /// </summary>
    private Dictionary<int, Vector2> ListsScrolls = new Dictionary<int, Vector2>();

    /// <summary>
    /// Les previews crees accompagne de leur objects de preview
    /// </summary>
    private Dictionary<int, AnimationPreviewer> AnimsPreviews = new Dictionary<int, AnimationPreviewer>();

    #endregion

    #region Proprietes ##########################################################################

    /// <summary>
    /// L'emplacement generique des assets
    /// </summary>
    protected abstract string Save_Path { get; }

    /// <summary>
    /// Le nom generique des assets
    /// </summary>
    protected abstract string SaveFileName { get; }

    /// <summary>
    /// La taille par defaut des fenetre de l'editeur.
    /// </summary>
    protected Vector2 DefaultWindowSize { get { return new Vector2(500, 900); } }

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
    /// le style des grilles de nodes.
    /// </summary>
    protected GUIStyle style_grid;

    /// <summary>
    /// le style des Nodes.
    /// </summary>
    protected GUIStyle style_node;

    /// <summary>
    /// le style des Nodes speciaux.
    /// </summary>
    protected GUIStyle style_nodeSpecials;

    /// <summary>
    /// le style des labels.
    /// </summary>
    protected GUIStyle style_label;

    /// <summary>
    /// le style des selecteur d'objet.
    /// </summary>
    protected GUILayoutOption[] style_objSelect;

    #endregion

    #region Events ######################################################################

    /// <summary>
    /// A different saving manner, for modules that require it.
    /// </summary>
    protected Action customSave;

    /// <summary>
    /// Action on selection, for modules that require custom selection method.
    /// </summary>
    protected Action SelectAction;

    #endregion

    #region Signals ##########################################################################


    /// <summary>
    /// Appellee au demarrage de la fenetre, a utiliser a la place de OnEnable dans les fenetres heritantes
    /// </summary>
    protected virtual void OnInitialize()
    {

    }

    /// <summary>
    /// Appellee lorsqu'on ferme la fenetre.
    /// </summary>
    protected virtual void OnQuit()
    {

    }

    /// <summary>
    /// Appellee a chaque rafraichissement de la fenetre, a utiliser a la place de onGUI dans les fenetres heritantes
    /// </summary>
    protected virtual void OnRedraw()
    {
        GUILayout.BeginVertical();
        OnHeaderRedraw();
        GUILayout.BeginHorizontal();
        //left panel
        try
        {
            if (currentEditorMode != EditorMode.DataEdition && dataList != null)
            {
                string[] names = new string[dataList.Count];
                if (dataList.Count > 0)
                {
                    //TODO: Manage Traduction of displayed names
                    for (int i = 0, len = dataList.Count; i < len; i++)
                    {
                        names[i] = dataList[i].Id + " -> " + dataList[i].Name;
                    }
                }
                //filtering here
                //TODO: function to filter the list here
                //sorting here
                //TODO: function to sort the list here
                //displaying
                ScrollablePanel(() =>
                {
                    //search and filter
                    //listing
                    GroupGUI(() =>
                    {
                        MakeList(selectDataIndex, names, index => selectDataIndex = index, dataList.ToArray());
                        //if (selectDataIndex >= 0 && selectDataIndex < dataList.Count)
                        //    data = dataList[selectDataIndex];
                        OnListButtons();
                    }, "Items List");
                    //foot panel
                    OnFootRedraw();
                    //save panel
                    SaveBarPanel();

                }, true);
            }
        }
        catch (Exception e)
        {
            PulseDebug.LogError("Exeption thrown " + e.Message);
            CloseWindow();
        }
        ScrollablePanel(() =>
        {
            //right panel
            OnBodyRedraw();
        });
        GUILayout.EndHorizontal();
        GUILayout.BeginHorizontal();
        ////save panel
        //SaveBarPanel();
        ////foot panel
        //OnFootRedraw();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Appellee a chaque rafraichissement de la fenetre, a l'entete.
    /// </summary>
    protected virtual void OnHeaderRedraw()
    {

    }

    /// <summary>
    /// Appellee a chaque rafraichissement de la fenetre, au corps de page.
    /// </summary>
    protected virtual void OnBodyRedraw()
    {

    }

    /// <summary>
    /// Appellee a chaque rafraichissement de la fenetre, au pied de page.
    /// </summary>
    protected virtual void OnFootRedraw()
    {

    }

    /// <summary>
    /// Appellee a chaque rafraichissement de la fenetre, pour afficher les bouttons de pied de liste.
    /// </summary>
    protected virtual void OnListButtons()
    {
        StandartListButtons();
    }

    /// <summary>
    /// Au changement d'une selection dans une Entete.
    /// </summary>
    protected virtual void OnHeaderChange()
    {

    }

    /// <summary>
    /// Au changement d'une selection dans une liste ou grille.
    /// </summary>
    protected virtual void OnListChange()
    {

    }

    #endregion

    #region GuiMethods ##########################################################################

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
        //Nodes
        style_node = new GUIStyle("Button");
        style_node.focused.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        style_node.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node0.png") as Texture2D;
        style_node.normal.textColor = Color.white;
        style_node.hover.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node2.png") as Texture2D;
        style_node.hover.textColor = Color.white;
        style_node.alignment = TextAnchor.MiddleCenter;
        style_node.fontStyle = FontStyle.Bold;
        {
            int border = 15;
            style_node.border = new RectOffset(border, border, border, border);
        }
        //Nodes special
        style_nodeSpecials = new GUIStyle("Button");
        style_nodeSpecials.normal.background = EditorGUIUtility.Load("builtin skins/darkskin/images/node1.png") as Texture2D;
        style_nodeSpecials.normal.textColor = Color.white;
        style_nodeSpecials.alignment = TextAnchor.MiddleCenter;
        {
            int border = 15;
            style_nodeSpecials.border = new RectOffset(border, border, border, border);
        }
        //Grid
        style_grid = new GUIStyle(GUI.skin.window);
        Vector2Int scale = new Vector2Int(1024, 1024 * (16 / 9));
        var gridTexture = new Texture2D(scale.x, scale.y);
        float caseRatio = 0.95f;
        float linesTickness = (1 - caseRatio) / 2;
        float aspectratio = gridTexture.width / gridTexture.height;
        int caseNumX = 64;
        int caseNumY = Mathf.RoundToInt(caseNumX * aspectratio);
        int caseSizeX = gridTexture.width / caseNumX;
        int caseSizeY = gridTexture.height / caseNumY;
        for (int i = 0; i < gridTexture.width; i++)
        {
            float progressionX = i % caseSizeX;
            float xpercent = (progressionX / caseSizeX);
            bool inMiddleX = xpercent > linesTickness && xpercent <= (caseRatio + linesTickness);
            for (int j = 0; j < gridTexture.height; j++)
            {
                float progressionY = j % caseSizeY;
                float ypercent = (progressionY / caseSizeY);
                bool inMiddleY = ypercent > linesTickness && ypercent <= (caseRatio + linesTickness);
                Color c = inMiddleX && inMiddleY ? new Color(0, 0, 0, 0.15f) : new Color(1, 1, 1, 0.5f);
                gridTexture.SetPixel(i, j, c);
            }
        }
        gridTexture.Apply();
        style_grid.normal.background = gridTexture;

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
            if (str.Length > i)
                cutted[i] = str[i];
        string ret = new string(cutted);
        return ret;
    }

    /// <summary>
    /// Faire un group d'items
    /// </summary>
    /// <param name="guiFunctions"></param>
    /// <param name="groupTitle"></param>
    protected void GroupGUI(Action guiFunctions, int widht)
    {
        GUILayout.BeginVertical("", style_group, new[] { GUILayout.Width(widht) });
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
    protected void GroupGUInoStyle(Action guiFunctions, int width)
    {
        GUILayout.BeginVertical("", style_group, new[] { GUILayout.Width(width) });
        if (guiFunctions != null)
            guiFunctions.Invoke();
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Faire un group d'items
    /// </summary>
    /// <param name="guiFunctions"></param>
    /// <param name="groupTitle"></param>
    protected void GroupGUI(Action guiFunctions, string groupTitle, Vector2 size)
    {
        List<GUILayoutOption> options = new List<GUILayoutOption>();
        if (size.y > 0)
            options.Add(GUILayout.Height(size.y));
        if (size.x > 0)
            options.Add(GUILayout.Width(size.x));
        GUILayout.BeginVertical(groupTitle, style_group, options.ToArray());
        GUILayout.BeginHorizontal();
        GUILayout.BeginVertical("GroupBox");
        if (guiFunctions != null)
            guiFunctions.Invoke();
        GUILayout.EndVertical();
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Faire un group d'items
    /// </summary>
    /// <param name="guiFunctions"></param>
    /// <param name="groupTitle"></param>
    protected void GroupGUI(Action guiFunctions, string groupTitle = "", int height = 0)
    {
        List<GUILayoutOption> options = new List<GUILayoutOption>();
        if (height > 0)
            options.Add(GUILayout.Height(height));
        GUILayout.BeginVertical(groupTitle, style_group, options.ToArray());
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
        List<GUILayoutOption> options = new List<GUILayoutOption>();
        if (height > 0)
            options.Add(GUILayout.Height(height));
        GUILayout.BeginVertical(groupTitle, style_group, options.ToArray());
        if (guiFunctions != null)
            guiFunctions.Invoke();
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Panel, generalement en bas de fenetre conteneant le plus souvent les bouttons 'save' et 'cancel'
    /// </summary>
    /// <param name="actionButtons"></param>
    protected void SaveCancelPanel(params KeyValuePair<string, Action>[] actionButtons)
    {
        GroupGUInoStyle(() =>
        {
            GUILayout.BeginHorizontal();
            for (int i = 0; i < actionButtons.Length; i++)
            {
                if (GUILayout.Button(actionButtons[i].Key)) { if (actionButtons[i].Value != null) actionButtons[i].Value.Invoke(); }
            }
            GUILayout.EndHorizontal();
        }, "", 50);
    }

    /// <summary>
    /// Un panel classique de sauvegarde.
    /// </summary>
    /// <param name="_toSave"></param>
    /// <param name="_whereSave"></param>
    protected void SaveBarPanel()
    {
        switch (currentEditorMode)
        {
            case EditorMode.Edition:
                SaveCancelPanel(new[] {
                        new KeyValuePair<string, Action>("Save", ()=> {
                            if (customSave != null)
                                customSave.Invoke();
                            else
                                SaveAsset();
                        }),
                        new KeyValuePair<string, Action>("Close", ()=> {
                            if (EditorUtility.DisplayDialog("Warning", "The Changes you made won't be saved.\n Proceed?","Yes","No"))
                                Close();
                        })
                    });
                break;
            case EditorMode.Selection:
                SaveCancelPanel(new[] {
                        new KeyValuePair<string, Action>("Select", ()=> {
                            if (customSave != null)
                                customSave.Invoke();
                            else
                                SaveAsset();
                            if (SelectAction != null)
                                SelectAction.Invoke();
                            else
                            {
                                //if(onSelectionEvent != null)
                                //    onSelectionEvent.Invoke(data, new EditorEventArgs{ dataObjectLocation = ((IData)data).Location });
                            }
                            Close();
                        }),
                        new KeyValuePair<string, Action>("Cancel", ()=> { if(EditorUtility.DisplayDialog("Warning", "The Selection you made won't be saved.\n Proceed?","Yes","No")) Close();})
                    });
                break;
            case EditorMode.DataEdition:
                SaveCancelPanel(new[] {
                        new KeyValuePair<string, Action>("Save", ()=> {
                            if (customSave != null)
                                customSave.Invoke();
                            else
                                SaveAsset();
                            Close();
                        }),
                        new KeyValuePair<string, Action>("Close", ()=> { if(EditorUtility.DisplayDialog("Warning", "The Changes you made won't be saved.\n Proceed?","Yes","No")) Close();})
                    });
                break;
            case EditorMode.Preview:
                break;
            case EditorMode.NodeGraph:
                break;
        }
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
        if (ListsScrolls == null)
            ListsScrolls = new Dictionary<int, Vector2>();
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
    /// Faire une liste d'elements, et renvoyer l'element selectionne en envoyant un signal a la modification.
    /// </summary>
    /// <param name="_index"></param>
    /// <param name="_collection"></param>
    /// <returns></returns>
    protected int MakeList(int _index, string[] _collection, Action<int> beforeChange = null, T[] dataCollection = null)
    {
        List<GUIContent> listContent = new List<GUIContent>();
        for (int i = 0; i < _collection.Length; i++)
        {
            var name = _collection[i];
            char[] titleChars = new char[LIST_MAX_CHARACTERS];
            string pointDeSuspension = string.Empty;
            if (!string.IsNullOrEmpty(name))
            {
                for (int j = 0; j < titleChars.Length; j++)
                    if (j < name.Length)
                        titleChars[j] = name[j];
            }
            if (name.Length >= titleChars.Length)
                pointDeSuspension = "...";
            string title = string.IsNullOrEmpty(name) ? "<<<< None >>>>" : new string(titleChars) + pointDeSuspension;
            listContent.Add(new GUIContent { text = (i + 1) + " | " + title });
        }
        int tmp = ListItems(_index, listContent.ToArray());
        if (beforeChange != null)
            beforeChange.Invoke(tmp);
        if (tmp != _index)
            ListChange();
        if (dataCollection != null && dataCollection.Length > 0)
        {
            if (tmp >= 0 && tmp < dataCollection.Length)
                data = dataCollection[tmp];
            else
                data = null;
        }
        return tmp;
    }

    /// <summary>
    /// Faire une grille de 4 collones d'elements, et renvoyer l'element selectionne en envoyant un signal a la modification.
    /// </summary>
    /// <param name="_index"></param>
    /// <param name="_collection"></param>
    /// <returns></returns>
    protected int MakeGrid(int _index, string[] _collection, params T[] dataCollection)
    {
        List<GUIContent> listContent = new List<GUIContent>();
        for (int i = 0; i < _collection.Length; i++)
        {
            var name = _collection[i];
            char[] titleChars = new char[LIST_MAX_CHARACTERS];
            string pointDeSuspension = string.Empty;
            if (!string.IsNullOrEmpty(name))
            {
                for (int j = 0; j < titleChars.Length; j++)
                    if (j < name.Length)
                        titleChars[j] = name[j];
            }
            if (name.Length >= titleChars.Length)
                pointDeSuspension = "...";
            string title = string.IsNullOrEmpty(name) ? "<<<< None >>>>" : new string(titleChars) + pointDeSuspension;
            listContent.Add(new GUIContent { text = i + "-" + title });
        }
        int tmp = GridItems(_index, 4, listContent.ToArray());
        if (tmp != _index)
            ListChange();
        if (dataCollection != null && dataCollection.Length > 0)
        {
            if (tmp >= 0 && tmp < dataCollection.Length)
                data = dataCollection[tmp];
            else
                data = null;
        }
        return tmp;
    }

    /// <summary>
    /// Faire une entete d'elements, et renvoyer l'element selectionne en envoyant un signal a la modification.
    /// </summary>
    /// <param name="_index"></param>
    /// <param name="_collection"></param>
    /// <returns></returns>
    protected int MakeHeader(int _index, string[] _collection, Action<int> beforeEmitSignal = null)
    {
        int selId = GUILayout.Toolbar(_index, _collection);
        if (beforeEmitSignal != null)
            beforeEmitSignal.Invoke(selId);
        if (selId != _index || data == null)
            HeaderChange();
        return selId;
    }

    /// <summary>
    /// faire un panel scroolable verticalement.
    /// </summary>
    /// <param name="guiFunctions"></param>
    /// <param name="groupTitle"></param>
    protected void ScrollablePanel(Action guiFunctions = null, bool fixedSize = false)
    {
        scrollPanCount++;
        Vector2 scroolPos = Vector2.zero;
        if (PanelsScrools == null)
            PanelsScrools = new Dictionary<int, Vector2>();
        if (PanelsScrools.ContainsKey(scrollPanCount))
            scroolPos = PanelsScrools[scrollPanCount];
        else
            PanelsScrools.Add(scrollPanCount, scroolPos);
        var options = new[] { /*GUILayout.MinWidth(100),*/ GUILayout.Width(minSize.x / 2) };
        scroolPos = GUILayout.BeginScrollView(scroolPos, fixedSize ? options : null);
        GUILayout.BeginVertical();
        if (guiFunctions != null)
            guiFunctions.Invoke();
        GUILayout.EndVertical();
        GUILayout.EndScrollView();
        PanelsScrools[scrollPanCount] = scroolPos;
    }

    /// <summary>
    /// The standart Add, Remove, Copy/Paste buttons on list of Datas.
    /// </summary>
    protected void StandartListButtons()
    {
        GUILayout.BeginVertical();
        GUILayout.BeginHorizontal();
        if (GUILayout.Button("Add"))
        {
            int maxId = dataList != null ? dataList.Count : 0;
            var item = ScriptableObject.CreateInstance<T>();
            if (item != null)
            {
                WriteField<BaseSerializedData>("_id", item, maxId);
                string path = Path.Combine(GAME_RES_PATH + Save_Path, SaveFileName + "_" + maxId + ".asset");
                if (delAssetPath.Contains(path))
                {
                    delAssetPath.Remove(path);
                }
                dataList.Add(item);
            }
        }
        if (dataList == null || dataList.Count <= 0)
        {
            GUILayout.EndHorizontal();
            GUILayout.EndVertical();
            return;
        }
        if (data != null)
        {
            if (GUILayout.Button("Remove"))
            {
                int index = dataList.FindIndex(item => { return ReferenceEquals(item, data); });
                if (index >= 0)
                {
                    string path = AssetDatabase.GetAssetPath(dataList[index]);
                    if (!string.IsNullOrEmpty(path))
                    {
                        delAssetPath.Add(path);
                    }
                    dataList.RemoveAt(index);
                }
                ListChange();
            }
            //if (clipBoard != null)
            //{
            //    if (GUILayout.Button("Paste"))
            //    {
            //        int maxID = 0;
            //        for (int i = 0; i < dataList.Count; i++)
            //        {
            //            var iiTem = dataList[i] as IData;
            //            if (iiTem == null)
            //                continue;
            //            if (iiTem.Location.id > maxID)
            //                maxID = iiTem.Location.id;
            //        }
            //        var iClipBoard = clipBoard as IData;
            //        if (iClipBoard != null)
            //        {
            //            DataLocation loc = iClipBoard.Location;
            //            loc.id = maxID + 1;
            //            iClipBoard.Location = loc;
            //            int nextID = selectDataIndex + 1;
            //            if (nextID >= 0 && nextID < dataList.Count)
            //            {
            //                dataList.Insert(nextID, iClipBoard);
            //            }
            //            else
            //                dataList.Add(iClipBoard);
            //            ListChange();
            //        }
            //    }
            //    if (GUILayout.Button("X"))
            //    {
            //        clipBoard = null;
            //    }
            //}
            //else
            //{
            //    if (GUILayout.Button("Copy"))
            //    {
            //        clipBoard = Core.ObjectClone(data);
            //    }
            //}
        }
        GUILayout.EndHorizontal();
        GUILayout.EndVertical();
    }

    /// <summary>
    /// Previsualize une animation
    /// </summary>
    protected void PreviewAnAnimation(Motion _motion, float aspectRatio = 1.77f, GameObject _target = null, params (GameObject go, HumanBodyBones bone, Vector3 offset, Vector3 rotation)[] accessories)
    {
        animPreviewCount++;
        AnimationPreviewer preview = null;
        if (AnimsPreviews == null)
            AnimsPreviews = new Dictionary<int, AnimationPreviewer>();
        if (AnimsPreviews.ContainsKey(animPreviewCount))
            preview = AnimsPreviews[animPreviewCount];
        else
            AnimsPreviews.Add(animPreviewCount, new AnimationPreviewer());
        if (!preview && AnimsPreviews.ContainsKey(animPreviewCount))
            preview = AnimsPreviews[animPreviewCount];
        if (!preview)
            return;
        preview.Previsualize(_motion, aspectRatio, _target, accessories);
        AnimsPreviews[animPreviewCount] = preview;
    }

    /// <summary>
    /// Reset the previews
    /// </summary>
    protected void ResetPreviews(bool destroy = false)
    {
        if (AnimsPreviews == null)
            return;
        //for (int len = AnimsPreviews.Count, i = len; i >= 0; i--)
        //{
        //    var preview = AnimsPreviews.ElementAt(i);
        //    var key = preview.Key;
        //    AnimsPreviews[key].Destroy();
        //}
        //if (destroy)
        AnimsPreviews.Clear();
    }

    #endregion

    #region Mono #######################################################################

    private void Update()
    {
        Repaint();
    }

    private void OnEnable()
    {
        minSize = new Vector2(600, 600);
        Focus();
        var resPath = GAME_RES_PATH + Save_Path;
        CreatePath(resPath);
        OnInitialize();
    }

    private void OnGUI()
    {
        StyleSetter();
        listViewCount = 0;
        scrollPanCount = 0;
        animPreviewCount = 0;
        OnRedraw();
    }

    private void OnDisable()
    {
        clipBoard = null;
        ResetPreviews(true);
        //if (currentEditorMode == EditorMode.DataEdition)
        //{
        //    SaveAsset(asset, originalAsset);
        //}
        //RefreshCache(editorDataType);
        OnQuit();
        //OnCacheRefresh = delegate { };
        //CloseWindow();
    }

    #endregion

    #region Methods #############################################################################

    /// <summary>
    /// Lis dans un champs prive de la data ayant l'attribut [serializeField]
    /// </summary>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    protected object ReadField<Q>(string fieldName, object target)
    {
        if (target == null)
            return default;
        if (string.IsNullOrEmpty(fieldName))
            return default;
        var fields = typeof(Q).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (fields == null)
            return default;
        if (fields.GetCustomAttribute<SerializeField>() == null)
            return default;
        return fields.GetValue(target);
    }

    /// <summary>
    /// Ecrit dans un champs prive de la data ayant l'attribut [serializeField]
    /// </summary>
    /// <param name="fieldName"></param>
    /// <returns></returns>
    protected bool WriteField<Q>(string fieldName, object target, object value)
    {
        if (target == null)
            return false;
        if (string.IsNullOrEmpty(fieldName))
            return false;
        var fields = typeof(Q).GetField(fieldName, BindingFlags.NonPublic | BindingFlags.Instance);
        if (fields == null)
            return false;
        if (fields.GetCustomAttribute<SerializeField>() == null)
            return false;
        fields.SetValue(target, value);
        return true;
    }

    /// <summary>
    /// Create a path
    /// </summary>
    /// <param name="path"></param>
    protected void CreatePath(string path)
    {
        if (string.IsNullOrEmpty(path))
            return;
        var pathParts = path.Split('/');
        if (pathParts.Length <= 0)
            return;
        if (pathParts[0].ToUpper() != "ASSETS")
            return;
        string combinedPath = pathParts[0];
        for (int i = 1; i < pathParts.Length; i++)
        {
            string nextCombinedPath = combinedPath + "/" + pathParts[i];
            if (!AssetDatabase.IsValidFolder(nextCombinedPath))
            {
                AssetDatabase.CreateFolder(combinedPath, pathParts[i]);
                AssetDatabase.SaveAssets();
            }
            combinedPath = nextCombinedPath;
        }
    }

    /// <summary>
    /// Pour sauvegarder des changements effectues sur un asset clone.
    /// </summary>
    /// <param name="edited"></param>
    /// <param name="loaded"></param>
    protected void SaveAsset()
    {
        if (!string.IsNullOrEmpty(SaveFileName))
        {
            for (int i = 0; i < dataList.Count; i++)
            {
                if (dataList[i] == null)
                    continue;
                string path = Path.Combine(GAME_RES_PATH + Save_Path, SaveFileName + "_" + dataList[i].Id + ".asset");
                if (AssetDatabase.LoadAssetAtPath<T>(path) == null)
                {
                    AssetDatabase.CreateAsset(dataList[i], path);
                }
                else
                {
                    EditorUtility.SetDirty(dataList[i]);
                }
            }
        }
        else
        {
            PulseDebug.LogError("Invalid File Name");
        }
        for (int i = 0; i < delAssetPath.Count; i++)
        {
            if (string.IsNullOrEmpty(delAssetPath[i]))
                continue;
            if (AssetDatabase.LoadAssetAtPath<T>(delAssetPath[i]) != null)
            {
                AssetDatabase.DeleteAsset(delAssetPath[i]);
            }
        }
        AssetDatabase.SaveAssets();
    }

    /// <summary>
    /// ferme la fenetre
    /// </summary>
    private void CloseWindow()
    {
        EditorUtility.UnloadUnusedAssetsImmediate();
    }

    /// <summary>
    /// Au changement d'une selection dans une Entete.
    /// </summary>
    private void HeaderChange()
    {
        if (data != null)
        {
            SaveAsset();
        }
        originalAsset = null;
        data = null;
        selectDataIndex = -1;
        dataList.Clear();
        clipBoard = null;
        GUI.FocusControl(null);
        OnInitialize();
        OnHeaderChange();
    }

    /// <summary>
    /// Au changement d'une selection dans une liste ou grille.
    /// </summary>
    private void ListChange()
    {
        //if (selectDataIndex >= 0 && data != null)
        //{
        //    var location_Prop = dataList[0].GetType().GetProperty("Location");
        //    if (location_Prop != null)
        //    {
        //        DataLocation locValue = (DataLocation)location_Prop.GetValue(data);
        //        if (locValue != default(DataLocation))
        //        {
        //            int correspondingIndex = dataList.FindIndex(dt =>
        //            {
        //                DataLocation objLoc = (DataLocation)location_Prop.GetValue(dt);
        //                return locValue == objLoc;
        //            });
        //            selectDataIndex = correspondingIndex;
        //        }
        //    }
        //}
        data = null;
        dataID = -1;
        GUI.FocusControl(null);
        //OnInitialize();
        OnListChange();
    }

    #endregion

    #region utils #########################################################################

    /// <summary>
    /// Return a texture colored.
    /// </summary>
    /// <param name="col"></param>
    /// <returns></returns>
    public static Texture2D ColorToTexture(Color col)
    {
        return default;
    }

    #endregion
}

/// <summary>
/// Fait la previsualisation d'une animation.
/// </summary>
public class AnimationPreviewer
{

    #region Attributes >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    #region time control ###############################################

    /// <summary>
    /// The playback time.
    /// </summary>
    private float playBackTime;

    /// <summary>
    /// The normalized returned playback time.
    /// </summary>
    private float NTime;

    /// <summary>
    /// The returned playback time.
    /// </summary>
    private float pTime;

    /// <summary>
    /// The delta of playbackTime between frames.
    /// </summary>
    private float deltaTime;

    #endregion

    #region Animation ##################################################


    /// <summary>
    /// The playback motion.
    /// </summary>
    private Motion playBackMotion;

    /// <summary>
    /// The target's animator.
    /// </summary>
    private Animator targetAnimator;

    /// <summary>
    /// The target's animator runtimeController.
    /// </summary>
    private UnityEditor.Animations.AnimatorController rtController;

    /// <summary>
    /// The target's animator runtimeController's state machine.
    /// </summary>
    private UnityEditor.Animations.AnimatorStateMachine animStateMachine;

    /// <summary>
    /// The target's animator runtimeController's state.
    /// </summary>
    private UnityEditor.Animations.AnimatorState animState;

    /// <summary>
    /// Active when previwe is playing anim.
    /// </summary>
    private bool isPlaying;

    #endregion

    #region Rendering ##################################################


    /// <summary>
    /// the preview renderer.
    /// </summary>
    private PreviewRenderUtility previewRenderer;

    /// <summary>
    /// Le chimin d'acces de l'avatrar par defaut.
    /// </summary>
    public const string previewAvatarPath = "Assets/Scripts/Pulse Engine/Core/Res/defaultAvatar.prefab";

    /// <summary>
    /// Le chimin d'acces de l'avatrar par defaut.
    /// </summary>
    public const string previewAvatarFloorPath = "Assets/Scripts/Pulse Engine/Core/Res/Plane.prefab";

    #endregion

    #region World Transforms ###########################################


    /// <summary>
    /// the target to render.
    /// </summary>
    private GameObject previewAvatar;

    /// <summary>
    /// the target's accessories to render.
    /// </summary>
    private Dictionary<HumanBodyBones, (GameObject go, Vector3 offset, Vector3 RotOffset)> accesoriesPool = new Dictionary<HumanBodyBones, (GameObject go, Vector3 offset, Vector3 RotOffset)>();

    /// <summary>
    /// arrow indicator
    /// </summary>
    private GameObject directionArrow;

    /// <summary>
    /// root indicator
    /// </summary>
    private GameObject rootGameObject;

    /// <summary>
    /// The grond plane mesh.
    /// </summary>
    private GameObject floorPlane;

    /// <summary>
    /// the floor texture.
    /// </summary>
    private Texture2D floorTexture;

    /// <summary>
    /// the floor material.
    /// </summary>
    private Material floorMaterial;

    /// <summary>
    /// the preview cam pivot offset.
    /// </summary>
    private Vector3 pivotOffset;

    #endregion

    #region Navigation #################################################

    /// <summary>
    /// the zooming factor
    /// </summary>
    private float zoomFactor = 3;

    /// <summary>
    /// the pan angle around target.
    /// </summary>
    private float panAngle = 50;

    /// <summary>
    /// the tilt angle around target.
    /// </summary>
    private float tiltAngle = 50;

    /// <summary>
    /// the target's scale.
    /// </summary>
    private float targetScale = 1;

    /// <summary>
    /// the view tool used right now
    /// </summary>
    private ViewTool viewTool = ViewTool.None;

    #endregion


    #endregion

    #region public Methods >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// Render the preview.
    /// </summary>
    /// <param name="_motion"></param>
    /// <param name="_target"></param>
    public float Previsualize(Motion _motion, float aspectRatio = 1.77f, GameObject _target = null, params (GameObject go, HumanBodyBones bone, Vector3 offset, Vector3 rotation)[] accessories)
    {
        GUILayout.BeginVertical("GroupBox");
        if (!_motion)
        {
            Reset();
            RenderNull();
            GUILayout.EndVertical();
            return 0;
        }
        if (Initialize(_motion, _target, accessories))
        {
            //targetScale = EditorGUILayout.FloatField("Scale", targetScale);
            //zoomFactor = EditorGUILayout.FloatField("distance", zoomFactor);
            //panAngle = EditorGUILayout.FloatField("Pan", panAngle);
            //tiltAngle = EditorGUILayout.FloatField("tilt", tiltAngle);
            //pivotOffset.y = EditorGUILayout.FloatField("Height", pivotOffset.y);
            //EditorGUILayout.FloatField("FPS", 1 / Time.deltaTime);
            var r = GUILayoutUtility.GetAspectRect(aspectRatio);
            Rect rect2 = r;
            rect2.yMin += 20f;
            rect2.height = Mathf.Max(rect2.height, 64f);
            int controlID = GUIUtility.GetControlID("Preview".GetHashCode(), FocusType.Passive, rect2);
            Event current = Event.current;
            EventType typeForControl = current.GetTypeForControl(controlID);
            if (typeForControl == EventType.Repaint)
            {
                RenderPreview(r);
            }
            int controlID2 = GUIUtility.GetControlID("Preview".GetHashCode(), FocusType.Passive);
            typeForControl = current.GetTypeForControl(controlID2);
            HandleViewTool(Event.current, typeForControl, 0, r);
            TimeControl();

        }
        else
            RenderNull();
        GUILayout.EndVertical();

        return pTime;
    }

    #endregion

    #region private Methods >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// Initialise the avatar.
    /// </summary>
    /// <param name="_avatar"></param>
    private bool Initialize(Motion _motion, GameObject _avatar = null, params (GameObject go, HumanBodyBones bone, Vector3 offset, Vector3 rotation)[] accessories)
    {
        if (previewRenderer == null)
        {
            previewRenderer = new PreviewRenderUtility();
            pivotOffset = new Vector3(0, 1, 0);
        }
        if (previewRenderer.camera != null)
        {
            //Camera Transform
            var cx = zoomFactor * Mathf.Cos(panAngle * Mathf.Deg2Rad);
            var cy = zoomFactor * Mathf.Sin(tiltAngle * Mathf.Deg2Rad);
            var cz = zoomFactor * Mathf.Sin(panAngle * Mathf.Deg2Rad);
            if (previewRenderer.camera)
            {
                previewRenderer.camera.transform.localPosition = pivotOffset + new Vector3(cx, cy, cz);
                previewRenderer.camera.transform.LookAt(pivotOffset, Vector3.up);

                //Camera config
                previewRenderer.camera.fieldOfView = 60;
                previewRenderer.camera.nearClipPlane = 0.01f;
                previewRenderer.camera.farClipPlane = 100;

                //Lights and FX
                SphericalHarmonicsL2 ambientProbe = RenderSettings.ambientProbe;
                SetupPreviewLightingAndFx(ambientProbe);
            }
            if (!playBackMotion || (playBackMotion != _motion && _motion != null))
            {
                playBackMotion = _motion;
            }
            if (floorTexture == null)
            {
                floorTexture = (Texture2D)EditorGUIUtility.Load("Avatar/Textures/AvatarFloor.png");
            }
            if (floorMaterial == null)
            {
                Shader shader = EditorGUIUtility.LoadRequired("Previews/PreviewPlaneWithShadow.shader") as Shader;
                floorMaterial = new Material(shader);
                floorMaterial.mainTexture = floorTexture;
                floorMaterial.mainTextureScale = Vector2.one * 5f * 4f;
                floorMaterial.SetVector("_Alphas", new Vector4(0.5f, 0.3f, 0f, 0f));
                floorMaterial.hideFlags = HideFlags.HideAndDontSave;
                floorMaterial = new Material(floorMaterial);
            }
            if (!floorPlane)
            {
                //floorPlane = UnityEngine.Object.Instantiate((GameObject)EditorGUIUtility.Load(PulseEditorMgr.previewAvatarFloorPath));
                var original = (GameObject)EditorGUIUtility.Load(previewAvatarFloorPath);
                floorPlane = UnityEngine.Object.Instantiate<GameObject>(original, previewRenderer.camera.transform);
                var render = floorPlane.GetComponent<MeshRenderer>();
                if (render)
                {
                    render.material = floorMaterial;
                }
                ResetTransform(floorPlane);
                previewRenderer.AddSingleGO(floorPlane);
            }
            if (!directionArrow)
            {
                var original = (GameObject)EditorGUIUtility.Load("Avatar/dial_flat.prefab");
                //directionArrow = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity);
                //directionArrow.hideFlags = HideFlags.HideAndDontSave;
                //directionArrow = previewRenderer.InstantiatePrefabInScene(original);
                directionArrow = UnityEngine.Object.Instantiate<GameObject>(original, previewRenderer.camera.transform);
                ResetTransform(directionArrow);
                previewRenderer.AddSingleGO(directionArrow);
            }
            if (!rootGameObject)
            {
                var original = (GameObject)EditorGUIUtility.Load("Avatar/root.fbx");
                //rootGameObject = UnityEngine.Object.Instantiate(original, Vector3.zero, Quaternion.identity);
                rootGameObject = UnityEngine.Object.Instantiate<GameObject>(original, previewRenderer.camera.transform);
                ResetTransform(rootGameObject);
                previewRenderer.AddSingleGO(rootGameObject);
            }
            if (!previewAvatar)
            {
                previewAvatar = GetAvatar(ref _avatar);
                previewAvatar.hideFlags = HideFlags.HideAndDontSave;
                previewRenderer.AddSingleGO(previewAvatar);
            }
            if (!targetAnimator)
            {
                if (previewAvatar)
                {
                    targetAnimator = previewAvatar.GetComponentInChildren<Animator>();
                    if (!targetAnimator)
                    {
                        Reset();
                        return false;
                    }

                    targetAnimator.enabled = false;
                    targetAnimator.cullingMode = AnimatorCullingMode.AlwaysAnimate;
                    targetAnimator.logWarnings = false;
                    targetAnimator.fireEvents = false;
                    InitController();
                }
                else
                {
                    Reset();
                    return false;
                }
            }
            for (int i = 0, len = accessories.Length; i < len; i++)
            {
                var accessory = accessories[i];
                if (accessory.go == null)
                    continue;
                if (accesoriesPool == null)
                    accesoriesPool = new Dictionary<HumanBodyBones, (GameObject go, Vector3 offset, Vector3 RotOffset)>();
                if (accesoriesPool.ContainsKey(accessory.bone))
                {
                    if (accesoriesPool[accessory.bone].go.name.Contains(accessory.go.name))
                    {
                        accesoriesPool[accessory.bone] = (accesoriesPool[accessory.bone].go, accessory.offset, accessory.rotation);
                        continue;
                    }
                }
                var acc = UnityEngine.Object.Instantiate<GameObject>(accessory.go, previewRenderer.camera.transform);
                ResetTransform(acc);
                acc.hideFlags = HideFlags.HideAndDontSave;
                accesoriesPool.Add(accessory.bone, (acc, accessory.offset, accessory.rotation));
                previewRenderer.AddSingleGO(acc);
            }
            return true;
        }

        return false;
    }

    #region time control ###########################################

    /// <summary>
    /// the control time interface.
    /// </summary>
    private void TimeControl()
    {
        var clip = (AnimationClip)playBackMotion;
        if (clip == null)
            return;
        //Display controls
        try
        {
            GUILayout.BeginHorizontal("HelpBox");
            if (GUILayout.Button(isPlaying ? "||" : ">>", new[] { GUILayout.Width(30), GUILayout.Height(20) }))
            {
                isPlaying = !isPlaying;
            }
            if (isPlaying)
            {
                var rect2 = GUILayoutUtility.GetRect(50, 20);
                EditorGUI.ProgressBar(rect2, pTime / clip.length, "");
            }
            else
            {
                pTime = EditorGUILayout.Slider(pTime / clip.length, 0, 1) * clip.length;
            }
            GUILayout.EndHorizontal();
        }
        catch (Exception e)
        {
            throw e;
        }
    }

    #endregion

    #region Animation ###########################################

    /// <summary>
    /// Initialise the animator controller
    /// </summary>
    private void InitController()
    {
        if (playBackMotion.legacy || targetAnimator == null)
        {
            return;
        }
        bool flag = true;
        if (rtController == null)
        {
            rtController = new UnityEditor.Animations.AnimatorController();
            //rtController.pushUndo = false;
            rtController.hideFlags = HideFlags.HideAndDontSave;
            rtController.AddLayer("preview");
            rtController.layers[0].iKPass = true;
            animStateMachine = rtController.layers[0].stateMachine;
            //animState.pushUndo = false;
            animStateMachine.hideFlags = HideFlags.HideAndDontSave;
            flag = false;
        }
        if (animState == null)
        {
            animState = animStateMachine.AddState("preview");
            //animState.pushUndo = false;
            UnityEditor.Animations.AnimatorControllerLayer[] layers2 = rtController.layers;
            animState.motion = playBackMotion;
            rtController.layers = layers2;
            animState.hideFlags = HideFlags.HideAndDontSave;
            flag = false;
        }
        UnityEditor.Animations.AnimatorController.SetAnimatorController(targetAnimator, rtController);
        if (targetAnimator.runtimeAnimatorController != rtController)
        {
            UnityEditor.Animations.AnimatorController.SetAnimatorController(targetAnimator, rtController);
        }
        if (!flag)
        {
            targetAnimator.Play(0, 0, 0f);
            targetAnimator.Update(0f);
        }
    }

    /// <summary>
    /// Animate the target
    /// </summary>
    private void Animate()
    {
        float delta = playBackTime - deltaTime;
        deltaTime = playBackTime;
        if (!previewAvatar || !targetAnimator)
            return;
        bool flag = Event.current.type == EventType.Repaint;
        AnimationClip animationClip = playBackMotion as AnimationClip;
        AnimationClipSettings animationClipSettings = AnimationUtility.GetAnimationClipSettings(animationClip);
        animationClipSettings.loopBlend = true;
        float dTime = 0.033f * 0.5f;
        if (flag)
        {
        }
        if (flag && previewAvatar != null)
        {
            if (isPlaying)
            {
                //playBackTime += (14 / animationClip.frameRate) * dTime;
                playBackTime += dTime;
                pTime = playBackTime;
                if (pTime >= animationClipSettings.stopTime)
                    isPlaying = animationClipSettings.loopTime;
                pTime %= animationClipSettings.stopTime;
            }
            if (!animationClip.legacy && targetAnimator != null)
            {
                if (animState != null)
                {
                    animState.iKOnFeet = true;
                }
                float normalizedTime = (animationClipSettings.stopTime - animationClipSettings.startTime == 0f) ? 0f
                    : ((playBackTime - animationClipSettings.startTime) / (animationClipSettings.stopTime - animationClipSettings.startTime));
                targetAnimator.Play(0, 0, normalizedTime);
                //targetAnimator.Update(isPlaying ? (14 / animationClip.frameRate) * dTime : delta);
                targetAnimator.Update(isPlaying ? dTime : delta);
                playBackTime = pTime;
                NTime = normalizedTime;
            }
            else
            {
                animationClip.SampleAnimation(previewAvatar, playBackTime);
            }
        }
    }

    #endregion

    #region Rendering ###########################################

    /// <summary>
    /// Render the preview.
    /// </summary>
    private void RenderPreview(Rect r)
    {
        if (previewRenderer == null)
            return;
        //Animation
        Animate();
        //Positionning objects
        PositionPreviewObjects();
        //Positioning floor
        AdjustFloorPosition();
        //Start Rendering
        previewRenderer.BeginPreview(r, GUIStyle.none);
        bool fog = RenderSettings.fog;
        Unsupported.SetRenderSettingsUseFogNoDirty(false);
        previewRenderer.camera.Render();
        Unsupported.SetRenderSettingsUseFogNoDirty(fog);
        //End rendering.
        Texture texture = previewRenderer.EndPreview();
        GUI.DrawTexture(r, texture);
    }

    /// <summary>
    /// Render empty preview.
    /// </summary>
    private static void RenderNull()
    {
        var r = GUILayoutUtility.GetAspectRect(16f / 9);
        EditorGUI.DropShadowLabel(new Rect(r.position.x, r.position.y / 2, r.width, r.height), "No Motion to preview.\nPlease select a valid motion to preview.");
    }

    /// <summary>
    /// Set up the lights and FX
    /// </summary>
    /// <param name="probe"></param>
    private void SetupPreviewLightingAndFx(SphericalHarmonicsL2 probe)
    {
        previewRenderer.lights[0].intensity = 1;
        previewRenderer.lights[0].color = Color.white;
        previewRenderer.lights[0].transform.rotation = Quaternion.Euler(45, 0, 0);
        previewRenderer.lights[1].intensity = 1;
        previewRenderer.lights[1].transform.rotation = Quaternion.Euler(-135, 0, 0);
        previewRenderer.lights[1].color = Color.white;
        RenderSettings.ambientMode = AmbientMode.Skybox;
        RenderSettings.ambientLight = new Color(0.1f, 0.1f, 0.1f, 1f);
        RenderSettings.ambientProbe = probe;
    }

    #endregion

    #region World Transforms ###########################################

    /// <summary>
    /// Position the gameobjects in scene.
    /// </summary>
    private void PositionPreviewObjects()
    {
        var position = targetAnimator.rootPosition;
        position.y = Mathf.Clamp(position.y, 0, position.y);
        previewAvatar.transform.position = position;
        previewAvatar.transform.rotation = targetAnimator.rootRotation;
        previewAvatar.transform.localScale = Vector3.one * targetScale;
        var offset = Vector3.Lerp(pivotOffset, position, Time.deltaTime * 10);
        pivotOffset = new Vector3(offset.x, pivotOffset.y, offset.z);
        directionArrow.transform.position = targetAnimator.rootPosition;
        var rot = Quaternion.Euler(0, targetAnimator.bodyRotation.eulerAngles.y, 0);
        directionArrow.transform.rotation = rot;
        directionArrow.transform.localScale = Vector3.one * targetScale * 2f;
        rootGameObject.transform.position = pivotOffset;
        rootGameObject.transform.rotation = Quaternion.identity;
        rootGameObject.transform.localScale = Vector3.one * targetScale * 0.25f;

        foreach (var acc in accesoriesPool)
        {
            if (targetAnimator)
            {
                var bone = targetAnimator.GetBoneTransform(acc.Key);
                if (acc.Value.go.transform.parent != bone)
                    acc.Value.go.transform.SetParent(bone);
                acc.Value.go.transform.localPosition = acc.Value.offset;
                acc.Value.go.transform.localRotation = Quaternion.Euler(acc.Value.RotOffset);
            }
        }
    }


    /// <summary>
    /// Adjust floorMaterial.
    /// </summary>
    private void AdjustFloorPosition()
    {
        if (!floorPlane)
            return;
        float displacement = 5 / ((AnimationClip)playBackMotion).frameRate;
        Vector3 position = new Vector3(previewAvatar.transform.position.x, previewAvatar.transform.position.y < 0 ? previewAvatar.transform.position.y : 0, previewAvatar.transform.position.z);
        floorPlane.transform.position = position;
        if (!floorMaterial)
            return;
        Vector2 floorTexOffset = -new Vector2(position.x * displacement, position.z * displacement);
        floorMaterial.mainTextureOffset = floorTexOffset;
    }

    /// <summary>
    /// Reset the transform of a GO and void his parent.
    /// </summary>
    /// <param name="go"></param>
    private void ResetTransform(GameObject go)
    {
        if (go == null)
            return;
        go.transform.SetParent(null);
        go.transform.position = Vector3.zero;
        go.transform.rotation = Quaternion.Euler(Vector3.zero);
    }

    #endregion

    #region Navigation ###########################################

    /// <summary>
    /// Handle the mouse up event
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="id"></param>
    private void HandleMouseUp(Event evt, int id)
    {
        if (GUIUtility.hotControl == id)
        {
            viewTool = ViewTool.None;
            GUIUtility.hotControl = 0;
            EditorGUIUtility.SetWantsMouseJumping(0);
            viewTool = ViewTool.None;
            evt.Use();
        }
    }

    /// <summary>
    /// Handle the mouse down event.
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="id"></param>
    /// <param name="previewRect"></param>
    private void HandleMouseDown(Event evt, int id, Rect previewRect)
    {
        if (viewTool != 0 && previewRect.Contains(evt.mousePosition))
        {
            EditorGUIUtility.SetWantsMouseJumping(1);
            if (evt.button == 0)
            {
                viewTool = ViewTool.Orbit;
            }
            else if (evt.button == 2)
            {
                viewTool = ViewTool.Pan;
            }
            evt.Use();
            GUIUtility.hotControl = id;
        }
    }

    /// <summary>
    /// handle the mouse drag event.
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="id"></param>
    /// <param name="previewRect"></param>
    private void HandleMouseDrag(Event evt, int id, Rect previewRect)
    {
        if (!(previewRenderer == null) && GUIUtility.hotControl == id)
        {
            switch (viewTool)
            {
                case ViewTool.Orbit:
                    DoAvatarPreviewOrbit(evt, previewRect);
                    break;
                case ViewTool.Pan:
                    DoAvatarPreviewPan(evt, previewRect);
                    break;
                case ViewTool.Zoom:
                    DoAvatarPreviewZoom(evt, (0f - HandleUtility.niceMouseDeltaZoom) * ((!evt.shift) ? 0.5f : 2f), previewRect);
                    break;
                default:
                    break;
            }
        }
    }

    /// <summary>
    /// Pan the camera.
    /// </summary>
    /// <param name="evt"></param>
    private void DoAvatarPreviewPan(Event evt, Rect previewRect)
    {
        Vector2 camPivot = new Vector2(0, -pivotOffset.y);
        camPivot -= evt.delta * ((!evt.shift) ? 1 : 3) / Mathf.Min(previewRect.width, previewRect.height) * 12f;
        //camPivot.y = Mathf.Clamp(camPivot.y, 0, 2);
        pivotOffset.y = -camPivot.y;
        pivotOffset.y = Mathf.Clamp(pivotOffset.y, 0, 2);
        evt.Use();
    }

    /// <summary>
    /// Handle the view tool
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="eventType"></param>
    /// <param name="id"></param>
    /// <param name="previewRect"></param>
    protected void HandleViewTool(Event evt, EventType eventType, int id, Rect previewRect)
    {
        switch (eventType)
        {
            case EventType.MouseMove:
            case EventType.KeyDown:
            case EventType.KeyUp:
                break;
            case EventType.ScrollWheel:
                DoAvatarPreviewZoom(evt, HandleUtility.niceMouseDeltaZoom * ((!evt.shift) ? 0.5f : 2f), previewRect);
                break;
            case EventType.MouseDown:
                HandleMouseDown(evt, id, previewRect);
                break;
            case EventType.MouseUp:
                HandleMouseUp(evt, id);
                break;
            case EventType.MouseDrag:
                HandleMouseDrag(evt, id, previewRect);
                break;
        }
    }

    /// <summary>
    /// Orbit around
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="previewRect"></param>
    private void DoAvatarPreviewOrbit(Event evt, Rect previewRect)
    {
        Vector2 camAxis = new Vector2(panAngle, -tiltAngle);
        camAxis -= evt.delta * ((!evt.shift) ? 1 : 3) / Mathf.Min(previewRect.width, previewRect.height) * 140f;
        camAxis.y = Mathf.Clamp(camAxis.y, -90f, 90f);
        panAngle = camAxis.x;
        tiltAngle = -camAxis.y;
        evt.Use();
    }

    /// <summary>
    /// zoom
    /// </summary>
    /// <param name="evt"></param>
    /// <param name="delta"></param>
    private void DoAvatarPreviewZoom(Event evt, float delta, Rect previewRect)
    {
        if (previewRect.Contains(evt.mousePosition))
        {
            float num = (0f - delta) * 0.05f;
            zoomFactor += zoomFactor * num;
            zoomFactor = Mathf.Max(zoomFactor, targetScale / 10f);
            evt.Use();
        }
    }

    #endregion

    #region Misc ###########################################

    /// <summary>
    /// get the avatar.
    /// </summary>
    /// <param name="original"></param>
    /// <returns></returns>
    private GameObject GetAvatar(ref GameObject original)
    {
        GameObject o = original ? original : (GameObject)EditorGUIUtility.Load(previewAvatarPath);
        var copy = UnityEngine.Object.Instantiate<GameObject>(o, previewRenderer.camera.transform);
        ResetTransform(copy);
        return copy;
        //return UnityEngine.Object.Instantiate(o, Vector3.zero, Quaternion.identity);
    }

    /// <summary>
    /// Reset the preview.
    /// </summary>
    private void Reset()
    {
        if (previewRenderer != null)
        {
            previewRenderer.Cleanup();
        }
        playBackMotion = null;
        playBackTime = 0;
    }

    /// <summary>
    /// Close the rendrepreview
    /// </summary>
    public void Destroy()
    {
        Reset();
    }

    public static implicit operator bool(AnimationPreviewer v)
    {
        return v != null;
    }

    ~AnimationPreviewer()
    {
        Reset();
    }

    #endregion

    #endregion

}

#endregion
