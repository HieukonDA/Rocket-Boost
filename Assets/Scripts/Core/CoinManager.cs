using System.Collections.Generic;
using UnityEngine;

public class CoinManager : MonoBehaviour, ICoinManager
{
    public static CoinManager Instance { get; private set; }

    [SerializeField] private GameObject coinPrefab;

    private ObjectPool<GameObject> coinPool;
    private int coinCount = 0;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        
        coinPool = new ObjectPool<GameObject>(() => InstantiateCoin(), 20, transform); // Pool 20 coin ban đầu
    }

    public void SpawnCoins(Transform segment)
    {
        Debug.LogWarning($"CoinManager: SpawnCoins called for segment: {segment.name}");
       // Tìm CoinSpawnPoints trong segment
        Transform spawnPointsParent = segment.Find("CoinSpawnPoints");
        if (spawnPointsParent == null)
        {
            Debug.LogWarning("No CoinSpawnPoints found in segment: " + segment.name);
            return;
        }

        // Lấy tất cả CoinPoint
        List<Transform> coinPoints = new List<Transform>();
        foreach (Transform child in spawnPointsParent)
        {
            if (child.CompareTag("CoinPoint"))
            {
                coinPoints.Add(child);
            }
        }

        if (coinPoints.Count == 0)
        {
            Debug.LogWarning("No CoinPoints found in CoinSpawnPoints!");
            return;
        }

        // Sinh coin tại tất cả CoinPoint
        foreach (Transform point in coinPoints)
        {
            GameObject coin = coinPool.Get();
            coin.transform.position = point.position; // Dùng vị trí toàn cục của CoinPoint
            coin.transform.rotation = Quaternion.identity;
            coin.SetActive(true);
            Debug.LogWarning("Coin spawned at: " + coin.transform.position);
        }
    }

    public void CollectCoin()
    {
        coinCount++;
        HUDManager.Instance.UpdateScore(coinCount); 
        Debug.Log("Coin collected! Total: " + coinCount);
    }

    public int GetCoinCount()
    {
        return coinCount;
    }

    private GameObject InstantiateCoin()
    {
        GameObject coin = Instantiate(coinPrefab);
        coin.tag = "Coin";
        coin.SetActive(false);
        return coin;
    }

    public void CleanupExistingCoin()
    {
        foreach (GameObject coin in coinPool.GetActiveObjects())
        {
            if (coin.activeSelf)
            {
                coin.SetActive(false);
                Debug.Log("CoinManager: Deactivated coin at: " + coin.transform.position);
            }
        }
    }

    public void ResetCoinCount()
    {
        coinCount = 0;
        Debug.Log("CoinManager: Coin count reset to 0.");
    }

    public GameObject FindCoinAtPosition(Vector3 position)
    {
        foreach (GameObject coin in coinPool.GetActiveObjects())
        {
            if (coin.activeSelf && Vector3.Distance(coin.transform.position, position) < 0.01f)
            {
                return coin; // Tìm thấy coin active tại vị trí
            }
        }
        return null; // Không tìm thấy
    }
}