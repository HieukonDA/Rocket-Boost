using UnityEngine;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
   public static HUDManager Instance { get; private set; }
    [SerializeField] private Text scoreText;

    private int score;

    private void Awake()
    {
        if (Instance != null) { Destroy(gameObject); return; }
        Instance = this;
        DontDestroyOnLoad(gameObject);
    }

    public void UpdateScore(int newScore)
    {
        score = newScore;
        scoreText.text = $"Coins: {score}";
    }

    public int GetScore() => score;
}