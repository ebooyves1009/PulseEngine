using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using System.Linq;
using PulseEngine.Datas;


//TODO: Continuer d'implementer des fonction au fil des besoins du module, sans oublier les fonction d'access aux datas inGame.
namespace PulseEngine.Modules.Localisator
{
    /// <summary>
    /// Le Manager du Module Localisator.
    /// </summary>
    public static class LocalisationManager
    {

        #region Static Attributes and properties #########################################

        /// <summary>
        /// Le chemin d'access des datas.
        /// </summary>
        public static string AssetsPath { get => "LocalisationDatas"; }

        #endregion

        #region Methods ##################################################################

        /// <summary>
        /// Retourne la data textuelle correspondante aux info renseignes dans la BD, ou revoi null.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_tradDataType"></param>
        /// <param name="_langage"></param>
        /// <returns></returns>
        public static async Task<string> TextData(int _id, DatalocationField _field, TradDataTypes _tradDataType, Languages _langage)
        {
            string result = string.Empty;
            LocalisationLibrary handle = null;
            Localisationdata data = null;
            string key = "Localisator_" + LanguageConverter(_langage) + "_" + _tradDataType.ToString();
            var locationsIlist = await Addressables.LoadResourceLocationsAsync(key).Task;
            if (locationsIlist == null || locationsIlist.Count <= 0)
                return string.Empty;
            var location = locationsIlist.FirstOrDefault();
            if(location != null)
            {
                handle = await Addressables.LoadAssetAsync<LocalisationLibrary>(location.PrimaryKey).Task;
            }
            if (handle)
            {
                data = handle.DatasList.Find(d => { return d.ID == _id; });
            }
            if (data == null)
                return string.Empty;

            switch (_field)
            {
                case DatalocationField.title:
                    result = data.Title.textField;
                    break;
                case DatalocationField.header:
                    result = data.Header.textField;
                    break;
                case DatalocationField.banner:
                    result = data.Banner.textField;
                    break;
                case DatalocationField.groupName:
                    result = data.GroupName.textField;
                    break;
                case DatalocationField.toolTip:
                    result = data.ToolTip.textField;
                    break;
                case DatalocationField.description:
                    result = data.Description.textField;
                    break;
                case DatalocationField.details:
                    result = data.Details.textField;
                    break;
                case DatalocationField.infos:
                    result = data.Infos.textField;
                    break;
                case DatalocationField.child1:
                    result = data.Child1.textField;
                    break;
                case DatalocationField.child2:
                    result = data.Child2.textField;
                    break;
                case DatalocationField.child3:
                    result = data.Child3.textField;
                    break;
                case DatalocationField.child4:
                    result = data.Child4.textField;
                    break;
                case DatalocationField.child5:
                    result = data.Child5.textField;
                    break;
                case DatalocationField.child6:
                    result = data.Child6.textField;
                    break;
                case DatalocationField.footPage:
                    result = data.FootPage.textField;
                    break;
                case DatalocationField.conclusion:
                    result = data.Conclusion.textField;
                    break;
                case DatalocationField.end:
                    result = data.End.textField;
                    break;
            }

            var parts = result.Split(' ');
            result = string.Empty;
            for (int i = 0; i < parts.Length; i++)
            {
                var p = parts[i];
                if (p.Length > 1 && p[0] == '#' && p[p.Length - 1] == '#')
                {
                    var code = p.Split('#')[1];
                    var subParts = code.Split('_');
                    if(subParts.Length > 1)
                    {
                        int id = 0;
                        int type = 0;
                        if (int.TryParse(subParts[0], out id) && int.TryParse(subParts[1], out type))
                        {
                            string subkey = "Localisator_" + LanguageConverter(_langage) + "_" + ((TradDataTypes)type).ToString();
                            var sublocationsIlist = await Addressables.LoadResourceLocationsAsync(subkey).Task;
                            if (locationsIlist == null || locationsIlist.Count <= 0)
                                continue;
                            var sublocation = sublocationsIlist.FirstOrDefault();
                            if (sublocation != null)
                            {
                                var subHandle = await Addressables.LoadAssetAsync<LocalisationLibrary>(location.PrimaryKey).Task;
                                if (subHandle)
                                {
                                    var subData = subHandle.DatasList.Find(d => { return d.ID == id; });
                                    if(subData != null)
                                        parts[i] = subData.Title.textField;
                                }
                            }
                        }
                    }
                }
            }
            return string.Join(" ",parts);
        }

        /// <summary>
        /// Retourne la data vocale correspondante aux info renseignes dans la BD, ou revoi null.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_tradDataType"></param>
        /// <param name="_langage"></param>
        /// <returns></returns>
        public static async Task<AudioClip> AudioData(int _id, DatalocationField _field, TradDataTypes _tradDataType, Languages _langage)
        {
            AudioClip result = null;
            LocalisationLibrary handle = null;
            Localisationdata data = null;
            string key = "Localisator_" + LanguageConverter(_langage) + "_" + _tradDataType.ToString();
            var locationsIlist = await Addressables.LoadResourceLocationsAsync(key).Task;
            if (locationsIlist == null || locationsIlist.Count <= 0)
                return null;
            var location = locationsIlist.FirstOrDefault();
            if(location != null)
            {
                handle = await Addressables.LoadAssetAsync<LocalisationLibrary>(location.PrimaryKey).Task;
            }
            if (handle)
            {
                data = handle.DatasList.Find(d => { return d.ID == _id; });
            }
            if (data == null)
                return null;

            switch (_field)
            {
                case DatalocationField.title:
                    result = data.Title.audioField;
                    break;
                case DatalocationField.header:
                    result = data.Header.audioField;
                    break;
                case DatalocationField.banner:
                    result = data.Banner.audioField;
                    break;
                case DatalocationField.groupName:
                    result = data.GroupName.audioField;
                    break;
                case DatalocationField.toolTip:
                    result = data.ToolTip.audioField;
                    break;
                case DatalocationField.description:
                    result = data.Description.audioField;
                    break;
                case DatalocationField.details:
                    result = data.Details.audioField;
                    break;
                case DatalocationField.infos:
                    result = data.Infos.audioField;
                    break;
                case DatalocationField.child1:
                    result = data.Child1.audioField;
                    break;
                case DatalocationField.child2:
                    result = data.Child2.audioField;
                    break;
                case DatalocationField.child3:
                    result = data.Child3.audioField;
                    break;
                case DatalocationField.child4:
                    result = data.Child4.audioField;
                    break;
                case DatalocationField.child5:
                    result = data.Child5.audioField;
                    break;
                case DatalocationField.child6:
                    result = data.Child6.audioField;
                    break;
                case DatalocationField.footPage:
                    result = data.FootPage.audioField;
                    break;
                case DatalocationField.conclusion:
                    result = data.Conclusion.audioField;
                    break;
                case DatalocationField.end:
                    result = data.End.audioField;
                    break;
            }
            return result;
        }

        /// <summary>
        /// Retourne la data Image correspondante aux info renseignes dans la BD, ou revoi null.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_tradDataType"></param>
        /// <param name="_langage"></param>
        /// <returns></returns>
        public static async Task<Sprite> ImageData(int _id, DatalocationField _field, TradDataTypes _tradDataType, Languages _langage)
        {
            Sprite result = null;
            LocalisationLibrary handle = null;
            Localisationdata data = null;
            string key = "Localisator_" + LanguageConverter(_langage) + "_" + _tradDataType.ToString();
            var locationsIlist = await Addressables.LoadResourceLocationsAsync(key).Task;
            if (locationsIlist == null || locationsIlist.Count <= 0)
                return null;
            var location = locationsIlist.FirstOrDefault();
            if (location != null)
            {
                handle = await Addressables.LoadAssetAsync<LocalisationLibrary>(location.PrimaryKey).Task;
            }
            if (handle)
            {
                data = handle.DatasList.Find(d => { return d.ID == _id; });
            }
            if (data == null)
                return null;

            switch (_field)
            {
                case DatalocationField.title:
                    result = data.Title.imageField;
                    break;
                case DatalocationField.header:
                    result = data.Header.imageField;
                    break;
                case DatalocationField.banner:
                    result = data.Banner.imageField;
                    break;
                case DatalocationField.groupName:
                    result = data.GroupName.imageField;
                    break;
                case DatalocationField.toolTip:
                    result = data.ToolTip.imageField;
                    break;
                case DatalocationField.description:
                    result = data.Description.imageField;
                    break;
                case DatalocationField.details:
                    result = data.Details.imageField;
                    break;
                case DatalocationField.infos:
                    result = data.Infos.imageField;
                    break;
                case DatalocationField.child1:
                    result = data.Child1.imageField;
                    break;
                case DatalocationField.child2:
                    result = data.Child2.imageField;
                    break;
                case DatalocationField.child3:
                    result = data.Child3.imageField;
                    break;
                case DatalocationField.child4:
                    result = data.Child4.imageField;
                    break;
                case DatalocationField.child5:
                    result = data.Child5.imageField;
                    break;
                case DatalocationField.child6:
                    result = data.Child6.imageField;
                    break;
                case DatalocationField.footPage:
                    result = data.FootPage.imageField;
                    break;
                case DatalocationField.conclusion:
                    result = data.Conclusion.imageField;
                    break;
                case DatalocationField.end:
                    result = data.End.imageField;
                    break;
            }
            return result;
        }

        #endregion

        #region Helpers ################################################################

        /// <summary>
        /// Converti une langue en code de Langue.
        /// </summary>
        /// <param name="langue">La langue source.</param>
        /// <returns></returns>
        public static string LanguageConverter(Languages langue)
        {
            switch (langue)
            {
                case Languages.English:
                    return "EN";
                default:
                    return "FR";
            }
        }

        #endregion

    }

}
