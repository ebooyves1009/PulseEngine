using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using UnityEditor;
using UnityEditor.AddressableAssets;
using UnityEditor.AddressableAssets.Settings;
using UnityEngine;
using UnityEngine.AddressableAssets;
using UnityEngine.ResourceManagement.AsyncOperations;
using UnityEngine.ResourceManagement.ResourceLocations;

namespace PulseEngine
{

    #region Libraries <><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><><>

    /// <summary>
    /// La classe mere des libraies d'assets
    /// </summary>
    [System.Serializable]
    public class CoreLibrary : ScriptableObject
    {

        #region Attributes >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        /// <summary>
        /// Usually the GD scope, it's the first asset filter parameter
        /// </summary>
        [SerializeField]
        protected int libraryMainLocation;

        /// <summary>
        /// Usually the zone, it's the second asset filter parameter
        /// </summary>
        [SerializeField]
        protected int librarySecLocation;

        #endregion

        #region Properties >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        public virtual List<IData> DataList { get; set; }

        #endregion

        #region Methods >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

        /// <summary>
        /// Get all module datas with specified parameters
        /// </summary>
        /// <returns></returns>
        public static async Task<List<T>> GetAllDatas<T>(CancellationToken ct) where T : IData
        {
            string keyNamePart = typeof(T).Name;
            List<string> keys = new List<string>();
            foreach (var loc in Addressables.ResourceLocators)
            {
                List<object> _keys = new List<object>(loc.Keys);
                for (int i = 0; i < _keys.Count; i++)
                {
                    var ks = _keys[i] as string;
                    if (ks == null)
                        continue;
                    if (string.IsNullOrEmpty(ks))
                        continue;
                    if (ks.Contains(keyNamePart))
                        keys.Add(ks);
                }
            }
            List<T> output = new List<T>();
            CoreLibrary library = null;
            bool waiting = false;
            int k = 0;
            for (int i = 0; i < keys.Count; i++)
            {
                k = i;
                try
                {
                    waiting = true;
                    Addressables.LoadAssetAsync<CoreLibrary>(keys[k]).Completed += hdl =>
                    {
                        waiting = false;
                        if (hdl.Status == AsyncOperationStatus.Succeeded)
                            library = hdl.Result;
                    };
                }
                catch (Exception e)
                {
                    if (e.GetType() == typeof(UnityEngine.UnityException))
                    {
                        waiting = true;
                        Addressables.InitializeAsync().Completed += hdl =>
                        {
                            waiting = false;
                        };
                        await Core.WaitPredicate(() => { return !waiting; }, ct);
                        try
                        {
                            waiting = true;
                            Addressables.LoadAssetAsync<CoreLibrary>(keys[k]).Completed += hdl =>
                            {
                                waiting = false;
                                if (hdl.Status == AsyncOperationStatus.Succeeded)
                                    library = hdl.Result;
                            };
                        }
                        catch
                        {
                            return null;
                        }
                    }
                    return null;
                }
                await Core.WaitPredicate(() => { return !waiting; }, ct, 1000);
                if (library == null)
                    continue;
                var datalist = Core.LibraryClone(library).DataList;
                output.AddRange(datalist.ConvertAll(new Converter<object, T>(data => { return (T)data; })));
                library = null;
            }
            return output.Count > 0 ? output.FindAll(item => { return item != null; }) : null;
        }

        /// <summary>
        /// Get all module datas with specified parameters
        /// </summary>
        /// <returns></returns>
        public static async Task<List<T>> GetDatas<T>(DataLocation _location, CancellationToken ct) where T : IData
        {
            StringBuilder str = new StringBuilder();
            str.Append(typeof(T).Name).Append("_").Append(_location.globalLocation).Append("_").Append(_location.localLocation);
            string path = str.ToString();
            IList<IResourceLocation> location = null;
            bool seeking = false;
            try
            {
                seeking = true;
                Addressables.LoadResourceLocationsAsync(path).Completed += hdl =>
                {
                    seeking = false;
                    if (hdl.Status == AsyncOperationStatus.Succeeded)
                        location = hdl.Result;
                };
            }
            catch (Exception e)
            {
                if (e.GetType() == typeof(UnityEngine.UnityException))
                {
                    seeking = true;
                    Addressables.InitializeAsync().Completed += hdl =>
                    {
                        seeking = false;
                    };
                    await Core.WaitPredicate(() => { return !seeking; }, ct);
                    try
                    {
                        seeking = true;
                        Addressables.LoadResourceLocationsAsync(path).Completed += hdl =>
                        {
                            seeking = false;
                            if (hdl.Status == AsyncOperationStatus.Succeeded)
                                location = hdl.Result;
                        };
                    }
                    catch
                    {
                        return null;
                    }
                }
                return null;
            }
            await Core.WaitPredicate(() => { return !seeking; }, ct, 1000);

            if (location == null || location.Count <= 0)
                return null;
            var key = location[0].PrimaryKey;
            CoreLibrary library = null;
            seeking = true;
            Addressables.LoadAssetAsync<CoreLibrary>(key).Completed += hdl =>
            {
                seeking = false;
                if (hdl.Status == AsyncOperationStatus.Succeeded)
                    library = hdl.Result;
            };
            await Core.WaitPredicate(() => { return !seeking; }, ct, 1000);

            if (library == null)
                return null;
            var datalist = Core.LibraryClone(library).DataList;
            return datalist.FindAll(d => { return d != null; }).ConvertAll(new Converter<object, T>(data => { return (T)data; }));
        }

        /// <summary>
        /// Get module data with ID
        /// </summary>
        /// <returns></returns>
        public static async Task<T> GetData<T>(DataLocation _location, CancellationToken ct) where T : IData
        {
            var list = await GetDatas<T>(_location, ct);
            if (list != null)
                return list.Find(data => { return data.Location.id == _location.id; });
            return default(T);
        }

#if UNITY_EDITOR

        /// <summary>
        /// Cree l'asset.
        /// </summary>
        /// <returns></returns>
        public static bool Save<T, Q>(string assetsPath, params object[] locationFilters) where T : IData where Q : CoreLibrary
        {
            string fileName = typeof(T).Name;
            for (int i = 0; i < locationFilters.Length; i++)
            {
                fileName += "_" + (int)locationFilters[i];
            }
            fileName += ".asset";
            string path = assetsPath;
            string folderPath = string.Join("/", Core.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", Core.Path_GAMERESSOURCES, path, fileName);
            if (!AssetDatabase.IsValidFolder(Core.Path_GAMERESSOURCES))
                return false;
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(Core.Path_GAMERESSOURCES, path);
                AssetDatabase.SaveAssets();
            }
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                Q asset = ScriptableObject.CreateInstance<Q>();

                try
                {
                    asset.libraryMainLocation = locationFilters.Length > 0 ? (int)locationFilters[0] : 0;
                }
                catch (Exception e)
                {
                    if (e.GetType() != typeof(InvalidCastException))
                        throw new Exception("Unknow exeption occured when saving " + fileName);
                }
                try
                {
                    asset.librarySecLocation = locationFilters.Length > 1 ? (int)locationFilters[1] : 0;
                }
                catch (Exception e)
                {
                    if (e.GetType() != typeof(InvalidCastException))
                        throw new Exception("Unknow exeption occured when saving " + fileName);
                }
                AssetDatabase.CreateAsset(asset, fullPath);
                AssetDatabase.SaveAssets();
                //Make a gameobject an addressable
                var settings = AddressableAssetSettingsDefaultObject.Settings;
                if (settings != null)
                {
                    AddressableAssetGroup g = settings.DefaultGroup;
                    if (g != null)
                    {
                        var guid = AssetDatabase.AssetPathToGUID(fullPath);
                        //This is the function that actually makes the object addressable
                        var entry = settings.CreateOrMoveEntry(guid, g);

                        //simplify entry names
                        var parts = fullPath.Split(new[] { '/', '.' });
                        entry.SetAddress(parts[parts.Length - 2], true);

                        //You'll need these to run to save the changes!
                        settings.SetDirty(AddressableAssetSettings.ModificationEvent.EntryMoved, entry, true);
                        AssetDatabase.SaveAssets();
                    }
                }
                return true;
            }
            return false;
        }

        /// <summary>
        /// Verifie si l'asset existe.
        /// </summary>
        /// <returns></returns>
        public static bool Exist<T, Q>(string assetsPath, params object[] locationFilters) where T : IData where Q : CoreLibrary
        {
            string fileName = typeof(T).Name;
            for (int i = 0; i < locationFilters.Length; i++)
            {
                fileName += "_" + (int)locationFilters[i];
            }
            fileName += ".asset";
            string path = assetsPath;
            string folderPath = string.Join("/", Core.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", Core.Path_GAMERESSOURCES, path, fileName);
            if (!AssetDatabase.IsValidFolder(Core.Path_GAMERESSOURCES))
                return false;
            if (!AssetDatabase.IsValidFolder(folderPath))
            {
                AssetDatabase.CreateFolder(Core.Path_GAMERESSOURCES, path);
                AssetDatabase.SaveAssets();
            }
            if (AssetDatabase.IsValidFolder(folderPath))
            {
                if (AssetDatabase.LoadAssetAtPath<Q>(fullPath) == null)
                    return false;
                else
                    return true;
            }
            return false;
        }

        /// <summary>
        /// Charge l'asset.
        /// </summary>
        /// <returns></returns>
        public static Q Load<T, Q>(string assetsPath, params object[] locationFilters) where T : IData where Q : CoreLibrary
        {
            string fileName = typeof(T).Name;
            for (int i = 0; i < locationFilters.Length; i++)
            {
                fileName += "_" + (int)locationFilters[i];
            }
            fileName += ".asset";
            string path = assetsPath;
            string folderPath = string.Join("/", Core.Path_GAMERESSOURCES, path);
            string fullPath = string.Join("/", Core.Path_GAMERESSOURCES, path, fileName);
            if (Exist<T, Q>(assetsPath, locationFilters))
            {
                return AssetDatabase.LoadAssetAtPath(fullPath, typeof(Q)) as Q;
            }
            else
                return null;
        }

#endif

        #endregion
    }

    #endregion
}
