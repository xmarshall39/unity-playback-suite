using UnityEngine;
using UnityEngine.EventSystems;

namespace UPBS.UI
{
    public class PBAutoEventSystem : MonoBehaviour
    {
        void Start()
        {
            if(GameObject.FindObjectOfType<EventSystem>() == null)
            {
                var go = new GameObject();
                go.AddComponent<EventSystem>();
                go.AddComponent<StandaloneInputModule>();
                go.transform.parent = this.transform.parent;
            }
        }
    }
}

