using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Core;
using System.Threading.Tasks;
using UnityEngine.AddressableAssets;
using System.Linq;


//TODO: Continuer d'implementer des fonction au fil des besoins du module, sans oublier les fonction d'access aux datas inGame.
namespace PulseEngine.Module.Localisator
{
    /// <summary>
    /// Le Manager du Module Localisator.
    /// </summary>
    public static class LocalisationManager
    {
        #region Enums and  structs #######################################################

        /// <summary>
        /// l'enumeration des champs d'un donnee de localisation.
        /// </summary>
        public enum DatalocationField
        {
            title,
            header,
            banner,
            groupName,
            toolTip,
            description,
            details,
            infos,
            child1,
            child2,
            child3,
            child4,
            child5,
            child6,
            footPage,
            conclusion,
            end,
        }

        #endregion

        #region Module Globals ###################################################################

        /// <summary>
        /// Le chemin d'access des donneens de localisation.
        /// </summary>
        public static string AssetsPath { get { return "LocalisationDatas"; } }

        #endregion

        #region Utils and helpers ################################################################

        /// <summary>
        /// Converti une langue en code de Langue.
        /// </summary>
        /// <param name="langue">La langue source.</param>
        /// <returns></returns>
        public static string LanguageConverter(PulseCore_GlobalValue_Manager.Languages langue)
        {
            switch (langue)
            {
                case PulseCore_GlobalValue_Manager.Languages.English:
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
        /// Retourne la data correspondante aux info renseignes dans la BD, ou revoi null.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="dataType"></param>
        /// <param name="langage"></param>
        /// <returns></returns>
        public static async Task<string> TextData(int _id, DatalocationField _field, int dataType, int langage)
        {
            string result = string.Empty;
            LocalisationLibrary handle = null;
            Localisationdata data = null;
            string key = "Localisator_" + LanguageConverter((PulseCore_GlobalValue_Manager.Languages)langage) + "_" + ((PulseCore_GlobalValue_Manager.DataType)dataType).ToString();
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
                data = handle.LocalizedDatas.Find(d => { return d.Trad_ID == _id; });
            }
            if (data == null)
                return string.Empty;

            switch (_field)
            {
                case DatalocationField.title:
                    result = data.Title;
                    break;
                case DatalocationField.header:
                    result = data.Header;
                    break;
                case DatalocationField.banner:
                    result = data.Banner;
                    break;
                case DatalocationField.groupName:
                    result = data.GroupName;
                    break;
                case DatalocationField.toolTip:
                    result = data.ToolTip;
                    break;
                case DatalocationField.description:
                    result = data.Description;
                    break;
                case DatalocationField.details:
                    result = data.Details;
                    break;
                case DatalocationField.infos:
                    result = data.Infos;
                    break;
                case DatalocationField.child1:
                    result = data.Child1;
                    break;
                case DatalocationField.child2:
                    result = data.Child2;
                    break;
                case DatalocationField.child3:
                    result = data.Child3;
                    break;
                case DatalocationField.child4:
                    result = data.Child4;
                    break;
                case DatalocationField.child5:
                    result = data.Child5;
                    break;
                case DatalocationField.child6:
                    result = data.Child6;
                    break;
                case DatalocationField.footPage:
                    result = data.FootPage;
                    break;
                case DatalocationField.conclusion:
                    result = data.Conclusion;
                    break;
                case DatalocationField.end:
                    result = data.End;
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
                            string subkey = "Localisator_" + LanguageConverter((PulseCore_GlobalValue_Manager.Languages)langage) + "_" + ((PulseCore_GlobalValue_Manager.DataType)type).ToString();
                            var sublocationsIlist = await Addressables.LoadResourceLocationsAsync(subkey).Task;
                            if (locationsIlist == null || locationsIlist.Count <= 0)
                                continue;
                            var sublocation = sublocationsIlist.FirstOrDefault();
                            if (sublocation != null)
                            {
                                var subHandle = await Addressables.LoadAssetAsync<LocalisationLibrary>(location.PrimaryKey).Task;
                                if (subHandle)
                                {
                                    var subData = subHandle.LocalizedDatas.Find(d => { return d.Trad_ID == id; });
                                    if(subData != null)
                                        parts[i] = subData.Title;
                                }
                            }
                        }
                    }
                }
            }
            return string.Join(" ",parts);
        }
        #endregion
    }
}
