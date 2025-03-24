using Unity.VisualScripting;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    public static GameManager Instance { get; private set; }
    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        Application.targetFrameRate = 30;
        Debug.Log("Target FPS í on");
        if (AudioManager.Instance != null)
        {
            AudioManager.Instance.PlayMusic("theme");
        }
        else
        {
            Debug.LogError("AudioManager is null!");
        }

        DontDestroyOnLoad(gameObject);
    }
}

