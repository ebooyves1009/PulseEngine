using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEditor;
using PulseEngine.Module.StatHandler;
using UnityEditor;
using PulseEditor.Globals;
using PulseEngine.Globals;
using PulseEngine.Modules;
using PulseEngine.Modules.StatHandler;
using System;

namespace PulseEditor.Modules.StatHandler
{
    /// <summary>
    /// L'editeur de stats.
    /// </summary>
    public class StatEditor : PulseEditorMgr
    {
        #region Fonctionnal Attributes ################################################################

        /// <summary>
        /// La stat en cours de modification.
        /// </summary>
        private StatData data;

        #endregion
        #region Visual Attributes ################################################################



        #endregion
        #region Fonctionnal Methods ################################################################

        #endregion
        #region Visual Methods ################################################################

        /// <summary>
        /// Open the stat window.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="winName"></param>
        public static void OpenStatWindow(StatData data, Action<object> returnedStat, string winName = "")
        {
            if (data == null)
                return;
            var window = GetWindow<StatEditor>(true, winName);
            if (returnedStat != null)
            {
                window.onSelectionEvent += (obj, arg) =>
                {
                    returnedStat.Invoke(obj);
                };
            }
            window.editedData = data;
            window.data = PulseEngineMgr.DeepCopy(data);
            window.ShowAuxWindow();
        }

        #endregion
        #region Common Windows ################################################################

        /// <summary>
        /// Refresh.
        /// </summary>
        protected override void OnRedraw()
        {
            base.OnRedraw();
            if (data == null)
                return;
            ScrollablePanel(() =>
            {
                GroupGUI(() =>
                {
                    GUILayout.BeginVertical();

                    //Sante
                    EditorGUILayout.LabelField("Specs", style_label);
                    data.Age = EditorGUILayout.FloatField("Age: ", data.Age);
                    data.SanteMax = EditorGUILayout.FloatField("Sante Maximale: ", data.SanteMax);
                    data.Sante = EditorGUILayout.FloatField("Sante: ", data.Sante);
                    data.EnduranceMax = EditorGUILayout.FloatField("EnduranceMax: ", data.EnduranceMax);
                    data.Endurance = EditorGUILayout.FloatField("Endurance: ", data.Endurance);
                    data.SouffleMax = EditorGUILayout.FloatField("SouffleMax: ", data.SouffleMax);
                    data.Souffle = EditorGUILayout.FloatField("Souffle: ", data.Souffle);
                    data.Masse = EditorGUILayout.FloatField("Masse: ", data.Masse);
                    data.Taille = EditorGUILayout.FloatField("Taille: ", data.Taille);
                    GUILayout.Space(5);

                    //Capacites
                    EditorGUILayout.LabelField("Abilities", style_label);
                    data.Force = EditorGUILayout.FloatField("Force: ", data.Force);
                    data.Intelligence = EditorGUILayout.FloatField("Intelligence: ", data.Intelligence);
                    data.Sagesse = EditorGUILayout.FloatField("Sagesse: ", data.Sagesse);
                    data.Dexterite = EditorGUILayout.FloatField("Dexterite: ", data.Dexterite);
                    GUILayout.Space(5);

                    //Bahaviour
                    EditorGUILayout.LabelField("Behaviours", style_label);
                    data.Fierte = EditorGUILayout.FloatField("Fierte: ", data.Fierte);
                    data.Engoument = EditorGUILayout.FloatField("Engoument: ", data.Engoument);
                    data.Karma = EditorGUILayout.FloatField("Karma: ", data.Karma);
                    data.Paranormal = EditorGUILayout.FloatField("Mana: ", data.Paranormal);
                    GUILayout.Space(5);

                    GUILayout.EndVertical();
                }, "Stats");

                SaveCancelPanel(new[] {
                        new KeyValuePair<string, System.Action> ( "Save", () => {
                            if(onSelectionEvent != null)
                                onSelectionEvent.Invoke(data, null);
                            Close(); } ),
                        new KeyValuePair<string, System.Action>("Cancel", () => { Close(); })
                    });
            });
        }

        #endregion
        #region Helpers & Tools ################################################################

        #endregion
    }
}