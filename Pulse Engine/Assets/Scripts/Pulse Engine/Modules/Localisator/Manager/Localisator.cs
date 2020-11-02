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
    public static class Localisator
    {

        #region Static Attributes and properties #########################################

        #region colors .................................................

        /// <summary>
        /// the hightlight color of the person name
        /// </summary>
        public static Color PersonNameColor = Color.red;

        /// <summary>
        /// the hightlight color of the object name
        /// </summary>
        public static Color ObjectNameColor = Color.green;

        /// <summary>
        /// the hightlight color of the place name
        /// </summary>
        public static Color PlaceNameColor = Color.blue;

        #endregion


        #endregion

        #region Methods ##################################################################

        /// <summary>
        /// Retourne la data textuelle correspondante aux info renseignes dans la BD, ou revoi null.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_tradDataType"></param>
        /// <param name="_langage"></param>
        /// <returns></returns>
        public static async Task<string> TextData(DataLocation _location, DatalocationField _field, bool hightLigth = false)
        {
            string result = string.Empty;
            Localisationdata data = await CoreData.GetData<Localisationdata,LocalisationLibrary>(_location);
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
                    if(subParts.Length > 2)
                    {
                        DataLocation subLocation = new DataLocation();
                        if (int.TryParse(subParts[0], out subLocation.id) && int.TryParse(subParts[1], out subLocation.globalLocation) && int.TryParse(subParts[2], out subLocation.localLocation))
                        {
                            var subData = await CoreData.GetData<Localisationdata, LocalisationLibrary>(subLocation);
                            if (subData != null)
                            {
                                int fieldID = -1;
                                if (subParts.Length > 3 && int.TryParse(subParts[3], out fieldID))
                                {
                                    //recursive
                                    parts[i] = await TextData(subLocation, (DatalocationField)fieldID, hightLigth);
                                    //One way
                                    //parts[i] = subData.GetTradField((DatalocationField)fieldID).textField;
                                    if (hightLigth && (DatalocationField)fieldID == DatalocationField.title)
                                    {
                                        switch ((TradDataTypes)subLocation.localLocation)
                                        {
                                            default:
                                                parts[i] = parts[i];
                                                break;
                                            case TradDataTypes.Person:
                                                parts[i] = parts[i].Hightlight(PersonNameColor);
                                                break;
                                            case TradDataTypes.Document:
                                                parts[i] = parts[i].Hightlight(ObjectNameColor);
                                                break;
                                            case TradDataTypes.Place:
                                                parts[i] = parts[i].Hightlight(PlaceNameColor);
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    //recursive
                                    parts[i] = await TextData(subLocation, DatalocationField.title, hightLigth);
                                    //One way
                                    //parts[i] = subData.Title.textField;
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
        public static async Task<AudioClip> AudioData(DataLocation _location, DatalocationField _field)
        {
            AudioClip result = null;
            Localisationdata data = await CoreData.GetData<Localisationdata, LocalisationLibrary>(_location);
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
        public static async Task<Sprite> ImageData(DataLocation _location, DatalocationField _field)
        {
            Sprite result = null;
            Localisationdata data = await CoreData.GetData<Localisationdata, LocalisationLibrary>(_location);
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
