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
    #region Localisation >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// La Data de localisation contenu dans un asset de localisation, dans une langue precise.
    /// </summary>
    [System.Serializable]
    public class Localisationdata : IData
    {
        #region Attributes ###############################################################

        [SerializeField]
        private DataLocation location;

        [SerializeField]
        private TradField title;

        [SerializeField]
        private TradField header;

        [SerializeField]
        private TradField banner;

        [SerializeField]
        private TradField groupName;

        [SerializeField]
        private TradField toolTip;

        [SerializeField]
        private TradField description;

        [SerializeField]
        private TradField details;

        [SerializeField]
        private TradField infos;

        [SerializeField]
        private TradField child1;

        [SerializeField]
        private TradField child2;

        [SerializeField]
        private TradField child3;

        [SerializeField]
        private TradField child4;

        [SerializeField]
        private TradField child5;

        [SerializeField]
        private TradField child6;

        [SerializeField]
        private TradField footPage;

        [SerializeField]
        private TradField conclusion;

        [SerializeField]
        private TradField end;

        #endregion

        #region Proprietes ##################################################################

        /// <summary>
        /// L'id de traduction.
        /// </summary>
        public DataLocation Location
        {
            get { return location; }
            set
            {
                var tmp = value;
                tmp.dType = DataTypes.Localisation;
                location = tmp;
            }
        }

        /// <summary>
        /// Le titre.
        /// </summary>
        public TradField Title { get { return title; } set { title = value; } }

        /// <summary>
        /// L'entete.
        /// </summary>
        public TradField Header { get { return header; } set { header = value; } }

        /// <summary>
        /// La banniere.
        /// </summary>
        public TradField Banner { get { return banner; } set { banner = value; } }

        /// <summary>
        /// Le nom de groupe.
        /// </summary>
        public TradField GroupName { get { return groupName; } set { groupName = value; } }

        /// <summary>
        /// Le texte au survol.
        /// </summary>
        public TradField ToolTip { get { return toolTip; } set { toolTip = value; } }

        /// <summary>
        /// La description.
        /// </summary>
        public TradField Description { get { return description; } set { description = value; } }

        /// <summary>
        /// Les details.
        /// </summary>
        public TradField Details { get { return details; } set { details = value; } }

        /// <summary>
        /// Les details avances.
        /// </summary>
        public TradField Infos { get { return infos; } set { infos = value; } }

        /// <summary>
        /// Le sous texte 1
        /// </summary>
        public TradField Child1 { get { return child1; } set { child1 = value; } }

        /// <summary>
        /// Le sous texte 2
        /// </summary>
        public TradField Child2 { get { return child2; } set { child2 = value; } }

        /// <summary>
        /// Le sous texte 3
        /// </summary>
        public TradField Child3 { get { return child3; } set { child3 = value; } }

        /// <summary>
        /// Le sous texte 4
        /// </summary>
        public TradField Child4 { get { return child4; } set { child4 = value; } }

        /// <summary>
        /// Le sous texte 5
        /// </summary>
        public TradField Child5 { get { return child5; } set { child5 = value; } }

        /// <summary>
        /// Le sous texte 6
        /// </summary>
        public TradField Child6 { get { return child6; } set { child6 = value; } }

        /// <summary>
        /// Le pied de page.
        /// </summary>
        public TradField FootPage { get { return footPage; } set { footPage = value; } }

        /// <summary>
        /// La conclusion.
        /// </summary>
        public TradField Conclusion { get { return conclusion; } set { conclusion = value; } }

        /// <summary>
        /// Le ending / les credits
        /// </summary>
        public TradField End { get { return end; } set { end = value; } }

        #endregion

        #region methods ###################################################################

        /// <summary>
        /// Retourne la data textuelle correspondante aux info renseignes dans la BD, ou revoi null.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_tradDataType"></param>
        /// <param name="_langage"></param>
        /// <returns></returns>
        public static async Task<string> TextData(DataLocation _location, DatalocationField _field, CancellationToken ct, params Color[] hightLigths)
        {
            string result = string.Empty;
            Localisationdata data = await CoreLibrary.GetData<Localisationdata>(_location, ct);
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
                    if (subParts.Length > 2)
                    {
                        DataLocation subLocation = new DataLocation();
                        if (int.TryParse(subParts[0], out subLocation.id) && int.TryParse(subParts[1], out subLocation.globalLocation) && int.TryParse(subParts[2], out subLocation.localLocation))
                        {
                            var subData = await CoreLibrary.GetData<Localisationdata>(subLocation, ct);
                            if (subData != null)
                            {
                                int fieldID = -1;
                                if (subParts.Length > 3 && int.TryParse(subParts[3], out fieldID))
                                {
                                    //recursive
                                    parts[i] = await TextData(subLocation, (DatalocationField)fieldID, ct, hightLigths);
                                    //One way
                                    //parts[i] = subData.GetTradField((DatalocationField)fieldID).textField;
                                    if (hightLigths.Length > 0 && (DatalocationField)fieldID == DatalocationField.title)
                                    {
                                        switch ((TradDataTypes)subLocation.localLocation)
                                        {
                                            default:
                                                parts[i] = parts[i];
                                                break;
                                            case TradDataTypes.Person:
                                                parts[i] = parts[i].Hightlight(hightLigths.Length > 0? hightLigths[0] : default);
                                                break;
                                            case TradDataTypes.Document:
                                                parts[i] = parts[i].Hightlight(hightLigths.Length > 1 ? hightLigths[1] : default);
                                                break;
                                            case TradDataTypes.Place:
                                                parts[i] = parts[i].Hightlight(hightLigths.Length > 2 ? hightLigths[2] : default);
                                                break;
                                        }
                                    }
                                }
                                else
                                {
                                    //recursive
                                    parts[i] = await TextData(subLocation, DatalocationField.title, ct, hightLigths);
                                    //One way
                                    //parts[i] = subData.Title.textField;
                                }
                            }
                        }
                    }
                }
            }
            return string.Join(" ", parts);
        }

        /// <summary>
        /// Retourne la data vocale correspondante aux info renseignes dans la BD, ou revoi null.
        /// </summary>
        /// <param name="_id"></param>
        /// <param name="_tradDataType"></param>
        /// <param name="_langage"></param>
        /// <returns></returns>
        public static async Task<AudioClip> AudioData(DataLocation _location, DatalocationField _field, CancellationToken ct)
        {
            AudioClip result = null;
            Localisationdata data = await CoreLibrary.GetData<Localisationdata>(_location, ct);
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
        public static async Task<Sprite> ImageData(DataLocation _location, DatalocationField _field, CancellationToken ct)
        {
            Sprite result = null;
            Localisationdata data = await CoreLibrary.GetData<Localisationdata>(_location, ct);
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
    }

    /// <summary>
    /// Le type de Base de toute data traductible.
    /// </summary>
    public abstract class LocalisableData
    {
        #region Attributes #########################################################

        [SerializeField]
        private DataLocation tradLocation;

        #endregion

        #region Properties #########################################################

        /// <summary>
        /// Traduction file ID.
        /// </summary>
        public DataLocation TradLocation
        {
            get { return tradLocation; }
            set
            {
                var tmp = value;
                tmp.dType = DataTypes.Localisation;
                tradLocation = tmp;
            }
        }

        #endregion
    }

    #endregion

    #region Animation >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// La data d'animation.
    /// </summary>
    [System.Serializable]
    public class AnimaData : IData
    {
        #region Attributs #########################################################

        [SerializeField]
        private DataLocation location;
        [SerializeField]
        private List<AnimeCommand> eventList = new List<AnimeCommand>();
        [SerializeField]
        private List<AnimePhaseTimeStamp> phaseAnims = new List<AnimePhaseTimeStamp>();
        [SerializeField]
        private int physicPlace;
        [SerializeField]
        private AnimationClip motion;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// L'id de l'animation dans la BD des anima data.
        /// </summary>
        public DataLocation Location
        {
            get { return location; }
            set
            {
                var tmp = value;
                tmp.dType = DataTypes.Anima;
                location = tmp;
            }
        }

        /// <summary>
        /// La liste des evenements au cours de l'animation.
        /// </summary>
        public List<AnimeCommand> EventList { get { return eventList; } set { eventList = value; } }

        /// <summary>
        /// La phase d'animation en cours.
        /// </summary>
        public List<AnimePhaseTimeStamp> PhaseAnims { get { return phaseAnims; } set { phaseAnims = value; } }

        /// <summary>
        /// Le lieux physique, terre, aux air ... ou il est possible d'effectuer l'action.
        /// </summary>
        public PhysicSpaces PhysicPlace { get { return (PhysicSpaces)physicPlace; } set { physicPlace = (int)value; } }

        /// <summary>
        /// L'animation lue.
        /// </summary>
        public AnimationClip Motion { get { return motion; } set { motion = value; } }

        #endregion
    }

    #endregion

    #region Character >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// La Data d'un character.
    /// </summary>
    [System.Serializable]
    public class CharacterData : LocalisableData, IData
    {
        #region Attributs #########################################################

        [SerializeField]
        private DataLocation location;
        [SerializeField]
        private MindStat stats;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// l'id dans BD.
        /// </summary>
        public DataLocation Location
        {
            get { return location; }
            set
            {
                var tmp = value;
                tmp.dType = DataTypes.Character;
                location = tmp;
            }
        }

        /// <summary>
        /// Les stats du character.
        /// </summary>
        public MindStat Stats { get => stats; set => stats = value; }

        #endregion
    }

    #endregion

    #region Combat Sysytem >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// La data d'une arme.
    /// </summary>
    [System.Serializable]
    public class WeaponData : LocalisableData, IData
    {
        #region Attributs #########################################################

        [SerializeField]
        private DataLocation location;
        [SerializeField]
        private float range;
        [SerializeField]
        private int damageType;
        [SerializeField]
        private float damageValues;
        [SerializeField]
        private float merchantValue;
        [SerializeField]
        private List<int> materials = new List<int>();
        [SerializeField]
        private PhysicStats physicProperties;
        [SerializeField]
        private DataLocation idle_move;
        [SerializeField]
        private DataLocation draw_move;
        [SerializeField]
        private DataLocation sheath_move;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// l'id de l'arme dans la bd des armes.
        /// </summary>
        public DataLocation Location
        {
            get { return location; }
            set
            {
                var tmp = value;
                tmp.dType = DataTypes.Weapon;
                location = tmp;
            }
        }

        /// <summary>
        /// La portee de l'arme.
        /// </summary>
        public float Range { get => range; set => range = value; }

        /// <summary>
        /// le type de degat que l'arme inflige.
        /// </summary>
        public TypeDegatArme TypeDegats { get => (TypeDegatArme)damageType; set => damageType = (int)value; }

        /// <summary>
        /// la valeur des degats infliges.
        /// </summary>
        public float Degats { get => damageValues; set => damageValues = value; }

        /// <summary>
        /// La valeur, prix de l'arme.
        /// </summary>
        public float Cost { get => merchantValue; set => merchantValue = value; }

        /// <summary>
        /// La liste des materiaux en lesquels sont faites chaque partie de l'arme, un pour chaque objet.
        /// </summary>
        public List<PhysicMaterials> Materiaux
        {
            get
            {
                return materials.ConvertAll(new System.Converter<int, PhysicMaterials>(integer => { return (PhysicMaterials)integer; }));
            }
            set
            {
                materials = value.ConvertAll(new System.Converter<PhysicMaterials, int>(physic => { return (int)physic; }));
            }
        }


        /// <summary>
        /// Les proprietes physiques.
        /// </summary>
        public PhysicStats PhysicProperties { get => physicProperties; set => physicProperties = value; }

        /// <summary>
        /// l'idle avec l'arme.
        /// </summary>
        public DataLocation IdleMove { get => idle_move; set => idle_move = value; }

        /// <summary>
        /// le degainage avec l'arme.
        /// </summary>
        public DataLocation DrawMove { get => draw_move; set => draw_move = value; }

        /// <summary>
        /// le rengainage avec l'arme.
        /// </summary>
        public DataLocation SheathMove { get => sheath_move; set => sheath_move = value; }


        #endregion
    }

    #endregion

    #region Properties and stats >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    /// <summary>
    /// La data statistique Physique associee a des objets.
    /// </summary>
    [System.Serializable]
    public struct PhysicStats
    {
        #region Attributs #########################################################

        //space occupation
        [SerializeField]
        private float volume;
        [SerializeField]
        private float density;

        //physic properties
        [SerializeField]
        private float mass;
        [SerializeField]
        private float inflammability;
        [SerializeField]
        private float frozability;

        //mecanics properties
        [SerializeField]
        private float resistance;
        [SerializeField]
        private float elasticity;
        [SerializeField]
        private float roughness;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// The spacial volume of the object
        /// </summary>
        public float Volume { get => volume; set => volume = Mathf.Clamp(value, 0, value); }

        /// <summary>
        /// The object density; how hard is to penetrate the object.
        /// </summary>
        public float Density { get => density; set => density = Mathf.Clamp(value, 0, value); }

        /// <summary>
        /// The object mass
        /// </summary>
        public float Mass { get => mass; set => mass = Mathf.Clamp(value, 0, value); }

        /// <summary>
        /// The flamability, how easy is the object catch fire.
        /// </summary>
        public float Inflammability { get => inflammability; set => inflammability = Mathf.Clamp(value, 0, value); }

        /// <summary>
        /// how easy the object get frozen at low tempratures.
        /// </summary>
        public float Frozability { get => frozability; set => frozability = Mathf.Clamp(value, 0, value); }

        /// <summary>
        /// the object resistance; how hard the object is to break.
        /// </summary>
        public float Resistance { get => resistance; set => resistance = Mathf.Clamp(value, 0, value); }

        /// <summary>
        /// The object's elasticity. how much the object can be bended.
        /// </summary>
        public float Elasticity { get => elasticity; set => elasticity = Mathf.Clamp(value, 0, value); }

        /// <summary>
        /// The object roughness. how much grip the friction against this object make.
        /// </summary>
        public float Roughness { get => roughness; set => roughness = Mathf.Clamp(value, 0, value); }

        #endregion
    }

    /// <summary>
    /// La data statistique vitale associee a des objets.
    /// </summary>
    [System.Serializable]
    public struct VitalStat
    {
        #region Attributs #########################################################

        [SerializeField]
        private float health;
        [SerializeField]
        private float longevity;
        [SerializeField]
        private float age;
        [SerializeField]
        private float karma;
        [SerializeField]
        private PhysicStats phycics_stats;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// quantite de vie d'un objet.
        /// </summary>
        public float Health { get { return health; } set { health = Mathf.Clamp(value, 0, longevity); } }

        /// <summary>
        /// The object longevity or maximum health
        /// </summary>
        public float Longevity { get { return longevity; } set { longevity = Mathf.Clamp(value, 0, value); } }

        /// <summary>
        /// The age. how old is the object.
        /// </summary>
        public float Age { get { return age; } set { age = Mathf.Clamp(value, 0, value); } }

        /// <summary>
        /// The Karma.how much luck you got.
        /// </summary>
        public float Karma { get { return karma; } set { karma = value; } }

        /// <summary>
        /// The physic stats.
        /// </summary>
        public PhysicStats PhycicsStats { get => phycics_stats; set => phycics_stats = value; }

        #endregion
    }


    /// <summary>
    /// La data statistique physiques associee a des etres vivants.
    /// </summary>
    [System.Serializable]
    public struct BodyStats
    {
        #region Attributs #########################################################

        [SerializeField]
        private float strenght;
        [SerializeField]
        private float endurance;
        [SerializeField]
        private float enduranceMax;
        [SerializeField]
        private float souffle;
        [SerializeField]
        private float souffleMax;
        [SerializeField]
        private float speed;
        [SerializeField]
        private VitalStat vital_stat;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// The strenght of the target.
        /// </summary>
        public float Strenght { get => strenght; set => strenght = value; }

        /// <summary>
        /// The Endurance of the target. how much time it can keep high level of physic performances.
        /// </summary>
        public float Endurance { get => endurance; set => endurance = value; }

        /// <summary>
        /// The maximum endurance. 
        /// </summary>
        public float EnduranceMax { get => enduranceMax; set => enduranceMax = value; }

        /// <summary>
        /// How much time it can keep without breathing.
        /// </summary>
        public float Souffle { get => souffle; set => souffle = value; }

        /// <summary>
        /// The maximum possible souffle.
        /// </summary>
        public float SouffleMax { get => souffleMax; set => souffleMax = value; }

        /// <summary>
        /// the movement and reactivity speeds
        /// </summary>
        public float Speed { get => speed; set => speed = value; }

        /// <summary>
        /// the Vital Stats.
        /// </summary>
        public VitalStat VitalStats { get => vital_stat; set => vital_stat = value; }

        #endregion
    }

    /// <summary>
    /// La data statistique Mentale associee a des etres vivants.
    /// </summary>
    [System.Serializable]
    public struct MindStat
    {
        #region Attributs #########################################################

        [SerializeField]
        private float intelligence;
        [SerializeField]
        private float experience;
        [SerializeField]
        private float dexterity;
        [SerializeField]
        private float sociability;
        [SerializeField]
        private float determination;
        [SerializeField]
        private float madness;
        [SerializeField]
        private BodyStats body_stat;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// The intelligence. Capacity to learn contents or the required level to do some actions.
        /// </summary>
        public float Intelligence { get => intelligence; set => intelligence = value; }

        /// <summary>
        /// The experience.Capacity to learn from mistakes and avoid them next time or the mind level
        /// </summary>
        public float Experience { get => experience; set => experience = value; }

        /// <summary>
        /// Dexterity. The target skill level.
        /// </summary>
        public float Dexterity { get => dexterity; set => dexterity = value; }

        /// <summary>
        /// the dependance from others.
        /// </summary>
        public float Sociability { get => sociability; set => sociability = value; }

        /// <summary>
        /// Capacity to try and retry.
        /// </summary>
        public float Determination { get => determination; set => determination = value; }

        /// <summary>
        /// Probability to be imprevisible.
        /// </summary>
        public float Madness { get => madness; set => madness = value; }

        /// <summary>
        /// The body stats.
        /// </summary>
        public BodyStats BodyStats { get => body_stat; set => body_stat = value; }

        #endregion
    }

    #endregion

    #region Commander >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>


    /// <summary>
    /// The command sequence executed.
    /// </summary>
    [System.Serializable]
    public class CommandSequence : IEquatable<CommandSequence>, IData, IEditorGraph
    {
        #region Attributes #####################################################################################

        [SerializeField]
        private DataLocation location;
        [SerializeField]
        public List<Command> specialCmds = new List<Command>();
        [SerializeField]
        private List<Command> sequence;
        [SerializeField]
        private string label;

        #endregion

        #region Properties #####################################################################################

        /// <summary>
        /// The sequence location in the datalist
        /// </summary>
        public DataLocation Location { get => location; set => location = value; }

        /// <summary>
        /// The command sequence list.
        /// </summary>
        public List<Command> Sequence
        {
            get
            {
                if (sequence == null)
                    sequence = new List<Command>();
                return sequence;
            }
            set => sequence = value;
        }

        /// <summary>
        /// Return the default command in this sequence.
        /// </summary>
        public Command DefaultCommand
        {
            get
            {
                return Sequence[0];
            }
        }

        /// <summary>
        /// The sequence label.
        /// </summary>
        public string Label { get => label; set => label = value; }

        #endregion

        #region Methods #####################################################################################

        public bool Equals(CommandSequence other)
        {
            return Location.Equals(other.Location);
        }

        /// <summary>
        /// Link two commands.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="child"></param>
        public bool Link(CommandPath parent, CommandPath child)
        {
            if (Sequence == null)
                return false;
            int parentIndex = Sequence.FindIndex(p => { return p.CmdPath == parent; });
            int childIndex = Sequence.FindIndex(p => { return p.CmdPath == child; });
            if ((parentIndex < 0 || parentIndex >= Sequence.Count) && parent != CommandPath.EntryPath)
                return false;
            if ((childIndex < 0 || childIndex >= Sequence.Count) && !(child == CommandPath.BreakPath || child == CommandPath.ExitPath))
                return false;
            if (parentIndex == childIndex)
                return false;
            if (parent != CommandPath.EntryPath)
            {
                var cmd = Sequence[parentIndex];
                AddOutput(ref cmd, child);
                Sequence[parentIndex] = cmd;
            }
            if (!(child == CommandPath.BreakPath || child == CommandPath.ExitPath))
            {
                var cmd = Sequence[childIndex];
                AddInput(ref cmd, parent);
                Sequence[childIndex] = cmd;
            }
            return true;
        }

        /// <summary>
        /// Action made on Command before it's been deleted.
        /// </summary>
        /// <param name="c"></param>
        public void UnLinkAll(CommandPath cmd)
        {
            if (Sequence == null)
                return;
            BreakInputs(cmd);
            BreakOutputs(cmd);
        }

        /// <summary>
        /// Break connctions between two commands command.
        /// </summary>
        /// <param name="sq"></param>
        /// <param name="target"></param>
        public void BreakConnection(CommandPath from, CommandPath to, bool forceParent = false)
        {
            if (Sequence == null)
                return;
            int index1 = Sequence.FindIndex(c => { return c.CmdPath == from; });
            int index2 = Sequence.FindIndex(c => { return c.CmdPath == to; });
            if (index1 < 0)
                return;
            Command c1 = Sequence[index1];
            Command c2 = Command.NullCmd;
            if (c1.IsChild(to) && !forceParent)
            {
                {
                    var outputs = c1.Outputs;
                    outputs.Remove(to);
                    c1.Outputs = outputs;
                    Sequence[index1] = c1;
                }
                if(index2 >= 0)
                {
                    c2 = Sequence[index2];
                    var inputs = c2.Inputs;
                    inputs.Remove(from);
                    c2.Inputs = inputs;
                    Sequence[index2] = c2;
                }
            }
            else if (c1.IsParent(to))
            {
                {
                    var inputs = c1.Inputs;
                    inputs.Remove(to);
                    c1.Inputs = inputs;
                    Sequence[index1] = c1;
                }

                if (index2 >= 0)
                {
                    c2 = Sequence[index2];
                    var outputs = c2.Outputs;
                    outputs.Remove(from);
                    c2.Outputs = outputs;
                    Sequence[index2] = c2;
                }
            }
        }

        /// <summary>
        /// Break all parent connections of a command.
        /// </summary>
        /// <param name="sq"></param>
        /// <param name="target"></param>
        public void BreakInputs(CommandPath cmd)
        {
            if (Sequence == null)
                return;
            int index = Sequence.FindIndex(c => { return c.CmdPath == cmd; });
            if (index < 0)
                return;
            for (int i = 0; i < Sequence[index].Inputs.Count; i++)
            {
                BreakConnection(Sequence[index].CmdPath, Sequence[index].Inputs[i], true);
            }
        }

        /// <summary>
        /// Break all childdren connections of a command.
        /// </summary>
        /// <param name="sq"></param>
        /// <param name="target"></param>
        public void BreakOutputs(CommandPath cmd)
        {
            if (Sequence == null)
                return;
            int index = Sequence.FindIndex(c => { return c.CmdPath == cmd; });
            if (index < 0)
                return;
            for (int i = 0; i < Sequence[index].Outputs.Count; i++)
            {
                BreakConnection(Sequence[index].CmdPath, Sequence[index].Outputs[i]);
            }
        }

        /// <summary>
        /// Add a Parent to a command
        /// </summary>
        /// <param name="c"></param>
        public void AddInput(ref Command cmd, CommandPath cmdPath)
        {
            if (cmd.Inputs == null)
                cmd.Inputs = new List<CommandPath>();
            if (Sequence == null)
                return;
            if (cmdPath == CommandPath.NullPath || cmd.IsParent(cmdPath))
                return;
            cmd.Inputs.Add(cmdPath);
        }

        /// <summary>
        /// Add a child to a command
        /// </summary>
        /// <param name="c"></param>
        public void AddOutput(ref Command cmd, CommandPath cmdPath)
        {
            if (cmd.Outputs == null)
                cmd.Outputs = new List<CommandPath>();
            if (Sequence == null)
                return;
            if (cmdPath == CommandPath.NullPath || cmd.IsChild(cmdPath))
                return;
            cmd.Outputs.Add(cmdPath);
        }


#if UNITY_EDITOR

        public Vector2 GraphPosition { get; set; }
        public float GraphScale { get; set; }
        public Matrix4x4 TransformMatrix { get; set; }
        public List<IEditorNode> Nodes { get; set; }
        public GenericMenu ContextMenu { get; set; }
        public bool InitializedGraph { get; set; }
        public IEditorNode LinkRequester { get; set; }

        public void InitializeGraph()
        {
            //Forcing base special commands.
            if (specialCmds == null)
                specialCmds = new List<Command>();
            if (specialCmds.Count <= 0)
                specialCmds.Add(new Command { CmdPath = CommandPath.EntryPath });
            if (specialCmds[0].CmdPath != CommandPath.EntryPath)
                specialCmds[0] = new Command { CmdPath = CommandPath.EntryPath };
            if (specialCmds.Count <= 1)
                specialCmds.Add(new Command { CmdPath = CommandPath.ExitPath });
            if (specialCmds[1].CmdPath != CommandPath.ExitPath)
                specialCmds[1] = new Command { CmdPath = CommandPath.ExitPath };

            for (int i = 0; i < specialCmds.Count; i++)
            {
                specialCmds[i] = (Command)specialCmds[i].CreateNode();
            }
            for (int i = 0; i < sequence.Count; i++)
            {
                sequence[i] = (Command)sequence[i].CreateNode();
            }
            InitializedGraph = true;
        }

#endif

        #endregion
    }

    #endregion

    #region ... >>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>>

    #endregion
}
