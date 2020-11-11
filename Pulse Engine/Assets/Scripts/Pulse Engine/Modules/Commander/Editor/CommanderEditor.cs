using PulseEngine;
using PulseEditor;
using PulseEngine.Datas;
using PulseEngine.Modules.Commander;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using UnityEditor;
using UnityEngine;

namespace PulseEditor.Modules
{

    /// <summary>
    /// The Command editor.
    /// </summary>
    public class CommanderEditor: PulseEditorMgr
    {
        /// <Summary>
        /// Implement here
        /// 3- Static void RefreshCache(object _dictionnary, DataTypes _dtype); to refresh the static cache dictionnary.
        /// 4- Static bool registeredToRefresh; to Prevent from registering to OnCacheRefresh several times.
        /// </Summary>
        #region Static Accessors ################################################################################################################################################################################################

#if UNITY_EDITOR //**********************************************************************

        ///<summary>
        /// Active when the editor is already registered to OnCacheRefresh event.
        ///</summary>
        public static bool registeredToRefresh;


        /// <summary>
        /// to refresh the static cache dictionnary
        /// </summary>
        public static void RefreshCache(object _dictionnary, DataTypes _dtype)
        {
            if (_dtype != DataTypes.CommandSequence)
                return;
            var dictionnary = _dictionnary as Dictionary<DataLocation, IData>;
            if (dictionnary == null)
                return;

            var allAsset = new List<CommandsLibrary>();
            foreach (var scp in Enum.GetValues(typeof(Scopes)))
            {
                foreach (var zn in Enum.GetValues(typeof(Zones)))
                {
                    if (CoreLibrary.Exist<CommandsLibrary>(AssetsPath, scp, zn))
                    {
                        var load = CoreLibrary.Load<CommandsLibrary>(AssetsPath, scp, zn);
                        if (load != null)
                            allAsset.Add(load);
                    }
                    else if (CoreLibrary.Save<CommandsLibrary>(AssetsPath, scp, zn))
                    {
                        var load = CoreLibrary.Load<CommandsLibrary>(AssetsPath, scp, zn);
                        if (load != null)
                            allAsset.Add(load);
                    }
                }
            }

            foreach (var entry in dictionnary)
            {
                if (entry.Value == null)
                {
                    var library = allAsset.Find(lib => { return lib.Scope == (Scopes)entry.Key.globalLocation && lib.Zone == (Zones)entry.Key.localLocation; });
                    if (library != null)
                    {
                        var data = library.DataList.Find(d =>
                        {
                            return d.Location.id == entry.Key.id;
                        }) as CommandSequence;
                        if (data != null)
                        {
                            dictionnary[entry.Key] = data;
                        }
                    }
                }
            }

        }

#endif
        #endregion

        /// <Summary>
        /// Declare here every attribute used for visual behaviour of the editor window.
        /// </Summary>
        #region Visual Attributes ################################################################################################################################################################################################

        ///<summary>
        /// The Node list
        ///</summary>
        public List<EditorNode> nodeList;

        #endregion

        /// <Summary>
        /// Declare here every attribute used for deep behaviour ot the editor window.
        /// </Summary>
        #region Fonctionnal Attributes ################################################################################################################################################################################################

        /// <summary>
        /// Le chemin d'access des datas.
        /// </summary>
        public const string AssetsPath = "CommanderDatas";

        /// <summary>
        /// L'index du node duquel faire la connexion
        /// </summary>
        private int indexConnectFrom = -1;

        #endregion

        /// <Summary>
        /// Implement here Methods To Open the window, and register to OnCacheRefresh
        /// </Summary>
        #region Door Methods ################################################################################################################################################################################################

        /// <summary>
        /// open Commander editor.
        /// </summary>
        [MenuItem(Menu_EDITOR_MENU + "Commands Editor")]
        public static void OpenEditor()
        {
            if (!registeredToRefresh)
            {
                //OnCacheRefresh += RefreshCache;
                //registeredToRefresh = true;
            }
            var window = GetWindow<CommanderEditor>();
            window.currentEditorMode = EditorMode.Edition;
            window.editorDataType = DataTypes.CommandSequence;
            window.Show();
        }

        /// <summary>
        /// open Commander selector.
        /// </summary>
        public static void OpenSelector(Action<object, EventArgs> onSelect)
        {
            if (!registeredToRefresh)
            {
                //OnCacheRefresh += RefreshCache;
                //registeredToRefresh = true;
            }
            var window = GetWindow<CommanderEditor>();
            window.currentEditorMode = EditorMode.Selection;
            window.editorDataType = DataTypes.CommandSequence;
            if (onSelect != null)
            {
                window.onSelectionEvent += (obj, arg) => {
                    onSelect.Invoke(obj, arg);
                };
            }
            window.Show();
        }

        /// <summary>
        /// open Commander Modifier.
        /// </summary>
        public static void OpenModifier(DataLocation _location)
        {
            if (!registeredToRefresh)
            {
                //OnCacheRefresh += RefreshCache;
                //registeredToRefresh = true;
            }
            var window = GetWindow<CommanderEditor>(true);
            window.currentEditorMode = EditorMode.DataEdition;
            window.editorDataType = DataTypes.CommandSequence;
            window.dataID = _location.id;
            window.assetMainFilter = _location.globalLocation;
            window.assetLocalFilter = _location.localLocation;
            window.OnInitialize();
            window.ShowModal();
        }

        #endregion

        /// <Summary>
        /// Implement here Methods related to GUI.
        /// </Summary>
        #region GUI Methods ################################################################################################################################################################################################

        /// <summary>
        /// The header.
        /// </summary>
        protected void Header()
        {
            ScopeSelector();
            ZonesSelector();
        }


        /// <summary>
        /// the selected CommandSequence datas.
        /// </summary>
        /// <param name="data"></param>
        protected void CommandDetails(CommandSequence data)
        {
            if (data == null)
                return;
            GroupGUI(() =>
            {
                data.Label = EditorGUILayout.TextField("Label: ",data.Label);
            }, "Command Sequence Parameters", 50);
            GroupGUI(() =>
            {
                var r = GUILayoutUtility.GetAspectRect(18f/9f);
                GUI.BeginGroup(r, style_grid);
                DrawNodes(data);
                DrawLinks(data);
                ProcessNodeEvents(Event.current);
                ProcessGraphEvents(Event.current, data);
                GUI.EndGroup();
            }, "Sequences");
            EndWindows();
            if (GUI.changed)
                Repaint();
        }


        #endregion

        /// <Summary>
        /// Implement here behaviours methods.
        /// </Summary>
        #region Fontionnal Methods ################################################################################################################################################################################################


        /// <summary>
        /// Initialisation.
        /// </summary>
        protected override void OnInitialize()
        {
            RefreshAssetSelect();
        }

        /// <summary>
        /// To call whenever we need to refresh asset selection.
        /// </summary>
        protected void RefreshAssetSelect()
        {
            var all = LibraryFiller(allAssets.ConvertAll<CommandsLibrary>(new Converter<ScriptableObject, CommandsLibrary>(target => { return (CommandsLibrary)target; })), (Scopes)assetMainFilter, (Zones)assetLocalFilter);
            allAssets = all.ConvertAll<CoreLibrary>(new Converter<CommandsLibrary, CoreLibrary>(target => { return target; }));
            allAssets.ForEach(a => {
                if (((CommandsLibrary)a).Scope == (Scopes)assetMainFilter && ((CommandsLibrary)a).Zone == (Zones)assetLocalFilter)
                    originalAsset = a;
            });
            if (originalAsset)
                asset = originalAsset;
        }

        /// <summary>
        /// Executed at item selection in selection mode
        /// </summary>
        protected void SelectItem()
        {
            if (onSelectionEvent != null)
            {
                IData idata = data as IData;
                onSelectionEvent.Invoke(data, new EditorEventArgs { dataObjectLocation = idata != null ? idata.Location : default });
            }
        }

        /// <summary>
        /// To display the nodes in the graph
        /// </summary>
        protected void DrawNodes(CommandSequence data)
        {
            if (data == null)
                return;
            if (nodeList == null || nodeList.Count != data.Sequence.Count)
            {
                nodeList = new List<EditorNode>();
                nodeList.Clear();
                int len = data.Sequence.Count;
                int k = 0;
                for (int i = 0; i < len; i++)
                {
                    k = i;
                    var node = new EditorNode(data.Sequence[k].NodePosition, new Vector2(200, 120), style_node);
                    node.NodeIndex = k;
                    node.MoveFunction = val =>
                    {
                        var sq = data.Sequence[k];
                        sq.NodePosition = val;
                        data.Sequence[k] = sq;
                    };
                    node.ContextActions.Add(("Parent To", val =>
                    {
                        indexConnectFrom = node.NodeIndex;
                    }
                    ));
                    node.ConnectAction = () =>
                    {
                        if (indexConnectFrom >= 0 && indexConnectFrom < data.Sequence.Count)
                        {
                            data.Link(indexConnectFrom, node.NodeIndex);
                            indexConnectFrom = -1;
                        }
                    };
                    nodeList.Add(node);
                }
            }
            else
            {
                int k = 0;
                for (int i = 0; i < nodeList.Count; i++)
                {
                    k = i;
                    nodeList[k].NodeTitle = data.Sequence[k].Path.label;
                    nodeList[k].Draw();
                }
            }
        }
        
        /// <summary>
        /// To display the nodes Links in the graph
        /// </summary>
        protected void DrawLinks(CommandSequence data)
        {
            for(int i = 0; i < data.Sequence.Count; i++)
            {
                var a = data.Sequence[i];
                var parentPath = a.GetParent(data);
                int index = data.Sequence.FindIndex(t => { return t.Path.Equals(parentPath); });
                if(!parentPath.Equals(CommandPath.EmptyPath) && index >= 0)
                {
                    EditorNode.Connect(nodeList[i], nodeList[index]);
                }
            }
        }

        /// <summary>
        /// To process events on Nodes
        /// </summary>
        /// <param name="e"></param>
        private void ProcessNodeEvents(Event e)
        {
            if (nodeList != null)
            {
                for (int i = nodeList.Count - 1; i >= 0; i--)
                {
                    bool guiChanged = nodeList[i].ProcessEvents(e);

                    if (guiChanged)
                    {
                        GUI.changed = true;
                    }
                }
            }
        }


        /// <summary>
        /// To process events in the graph
        /// </summary>
        protected void ProcessGraphEvents(Event e, CommandSequence data)
        {
            if (data == null)
                return;
            switch (e.type)
            {
                case EventType.MouseDown:
                    if (e.button == 1)
                    {
                        ProcessContextMenu(e.mousePosition, data);
                    }
                    if (e.button == 0)
                    {
                        if (indexConnectFrom >= 0 && indexConnectFrom < data.Sequence.Count)
                        {
                            data.UnLink(indexConnectFrom, -1);
                        }
                        indexConnectFrom = -1;
                    }
                    break;
            }
        }


        /// <summary>
        /// Show the context menu.
        /// </summary>
        /// <param name="mousePosition"></param>
        private void ProcessContextMenu(Vector2 mousePosition, CommandSequence data)
        {
            GenericMenu genericMenu = new GenericMenu();
            genericMenu.AddItem(new GUIContent("Add Command"), false, () => OnClickAddCMD(mousePosition, data));
            genericMenu.ShowAsContext();
        }

        /// <summary>
        /// To add a command on the current sequencer on context menu.
        /// </summary>
        /// <param name="mousePosition"></param>
        /// <param name="data"></param>
        private void OnClickAddCMD(Vector2 mousePosition, CommandSequence data)
        {
            if (data == null)
                return;
            var sq = data.Sequence;
            var cmd = new Command { Parent = CommandPath.EmptyPath, NodePosition = mousePosition, Path = new CommandPath { label = "Command "+(sq.Count + 1)} };
            sq.Add(cmd);
        }


        #endregion

        /// <Summary>
        /// Implement here overrides methods.
        /// </Summary>
        #region Program FLow Methods ################################################################################################################################################################################################

        protected override void OnHeaderRedraw()
        {
            Header();
        }

        /// <summary>
        /// on item changed on a list.
        /// </summary>
        protected override void OnListChange()
        {
            if (nodeList != null)
            {
                nodeList.Clear();
                nodeList = null;
            }
        }

        /// <summary>
        /// on header changed.
        /// </summary>
        protected override void OnHeaderChange()
        {
        }

        protected override void OnBodyRedraw()
        {
            CommandDetails((CommandSequence)data);
        }

        #endregion

        /// <Summary>
        /// Implement here miscelaneous methods relative to the module in editor mode.
        /// </Summary>
        #region Helpers & Tools ################################################################################################################################################################################################

        /// <summary>
        /// Charge tous les assets d'arme.
        /// </summary>
        /// <param name="inputLibrary"></param>
        /// <returns></returns>
        private static List<CommandsLibrary> LibraryFiller(List<CommandsLibrary> inputLibrary, Scopes _scope, Zones _zone)
        {
            if (inputLibrary == null)
                inputLibrary = new List<CommandsLibrary>();
            if (CoreLibrary.Exist<CommandsLibrary>(AssetsPath, _scope, _zone))
                inputLibrary.Add(CoreLibrary.Load<CommandsLibrary>(AssetsPath, _scope, _zone));
            else if (CoreLibrary.Save<CommandsLibrary>(AssetsPath, _scope, _zone))
                inputLibrary.Add(CoreLibrary.Load<CommandsLibrary>(AssetsPath, _scope, _zone));
            return inputLibrary;
        }

        #endregion
    }
}
