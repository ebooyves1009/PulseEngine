using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Modules.Anima;
using PulseEngine.Modules;
using PulseEditor.Globals;
using PulseEngine.Globals;
using System;
using UnityEditor;
using UnityEditor.Animations;
using System.Linq;

namespace PulseEditor.Modules.Anima
{
    /// <summary>
    /// l'editeur d'animation.
    /// </summary>
    public class AnimaEditor : PulseEditorMgr
    {
        #region Fonctionnal Attributes ################################################################

        /// <summary>
        /// Le temps actuel dans l'animation jouee.
        /// </summary>
        private float timeInCurrentAnimation = 0;

        /// <summary>
        /// La phase d'animation en cours.
        /// </summary>
        private AnimaPhase currentAnimPhase;

        /// <summary>
        /// L'event d'animation en cours.
        /// </summary>
        private CommandAction currentAnimCommand;

        #endregion

        #region Visual Attributes ################################################################

        /// <summary>
        /// La categoie selectionne
        /// </summary>
        private AnimaCategory selectedCategory;

        /// <summary>
        /// Le types selectionne.
        /// </summary>
        private AnimaType selectedType;

        /// <summary>
        /// Le scroll de la liste des events.
        /// </summary>
        private Vector2 eventListScroll;

        /// <summary>
        /// La previsualisations de l'animation
        /// </summary>
        private Previewer animPreview;

        #endregion

        #region Fonctionnal Methods ################################################################

        /// <summary>
        /// initialise toutes les assets.
        /// </summary>
        private void Initialisation()
        {
            allAssets.Clear();
            editedAsset = null;
            editedData = null;
            selectDataIndex = -1;
            if (animPreview != null)
                animPreview.Destroy();
            foreach (AnimaCategory category in Enum.GetValues(typeof(AnimaCategory)))
            {
                foreach (AnimaType type in Enum.GetValues(typeof(AnimaType)))
                {
                    if (AnimaLibrary.Exist(category, type))
                        allAssets.Add(AnimaLibrary.Load(category, type));
                    else if (AnimaLibrary.Save(category, type))
                        allAssets.Add(AnimaLibrary.Load(category, type));
                }
            }
            asset = allAssets.Find(ass => { return ((AnimaLibrary)ass).AnimCategory == selectedCategory && ((AnimaLibrary)ass).AnimType == selectedType; });
            if (asset)
            {
                editedAsset = asset;
            }
            if(windowOpenMode == EditorMode.ItemEdition && dataID >= 0)
            {
                editedData = ((AnimaLibrary)editedAsset).DataList.Find(dd => { return dd.ID == dataID; });
            }
            animPreview = new Previewer();
        }

        /// <summary>
        /// A la selection d'un item.
        /// </summary>
        private void onSelect()
        {
            if (onSelectionEvent != null)
                onSelectionEvent.Invoke(editedData, null);
        }

        #endregion

        #region Static Methods ################################################################

        /// <summary>
        /// open Anima editor.
        /// </summary>
        [MenuItem(Menu_EDITOR_MENU + "Anima Editor")]
        public static void OpenEditor()
        {
            var window = GetWindow<AnimaEditor>();
            window.windowOpenMode = EditorMode.Normal;
            window.Show();
        }

        /// <summary>
        /// open Anima selector.
        /// </summary>
        public static void OpenSelector(Action<object, EventArgs> onSelect)
        {
            var window = GetWindow<AnimaEditor>();
            window.windowOpenMode = EditorMode.Selector;
            if (onSelect != null)
            {
                window.onSelectionEvent += (obj, arg) => {
                    onSelect.Invoke(obj, arg);
                };
            }
            window.Initialisation();
            window.Show();
        }

        /// <summary>
        /// open Anima speceific selector.
        /// </summary>
        public static void OpenSelector(Action<object, EventArgs> onSelect, AnimaCategory _cat, AnimaType _typ)
        {
            var window = GetWindow<AnimaEditor>();
            window.windowOpenMode = EditorMode.specialSelect;
            window.selectedCategory = _cat;
            window.selectedType = _typ;
            if (onSelect != null)
            {
                window.onSelectionEvent += (obj, arg) => {
                    onSelect.Invoke(obj, arg);
                };
            }
            window.Initialisation();
            window.Show();
        }

        /// <summary>
        /// open Anima Modifier.
        /// </summary>
        public static void OpenModifier(int _id, AnimaCategory _cat, AnimaType _typ)
        {
            var window = GetWindow<AnimaEditor>(true);
            window.windowOpenMode = EditorMode.ItemEdition;
            window.selectedCategory = _cat;
            window.selectedType = _typ;
            window.dataID = _id;
            window.Initialisation();
            window.ShowAuxWindow();
        }

        #endregion

        #region Visual Methods ################################################################


        /// <summary>
        /// refraichie.
        /// </summary>
        protected override void OnRedraw()
        {
            base.OnRedraw();
            if (windowOpenMode == EditorMode.Normal)
            {
                if (!Header())
                {
                    return;
                }
            }
            GUILayout.BeginHorizontal();
            if (windowOpenMode != EditorMode.ItemEdition)
            {
                ScrollablePanel(() =>
                {
                    ListAnimations((AnimaLibrary)editedAsset);
                    Foot();
                }, true);
            }
            ScrollablePanel(() =>
            {
                AnimDetails((AnimaData)editedData);
                if (windowOpenMode == EditorMode.ItemEdition)
                    Foot();
            });
            GUILayout.EndHorizontal();
        }

        /// <summary>
        /// initialise.
        /// </summary>
        protected override void OnInitialize()
        {
            Initialisation();
        }

        /// <summary>
        /// a la fermeture.
        /// </summary>
        protected override void OnQuit()
        {
            if (animPreview != null)
                animPreview.Destroy();
        }


        protected override void OnListChange()
        {
            if (animPreview != null)
                animPreview.Destroy();
            animPreview = null;
            animPreview = new Previewer();
        }

        protected override void OnHeaderChange()
        {
            if (animPreview != null)
                animPreview.Destroy();
            animPreview = null;
            animPreview = new Previewer();
        }

        #endregion

        #region Common Windows ################################################################

        /// <summary>
        /// l'entete.
        /// </summary>
        /// <returns></returns>
        public bool Header()
        {
            int categorySelect = (int)selectedCategory;
            int typeSelect = (int)selectedType;
            GroupGUInoStyle(() =>
            {
                MakeHeader((int)selectedCategory, Enum.GetNames(typeof(AnimaCategory)), index => { selectedCategory = (AnimaCategory)index; });
            }, "Category", 50);
            GroupGUInoStyle(() =>
            {
                MakeHeader((int)selectedType, Enum.GetNames(typeof(AnimaType)), index => { selectedType = (AnimaType)index; });
            }, "Type", 50);
            if (editedAsset)
                return true;
            else
                return false;
        }

        /// <summary>
        /// Le peid de page.
        /// </summary>
        public void Foot()
        {
            SaveBarPanel(onSelect);
        }

        /// <summary>
        /// Liste les animations.
        /// </summary>
        /// <param name="library"></param>
        public void ListAnimations(AnimaLibrary library)
        {
            if (!library)
                return;
            Func<bool> listCompatiblesmode = () =>
            {
                switch (windowOpenMode)
                {
                    case EditorMode.Normal:
                        return true;
                    case EditorMode.Selector:
                        return true;
                    case EditorMode.ItemEdition:
                        return false;
                    case EditorMode.Preview:
                        return false;
                    case EditorMode.specialSelect:
                        return true;
                    default:
                        return false;
                }
            };
            if (listCompatiblesmode())
            {
                GroupGUI(() =>
                {
                    GUILayout.BeginVertical();
                    List<string> listContent = new List<string>();
                    int maxId = 0;
                    for (int i = 0; i < library.DataList.Count; i++)
                    {
                        var data = library.DataList[i];
                        var name = data.Motion ? data.Motion.name : "null";
                        listContent.Add(name);
                    }
                    selectDataIndex = MakeList(selectDataIndex, listContent.ToArray());
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("+"))
                    {
                        library.DataList.Add(new AnimaData {
                            ID = maxId + 1, AnimLayer = AnimaManager.LayerFromType(selectedType),
                            IsHumanMotion = selectedCategory == AnimaCategory.humanoid });
                    }
                    if (selectDataIndex >= 0 && selectDataIndex < library.DataList.Count)
                    {
                        if (GUILayout.Button("-"))
                        {
                            library.DataList.RemoveAt(selectDataIndex);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }, "Anima Datas List");
                if (selectDataIndex >= 0 && selectDataIndex < library.DataList.Count)
                    editedData = library.DataList[selectDataIndex];
                else
                    editedData = null;
            }
        }

        /// <summary>
        /// details.
        /// </summary>
        /// <param name="data"></param>
        public void AnimDetails(AnimaData data)
        {
            if (data == null)
                return;
            GroupGUI(() =>
            {
                //ID
                EditorGUILayout.LabelField("ID: " + data.ID, EditorStyles.boldLabel);
                if (windowOpenMode == EditorMode.Selector)
                {
                    //Motion
                    if (animPreview != null)
                        timeInCurrentAnimation = animPreview.Previsualize(data.Motion, 4 / 3);
                    return;
                }
                else
                {
                    //Motion
                    if (animPreview != null)
                        timeInCurrentAnimation = animPreview.Previsualize(data.Motion, 18 / 9);
                }
                var newMotion = EditorGUILayout.ObjectField("Motion ", data.Motion, typeof(AnimationClip), false) as AnimationClip;
                if (newMotion != data.Motion)
                {
                    if (EditorUtility.DisplayDialog("Warning", "By changing motion, you will lost all data configured from it.\n Proceed?", "Yes", "No"))
                    {
                        data.Motion = newMotion;
                        var tmpPhase = data.PhaseAnims;
                        tmpPhase.Clear();
                        data.PhaseAnims = tmpPhase;
                        var tmpEv = data.EventList;
                        tmpEv.Clear();
                        data.EventList = tmpEv;
                        if (animPreview != null)
                            animPreview.Destroy();
                        animPreview = null;
                        animPreview = new Previewer();
                    }
                } 
                GUILayout.Space(10);
                //is human
                EditorGUILayout.Toggle("Is Human Motion", (data.Motion ? data.Motion.isHumanMotion : false));
                GUILayout.Space(10);
                //Name
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Name: " , EditorStyles.boldLabel);
                EditorGUILayout.LabelField((data.Motion? data.Motion.name : string.Empty));
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                //Layer
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Animator Layer: " , EditorStyles.boldLabel);
                data.AnimLayer = AnimaManager.LayerFromType(selectedType);
                EditorGUILayout.LabelField(data.AnimLayer.ToString());
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                //Anim Space
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Physic Place, This motion takes places in: ", EditorStyles.boldLabel);
                data.PhysicPlace = (PhysicSpaces)EditorGUILayout.EnumPopup(data.PhysicPlace);
                GUILayout.EndHorizontal();
                GUILayout.Space(10);


                #region Anim phases ---------------------------------------------------------------------------------------------------------------------------------------

                GUILayout.BeginVertical("HELPBOX");
                EditorGUILayout.LabelField("Animation phase", EditorStyles.boldLabel);
                GUILayout.Space(15);
                GUILayout.BeginHorizontal();
                int indexPhase = -1;
                for (int i = 0; i < data.PhaseAnims.Count; i++) {
                    var phase = data.PhaseAnims[i];
                    if (phase.timeStamp.time <= timeInCurrentAnimation && timeInCurrentAnimation < (phase.timeStamp.time + phase.timeStamp.duration))
                    {
                        //currentAnimPhase = phase.phase;
                        indexPhase = i;
                        break;
                    }
                }
                currentAnimPhase = (AnimaPhase)EditorGUILayout.EnumPopup("Phase",currentAnimPhase);
                if (GUILayout.Button( indexPhase>= 0?"Change":"Add" +" animPhase "+currentAnimPhase+" at "+timeInCurrentAnimation))
                {
                    if (indexPhase >= 0)
                    {
                        var tmpPhase = data.PhaseAnims;
                        tmpPhase[indexPhase] = new AnimePhaseTimeStamp
                        {
                            phase = currentAnimPhase,
                            timeStamp = new TimeStamp { time = timeInCurrentAnimation, duration = (data.Motion ? 1 / data.Motion.frameRate : 0.1f) }
                        };
                        data.PhaseAnims = tmpPhase;
                    }
                    else
                    {
                        var tmpPhase = data.PhaseAnims;
                        tmpPhase.Add(new AnimePhaseTimeStamp
                        {
                            phase = currentAnimPhase,
                            timeStamp = new TimeStamp { time = timeInCurrentAnimation, duration = (data.Motion ? 1 / data.Motion.frameRate : 0.1f) }
                        });
                        data.PhaseAnims = tmpPhase;
                    }
                }
                if (indexPhase >= 0)
                {
                    if (GUILayout.Button("Remove Phase"))
                    {
                        var tmpPhase = data.PhaseAnims;
                        tmpPhase.RemoveAt(indexPhase);
                        data.PhaseAnims = tmpPhase;
                    }
                }
                if (data.PhaseAnims.Count > 0)
                {
                    try
                    {
                        if (GUILayout.Button("Clear All Phases"))
                        {
                            if (EditorUtility.DisplayDialog("Warning", "Are you sure?", "Yes", "No"))
                            {
                                var tmpPhase = data.PhaseAnims;
                                tmpPhase.Clear();
                                data.PhaseAnims = tmpPhase;
                            }
                        }
                    }
                    catch { }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                EditorGUILayout.LabelField("Current Anim Phase: ", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                //EditorGUILayout.LabelField(currentAnimPhase.ToString());
                for (int i = 0; i < data.PhaseAnims.Count; i++)
                {
                    var phase = data.PhaseAnims[i];
                    float time = phase.timeStamp.time;
                    float endValue = data.Motion ? data.Motion.length : 1;
                    if (i >= 0 && i < data.PhaseAnims.Count - 1)
                        endValue = data.PhaseAnims[i + 1].timeStamp.time;
                    if (phase.timeStamp.time <= timeInCurrentAnimation && timeInCurrentAnimation < (phase.timeStamp.time + phase.timeStamp.duration))
                    {
                        //currentAnimPhase = phase.phase;
                    }
                    EditorGUI.ProgressBar(GUILayoutUtility.GetRect(50,25), Mathf.InverseLerp(time, endValue, timeInCurrentAnimation), phase.phase.ToString());
                }
                GUILayout.EndHorizontal();
                GUILayout.EndVertical();
                GUILayout.Space(10);
                #endregion

                #region  Anim Events --------------------------------------------------------------------------------------------------------------------------------------------------------------
                GUILayout.BeginVertical("HELPBOX");
                EditorGUILayout.LabelField("Events", EditorStyles.boldLabel);
                GUILayout.Space(15);
                GUILayout.BeginHorizontal();
                int indexEvent = -1;
                for (int i = 0; i < data.EventList.Count; i++)
                {
                    var eventAt = data.EventList[i];
                    if (eventAt.timeStamp.time <= timeInCurrentAnimation && timeInCurrentAnimation < (eventAt.timeStamp.time + eventAt.timeStamp.duration))
                    {
                        currentAnimCommand = eventAt.command;
                        indexEvent = i;
                        break;
                    }
                }

                if (GUILayout.Button("Add Event at " + timeInCurrentAnimation))
                {
                    var tmpEvents = data.EventList;
                    tmpEvents.Add(new AnimeCommand
                    {
                        command = new CommandAction(),
                        timeStamp = new TimeStamp { time = timeInCurrentAnimation, duration = data.Motion ? (1 / data.Motion.frameRate) : 0 },
                        isOneTimeAction = true
                    });
                    data.EventList = tmpEvents;
                }
                if(indexEvent >= 0)
                {
                    if (GUILayout.Button("Remove Event at " + data.EventList[indexEvent].timeStamp.time))
                    {
                        var tmpEvents = data.EventList;
                        tmpEvents.RemoveAt(indexEvent);
                        data.EventList = tmpEvents;
                    }
                }
                if(data.EventList.Count > 0)
                {
                    try
                    {
                        if (GUILayout.Button("Clear All Events"))
                        {
                            if (EditorUtility.DisplayDialog("Warning", "Are you sure?", "Yes", "No"))
                            {
                                var tmpEv = data.EventList;
                                tmpEv.Clear();
                                data.EventList = tmpEv;
                            }
                        }
                    }
                    catch { }
                }

                GUILayout.EndHorizontal();
                GUILayout.Space(5);

                eventListScroll = GUILayout.BeginScrollView(eventListScroll, new[] { GUILayout.MinHeight(100)});
                GUILayout.BeginHorizontal();
                int k = 0;
                for(int i = 0; i < data.EventList.Count; i++)
                {
                    k = i;
                    var ev = data.EventList[k];
                    var time = ev.timeStamp.time;
                    var duration = ev.timeStamp.duration;
                    Color activeCol = Color.white;
                    if (time <= timeInCurrentAnimation && timeInCurrentAnimation < (time + duration))
                    {
                        activeCol = Color.green;
                    }
                    if (k >= 0)
                    {
                        var tmpEvents = data.EventList;
                        var evEnt = tmpEvents[k];
                        var timeStamp = evEnt.timeStamp;

                        GUILayout.BeginVertical("HELPBOX");
                        EditorGUILayout.LabelField("Event No: " + (k + 1) + (k < data.EventList.Count && k >= 0 ? " : " + data.EventList[k].command.code : " at " + timeInCurrentAnimation), EditorStyles.boldLabel);
                        GUILayout.Space(10);
                        GUILayout.BeginHorizontal();
                        try
                        {
                            EditorGUILayout.Knob(new Vector2(50, 50), Mathf.InverseLerp(time, time + duration, timeInCurrentAnimation) * 100, 0, 100, "", Color.gray, activeCol, true);
                        }
                        catch { }
                        GUILayout.BeginVertical();
                        //float duration = timeStamp.duration;
                        bool oneTime = evEnt.isOneTimeAction;
                        GUILayout.BeginHorizontal();
                        duration = EditorGUILayout.FloatField("Duration", duration);
                        if(GUILayout.Button("T",new[] { GUILayout.Width(18) }))
                        {
                            duration = timeInCurrentAnimation - timeStamp.time;
                        }
                        GUILayout.EndHorizontal();
                        duration = Mathf.Clamp(duration, (data.Motion ? 1 / data.Motion.frameRate : 0.03f), (data.Motion ? data.Motion.length : 1) - timeStamp.time);
                        if (duration > (data.Motion ? (2 / data.Motion.frameRate) : 0.06f))
                        {
                            oneTime = EditorGUILayout.Toggle("One time action", oneTime);
                        }
                        else
                            oneTime = true;

                        timeStamp.duration = duration;
                        evEnt.isOneTimeAction = oneTime;
                        evEnt.timeStamp = timeStamp;
                        tmpEvents[k] = evEnt;

                        GUILayout.EndVertical();
                        GUILayout.EndHorizontal();
                        if (GUILayout.Button("Customize Event"))
                        {
                            throw new NotImplementedException("You have to open the command modifier here");
                            //evEnt.command//TODO: Editor of Commands => action.
                        }
                        GUILayout.EndVertical();

                        data.EventList = tmpEvents;
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.EndScrollView();

                GUILayout.EndVertical();
                GUILayout.Space(10);
                #endregion

            }, "Details");
        }

        #endregion

        #region Mono #########################################################################################


        #endregion

        #region Helpers & Tools ################################################################

        /// <summary>
        /// Cree et configure des Runtime Animator controllers.
        /// </summary>
        public class AnimaMachineEditor : PulseEditorMgr
        {
            #region Attributs >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>


            /// <summary>
            /// The current animator runtimeController.
            /// </summary>
            private AnimatorController rtController;

            /// <summary>
            /// The target's animator runtimeController's state machine.
            /// </summary>
            private AnimatorStateMachine animStateMachine;

            /// <summary>
            /// The target's animator runtimeController's Layer.
            /// </summary>
            private AnimatorControllerLayer animLayer;

            /// <summary>
            /// The target's animator runtimeController's state.
            /// </summary>
            private AnimatorState animState;

            /// <summary>
            /// The selected transition.
            /// </summary>
            private AnimatorTransition animtransition;

            /// <summary>
            /// the selected layer index
            /// </summary>
            private int layerIndex;

            /// <summary>
            /// the selected state index
            /// </summary>
            private int stateIndex;

            /// <summary>
            /// the selected parameter index
            /// </summary>
            private int paramIndex;

            /// <summary>
            /// boolean enabled when creating a new parameter.
            /// </summary>
            private bool addingParam;

            /// <summary>
            /// le nom, en cours d'edition d'un parametre.
            /// </summary>
            private string paramName;

            /// <summary>
            /// la categorie du controller, humanoid, quadruped...
            /// </summary>
            private AnimaCategory controllerCategory;

            #endregion

            #region Statics >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

            /// <summary>
            /// Open the Editor.
            /// </summary>
            /// <param name="rtc"></param>
            /// <param name="ownwerName"></param>
            public static void Open(RuntimeAnimatorController rtc, AnimaCategory category, string ownwerName, Action<object,EventArgs> onDone)
            {
                var window = GetWindow<AnimaMachineEditor>(true);
                string path = ModuleConstants.AssetsPath;
                string folderPath = string.Join("/", PulseEngineMgr.Path_GAMERESSOURCES, path,"AnimatorControllers");
                if (rtc != null)
                    window.rtController = rtc as UnityEditor.Animations.AnimatorController;
                else
                {
                    if (!AssetDatabase.IsValidFolder(folderPath))
                        AssetDatabase.CreateFolder(string.Join("/", PulseEngineMgr.Path_GAMERESSOURCES, path), "AnimatorControllers");
                    window.rtController = AnimatorController.CreateAnimatorControllerAtPath(folderPath+"/"+ ownwerName + "_Controller.controller");
                    //Configuring
                    window.rtController.name = ownwerName + "_Controller";
                    foreach(AnimaLayer layer in Enum.GetValues(typeof(AnimaLayer)))
                    {
                        window.rtController.AddLayer(layer.ToString());
                    }
                    for(int i = 0, len = window.rtController.layers.Length; i < len; i++)
                    {
                        var layer = window.rtController.layers[i];
                        layer.defaultWeight = 1;
                        layer.blendingMode = AnimatorLayerBlendingMode.Override;
                        layer.iKPass = true;
                        var emptyState = layer.stateMachine.AddState("Empty");
                        layer.stateMachine.defaultState = emptyState;
                    }
                    AssetDatabase.SaveAssets();
                }
                if (onDone != null)
                    window.onSelectionEvent += (obj, arg) =>
                    {
                        onDone.Invoke(obj, arg);
                    };
                window.controllerCategory = category;
                window.Show();
            }

            #endregion

            #region Methods >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

            /// <summary>
            /// Appellee a la validation des modifications.
            /// </summary>
            protected void OnDone()
            {
                if (onSelectionEvent != null)
                    onSelectionEvent.Invoke((RuntimeAnimatorController)rtController, null);
                Close();
            }

            protected override void OnRedraw()
            {
                GUILayout.BeginVertical();
                GUILayout.BeginHorizontal();
                //First column
                ScrollablePanel(() =>
                {
                    ParametersList(rtController);
                    animLayer = LayerSelect(rtController);
                    animState = StateList(animLayer);
                });
                //Second Column
                ScrollablePanel(() =>
                {
                    StateDetails(animState);
                    animtransition = TransitionsList(animState);
                    TransitionDetails(animtransition);
                });
                //
                GUILayout.EndHorizontal();
                if (GUILayout.Button("Done"))
                {
                    OnDone();
                }
                GUILayout.EndVertical();
            }

            #endregion

            #region GUI Methods >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

            /// <summary>
            /// The Parameters view.
            /// </summary>
            protected void ParametersList(AnimatorController _controller)
            {
                if (_controller == null)
                    return;
                GroupGUI(() =>
                {
                    GUILayout.BeginVertical();
                    List<string> listContent = new List<string>();
                    for (int i = 0; i < _controller.parameters.Length; i++)
                    {
                        var parameter = _controller.parameters[i];
                        var name = parameter.name;
                        listContent.Add(name);
                    }
                    paramIndex = MakeList(paramIndex, listContent.ToArray());
                    GUILayout.Space(5);
                    if (addingParam)
                    {
                        GUILayout.BeginHorizontal();
                        paramName = EditorGUILayout.TextField("Parameter Name ", paramName);
                        if (GUILayout.Button("+B"))
                        {
                            _controller.AddParameter(paramName, AnimatorControllerParameterType.Bool);
                            addingParam = false;
                        }
                        if (GUILayout.Button("+T"))
                        {
                            _controller.AddParameter(paramName, AnimatorControllerParameterType.Trigger);
                            addingParam = false;
                        }
                        if (GUILayout.Button("+I"))
                        {
                            _controller.AddParameter(paramName, AnimatorControllerParameterType.Int);
                            addingParam = false;
                        }
                        if (GUILayout.Button("+F"))
                        {
                            _controller.AddParameter(paramName, AnimatorControllerParameterType.Float);
                            addingParam = false;
                        }
                        GUILayout.EndHorizontal();
                    }
                    GUILayout.BeginHorizontal();
                    if (!addingParam)
                    {
                        if (GUILayout.Button("+"))
                        {
                            addingParam = true;
                        }
                        if (paramIndex >= 0 && paramIndex < _controller.parameters.Length)
                        {
                            if (GUILayout.Button("-"))
                            {
                                _controller.RemoveParameter(_controller.parameters[paramIndex]);
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }, "Parameters");
            }

            /// <summary>
            /// The layer select view
            /// </summary>
            /// <param name="_controller"></param>
            /// <returns>returns the selected AnimatorcontrollerLayer selected</returns>
            protected AnimatorControllerLayer LayerSelect(AnimatorController _controller)
            {
                if (_controller == null)
                    return null;
                AnimatorControllerLayer layer = null;
                var layerNames = from l in _controller.layers
                                 select l.name;
                GroupGUI(() =>
                {
                    layerIndex = EditorGUILayout.Popup("Layer", layerIndex, layerNames.ToArray());
                }, "Layers",25);

                if (layerIndex >= 0 && layerIndex < _controller.layers.Length)
                    layer = _controller.layers[layerIndex];
                return layer;
            }

            /// <summary>
            /// List all the states on the specified layer.
            /// </summary>
            /// <param name="_animLayer"></param>
            /// <returns></returns>
            protected AnimatorState StateList(AnimatorControllerLayer _animLayer)
            {
                AnimatorState state = null;
                var machine = _animLayer.stateMachine;
                if (machine == null)
                    return null;
                var states = machine.states;
                GroupGUI(() =>
                {
                    GUILayout.BeginVertical();
                    List<string> listContent = new List<string>();
                    for (int i = 0; i < states.Length; i++)
                    {
                        var stateX = states[i].state;
                        var name = stateX.name;
                        listContent.Add(name);
                    }
                    stateIndex = MakeList(stateIndex, listContent.ToArray());
                    GUILayout.Space(5);
                    if (stateIndex >= 0 && stateIndex < states.Length)
                        state = states[stateIndex].state;
                    GUILayout.BeginHorizontal();
                    if (layerIndex > 0)
                    {
                        if (GUILayout.Button("+"))
                        {
                            var type = (AnimaType)(layerIndex - 1);
                            OpenSelector((obj, arg) =>
                            {
                                AnimaData data = obj as AnimaData;
                                if (data != null)
                                {
                                    var st = machine.AddState(data.Motion.name);
                                    st.motion = data.Motion;
                                    var stMachine = st.AddStateMachineBehaviour<AnimaStateMachine>();
                                    stMachine.AnimationData = data;
                                }
                            }, controllerCategory, type);
                        }
                        if (stateIndex >= 0 && stateIndex < states.Length)
                        {
                            if (states[stateIndex].state.behaviours.Length > 0)
                            {
                                var thatState = states[stateIndex].state.behaviours[0] as AnimaStateMachine;
                                if (GUILayout.Button("Edit"))
                                {
                                    OpenModifier(thatState.AnimationData.ID, controllerCategory, (AnimaType)layerIndex);
                                }
                                if (GUILayout.Button("Replace"))
                                {
                                    OpenSelector((obj, arg) =>
                                    {
                                        AnimaData data = obj as AnimaData;
                                        if (data != null)
                                        {
                                            thatState.AnimationData = data;
                                        }
                                    }, controllerCategory, (AnimaType)layerIndex);
                                }
                            }
                            if (GUILayout.Button("-"))
                            {
                                machine.RemoveState(states[stateIndex].state);
                            }
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }, "State List");
                return state;
            }

            /// <summary>
            /// Preview of the state's motion
            /// </summary>
            /// <param name="_animStetMotion"></param>
            protected void Preview(AnimatorState _animStetMotion)
            {

            }

            /// <summary>
            /// The StateDetails view.
            /// </summary>
            protected void StateDetails(AnimatorState _animState)
            {

            }

            /// <summary>
            /// The transition list of the state
            /// </summary>
            /// <param name="_animTransition"></param>
            protected AnimatorTransition TransitionsList(AnimatorState _animState)
            {
                return null;
            }

            /// <summary>
            /// The transition inspector
            /// </summary>
            /// <param name="_animTransition"></param>
            protected void TransitionDetails(AnimatorTransition _animTransition)
            {

            }

            #endregion
        }

        /// <summary>
        /// Utilitaires pour modifier les keyframes des animations.
        /// </summary>
        public class AnimationEdition : PulseEditorMgr
        {
            #region Attributs >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

            /// <summary>
            /// The source clip.
            /// </summary>
            private AnimationClip sourceClip;

            /// <summary>
            /// The modified clip.
            /// </summary>
            private AnimationClip modClip;

            /// <summary>
            /// the operation to apply on clip.
            /// </summary>
            private int selectedOperation;

            /// <summary>
            /// the operation to apply parameter
            /// </summary>
            private Vector3 opParams;

            /// <summary>
            /// the current step in the wizard.
            /// </summary>
            private int currentStep;

            #endregion

            #region Statics >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

            /// <summary>
            /// open Anima editor.
            /// </summary>
            [MenuItem(Menu_EDITOR_MENU + "Anima Editor Utils/Clip Editor")]
            public static void OpenEditor()
            {
                var window = GetWindow<AnimationEdition>(true);
                window.Show();
            }

            #endregion

            #region Methods >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

            /// <summary>
            /// save
            /// </summary>
            public void SaveClip()
            {
                modClip.name = sourceClip.name + "_mod";
                string path = string.Join("/", PulseEngineMgr.Path_GAMERESSOURCES, ModuleConstants.AssetsPath + "/AnimMods/" + modClip.name + ".anim");
                AssetDatabase.CreateAsset(modClip, path);
                AssetDatabase.SaveAssets();
            }

            /// <summary>
            /// reverse key frames order
            /// </summary>
            public void ReverseClip()
            {
                if (!sourceClip)
                    return;
                if (sourceClip != null)
                {
                    modClip = UnityEngine.Object.Instantiate<AnimationClip>(sourceClip);
                    modClip.ClearCurves();
                    foreach (var binding in AnimationUtility.GetObjectReferenceCurveBindings(sourceClip))
                    {
                        ObjectReferenceKeyframe[] keyframes = AnimationUtility.GetObjectReferenceCurve(sourceClip, binding);
                        ObjectReferenceKeyframe[] reversedFrames = new ObjectReferenceKeyframe[keyframes.Length];
                        for(int i = 0, len = keyframes.Length; i < len; i++)
                        {
                            reversedFrames[i] = new ObjectReferenceKeyframe { time = keyframes[i].time, value = keyframes[len - (1 + i)].value };
                        }
                        AnimationUtility.SetObjectReferenceCurve(modClip, binding, reversedFrames);
                    }
                }
            }

            /// <summary>
            /// scale clip lenght
            /// </summary>
            /// <param name="factor"></param>
            public void ScaleClip(float factor)
            {

            }

            #endregion

            #region GUI >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

            protected override void OnRedraw()
            {
                switch (currentStep)
                {
                    case 0:
                        GroupGUI(() =>
                        {
                            sourceClip = EditorGUILayout.ObjectField("Source clip", sourceClip, typeof(AnimationClip), false) as AnimationClip;
                            if(sourceClip != null)
                            {
                                if (GUILayout.Button("Next"))
                                    currentStep++;
                            }
                        },"Select Clip");
                        break;
                    case 1:
                        GroupGUI(() =>
                        {
                            selectedOperation = EditorGUILayout.Popup(selectedOperation, new[] { "Reverse", "Ajust lenght" });
                            if (selectedOperation == 1)
                            {
                                opParams.x = EditorGUILayout.FloatField("scale factor", opParams.x);
                                opParams.x = Mathf.Clamp(opParams.x, 0.1f, opParams.x);
                            }
                            GUILayout.BeginHorizontal();
                            if (GUILayout.Button("Previous"))
                            {
                                modClip = null;
                                currentStep--;
                            }
                            GUILayout.Space(10);
                            if (GUILayout.Button("Proceed"))
                            {
                                switch (selectedOperation)
                                {
                                    case 0:
                                        ReverseClip();
                                        break;
                                    case 1:
                                        ScaleClip(opParams.x);
                                        break;
                                }
                                currentStep++;
                            }
                            GUILayout.EndHorizontal();
                        },"Select Clip");
                        break;
                    case 2:
                        GUILayout.BeginHorizontal();
                        if (GUILayout.Button("Previous"))
                        {
                            modClip = null;
                            currentStep--;
                        }
                        GUILayout.Space(10);
                        if (GUILayout.Button("Save"))
                        {
                            SaveClip();
                        }
                        GUILayout.EndHorizontal();
                        break;
                }
            }

            #endregion
        }

        #endregion
    }
}