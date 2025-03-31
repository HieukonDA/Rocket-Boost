using UnityEngine;
using UnityEngine.EventSystems;

public class EventSystemManager
{
    public EventSystemManager(GameObject parent)
    {
        EventSystem existingEventSystem = GameObject.FindFirstObjectByType<EventSystem>();
        if (existingEventSystem != null && existingEventSystem.gameObject != parent)
        {
            Object.Destroy(existingEventSystem.gameObject);
        }
        if (!parent.GetComponent<EventSystem>())
        {
            parent.AddComponent<EventSystem>();
            parent.AddComponent<StandaloneInputModule>();
        }
    }
}