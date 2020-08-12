using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEditor;
using PulseEngine.Core;
using PulseEngine.Module.Anima;
using System;
using UnityEditor;
using PulseEngine.Module.PhysicSpace;
using PulseEngine.Module.Commands;

namespace PulseEditor.Module.Anima
{
    /// <summary>
    /// l'editeur d'animation.
    /// </summary>
    public class AnimaEditor : PulseEngine_Core_BaseEditor
    {
        #region Fonctionnal Attributes ################################################################

        /// <summary>
        /// Toutes les assets confondues
        /// </summary>
        private List<AnimaLibrary> allAsset = new List<AnimaLibrary>();

        /// <summary>
        /// l'asset correspondante au choix de category et type
        /// </summary>
        private AnimaLibrary asset;

        /// <summary>
        /// L'asset en cours d'edition.
        /// </summary>
        private AnimaLibrary editedAsset;

        /// <summary>
        /// La data en cors d'edition.
        /// </summary>
        private AnimaData dataEdited;

        /// <summary>
        /// Le temps actuel dans l'animation jouee.
        /// </summary>
        private float timeInCurrentAnimation = 0;

        /// <summary>
        /// La phase d'animation en cours.
        /// </summary>
        private AnimaManager.AnimPhase currentAnimPhase;

        /// <summary>
        /// L'event d'animation en cours.
        /// </summary>
        private CommanderManager.CommandAction currentAnimCommand;

        #endregion
        #region Visual Attributes ################################################################


        /// <summary>
        /// l'index de la data selectionne.
        /// </summary>
        private int selectedDataIdx;

        /// <summary>
        /// La categoie selectionne
        /// </summary>
        private AnimaManager.AnimaCategory selectedCategory;

        /// <summary>
        /// Le types selectionne.
        /// </summary>
        private AnimaManager.AnimationType selectedType;

        /// <summary>
        /// la fenetre de previsualisation.
        /// </summary>
        private AnimaPreview preview;

        #endregion
        #region Fonctionnal Methods ################################################################

        /// <summary>
        /// initialise toutes les assets.
        /// </summary>
        private void Initialisation()
        {
            allAsset.Clear();
            editedAsset = null;
            dataEdited = null;
            foreach (AnimaManager.AnimaCategory category in Enum.GetValues(typeof(AnimaManager.AnimaCategory)))
            {
                foreach (AnimaManager.AnimationType type in Enum.GetValues(typeof(AnimaManager.AnimationType)))
                {
                    if (AnimaLibrary.Exist(category, type))
                        allAsset.Add(AnimaLibrary.Load(category, type));
                    else if (AnimaLibrary.Create(category, type))
                        allAsset.Add(AnimaLibrary.Load(category, type));
                }
            }
            asset = allAsset.Find(ass => { return ass.AnimCategory == selectedCategory && ass.AnimType == selectedType; });
            if (asset)
            {
                editedAsset = asset;
            }
            preview = new AnimaPreview();
        }

        /// <summary>
        /// A la selection d'un item.
        /// </summary>
        private void onSelect()
        {
            throw new NotImplementedException();
        }

        #endregion
        #region Visual Methods ################################################################

        /// <summary>
        /// open Anima editor.
        /// </summary>
        [MenuItem(PulseCore_GlobalValue_Manager.Menu_EDITOR_MENU + "Anima Editor")]
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
            window.Show();
        }

        /// <summary>
        /// refraichie.
        /// </summary>
        protected override void OnRedraw()
        {
            base.OnRedraw();
            if (Header())
            {
                GUILayout.BeginHorizontal();
                ScrollablePanel(() =>
                {
                    ListAnimations(editedAsset);
                    Foot();
                },true);
                ScrollablePanel(() =>
                {
                    AnimDetails(dataEdited);
                });
                GUILayout.EndHorizontal();
            }
        }

        /// <summary>
        /// initialise.
        /// </summary>
        protected override void OnInitialize()
        {
            base.OnInitialize();
            Initialisation();
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
                selectedCategory = (AnimaManager.AnimaCategory)GUILayout.Toolbar((int)selectedCategory, Enum.GetNames(typeof(AnimaManager.AnimaCategory)));
            }, "Category", 50);
            GroupGUInoStyle(() =>
            {
                selectedType = (AnimaManager.AnimationType)GUILayout.Toolbar((int)selectedType, Enum.GetNames(typeof(AnimaManager.AnimationType)));
            }, "Type", 50);

            if (selectedCategory != (AnimaManager.AnimaCategory)categorySelect || selectedType != (AnimaManager.AnimationType)typeSelect)
            {
                Initialisation();
            }
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
            SaveBarPanel(editedAsset, asset, onSelect);
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
                    default:
                        return false;
                }
            };
            if (listCompatiblesmode())
            {
                GroupGUI(() =>
                {
                    GUILayout.BeginVertical();
                    List<GUIContent> listContent = new List<GUIContent>();
                    int maxId = 0;
                    for (int i = 0; i < library.AnimList.Count; i++)
                    {
                        var data = library.AnimList[i];
                        var name = data.Motion ? data.Motion.name : "null";
                        char[] titleChars = new char[LIST_MAX_CHARACTERS];
                        string pointDeSuspension = string.Empty;
                        try
                        {
                            if (data.ID > maxId) maxId = data.ID;
                            for (int j = 0; j < titleChars.Length; j++)
                                if (j < name.Length)
                                    titleChars[j] = name[j];
                            if (name.Length >= titleChars.Length)
                                pointDeSuspension = "...";
                        }
                        catch { }
                        string title = new string(titleChars) + pointDeSuspension;
                        listContent.Add(new GUIContent { text = data != null ? data.ID + "-" + title : "null data" });
                    }
                    selectedDataIdx = ListItems(selectedDataIdx, listContent.ToArray());
                    GUILayout.Space(5);
                    GUILayout.BeginHorizontal();
                    if (GUILayout.Button("+"))
                    {
                        library.AnimList.Add(new AnimaData { ID = maxId + 1, AnimLayer = (AnimaManager.AnimationLayer)selectedType, IsHumanMotion = selectedCategory == AnimaManager.AnimaCategory.humanoid });
                    }
                    if (selectedDataIdx >= 0 && selectedDataIdx < editedAsset.AnimList.Count)
                    {
                        if (GUILayout.Button("-"))
                        {
                            library.AnimList.RemoveAt(selectedDataIdx);
                        }
                    }
                    GUILayout.EndHorizontal();
                    GUILayout.EndVertical();
                }, "Anima Datas List");
                if (selectedDataIdx >= 0 && selectedDataIdx < editedAsset.AnimList.Count)
                    dataEdited = editedAsset.AnimList[selectedDataIdx];
                else
                    dataEdited = null;
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
                //Motion
                if (preview)
                    timeInCurrentAnimation = preview.RenderPreview(data.Motion, new Vector2(300, 250));
                var newMotion = EditorGUILayout.ObjectField("Motion ",data.Motion, typeof(AnimationClip), false) as AnimationClip;
                if(newMotion != data.Motion)
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
                //Name
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Animator Layer: " , EditorStyles.boldLabel);
                data.AnimLayer = (AnimaManager.AnimationLayer)selectedType;
                EditorGUILayout.LabelField(data.AnimLayer.ToString());
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                //Anim phases
                GUILayout.BeginHorizontal();
                int indexPhase = -1;
                for (int i = 0; i < data.PhaseAnims.Count; i++) {
                    var phase = data.PhaseAnims[i];
                    if (phase.timeStamp.time <= timeInCurrentAnimation && timeInCurrentAnimation < (phase.timeStamp.time + phase.timeStamp.duration))
                    {
                        currentAnimPhase = phase.phase;
                        indexPhase = i;
                        break;
                    }
                }
                currentAnimPhase = (AnimaManager.AnimPhase)EditorGUILayout.EnumPopup("Phase",currentAnimPhase);
                if (GUILayout.Button( indexPhase>= 0?"Change":"Add" +" animPhase "+currentAnimPhase+" at "+timeInCurrentAnimation))
                {
                    if (indexPhase >= 0)
                    {
                        var tmpPhase = data.PhaseAnims;
                        tmpPhase[indexPhase] = new AnimaManager.AnimePhaseTimeStamp
                        {
                            phase = currentAnimPhase,
                            timeStamp = new PulseCore_GlobalValue_Manager.TimeStamp { time = timeInCurrentAnimation, duration = (data.Motion ? 1 / data.Motion.frameRate : 0.1f) }
                        };
                        data.PhaseAnims = tmpPhase;
                    }
                    else
                    {
                        var tmpPhase = data.PhaseAnims;
                        tmpPhase.Add(new AnimaManager.AnimePhaseTimeStamp
                        {
                            phase = currentAnimPhase,
                            timeStamp = new PulseCore_GlobalValue_Manager.TimeStamp { time = timeInCurrentAnimation, duration = (data.Motion ? 1 / data.Motion.frameRate : 0.1f) }
                        });
                        data.PhaseAnims = tmpPhase;
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Current Anim Phase: ", EditorStyles.boldLabel);
                EditorGUILayout.LabelField(currentAnimPhase.ToString());
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                //Anim Space
                GUILayout.BeginHorizontal();
                EditorGUILayout.LabelField("Physic Place, This motion takes places in: ", EditorStyles.boldLabel);
                data.PhysicPlace = (PhysicManager.PhysicSpace)EditorGUILayout.EnumPopup(data.PhysicPlace);
                GUILayout.EndHorizontal();
                GUILayout.Space(10);
                //Anim phases
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

                if (GUILayout.Button((indexEvent >= 0 ? "Customize" : "Add") + " Event " + (indexEvent < data.EventList.Count && indexEvent >= 0? ""+data.EventList[indexEvent].command.code : "at "+timeInCurrentAnimation)))
                {
                    if (indexEvent >= 0)
                    {
                        var tmpEvents = data.EventList;
                        //tmpEvents[indexEvent] //TODO: Editor of this.
                        data.EventList = tmpEvents;
                    }
                    else
                    {
                        var tmpEvents = data.EventList;
                        tmpEvents.Add(new AnimaManager.AnimeCommand
                        {
                            command = new CommanderManager.CommandAction(),
                            timeStamp = new PulseCore_GlobalValue_Manager.TimeStamp { time = timeInCurrentAnimation, duration = data.Motion? (1 / data.Motion.frameRate) : 0 }
                        });
                        data.EventList = tmpEvents;
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(5);
                EditorGUILayout.LabelField("Events Triggereds: ", EditorStyles.boldLabel);
                GUILayout.BeginHorizontal();
                int k = 0;
                for(int i = 0; i < data.EventList.Count; i++)
                {
                    k = i;
                    var ev = data.EventList[k];
                    var time = ev.timeStamp.time;
                    var duration = ev.timeStamp.duration;
                    Color activeCol = Color.gray;
                    if (time <= timeInCurrentAnimation && timeInCurrentAnimation < (time + duration))
                    {
                        activeCol = Color.green;
                        try
                        {
                            EditorGUILayout.Knob(new Vector2(50, 50), Mathf.InverseLerp(time, time + duration, timeInCurrentAnimation) * 100, 0, 100, "% of " + ev.command.code, Color.gray, activeCol, true);
                        }
                        catch { }
                    }
                }
                GUILayout.EndHorizontal();
                GUILayout.Space(10);

            }, "Details");
        }

        private void Update()
        {
            if (dataEdited == null)
                return;
            Repaint();
        }

        #endregion
        #region Helpers & Tools ################################################################

        #endregion
    }
}