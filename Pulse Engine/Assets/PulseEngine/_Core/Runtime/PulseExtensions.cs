using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public static class PulseExtensions
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
    /// Check if an index is not out of range.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static bool IsInRange<T>(this T[] collection, int index) => collection != null && index >= 0 && index < collection.Length;

    /// <summary>
    /// Check if an index is not out of range.
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="index"></param>
    /// <returns></returns>
    public static bool IsInRange<T>(this List<T> collection, int index) => collection != null && index >= 0 && index < collection.Count;

    /// <summary>
    /// Return a collection of Type T based on a collection of type Q
    /// </summary>
    /// <typeparam name="Q"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static T[] CollectionOf<Q, T>(this Q[] collection, Func<Q, T> selector)
    {
        if (collection == null || selector == null)
            return null;
        T[] retCol = new T[collection.Length];
        for (int i = 0; i < collection.Length; i++)
        {
            retCol[i] = selector.Invoke(collection[i]);
        }
        return retCol;
    }

    /// <summary>
    /// Return a collection of Type T based on a collection of type Q
    /// </summary>
    /// <typeparam name="Q"></typeparam>
    /// <typeparam name="T"></typeparam>
    /// <param name="collection"></param>
    /// <param name="selector"></param>
    /// <returns></returns>
    public static List<T> CollectionOf<Q, T>(this List<Q> collection, Func<Q, T> selector)
    {
        if (collection == null || selector == null)
            return null;
        List<T> retCol = new List<T>();
        for (int i = 0; i < collection.Count; i++)
        {
            retCol.Add(selector.Invoke(collection[i]));
        }
        return retCol;
    }

    /// <summary>
    /// Is this object in the interval formed by min and max?
    /// </summary>
    /// <typeparam name="T"></typeparam>
    /// <param name="obj"></param>
    /// <param name="min">the minimal value</param>
    /// <param name="max">the maximum value</param>
    /// <param name="openMin">min comparator is open?</param>
    /// <param name="openMax">max comparator is open?</param>
    /// <returns></returns>
    public static bool InInterval<T>(this T obj, T min, T max, bool openMin = false, bool openMax = true) where T: IComparable<T>
    {
        bool minConditionMeet = openMin? obj.CompareTo(min) > 0 : obj.CompareTo(min) >= 0;
        bool maxConditionMeet = openMax? obj.CompareTo(max) < 0 : obj.CompareTo(max) <= 0;
        return minConditionMeet && maxConditionMeet;
    }

    /// <summary>
    /// Spread the value in parts on an min-max interval and get the value at a part index
    /// </summary>
    /// <param name="value">the value to spread</param>
    /// <param name="parts">the number of parts</param>
    /// <param name="index">the desired part index</param>
    /// <param name="minWeight">the minimum value</param>
    /// <param name="maxWeight">the maximum value</param>
    /// <returns></returns>
    public static float SpreadEvenly(this float value, int parts, int index, float minWeight = 0, float maxWeight = 1)
    {
        if (index < 0 || index >= parts)
            return 0;
        float outputValue = 0;
        int previousIndex = Mathf.Clamp(index - 1, 0, parts - 1);
        int nextIndex = Mathf.Clamp(index + 1, 0, parts - 1);
        float clampedWeight = Mathf.Clamp(value, minWeight, maxWeight);
        float totalWeightInterval = maxWeight - minWeight;
        float singlePartInterval = totalWeightInterval / parts;
        float thisPartMin = minWeight + previousIndex * singlePartInterval;
        float thisPartMax = maxWeight - singlePartInterval * ((parts - 1) - nextIndex);
        float thisPartPeak = index >= (parts - 1)
            ? thisPartMax
            : (index <= 0
                ? thisPartMin
                : (thisPartMax - thisPartMin) * 0.5f + thisPartMin);
        if (clampedWeight.InInterval(thisPartMin, thisPartPeak))
        {
            outputValue = Mathf.InverseLerp(thisPartMin, thisPartPeak, clampedWeight);
            return index <= 0 ? 1 : outputValue;
        }
        else if (clampedWeight.InInterval(thisPartPeak, thisPartMax, false, false))
        {
            outputValue = 1 - Mathf.InverseLerp(thisPartPeak, thisPartMax, clampedWeight);
            return index >= (parts - 1) ? 1 : outputValue;
        }
        return outputValue;
    }


    #endregion

    #region Private Functions #####################################################

    #endregion

}

