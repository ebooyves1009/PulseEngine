using System;
using System.Collections.Generic;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using System.Threading.Tasks;
using UnityEngine;
using System.Reflection;
using System.Threading;
using Unity.Collections;
using Unity.Jobs;
using Unity.Mathematics;
using Unity.Burst;


//TODO: Les Valeurs et fonctions globales seront ajoutees au fur et a mesure.

namespace PulseEngine
{
    #region Core >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// Les constantes et variables globales du pulse.
    /// </summary>
    public static class Core
    {
        #region Constants ###########################################################################

        /// <summary>
        /// Le repertoire ou seront stockes et d'ou seront charges les Assets du jeu in game et dans l'editeur. il sera Addressable.
        /// </summary>
        public const string Path_GAMERESSOURCES = "Assets/GameResources";

        /// <summary>
        /// La langue actuellement selectionnee dans le jeu.
        /// </summary>
        public static Languages currentLanguage;

        /// <summary>
        /// Le scope actuellement selectionnee dans L'editeur.
        /// </summary>
        public static Scopes currentScope;

        /// <summary>
        /// The correspondance between module types and enum at runtime, used by refelction.
        /// </summary>
        //public static Dictionary<ModulesManagers, Type> ManagersCache = new Dictionary<ModulesManagers, Type>();

        /// <summary>
        /// switch debug on or off.
        /// </summary>
        public static bool DebugMode = true;

        #endregion

        #region Methods ###########################################################################

        /// <summary>
        /// To reinitailize on domain reload.
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        public static void OnDomainReload()
        {

        }

        #endregion

        #region Jobs ###########################################################################

        /// <summary>
        /// A job to find items in an array
        /// </summary>
        /// <typeparam name="T"></typeparam>
        [BurstCompile]
        public struct FindIndexJob : IJobParallelFor
        {
            [ReadOnly]
            public NativeArray<PathNode> Collection;
            [ReadOnly]
            public NativeArray<PathNode> Items;
            [WriteOnly]
            public NativeArray<int2> Results;

            public void Execute(int index)
            {
                for(int i = 0; i < Items.Length; i++)
                {
                    if (Items[i].Equals(Collection[index]))
                    {
                        Results[index] = new int2(i, index);
                        break;
                    }
                }
            }
        }

        #endregion

        #region HelperMethods ###########################################################################

        /// <summary>
        /// Copie par valeur un scriptable.
        /// </summary>
        /// <returns></returns>
        public static Q LibraryClone<Q>(Q original) where Q : ScriptableObject
        {
            Q newOne = ScriptableObject.Instantiate(original);
            return newOne;
        }

        /// <summary>
        /// Copie par valeur un objet
        /// </summary>
        /// <returns></returns>
        public static T ObjectClone<T>(T original) where T : new()
        {

            if (!typeof(T).IsSerializable)
            {
                //throw new ArgumentException("The type must be serializable.", "source");
                return default(T);
            }

            // Don't serialize a null object, simply return the default for that object
            if (object.ReferenceEquals(original, null))
            {
                return default(T);
            }

            T nouvo = default;

            try
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BinaryFormatter bf = new BinaryFormatter();
                    bf.Context = new System.Runtime.Serialization.StreamingContext(System.Runtime.Serialization.StreamingContextStates.Clone);
                    bf.Serialize(ms, original);
                    ms.Position = 0;
                    nouvo = (T)bf.Deserialize(ms);
                }
            }
            catch
            {
                try
                {
                    string sourceJson = JsonUtility.ToJson(original);
                    nouvo = JsonUtility.FromJson<T>(sourceJson);
                }
                catch { }
            }

            return nouvo;
        }

        /// <summary>
        /// Wait until the predicate is true, or the timer goes on.
        /// </summary>
        /// <param name="predication"></param>
        /// <param name="ct"></param>
        /// <returns></returns>
        public static async Task WaitPredicate(Func<bool> predication, CancellationToken ct, int waitMillisec = -1)
        {
            DateTime startTime = DateTime.Now;
            double elapsedtime = 0;
            bool chrono = waitMillisec > 0;
            int cycles = int.MaxValue;
            for (int i = 0; i < cycles; i++)
            {
                if (ct.IsCancellationRequested)
                    break;
                if (predication.Invoke())
                {
                    break;
                }
                if (elapsedtime >= waitMillisec && chrono)
                {
                    break;
                }
                await Task.Yield();
                elapsedtime = (DateTime.Now - startTime).TotalMilliseconds;
            }
        }

        /// <summary>
        /// Execute a manager method async at runtime.
        /// </summary>
        /// <param name="mgrEnum"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task<T> ManagerAsyncMethod<T>(string methodName, params object[] parameters)
        {
            throw new NotImplementedException();
        //    if (ManagersCache == null)
        //        ManagersCache = new Dictionary<ModulesManagers, Type>();
        //    if (!ManagersCache.ContainsKey(mgrEnum))
        //    {
        //        string classPath = mgrEnum.ToString() + "." + mgrEnum.ToString();
        //        Type Mgrclass = Type.GetType("PulseEngine.Modules." + classPath);
        //        if (Mgrclass == null)
        //        {
        //            //TODO: Remove
        //            PulseDebug.Log("null Manager at " + classPath);
        //            return default;
        //        }
        //        ManagersCache.Add(mgrEnum, Mgrclass);
        //    }
        //    if (ManagersCache[mgrEnum] != null)
        //    {
        //        var Method = MethodFromClass(ManagersCache[mgrEnum], methodName);
        //        if (Method == null)
        //        {
        //            //TODO: Remove
        //            PulseDebug.Log("Null method");
        //            return default;
        //        }
        //        //TODO: Remove
        //        PulseDebug.Log("Method infos summary\n" +
        //            "name: " + Method.Name + "\n" +
        //            "is static: " + Method.IsStatic + "\n" +
        //            "returning: " + Method.ReturnType);
        //        try
        //        {
        //            Task<T> task = (Task<T>)Method.Invoke(null, parameters);
        //            //TODO: Remove
        //            PulseDebug.Log("its task of type " + typeof(T));
        //            await task.ConfigureAwait(false);
        //            //TODO: Remove
        //            PulseDebug.Log("task result is " + task.Result);
        //            T result = task.Result;
        //            return result;
        //        }
        //        catch (Exception e)
        //        {
        //            if (e.GetType() == typeof(InvalidCastException))
        //            {
        //                try
        //                {
        //                    T result = (T)Method.Invoke(null, parameters);
        //                    //TODO: Remove
        //                    PulseDebug.Log("its normal method");
        //                    return result;
        //                }
        //                catch (Exception r)
        //                {
        //                    //TODO: Remove
        //                    PulseDebug.Log("second exception occured : " + r.Message);
        //                    return default;
        //                }
        //            }
        //            //TODO: Remove
        //            PulseDebug.Log("exception occured but it's not an invalid cast. it's " + e + " || " + e.Message);
        //            return default;
        //        }
        //    }
        //    //TODO: Remove
        //    PulseDebug.Log("Null type in Core Manager cache");
        //    return default;
        }

        /// <summary>
        /// Execute a manager void method async at runtime.
        /// </summary>
        /// <param name="mgrEnum"></param>
        /// <param name="methodName"></param>
        /// <param name="parameters"></param>
        /// <returns></returns>
        public static async Task ManagerAsyncMethod(string methodName, params object[] parameters)
        {
            throw new NotImplementedException();
            //if (ManagersCache == null)
            //    ManagersCache = new Dictionary<ModulesManagers, Type>();
            //if (!ManagersCache.ContainsKey(mgrEnum))
            //{
            //    string classPath = mgrEnum.ToString() + "." + mgrEnum.ToString();
            //    Type Mgrclass = Type.GetType("PulseEngine.Modules." + classPath);
            //    if (Mgrclass == null)
            //    {
            //        return;
            //    }
            //    ManagersCache.Add(mgrEnum, Mgrclass);
            //}
            //if (ManagersCache[mgrEnum] != null)
            //{
            //    var Method = MethodFromClass(ManagersCache[mgrEnum], methodName);
            //    if (Method == null)
            //    {
            //        return;
            //    }
            //    try
            //    {
            //        Task task = (Task)Method.Invoke(null, parameters);
            //        await task.ConfigureAwait(false);
            //        return;
            //    }
            //    catch (Exception e)
            //    {
            //        if (e.GetType() == typeof(InvalidCastException))
            //        {
            //            try
            //            {
            //                Method.Invoke(null, parameters);
            //                //TODO: Remove
            //                PulseDebug.Log("its normal method");
            //                return;
            //            }
            //            catch (Exception r)
            //            {
            //                throw r;
            //            }
            //        }
            //        throw e;
            //    }
            //}
            ////TODO: Remove
            //PulseDebug.Log("Null type in Core Manager cache");
            //return;
        }

        /// <summary>
        /// Get a method from a class by reflection.
        /// </summary>
        /// <param name="t"></param>
        /// <param name="methodName"></param>
        /// <returns></returns>
        public static MethodInfo MethodFromClass(Type t, string methodName)
        {
            if (t == null)
                return null;
            MethodInfo i = t.GetMethod(methodName);
            return i;
        }

        /// <summary>
        /// Make a cubic interpolation between two positions.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector2 CubicLerp(Vector2 a, Vector2 b, Vector2 c, float t)
        {
            Vector2 p1 = Vector2.Lerp(a, b, t);
            Vector2 p2 = Vector2.Lerp(b, c, t);
            return Vector2.Lerp(p1, p2, t);
        }

        /// <summary>
        /// Make a Quadratic interpolation between two positions.
        /// </summary>
        /// <param name="a"></param>
        /// <param name="b"></param>
        /// <param name="c"></param>
        /// <param name="t"></param>
        /// <returns></returns>
        public static Vector2 Quadratic(Vector2 a, Vector2 b, Vector2 c, Vector2 d, float t)
        {
            Vector2 p1 = CubicLerp(a, b, c, t);
            Vector2 p2 = CubicLerp(b, c, d, t);
            return Vector2.Lerp(p1, p2, t);
        }

        /// <summary>
        /// Check if the index is in the list.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool IndexInCollection<T>(this List<T> collection, int index)
        {
            if (index >= 0 && index < collection.Count)
                return true;
            return false;
        }

        /// <summary>
        /// Check if index is in the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public static bool IndexInCollection<T>(this T[] collection, int index)
        {
            if (index >= 0 && index < collection.Length)
                return true;
            return false;
        }

        /// <summary>
        /// Return the corresponding egde's mid-point of a rect.
        /// </summary>
        /// <param name="_side">The specicied node edge</param>
        /// <returns></returns>
        public static Vector2[] GetNodeEdge(this IEditorNode node, NodeEdgeSide _side, float normalLenght = 100)
        {
            Vector2[] output = new Vector2[2];
            float normalValue = normalLenght;
            Vector2 point = node.NodeShape.center;
            Vector2 normal = node.NodeShape.center;
            switch (_side)
            {
                case NodeEdgeSide.upper:
                    point = node.NodeShape.center - Vector2.up * (node.NodeShape.height * 0.5f);
                    normal = point - Vector2.up * normalValue;
                    break;
                case NodeEdgeSide.lower:
                    point = node.NodeShape.center + Vector2.up * (node.NodeShape.height * 0.5f);
                    normal = point + Vector2.up * normalValue;
                    break;
                case NodeEdgeSide.lefty:
                    point = node.NodeShape.center - Vector2.right * (node.NodeShape.width * 0.5f);
                    normal = point - Vector2.right * normalValue;
                    break;
                case NodeEdgeSide.righty:
                    point = node.NodeShape.center + Vector2.right * (node.NodeShape.width * 0.5f);
                    normal = point + Vector2.right * normalValue;
                    break;
            }
            output[0] = point;
            output[1] = normal;
            return output;
        }

        /// <summary>
        /// Get the closest node's Edge
        /// </summary>
        /// <param name="_point">the point from wich calculation begins</param>
        /// <param name="_nodeRect">the node's rect where to find edge</param>
        /// <returns></returns>
        public static Vector2[] GetClosestNodeEdge(this IEditorNode node, Vector2 _point, Rect _nodeRect)
        {
            List<Vector2[]> sides = new List<Vector2[]>();
            float normal = Vector2.Distance(node.NodeShape.center, _point) * 0.2f;
            Vector2 center = _point;
            sides.Add(GetNodeEdge(node, NodeEdgeSide.lefty, normal));
            sides.Add(GetNodeEdge(node, NodeEdgeSide.righty, normal));
            sides.Add(GetNodeEdge(node, NodeEdgeSide.upper, normal));
            sides.Add(GetNodeEdge(node, NodeEdgeSide.lower, normal));
            sides.Sort((side1, side2) => { return ((side1[0] - center).sqrMagnitude.CompareTo((side2[0] - center).sqrMagnitude)); });
            return sides[0];
        }

        /// <summary>
        /// Evaluate a basic condition.
        /// </summary>
        /// <param name="rhValue"></param>
        /// <param name="operators"></param>
        /// <param name="lhValue"></param>
        /// <returns></returns>
        public static bool ConditionsEvaluator(float rhValue, ComparaisonOperators operators, float lhValue)
        {
            switch (operators)
            {
                case ComparaisonOperators.equals:
                    return rhValue == lhValue;
                case ComparaisonOperators.notEquals:
                    return rhValue != lhValue;
                case ComparaisonOperators.greaterThan:
                    return rhValue > lhValue;
                case ComparaisonOperators.lessThan:
                    return rhValue < lhValue;
                case ComparaisonOperators.greaterOrEquals:
                    return rhValue >= lhValue;
                case ComparaisonOperators.lessOrEquals:
                    return rhValue <= lhValue;
                default:
                    return false;
            }
        }

        #endregion

        #region Extensions ###########################################################################

        /// <summary>
        /// Limit a string to a certain number of characters.
        /// </summary>
        /// <param name="str"></param>
        /// <param name="maxCharacters"></param>
        /// <returns></returns>
        public static string Limit(this string str, int maxCharacters = 40)
        {
            if (str.Length <= maxCharacters || string.IsNullOrEmpty(str))
                return str;
            int maximum = (maxCharacters > 3 ? maxCharacters - 3 : maxCharacters);
            char[] chain = new char[maxCharacters];
            for (int i = 0; i < maximum; i++) { chain[i] = str[i]; }
            if (maximum < maxCharacters)
                for (int i = maximum; i < maxCharacters; i++) { chain[i] = '.'; }
            return new string(chain);
        }

        /// <summary>
        /// Encadre un texte dans des balises Html couleur
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static string Hightlight(this string txt, Color col)
        {
            if (string.IsNullOrEmpty(txt))
                return txt;
            string htmlCol = ColorUtility.ToHtmlStringRGB(col);
            return "<color=#" + htmlCol + ">" + txt + "</color>";
        }

        /// <summary>
        /// Encadre un texte dans des balises gras
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static string Bold(this string txt)
        {
            if (string.IsNullOrEmpty(txt))
                return txt;
            return "<b>" + txt + "</b>";
        }

        /// <summary>
        /// Encadre un texte dans des balises italic
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static string Italic(this string txt)
        {
            if (string.IsNullOrEmpty(txt))
                return txt;
            return "<i>" + txt + "</i>";
        }

        /// <summary>
        /// Encadre un texte dans des balises de soulignage
        /// </summary>
        /// <param name="txt"></param>
        /// <param name="col"></param>
        /// <returns></returns>
        public static string Underline(this string txt)
        {
            if (string.IsNullOrEmpty(txt))
                return txt;
            return "<u>" + txt + "</u>";
        }

        /// <summary>
        /// Return the selected field
        /// </summary>
        /// <param name="dta"></param>
        /// <param name="fld"></param>
        /// <returns></returns>
        public static TradField GetTradField(this Localisationdata dta, DatalocationField fld)
        {
            switch (fld)
            {
                case DatalocationField.title:
                    return dta.Title;
                case DatalocationField.header:
                    return dta.Header;
                case DatalocationField.banner:
                    return dta.Banner;
                case DatalocationField.groupName:
                    return dta.GroupName;
                case DatalocationField.toolTip:
                    return dta.ToolTip;
                case DatalocationField.description:
                    return dta.Description;
                case DatalocationField.details:
                    return dta.Details;
                case DatalocationField.infos:
                    return dta.Infos;
                case DatalocationField.child1:
                    return dta.Child1;
                case DatalocationField.child2:
                    return dta.Child2;
                case DatalocationField.child3:
                    return dta.Child3;
                case DatalocationField.child4:
                    return dta.Child4;
                case DatalocationField.child5:
                    return dta.Child5;
                case DatalocationField.child6:
                    return dta.Child6;
                case DatalocationField.footPage:
                    return dta.FootPage;
                case DatalocationField.conclusion:
                    return dta.Conclusion;
                case DatalocationField.end:
                    return dta.End;
                default:
                    return dta.Title;
            }
        }

        /// <summary>
        /// Add an item to an native array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <param name="alloc"></param>
        /// <returns></returns>
        public static void Add<T>(this NativeArray<T> collection, T item, Allocator alloc) where T: struct
        {
            //NativeArray<T> newCollection = new NativeArray<T>(collection.Length + 1, Allocator.Temp);
            //newCollection.
            ////newCollection.CopyFrom(collection);
            //newCollection[newCollection.Length - 1] = item;
            //collection.Dispose();
            ////collection = new NativeArray<T>(newCollection, alloc);
            //newCollection.Dispose();
        }

        /// <summary>
        /// Add an array of items that meet condition to an native array. null condition add all the array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <param name="alloc"></param>
        /// <returns></returns>
        public static NativeArray<T> AddRangeWhere<T>(this NativeArray<T> collection, NativeArray<T> itemCollection, Func<T,bool> condition, Allocator alloc) where T: struct
        {
            NativeArray<int> iterations = new NativeArray<int>(itemCollection.Length, Allocator.None);
            int k = 0;
            for(int i = 0; i < itemCollection.Length; i++)
            {
                if (condition != null)
                {
                    if (condition(itemCollection[i]))
                    {
                        iterations[k] = i;
                        k++;
                    }
                }
            }
            NativeArray<T> newCollection = new NativeArray<T>(collection.Length + k, alloc);
            newCollection.CopyFrom(collection);
            int j = 0;
            for (int i = 0; i < k; i++)
            {
                if (condition != null)
                {
                    if (condition(itemCollection[iterations[i]]))
                    {
                        newCollection[newCollection.Length - 1 + i] = itemCollection[iterations[i]];
                    }
                }
            }
            iterations.Dispose();
            collection.Dispose();
            return newCollection;
        }

        /// <summary>
        /// Remove an item to from native array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <param name="alloc"></param>
        /// <returns></returns>
        public static NativeArray<T> Remove<T>(this NativeArray<T> collection, T item, Allocator alloc) where T: struct, IEquatable<T>
        {
            bool found = false;
            int k = 0;
            NativeArray<T> newCollection = new NativeArray<T>(collection.Length - 1, alloc);
            for(int i = 0; i < collection.Length; i++)
            {
                if (collection[i].Equals(item) && !found)
                {
                    found = true;
                    continue;
                }
                if(newCollection.Length > k)
                    newCollection[k] = collection[i];
                k++;
            }
            if (!found)
            {
                newCollection.Dispose();
                return collection;
            }
            collection.Dispose();
            return newCollection;
        }

        /// <summary>
        /// Remove an item to at index in an native array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <param name="alloc"></param>
        /// <returns></returns>
        public static NativeArray<T> RemoveAt<T>(this NativeArray<T> collection, int index, Allocator alloc) where T: struct
        {
            int k = 0;
            NativeArray<T> newCollection = new NativeArray<T>(collection.Length - 1, alloc);
            for (int i = 0; i < collection.Length; i++)
            {
                if (i == index)
                {
                    continue;
                }
                if (newCollection.Length > k)
                    newCollection[k] = collection[i];
                k++;
            }
            collection.Dispose();
            return newCollection;
        }

        /// <summary>
        /// Return an item's index in native array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <param name="alloc"></param>
        /// <returns></returns>
        public static int IndexOfNoAlloc<T>(this NativeArray<T> collection, T item) where T : struct, IEquatable<T>
        {
            int k = -1;
            for (int i = 0; i < collection.Length; i++)
            {
                if (collection[i].Equals(item))
                {
                    k = i;
                    break;
                }
            }
            return k;
        }


        /// <summary>
        /// Return an item's index in native array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <param name="alloc"></param>
        /// <returns></returns>
        public static int ArrayIndexOf<T>(this NativeArray<T> collection, T item) where T : struct, IEquatable<T>
        {
            int k = -1;
            for (int i = 0; i < collection.Length; i++)
            {
                if (collection[i].Equals(item))
                {
                    k = i;
                    break;
                }
            }
            return k;
        }

        /// <summary>
        /// Sort an native array.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="collection"></param>
        /// <param name="item"></param>
        /// <param name="alloc"></param>
        /// <returns></returns>
        public static void Sort<T>(this NativeArray<T> collection, Allocator alloc) where T : struct, IComparable<T>
        {
            T item;
            for (int i = 0; i < collection.Length; i++)
            {
                item = collection[i];
                for(int j = 0; j < collection.Length; j++)
                {
                    if(item.CompareTo(collection[j]) < 0)
                    {
                        collection[i] = collection[j];
                        collection[j] = item;
                        item = collection[i];
                    }
                }
            }
        }

        #endregion
    }

    #endregion
}