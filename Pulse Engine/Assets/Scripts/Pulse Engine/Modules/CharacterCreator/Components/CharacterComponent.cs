using PulseEngine.Datas;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AddressableAssets;

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
                characterName = await Core.ManagerAsyncMethod<string>(ModulesManagers.Localisator, "TextData", new object[] { (object)data.TradLocation, (object)DatalocationField.title, (object)false });
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
            GameObject body = Instantiate(data.Character);
            body.transform.SetParent(transform);
            body.transform.localPosition = Vector3.zero;
            body.transform.localRotation = Quaternion.identity;
            animator = body.GetComponent<Animator>();
            if (animator == null)
                animator = body.AddComponent<Animator>();
            animator.avatar = data.AnimatorAvatar;
            animator.runtimeAnimatorController = data.AnimatorController;
            //animator.applyRootMotion = false;
            if (!gameObject.activeSelf)
                gameObject.SetActive(true);
            return true;
        }
    }
}
