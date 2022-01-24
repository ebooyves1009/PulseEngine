using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class Tools
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

    #endregion

    #region Public Functions ######################################################

    /// <summary>
    /// Return lower or upper value depending on the comparison between value and thresholdValue.
    /// </summary>
    /// <param name="value"></param>
    /// <param name="lowerValue"></param>
    /// <param name="upperValue"></param>
    /// <param name="thresholdValue"></param>
    /// <returns></returns>
    public static float ThresholdSwithcher(float value, float lowerValue, float upperValue, float thresholdValue)
    {
        return value < thresholdValue ? lowerValue : upperValue;
    }

    #endregion

    #region Private Functions #####################################################

    #endregion

    #region Jobs      #############################################################

    #endregion
}

