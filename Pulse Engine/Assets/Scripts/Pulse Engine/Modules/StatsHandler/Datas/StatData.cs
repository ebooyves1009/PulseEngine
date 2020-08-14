using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Globals;
using PulseEngine.Modules;



namespace PulseEngine.Modules.StatHandler
{
    /// <summary>
    /// La data statistique associee a des objets.
    /// </summary>
    [System.Serializable]
    public class StatData
    {
        #region Attributs #########################################################

        [SerializeField]
        private float sante;
        [SerializeField]
        private float santeMax;
        [SerializeField]
        private float intelligence;
        [SerializeField]
        private float sagesse;
        [SerializeField]
        private float force;
        [SerializeField]
        private float endurance;
        [SerializeField]
        private float enduranceMax;
        [SerializeField]
        private float souffle;
        [SerializeField]
        private float souffleMax;
        [SerializeField]
        private float dexterite;
        [SerializeField]
        private float masse;
        [SerializeField]
        private float taille;
        [SerializeField]
        private float age;
        [SerializeField]
        private float karma;
        [SerializeField]
        private float paranormal;
        [SerializeField]
        private float fierte;
        [SerializeField]
        private float engoument;

        #endregion

        #region Propriete #########################################################

        /// <summary>
        /// quantite de vie d'un objet.
        /// </summary>
        public float Sante { get { return sante; } set { sante = Mathf.Clamp(value, 0, santeMax); } }

        /// <summary>
        /// quantite de vie Maximale d'un objet.
        /// </summary>
        public float SanteMax { get { return santeMax; } set { santeMax = value; } }

        /// <summary>
        /// capacite a apprendre.
        /// </summary>
        public float Intelligence { get { return intelligence; } set { intelligence = value; } }

        /// <summary>
        /// experiences du vecu.
        /// </summary>
        public float Sagesse { get { return sagesse; } set { sagesse = value; } }

        /// <summary>
        /// Aptitude physiques.
        /// </summary>
        public float Force { get { return force; } set { force = value; } }

        /// <summary>
        /// Capacite a deployer des aptitudes sur une periode
        /// </summary>
        public float Endurance { get { return endurance; } set { endurance = Mathf.Clamp(value, 0, enduranceMax); } }

        /// <summary>
        /// Capacite Maximale a deployer des aptitudes sur une periode
        /// </summary>
        public float EnduranceMax { get { return enduranceMax; } set { enduranceMax = value; } }

        /// <summary>
        /// Capacite a se priver d'air.
        /// </summary>
        public float Souffle { get { return souffle; } set { souffle = Mathf.Clamp(value, 0, souffleMax); } }

        /// <summary>
        /// Capacite Maximal a se priver d'air.
        /// </summary>
        public float SouffleMax { get { return souffleMax; } set { souffleMax = value; } }

        /// <summary>
        /// Maniement, Talent.
        /// </summary>
        public float Dexterite { get { return dexterite; } set { dexterite = value; } }

        /// <summary>
        /// La masse de cet objet.
        /// </summary>
        public float Masse { get { return masse; } set { masse = value; } }

        /// <summary>
        /// La taille a la verticale d'un objet.
        /// </summary>
        public float Taille { get { return taille; } set { taille = value; } }

        /// <summary>
        /// L'age de l'objet.
        /// </summary>
        public float Age { get { return age; } set { age = value; } }

        /// <summary>
        /// Le rapport du bien fait/mal fait; exprimant aussi la chance.
        /// </summary>
        public float Karma { get { return karma; } set { karma = value; } }

        /// <summary>
        /// La magie, sorcellerie, capacite a effectuer des actions paranormales.
        /// </summary>
        public float Paranormal { get { return paranormal; } set { paranormal = value; } }

        /// <summary>
        /// La fierte, tendance a ne pas abandonner, capacite a ne pas etre effraye.
        /// </summary>
        public float Fierte { get { return fierte; } set { fierte = value; } }

        /// <summary>
        /// Exitation, aptitude a vouloir faire quelque chose dans les plus bref delais.
        /// </summary>
        public float Engoument { get { return engoument; } set { engoument = value; } }

        #endregion

        #region Methods #########################################################

        /// <summary>
        /// retourne la valeur d'une stat.
        /// </summary>
        /// <param name="_statType"></param>
        /// <returns></returns>
        public float GetStat(StatType _statType)
        {
            switch (_statType)
            {
                case StatType.Sante:
                    return Sante;
                case StatType.Intelligence:
                    return Intelligence;
                case StatType.Sagesse:
                    return Sagesse;
                case StatType.Force:
                    return Force;
                case StatType.Endurance:
                    return Endurance;
                case StatType.Souffle:
                    return Souffle;
                case StatType.Dexterite:
                    return Dexterite;
                case StatType.Masse:
                    return Masse;
                case StatType.Taille:
                    return Taille;
                case StatType.Age:
                    return Age;
                case StatType.Karma:
                    return Karma;
                case StatType.Paranormal:
                    return Paranormal;
                case StatType.Fierte:
                    return Fierte;
                case StatType.Engoument:
                    return Engoument;
                default:
                    return sante;
            }
        }

        /// <summary>
        /// Defini la valeur d'une stat.
        /// </summary>
        /// <param name="_statType"></param>
        /// <returns></returns>
        public void SetStat(StatType _statType, float value)
        {
            switch (_statType)
            {
                case StatType.Sante:
                    Sante = value;
                    break;
                case StatType.Intelligence:
                    Intelligence = value;
                    break;
                case StatType.Sagesse:
                    Sagesse = value;
                    break;
                case StatType.Force:
                    Force = value;
                    break;
                case StatType.Endurance:
                    Endurance = value;
                    break;
                case StatType.Souffle:
                    Souffle = value;
                    break;
                case StatType.Dexterite:
                    Dexterite = value;
                    break;
                case StatType.Masse:
                    Masse = value;
                    break;
                case StatType.Taille:
                    Taille = value;
                    break;
                case StatType.Age:
                    Age = value;
                    break;
                case StatType.Karma:
                    Karma = value;
                    break;
                case StatType.Paranormal:
                    Paranormal = value;
                    break;
                case StatType.Fierte:
                    Fierte = value;
                    break;
                case StatType.Engoument:
                    Engoument = value;
                    break;
                default:
                    sante = value;
                    break;
            }
        }

        #endregion
    }

}
