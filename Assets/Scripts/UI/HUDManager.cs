using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }
    [SerializeField] private Text scoreText;
    [SerializeField] private GameObject scoreTextHUB; // Prefab cho ScoreHUB

    private int finalScoreText = 0;
    private int coinCount;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateScore(int newcoinCount)
    {
        coinCount = newcoinCount;
        scoreText.text = $"Coins: {newcoinCount}";
    }

    public void ControllActiveScoreText(bool isActive)
    {
        if (scoreTextHUB == null)
        {
            Debug.LogError("ScoreHUB not found in the scene.");
            return;
        }

        scoreTextHUB.SetActive(isActive);
    }

    public int GetScore()
    {
        finalScoreText += coinCount;
        return finalScoreText;
    }
}