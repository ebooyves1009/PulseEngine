using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "AnimaClip", menuName = "AnimaClips", order = 1)]
public class AnimaMotion: ScriptableObject
{
    #region Constants #############################################################

    #endregion

    #region Variables #############################################################

    #endregion

    #region Statics   #############################################################

    #endregion

    #region Inner Types ###########################################################

    #endregion

    #region Properties ############################################################


    [field:SerializeField] public string Statename { get; set; }
    [field:SerializeField] public AnimationClip[] Clips { get; set; }
    [field:SerializeField] public int Priority { get; set; }
    public Action<float> UpdateAction { get; set; }

    #endregion

    #region Public Functions ######################################################

    #endregion

    #region Private Functions #####################################################

    #endregion

    #region Jobs      #############################################################

    #endregion

    #region MonoBehaviours ########################################################

    #endregion
}

