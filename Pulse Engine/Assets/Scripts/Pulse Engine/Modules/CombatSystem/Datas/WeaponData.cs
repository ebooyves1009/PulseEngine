using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Modules.StatHandler;
using PulseEngine.Globals;
using PulseEngine.Modules.Anima;
using PulseEngine.Modules.Localisator;
using System.Threading.Tasks;
using PulseEngine.Modules.PhysicSpace;

namespace PulseEngine.Modules.CombatSystem
{
    /// <summary>
    /// La data d'une arme.
    /// </summary>
    [System.Serializable]
    public class WeaponData : ITraductible
    {
        #region Attributs #########################################################

        [SerializeField]
        private int id;
        [SerializeField]
        private int idTrad;
        [SerializeField]
        private int tradDataType;
        [SerializeField]
        private int typeArme;
        [SerializeField]
        private float portee;
        [SerializeField]
        private int typeDegats;
        [SerializeField]
        private float degats;
        [SerializeField]
        private float valeur;
        [SerializeField]
        private List<int> materiaux;
        [SerializeField]
        private List<GameObject> weapons = new List<GameObject>();
        [SerializeField]
        private StatData stat_weapon;
        [SerializeField]
        private StatData stat_owner;
        [SerializeField]
        private List<AnimaData> handle_moves = new List<AnimaData>();
        [SerializeField]
        private AnimaData idle_move;
        [SerializeField]
        private List<AttackData> attackDatas = new List<AttackData>();
        [SerializeField]
        private List<DefenseData> defenseDatas = new List<DefenseData>();
        [SerializeField]
        private bool canParry;
        [SerializeField]
        private bool portable;
        [SerializeField]
        private List<WeaponPlace> restPlaces = new List<WeaponPlace>();
        [SerializeField]
        private List<WeaponPlace> carryPlaces = new List<WeaponPlace>();
        [SerializeField]
        private List<Vector3> projectilesOutPoints = new List<Vector3>();

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// l'id de l'arme dans la bd des armes.
        /// </summary>
        public int ID { get => id; set => id = value; }

        /// <summary>
        /// l'id des donnees textuelles de l'arme.
        /// </summary>
        public int IdTrad { get => idTrad; set => idTrad = value; }

        /// <summary>
        /// Le type de data traductible.
        /// </summary>
        public TradDataTypes TradType { get => (TradDataTypes)tradDataType; set => tradDataType = (int)value; }

        /// <summary>
        /// Le type d'arme.
        /// </summary>
        public WeaponType TypeArme { get { return (WeaponType)typeArme; } set { typeArme = (int)value; } }

        /// <summary>
        /// La portee de l'arme.
        /// </summary>
        public float Portee { get => portee; set => portee = value; }

        /// <summary>
        /// le type de degat que l'arme inflige.
        /// </summary>
        public TypeDegatArme TypeDegats { get => (TypeDegatArme)typeDegats; set => typeDegats = (int)value; }

        /// <summary>
        /// la valeur des degats infliges.
        /// </summary>
        public float Degats { get => degats; set => degats = value; }

        /// <summary>
        /// La valeur, prix de l'arme.
        /// </summary>
        public float Valeur { get => valeur; set => valeur = value; }

        /// <summary>
        /// La liste des materiaux en lesquels sont faites chaque partie de l'arme, un pour chaque objet.
        /// </summary>
        public List<PhysicMaterials> Materiaux
        {
            get
            {
                return materiaux.ConvertAll<PhysicMaterials>(new System.Converter<int, PhysicMaterials>(integer => { return (PhysicMaterials)integer; }));
            }
            set
            {
                materiaux = value.ConvertAll<int>(new System.Converter<PhysicMaterials, int>(physic => { return (int)physic; }));
            }
        }


        /// <summary>
        /// la liste des gameObjects qui constituent l'arme.
        /// </summary>
        public List<GameObject> Weapons { get => weapons; set => weapons = value; }

        /// <summary>
        /// Les stats propre a l'arme.
        /// </summary>
        public StatData StatWeapon { get => stat_weapon; set => stat_weapon = value; }

        /// <summary>
        /// Les stats que l'arme procure a son detenteur un fois equipe
        /// </summary>
        public StatData StatOwner { get => stat_owner; set => stat_owner = value; }

        /// <summary>
        /// Les mouvements de degainage et rengainage de l'arme.
        /// </summary>
        public List<AnimaData> HandleMoves { get => handle_moves; set => handle_moves = value; }

        /// <summary>
        /// l'idle avec l'arme.
        /// </summary>
        public AnimaData IdleMove { get => idle_move; set => idle_move = value; }

        /// <summary>
        /// La liste des mouvements offensifs de l'arme et leurs donnees.
        /// </summary>
        public List<AttackData> AttackDatas { get => attackDatas; set => attackDatas = value; }

        /// <summary>
        /// La liste des mouvements defensifs de l'arme et leurs donnees.
        /// </summary>
        public List<DefenseData> DefenseDatas { get => defenseDatas; set => defenseDatas = value; }

        /// <summary>
        /// L'arme peux elle parrer des attaques?
        /// </summary>
        public bool CanParry { get => canParry; set => canParry = value; }

        /// <summary>
        /// l'arme peut-elle etre transportee par son detenteur?
        /// </summary>
        public bool Portable { get => portable; set => portable = value; }

        /// <summary>
        /// Les emplacements rengaine de l'arme. un pour chaque object
        /// </summary>
        public List<WeaponPlace> RestPlaces { get => restPlaces; set => restPlaces = value; }


        /// <summary>
        /// Les emplacements degaine de l'arme. un pour chaque object
        /// </summary>
        public List<WeaponPlace> CarryPlaces { get => carryPlaces; set => carryPlaces = value; }

        /// <summary>
        /// Les points de sortie de projectiles pour les armes a distance.
        /// </summary>
        public List<Vector3> ProjectilesOutPoints { get => projectilesOutPoints; set => projectilesOutPoints = value; }


        #endregion

        #region Methods #########################################################

        /// <summary>
        /// Get the traducted texts.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task<string> GetTradText(DatalocationField field)
        {
            return await LocalisationManager.TextData(IdTrad, field, TradType, PulseEngineMgr.currentLanguage);
        }


        /// <summary>
        /// Get the traducted Image.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task<Sprite> GetTradSprite(DatalocationField field)
        {
            return await LocalisationManager.ImageData(IdTrad, field, TradType, PulseEngineMgr.currentLanguage);
        }


        /// <summary>
        /// Get the traducted Audio.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task<AudioClip> GetTradVoice(DatalocationField field)
        {
            return await LocalisationManager.AudioData(IdTrad, field, TradType, PulseEngineMgr.currentLanguage);
        }

        #endregion
    }
}