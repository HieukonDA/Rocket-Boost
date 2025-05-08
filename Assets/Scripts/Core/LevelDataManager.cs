using UnityEngine;

public class LevelDataManager : MonoBehaviour
{
    private Vector3 lastPlayerPosition;
    private bool isDead;
    private int coinCount;

    public void SaveLevelData(int currentLevel, int currentSeed, Vector3 lastPlayerPosition, bool isDead, int coinCount)
    {
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        PlayerPrefs.SetInt("CurrentSeed", currentSeed);
        PlayerPrefs.SetFloat("PlayerX", lastPlayerPosition.x);
        PlayerPrefs.SetFloat("PlayerY", lastPlayerPosition.y);
        PlayerPrefs.SetFloat("PlayerZ", lastPlayerPosition.z);
        PlayerPrefs.SetInt("IsDead", isDead ? 1 : 0);
        PlayerPrefs.SetInt("CoinCount", coinCount);
        PlayerPrefs.Save();
    }

    public void LoadLevelData(out int currentLevel, out int currentSeed, out Vector3 lastPlayerPosition, out bool isDead, out int coinCount)
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        currentSeed = PlayerPrefs.GetInt("CurrentSeed", (int)System.DateTime.Now.Ticks);
        lastPlayerPosition = new Vector3(
            PlayerPrefs.GetFloat("PlayerX", 0f),
            PlayerPrefs.GetFloat("PlayerY", 0f),
            PlayerPrefs.GetFloat("PlayerZ", 0f)
        );
        isDead = PlayerPrefs.GetInt("IsDead", 0) == 1;
        coinCount = PlayerPrefs.GetInt("CoinCount", 0);
        this.coinCount = coinCount;
        Debug.Log($"LevelDataManager: Loaded CoinCount = {coinCount}");
    }

    public void SetLastPlayerPosition(Vector3 position)
    {
        lastPlayerPosition = position;
    }

    public Vector3 GetLastPlayerPosition()
    {
        return lastPlayerPosition;
    }

    public void SetIsDead(bool value)
    {
        isDead = value;
    }

    public bool GetIsDead()
    {
        return isDead;
    }

    public void SetCoinCount(int value)
    {
        coinCount = value;
        PlayerPrefs.SetInt("CoinCount", coinCount);
        PlayerPrefs.Save();
        Debug.Log($"LevelDataManager: Set CoinCount = {coinCount}");
    }

    public int GetCoinCount()
    {
        return coinCount;
    }
}