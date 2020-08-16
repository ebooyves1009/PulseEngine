using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using System.Linq;
using PulseEngine.Globals;


//TODO: Continuer d'implementer des fonction au fil des besoins du module, sans oublier les fonction d'access aux datas inGame.
namespace PulseEngine.Modules.Localisator
{
    /// <summary>
    /// Le Manager du Module Localisator.
    /// </summary>
    public static class LocalisationManager
    {

        #region Utils and helpers ################################################################

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

        #region Static Attributes and properties #########################################


        #endregion

        #region Methods ##################################################################

        /// <summary>
        /// Retourne la data textuelle correspondante aux info renseignes dans la BD, ou revoi null.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="tradDataType"></param>
        /// <param name="langage"></param>
        /// <returns></returns>
        public static async Task<string> TextData(int _id, DatalocationField _field, TradDataTypes tradDataType, Languages langage)
        {
            string result = string.Empty;
            LocalisationLibrary handle = null;
            Localisationdata data = null;
            string key = "Localisator_" + LanguageConverter(langage) + "_" + tradDataType;
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
                    result = data.Title.s_textField;
                    break;
                case DatalocationField.header:
                    result = data.Header.s_textField;
                    break;
                case DatalocationField.banner:
                    result = data.Banner.s_textField;
                    break;
                case DatalocationField.groupName:
                    result = data.GroupName.s_textField;
                    break;
                case DatalocationField.toolTip:
                    result = data.ToolTip.s_textField;
                    break;
                case DatalocationField.description:
                    result = data.Description.s_textField;
                    break;
                case DatalocationField.details:
                    result = data.Details.s_textField;
                    break;
                case DatalocationField.infos:
                    result = data.Infos.s_textField;
                    break;
                case DatalocationField.child1:
                    result = data.Child1.s_textField;
                    break;
                case DatalocationField.child2:
                    result = data.Child2.s_textField;
                    break;
                case DatalocationField.child3:
                    result = data.Child3.s_textField;
                    break;
                case DatalocationField.child4:
                    result = data.Child4.s_textField;
                    break;
                case DatalocationField.child5:
                    result = data.Child5.s_textField;
                    break;
                case DatalocationField.child6:
                    result = data.Child6.s_textField;
                    break;
                case DatalocationField.footPage:
                    result = data.FootPage.s_textField;
                    break;
                case DatalocationField.conclusion:
                    result = data.Conclusion.s_textField;
                    break;
                case DatalocationField.end:
                    result = data.End.s_textField;
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
                            string subkey = "Localisator_" + LanguageConverter((Languages)langage) + "_" + ((DataTypes)type).ToString();
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
                                        parts[i] = subData.Title.s_textField;
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
        /// <param name="tradDataType"></param>
        /// <param name="langage"></param>
        /// <returns></returns>
        public static async Task<AudioClip> AudioData(int _id, DatalocationField _field, TradDataTypes tradDataType, Languages langage)
        {
            AudioClip result = null;
            LocalisationLibrary handle = null;
            Localisationdata data = null;
            string key = "Localisator_" + LanguageConverter(langage) + "_" + tradDataType;
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
                    result = data.Title.s_audioField;
                    break;
                case DatalocationField.header:
                    result = data.Header.s_audioField;
                    break;
                case DatalocationField.banner:
                    result = data.Banner.s_audioField;
                    break;
                case DatalocationField.groupName:
                    result = data.GroupName.s_audioField;
                    break;
                case DatalocationField.toolTip:
                    result = data.ToolTip.s_audioField;
                    break;
                case DatalocationField.description:
                    result = data.Description.s_audioField;
                    break;
                case DatalocationField.details:
                    result = data.Details.s_audioField;
                    break;
                case DatalocationField.infos:
                    result = data.Infos.s_audioField;
                    break;
                case DatalocationField.child1:
                    result = data.Child1.s_audioField;
                    break;
                case DatalocationField.child2:
                    result = data.Child2.s_audioField;
                    break;
                case DatalocationField.child3:
                    result = data.Child3.s_audioField;
                    break;
                case DatalocationField.child4:
                    result = data.Child4.s_audioField;
                    break;
                case DatalocationField.child5:
                    result = data.Child5.s_audioField;
                    break;
                case DatalocationField.child6:
                    result = data.Child6.s_audioField;
                    break;
                case DatalocationField.footPage:
                    result = data.FootPage.s_audioField;
                    break;
                case DatalocationField.conclusion:
                    result = data.Conclusion.s_audioField;
                    break;
                case DatalocationField.end:
                    result = data.End.s_audioField;
                    break;
            }
            return result;
        }

        /// <summary>
        /// Retourne la data Image correspondante aux info renseignes dans la BD, ou revoi null.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="tradDataType"></param>
        /// <param name="langage"></param>
        /// <returns></returns>
        public static async Task<Sprite> ImageData(int _id, DatalocationField _field, TradDataTypes tradDataType, Languages langage)
        {
            Sprite result = null;
            LocalisationLibrary handle = null;
            Localisationdata data = null;
            string key = "Localisator_" + LanguageConverter(langage) + "_" + tradDataType;
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
                    result = data.Title.s_imageField;
                    break;
                case DatalocationField.header:
                    result = data.Header.s_imageField;
                    break;
                case DatalocationField.banner:
                    result = data.Banner.s_imageField;
                    break;
                case DatalocationField.groupName:
                    result = data.GroupName.s_imageField;
                    break;
                case DatalocationField.toolTip:
                    result = data.ToolTip.s_imageField;
                    break;
                case DatalocationField.description:
                    result = data.Description.s_imageField;
                    break;
                case DatalocationField.details:
                    result = data.Details.s_imageField;
                    break;
                case DatalocationField.infos:
                    result = data.Infos.s_imageField;
                    break;
                case DatalocationField.child1:
                    result = data.Child1.s_imageField;
                    break;
                case DatalocationField.child2:
                    result = data.Child2.s_imageField;
                    break;
                case DatalocationField.child3:
                    result = data.Child3.s_imageField;
                    break;
                case DatalocationField.child4:
                    result = data.Child4.s_imageField;
                    break;
                case DatalocationField.child5:
                    result = data.Child5.s_imageField;
                    break;
                case DatalocationField.child6:
                    result = data.Child6.s_imageField;
                    break;
                case DatalocationField.footPage:
                    result = data.FootPage.s_imageField;
                    break;
                case DatalocationField.conclusion:
                    result = data.Conclusion.s_imageField;
                    break;
                case DatalocationField.end:
                    result = data.End.s_imageField;
                    break;
            }
            return result;
        }
        #endregion
    }

}
