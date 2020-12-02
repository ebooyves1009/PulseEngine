using System.Threading.Tasks;
using UnityEngine;

namespace PulseEngine.Modules.Components
{

    /// <summary>
    /// The Character component attached to a character on the scene.
    /// </summary>
    public class CharacterComponent: MonoBehaviour
    {
        public string characterName;
        public CharacterData data;

        public bool Initialize(CharacterData _data)
        {
            if (_data == null)
                return false;
            data = _data;
            Task namingTask = new Task(async () =>
            {
                //characterName = await Core.ManagerAsyncMethod<string>(ModulesManagers.Localisator, "TextData", new object[] { (object)data.TradLocation, (object)DatalocationField.title, (object)false });
                if (!string.IsNullOrEmpty(characterName))
                {
                    gameObject.name = characterName;
                    if(Camera.main != null)
                    {
                        Camera.main.transform.LookAt(transform);
                    }
                }
            });
            namingTask.RunSynchronously();
            var animator = GetComponentInChildren<Animator>();
            if (animator != null)
            {
                Destroy(animator.gameObject);
            }
            //animator.applyRootMotion = false;
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            return true;
        }
    }
}
