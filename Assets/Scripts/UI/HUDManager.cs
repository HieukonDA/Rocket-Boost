using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SocialPlatforms.Impl;
using UnityEngine.UI;

public class HUDManager : MonoBehaviour
{
    public static HUDManager Instance { get; private set; }
    [SerializeField] private Text scoreText;
    [SerializeField] private Image timerImage; // Prefab cho ScoreText
    [SerializeField] private Text timerText; // Prefab cho ScoreText
    [SerializeField] private GameObject scoreTextHUB; // Prefab cho ScoreHUB
    [SerializeField] private Text levelText; // Prefab cho GameOverPanel

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

    public void UpdateTimer(float timeLeft, float totalTime)
    {
        if (timerImage == null)
        {
            Debug.LogError("TimerImage not found in the scene.");
        }

        if (timeLeft > 0)
        {
            timerImage.gameObject.SetActive(true); // Hiện thanh thời gian
            float fillRatio = Mathf.InverseLerp(0, totalTime, timeLeft);
            timerImage.fillAmount = fillRatio; // Cập nhật hình ảnh thanh thời gian
            timerText.text = $"Time: {Mathf.Ceil(timeLeft)}s";
        }
        else
        {
            Debug.LogError("Time is up!");
            timerImage.fillAmount = 0; // Đặt lại thanh thời gian khi hết thời gian
            timerImage.gameObject.SetActive(false); // Ẩn thanh thời gian
        }
    }

    public Image GetImageTimer()
    {
        return timerImage;
    }

    public void SetLevelText(int level)
    {
        if (levelText == null)
        {
            Debug.LogError("LevelText not found in the scene.");
            return;
        }
        levelText.text = $"LV {level}";
    }
}