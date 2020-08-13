using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Core;
using PulseEngine.Module.Anima;
using System.Threading.Tasks;

namespace PulseEngine.Module.CombatSystem
{
    public static class CombatSystemManager
    {
        #region Enums ####################################################################

        /// <summary>
        /// Le type de degat qu'une arme inflige
        /// </summary>
        public enum TypeDegatArme
        {
            Impact,
            Cut,
            Piercing,
        }

        /// <summary>
        /// Le type d'une attaque, l'effet qu'elle provoque a l'adversaire dans la meilleurs conditions.
        /// </summary>
        public enum AttackType
        {
            Simple,
            Push,
            Pull,
            Downward,
            Upward,
            SpinAway,
            Pierce
        }

        /// <summary>
        /// La hauteur d'une attaque.
        /// </summary>
        public enum AttackHeight
        {
            middle,
            above,
            below
        }
        
        /// <summary>
        /// Le type d'une defense, le zones couvertes.
        /// </summary>
        public enum DefenseType
        {
            overral,
            forward,
            backward,
            Downward,
            Upward,
            left,
            right
        }

        /// <summary>
        /// Le type d'une arme.
        /// </summary>
        public enum WeaponType
        {
            aucun,
            shortRange,
            LongRange
        }

        #endregion
        #region Structures ####################################################################

        /// <summary>
        /// L'emplacement d'une arme sur le corps.
        /// </summary>
        [System.Serializable]
        public struct WeaponPlace
        {
            [SerializeField]
            private int parentBone;
            public Vector3 PositionOffset;
            public Quaternion RotationOffset;

            public HumanBodyBones ParentBone { get { return (HumanBodyBones)parentBone; } set { parentBone = (int)value; } }
        }

        /// <summary>
        /// Le donnes d'evenement d'animation d'une attaque.
        /// </summary>
        [System.Serializable]
        public struct AttackEvent
        {
            private int type;
            private int height;

            public PulseCore_GlobalValue_Manager.TimeStamp timeStamp;
            public AttackType Type{get{ return (AttackType)type; }set{ type = (int)value; }}
            public AttackHeight Height{get{ return (AttackHeight)height; }set{ height = (int)value; }}
        }

        #endregion
        #region Nested Classes ####################################################################

        #endregion
        #region Attributes ####################################################################

        /// <summary>
        /// Le sous repertoire de sauvegarde des assets d'arme.
        /// </summary>
        public static string AssetsPath { get => "Combat Assets"; }

        #endregion
        #region Methods ####################################################################

        /// <summary>
        /// recupere une arme a base de son ID.
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        //public static async Task<WeaponData> GetWeapon(int id)
        //{
        //    throw new System.NotImplementedException();
        //}

        #endregion
        #region Extension&Helpers ####################################################################

        #endregion
    }
}