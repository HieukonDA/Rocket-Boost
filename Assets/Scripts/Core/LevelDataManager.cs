using UnityEngine;

public class LevelDataManager : MonoBehaviour
{
    private Vector3 lastPlayerPosition;
    private bool isDead;

    public void SaveLevelData(int currentLevel, int currentSeed, Vector3 lastPlayerPosition, bool isDead)
    {
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        if (currentLevel > 3)
        {
            PlayerPrefs.SetInt("CurrentSeed", currentSeed);
            PlayerPrefs.SetFloat("PlayerX", lastPlayerPosition.x);
            PlayerPrefs.SetFloat("PlayerY", lastPlayerPosition.y);
            PlayerPrefs.SetFloat("PlayerZ", lastPlayerPosition.z);
            PlayerPrefs.SetInt("IsDead", isDead ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    public void LoadLevelData(out int currentLevel, out int currentSeed, out Vector3 lastPlayerPosition, out bool isDead)
    {
        currentLevel = PlayerPrefs.GetInt("CurrentLevel", 1);
        if (currentLevel > 3)
        {
            currentSeed = PlayerPrefs.GetInt("CurrentSeed", (int)System.DateTime.Now.Ticks);
            lastPlayerPosition = new Vector3(
                PlayerPrefs.GetFloat("PlayerX", 0f),
                PlayerPrefs.GetFloat("PlayerY", 0f),
                PlayerPrefs.GetFloat("PlayerZ", 0f)
            );
            isDead = PlayerPrefs.GetInt("IsDead", 0) == 1;
        }
        else
        {
            currentSeed = 0;
            lastPlayerPosition = Vector3.zero;
            isDead = false;
        }
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
}