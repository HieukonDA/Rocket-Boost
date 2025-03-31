using UnityEngine;

public interface ILevelManager
{
    void RestartLevel();
    void OnPlayerDeath(Vector3 deathPosition);
    void OnLevelComplete();
}