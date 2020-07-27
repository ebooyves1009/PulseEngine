using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PulseEngine.Core;


//TODO: Continuer d'implementer des fonction au fil des besoins du module, sans oublier les fonction d'access aux datas inGame.
namespace PulseEngine.Module.Localisator
{
    /// <summary>
    /// Le Manager du Module Localisator.
    /// </summary>
    public static class LocalisationManager
    {
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
    }
}
