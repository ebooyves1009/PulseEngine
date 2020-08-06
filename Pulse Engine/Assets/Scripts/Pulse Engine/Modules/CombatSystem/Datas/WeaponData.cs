using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Core;
using PulseEngine.Module.StatHandler;
using PulseEngine.Module.Anima;
using PulseEngine.Module.Localisator;
using System.Threading.Tasks;
using PulseEngine.Module.PhysicSpace;

namespace PulseEngine.Module.CombatSystem
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
        private List<AttackData> attackDatas = new List<AttackData>();
        [SerializeField]
        private List<DefenseData> defenseDatas = new List<DefenseData>();
        [SerializeField]
        private bool canParry;
        [SerializeField]
        private bool portable;
        [SerializeField]
        private List<CombatSystemManager.WeaponPlace> restPlaces = new List<CombatSystemManager.WeaponPlace>();
        [SerializeField]
        private List<CombatSystemManager.WeaponPlace> carryPlaces = new List<CombatSystemManager.WeaponPlace>();
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
        public PulseCore_GlobalValue_Manager.DataType TradDataType { get => (PulseCore_GlobalValue_Manager.DataType)tradDataType; set => tradDataType = (int)value; }

        /// <summary>
        /// Le type d'arme.
        /// </summary>
        public CombatSystemManager.WeaponType TypeArme { get { return (CombatSystemManager.WeaponType)typeArme; } set { typeArme = (int)value; } }

        /// <summary>
        /// La portee de l'arme.
        /// </summary>
        public float Portee { get => portee; set => portee = value; }

        /// <summary>
        /// le type de degat que l'arme inflige.
        /// </summary>
        public CombatSystemManager.TypeDegatArme TypeDegats { get => (CombatSystemManager.TypeDegatArme)typeDegats; set => typeDegats = (int)value; }

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
        public List<PhysicManager.PhysicMaterials> Materiaux
        {
            get
            {
                List<PhysicManager.PhysicMaterials> retList = new List<PhysicManager.PhysicMaterials>();
                if (materiaux == null)
                    materiaux = new List<int>();
                foreach (var mat in materiaux)
                    retList.Add((PhysicManager.PhysicMaterials)mat);
                return retList;
            }
            set
            {
                List<int> retList = new List<int>();
                foreach (var mat in value)
                    retList.Add((int)mat);
                materiaux = retList;
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
        public List<CombatSystemManager.WeaponPlace> RestPlaces { get => restPlaces; set => restPlaces = value; }


        /// <summary>
        /// Les emplacements degaine de l'arme. un pour chaque object
        /// </summary>
        public List<CombatSystemManager.WeaponPlace> CarryPlaces { get => carryPlaces; set => carryPlaces = value; }

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
        public async Task<string> GetTradText(LocalisationManager.DatalocationField field)
        {
            return await LocalisationManager.TextData(IdTrad, field, (int)tradDataType, (int)PulseCore_GlobalValue_Manager.currentLanguage);
        }

        #endregion
    }
}