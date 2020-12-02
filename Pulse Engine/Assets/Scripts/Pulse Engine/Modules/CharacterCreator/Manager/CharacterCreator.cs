using PulseEngine.Modules.Components;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace PulseEngine.Modules.CharacterCreator
{
    /// <summary>
    /// Le manager du mondule de creation de charactere.
    /// </summary>
    public static class CharacterCreator
    {
        #region Attributes ####################################################################

        /// <summary>
        /// Le pool des charcters dans la scene.
        /// </summary>
        public static List<CharacterComponent> CharactersPool { get; private set; }

        /// <summary>
        /// Active when the manager is ready.
        /// </summary>
        public static bool Ready { get; private set; }

        #endregion

        #region Methods ####################################################################

        /// <summary>
        /// Called on domain reload, to decrease enter playmode time.
        /// </summary>
        [RuntimeInitializeOnLoadMethod]
        private static void OnDomainReload()
        {
            Ready = false;
            ClearPool();
            Initialisation();
        }

        /// <summary>
        /// Initialise the module Manager.
        /// </summary>
        /// <returns></returns>
        public static void Initialisation()
        {

        }

        /// <summary>
        /// Spawn a character.
        /// </summary>
        /// <param name="position"></param>
        /// <param name="rotation"></param>
        /// <param name="parent"></param>
        public static async Task SpawnCharacter(DataLocation _charDataLoc, CancellationToken ct,  Vector3 position = default, Quaternion rotation = default, Transform parent = null)
        {
            CharacterData data = await CoreLibrary.GetData<CharacterData>(_charDataLoc, ct);
            if (data == null)
                return;
            if (CharactersPool == null)
                CharactersPool = new List<CharacterComponent>();
            if(CharactersPool.Count > 0)
            {
                var character = CharactersPool.Find(ch => { return !ch.gameObject.activeSelf; });
                if (character != null)
                {
                    character.Initialize(data);
                    character.transform.position = position;
                    character.transform.rotation = rotation;
                    character.transform.SetParent(parent);
                    return;
                }
            }
            GameObject newCharO = new GameObject("NewCharacter_"+ CharactersPool.Count);
            CharacterComponent newChar = newCharO.AddComponent<CharacterComponent>();
            newChar.transform.position = position;
            newChar.transform.rotation = rotation;
            newChar.transform.SetParent(parent);
            CharactersPool.Add(newChar);
            newChar.Initialize(data);
        }

        /// <summary>
        /// Get a character in CharacterPool
        /// </summary>
        /// <returns></returns>
        public static CharacterComponent GetCharacter(int ID)
        {
            return null;
        }

        /// <summary>
        /// Get a character in CharacterPool
        /// </summary>
        /// <returns></returns>
        public static CharacterComponent GetCharacter(string name)
        {
            return null;
        }

        /// <summary>
        /// Get a character in CharacterPool closest to a position
        /// </summary>
        /// <returns></returns>
        public static CharacterComponent GetClosestCharacter(Vector3 pos)
        {
            return null;
        }

        /// <summary>
        /// Get a random character in CharacterPool
        /// </summary>
        /// <returns></returns>
        public static CharacterComponent GetRandomCharacter(Vector3 pos)
        {
            return null;
        }

        /// <summary>
        /// Clear the CharacterPool
        /// </summary>
        /// <returns></returns>
        public static void ClearPool()
        {
            if (CharactersPool == null)
                return;
            CharactersPool.Clear();
        }

        #endregion

        #region Extension&Helpers ####################################################################


        #endregion
    }
}