using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Core;
using PulseEngine.Module.StatHandler;
using PulseEngine.Module.Localisator;
using System.Threading.Tasks;
using PulseEngine.Module.CombatSystem;

namespace PulseEngine.Module.CharacterCreator
{
    /// <summary>
    /// La Data d'un character.
    /// </summary>
    [System.Serializable]
    public class CharacterData : ITraductible
    {
        #region Attributs #########################################################

        [SerializeField]
        private int id;
        [SerializeField]
        private int idTrad;
        [SerializeField]
        private int tradDataType;
        [SerializeField]
        private StatData stats;
        [SerializeField]
        private GameObject character;
        [SerializeField]
        private RuntimeAnimatorController animatorController;
        [SerializeField]
        private Avatar animatorAvatar;
        [SerializeField]
        private List<Vector2Int> armurie;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// l'id dans BD.
        /// </summary>
        public int ID { get => id; set => id = value; }

        /// <summary>
        /// l'id des donnees de localisation.
        /// </summary>
        public int IdTrad { get => idTrad; set => idTrad = value; }

        /// <summary>
        /// le type de data de localisation.
        /// </summary>
        public PulseCore_GlobalValue_Manager.DataType TradDataType { get => (PulseCore_GlobalValue_Manager.DataType)tradDataType; set => tradDataType = (int)value; }

        /// <summary>
        /// Les stats du character.
        /// </summary>
        public StatData Stats{get=>stats;set=>stats = value;}

        /// <summary>
        /// Le prefab du character.
        /// </summary>
        public GameObject Character{get=>character;set=>character = value;}

        /// <summary>
        /// Controller d'animator du character, ses mouvements de base.
        /// </summary>
        public RuntimeAnimatorController AnimatorController{get=>animatorController;set=>animatorController = value;}

        /// <summary>
        /// L'avatar de la disposition des bones du character si il est humanoid.
        /// </summary>
        public Avatar AnimatorAvatar{get=>animatorAvatar;set=>animatorAvatar = value;}

        /// <summary>
        /// La liste des armes detenues par le personnage.
        /// </summary>
        public List<Vector2Int> Armurie{ get => armurie; set => armurie = value;}

        #endregion

        #region Methods #########################################################

        /// <summary>
        /// Recupere les donnees de localisation.
        /// </summary>
        /// <param name="field"></param>
        /// <returns></returns>
        public async Task<string> GetTradText(LocalisationManager.DatalocationField field)
        {
            return await LocalisationManager.TextData(idTrad, field, tradDataType, (int)PulseCore_GlobalValue_Manager.currentLanguage);
        }

        #endregion
    }
}