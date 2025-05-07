using UnityEngine;
using System.Collections.Generic;

public interface ICoinManager
{
    void SpawnCoins(Transform segment); // Sinh coin dựa trên CoinSpawnPoints
    void CollectCoin();
    int GetCoinCount();
    void CleanupExistingCoin();
    GameObject FindCoinAtPosition(Vector3 position);
}