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

        /// <summary>
        /// The grid rect.
        /// </summary>
        private Rect gridRect = new Rect();

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
        /// Le Path du node duquel faire la connexion
        /// </summary>
        private CommandPath selectedPath = CommandPath.NullPath;

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
            GUILayout.BeginHorizontal();
            GUILayout.BeginVertical();
            GroupGUI(() =>
            {
                data.Label = EditorGUILayout.TextField("Label: ",data.Label);
            }, "Command Sequence Parameters");
            int indexOfModified = data.Sequence.FindIndex(cmd => { return cmd.CmdNode != null && cmd.CmdNode.Selected; });
            if (indexOfModified >= 0 && indexOfModified < data.Sequence.Count)
            {
                GroupGUI(() =>
                {
                    var sequence = data.Sequence;
                    sequence[indexOfModified] = sequence[indexOfModified].CommandSettings();
                    data.Sequence = sequence;
                }, "Command Parameters");
            }
            GUILayout.EndVertical();
            GroupGUI(() =>
            {
                gridRect= GUILayoutUtility.GetAspectRect(18f / 9f);
                GUI.BeginGroup(gridRect, style_grid);
                DrawNodes(data);
                DrawLinks(data);
                ProcessNodeEvents(data, Event.current);
                ProcessGraphEvents(Event.current, data);
                GUI.EndGroup();
            }, "Sequences", new Vector2(1000, gridRect.height));
            GUILayout.EndHorizontal();
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
            int indexOfDefaultNode = -1;
            if (data.specialCmds == null || data.specialCmds.Count <= 0)
                data.SetSpecialNodes();
            for (int i = 0; i < data.specialCmds.Count; i++)
            {
                if (data.specialCmds[i].CmdNode == null)
                {
                    var scmd = data.specialCmds[i];
                    scmd.CreateNode(new Vector2(150, 30));
                    data.specialCmds[i] = scmd;
                    continue;
                }
                var com = data.specialCmds[i];
                com.CmdNode.Style = style_node;
                data.specialCmds[i] = com;
                data.specialCmds[i].CmdNode.NodeTitle = data.specialCmds[i].Path.Label;
                data.specialCmds[i].CmdNode.Draw();
            }
            for(int i = 0; i < data.Sequence.Count; i++)
            {
                int k = i;
                if (data.Sequence[k].CmdNode == null)
                {
                    var cmd = data.Sequence[k];
                    cmd.CreateNode(new Vector2(120, 80));
                    data.Sequence[k] = cmd;
                }
                else
                {
                    var cmd = data.Sequence[k];
                    if (cmd.Inputs.Contains(data.specialCmds[0].Path))
                        indexOfDefaultNode = k;
                    cmd.CmdNode.Style = new GUIStyle("Button");
                    if (data.Sequence[k].CmdNode.Selected)
                    {
                        cmd.CmdNode.Style = style_node;
                    }
                    cmd.CmdNode.SelectAction = () =>
                    {
                        data.DeselectAllNodes();
                    };
                    //Context menu parenting
                    try
                    {
                        cmd.CmdNode.ContextActions[0] = (("Link as child of...", () =>
                        {
                            selectedPath = cmd.Path;
                        }
                        ));
                    }
                    catch
                    {
                        cmd.CmdNode.ContextActions.Add(("Link as child of...", () =>
                        {
                            selectedPath = cmd.Path;
                        }
                        ));
                    }
                    //Context menu Delete
                    try
                    {
                        cmd.CmdNode.ContextActions[1] = (("Delete", () =>
                        {
                            var sq = data.Sequence;
                            int index = sq.FindIndex(c => { return c.Path == cmd.Path; });
                            if (index < 0)
                                return;
                            sq[index].UnLinkAll(data);
                            sq[index].CmdNode.UnlinkEvents();
                            sq.RemoveAt(index);
                            data.Sequence = sq;
                        }
                        ));
                    }
                    catch
                    {
                        cmd.CmdNode.ContextActions.Add(("Delete", () =>
                        {
                            var sq = data.Sequence;
                            int index = sq.FindIndex(c => { return c.Path == cmd.Path; });
                            if (index < 0)
                                return;
                            sq[index].UnLinkAll(data);
                            sq[index].CmdNode.UnlinkEvents();
                            sq.RemoveAt(index);
                            data.Sequence = sq;
                        }
                        ));
                    }
                    cmd.CmdNode.ConnectAction = () =>
                    {
                        if (selectedPath == CommandPath.NullPath)
                            return false;
                        int indexConnectFrom = data.Sequence.FindIndex(c => { return c.Path == selectedPath; });
                        if (indexConnectFrom >= 0 && indexConnectFrom < data.Sequence.Count)
                        {
                            Command.Link(data, data.Sequence[k].Path, selectedPath);
                            selectedPath = CommandPath.NullPath;
                            return true;
                        }
                        return false;
                    };
                    cmd.CmdNode.NodeTitle = data.Sequence[k].Path.Label + " || " + data.Sequence[k].Inputs.Count + " ; " + data.Sequence[k].Outputs.Count;
                    data.Sequence[k] = cmd;
                    data.Sequence[k].CmdNode.Draw();
                }
            }
            if (indexOfDefaultNode < 0)
            {
                for (int i = 0; i < data.Sequence.Count; i++)
                {
                    var cmd = data.Sequence[i];
                    cmd.UnLinkAll(data);
                    data.Sequence[i] = cmd;
                }
                if(data.Sequence.Count > 0)
                {
                    var cmd = data.Sequence[0];
                    Command.Link(data, data.specialCmds[0].Path, cmd.Path);
                    data.Sequence[0] = cmd;
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
                if (a.CmdNode == null)
                    continue;
                for(int j = 0; j < a.Inputs.Count; j++)
                {
                    if (a.Inputs[j] != CommandPath.EntryPath)
                        continue;
                    int index = data.specialCmds.FindIndex(t => { return t.Path == a.Inputs[j]; });
                    if (index >= 0)
                    {
                        if (data.specialCmds[index].CmdNode == null)
                            continue;
                        Node.Connect(data.specialCmds[index].CmdNode, a.CmdNode);
                    }
                }
            }
            for(int i = 0; i < data.Sequence.Count; i++)
            {
                var a = data.Sequence[i];
                if (a.CmdNode == null)
                    continue;
                for(int j = 0; j < a.Outputs.Count; j++)
                {
                    if (a.Outputs[j] == CommandPath.NullPath)
                        continue;
                    int index = data.Sequence.FindIndex(t => { return t.Path == a.Outputs[j]; });
                    if (index >= 0)
                    {
                        if (data.Sequence[index].CmdNode == null)
                            continue;
                        Node.Connect(a.CmdNode, data.Sequence[index].CmdNode);
                    }
                }
            }
        }

        /// <summary>
        /// To process events on Nodes
        /// </summary>
        /// <param name="e"></param>
        private void ProcessNodeEvents(CommandSequence data, Event e)
        {
            if (data == null || data.Sequence == null)
                return;
            if (data.specialCmds != null)
            {
                for (int i = 0; i < data.specialCmds.Count; i++)
                {
                    if (data.specialCmds[i].CmdNode == null)
                        continue;
                    bool guiChanged = data.specialCmds[i].CmdNode.ProcessEvents(e);

                    if (guiChanged)
                    {
                        GUI.FocusControl(null);
                        GUI.changed = true;
                    }
                }
            }
            for (int i = data.Sequence.Count - 1; i >= 0; i--)
            {
                if (data.Sequence[i].CmdNode == null)
                    continue;
                bool guiChanged = data.Sequence[i].CmdNode.ProcessEvents(e);

                if (guiChanged)
                {
                    GUI.FocusControl(null);
                    GUI.changed = true;
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
                        GUI.FocusControl(null);
                        ProcessContextMenu(e.mousePosition, data);
                    }
                    if (e.button == 0)
                    {
                        GUI.FocusControl(null);
                        if (data.Sequence != null)
                        {
                            int index = data.Sequence.FindIndex(cmd => { return cmd.Path == selectedPath; });
                            if (index >= 0)
                                data.Sequence[index].BreakInputs(data);
                        }
                        selectedPath = CommandPath.NullPath;
                        data.DeselectAllNodes();
                    }
                    break;
                case EventType.Repaint:
                    if (data.Sequence != null)
                    {
                        if (selectedPath == CommandPath.NullPath)
                            break;
                        int index = data.Sequence.FindIndex(cmd => { return cmd.Path == selectedPath; });
                        if (index >= 0 && data.Sequence[index].CmdNode != null)
                            Node.TryConnect(data.Sequence[index].CmdNode, e.mousePosition);
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
            var cmd = new Command { Path = new CommandPath(DateTime.Now), NodePosition = mousePosition };
            var p = cmd.Path;
            p.Label = "Command "+(sq.Count + 1);
            cmd.Path = p;
            sq.Add(cmd);
            data.Sequence = sq;
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
