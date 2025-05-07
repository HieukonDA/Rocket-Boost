using System.Collections.Generic;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;

public class LevelGenerator : MonoBehaviour, ILevelGenerator
{
    [SerializeField] private ObstacleConfig config;

    [SerializeField] private int levelNumber = 1;
    [SerializeField] private float spawnDistance = 45f;
    [SerializeField] private float despawnDistance = 250f;
    [SerializeField] private float levelHeight = 10f;

    //prefab cuar skill 
    [SerializeField] private GameObject skillCirclePrefab;

    private ObjectPool<GameObject> segmentPool;
    private List<GameObject> activeSegments = new List<GameObject>();
    private Transform player;
    private GameObject launchSegmentInstance;
    private ICoinManager coinManager; // Thêm CoinManager vào đây
    private SkillGenerator skillGenerator;

    private int segmentsPerLevel;
    private int spawnedSegmentCount;
    private int currentLevel; // Lưu levelNumber từ LevelManager
    private int currentSeed;

    private void Awake()
    {
        segmentPool = new ObjectPool<GameObject>(() => InstantiateMiddleSegment(), config.middleSegmentPrefabs.Length, transform);
        segmentsPerLevel = CalculateSegmentsPerLevel(levelNumber);
        coinManager = CoinManager.Instance; // Lấy CoinManager từ scene
        skillGenerator = new SkillGenerator(skillCirclePrefab, coinManager); // Khởi tạo SkillGenerator
    }

    private void Update()
    {
        if (player != null) // Chỉ chạy khi player đã được sinh
        {
            ManageSegments();
        }
    }

    private void ManageSegments()
    {
        float playerX = player.position.x;
        Transform lastSpawnPoint = GetLastSpawnPoint();

        if (spawnedSegmentCount < segmentsPerLevel && lastSpawnPoint.position.x < playerX + spawnDistance)
        {
            if (spawnedSegmentCount == segmentsPerLevel - 1)
            {
                SpawnFinishSegment(lastSpawnPoint);
            }
            else
            {
                SpawnNextMiddleSegment(lastSpawnPoint);
            }
        }

        for (int i = activeSegments.Count - 1; i >= 0; i--)
        {
            if (activeSegments[i].transform.position.x < playerX - despawnDistance)
            {
                segmentPool.Return(activeSegments[i]);
                activeSegments.RemoveAt(i);
            }
        }
    }

    public void GenerateLevel(int levelNumber, int seed)
    {
        currentLevel = levelNumber; // Level 1 = Level 4 trong LevelManager
        currentSeed = seed;
        segmentsPerLevel = CalculateSegmentsPerLevel(levelNumber);

        CleanupExistingLevel(); // Xóa các đoạn cũ và player cũ nếu có

        spawnedSegmentCount = 0;
        activeSegments.Clear();

        skillGenerator.ResetSkillCount(); // Reset số lượng skill đã sinh

        Random.InitState(seed); // Khởi tạo seed từ LevelManager
        SpawnLaunchSegment(); // Sinh đoạn launch
        SpawnPlayer(); // Sinh player trong scene procedural
    }

    private void CleanupExistingLevel()
    {
        // Xóa player cũ nếu có
        if (player != null)
        {
            Destroy(player.gameObject);
            player = null;
        }

        // Xóa các đoạn không pooled (launch và finish)
        foreach (GameObject segment in activeSegments)
        {
            if (!segmentPool.Contains(segment)) // Chỉ xóa nếu không thuộc pool
            {
                Debug.Log("Destroying non-pooled segment: " + segment.name);
                Destroy(segment);
            }
            else
            {
                segmentPool.Return(segment); // Trả về pool nếu là đoạn giữa
            }
        }
        activeSegments.Clear();
        launchSegmentInstance = null;

        // Deactive tất cả coin trước khi sinh level mới
        if (coinManager != null)
        {
            coinManager.CleanupExistingCoin();
            Debug.Log("LevelGenerator: Deactivated all coins before cleanup.");
        }
        else
        {
            Debug.LogWarning("LevelGenerator: coinManager is null during cleanup!");
        }

        // Kiểm tra chắc chắn không còn đoạn đầu nào trong scene
        GameObject existingLaunch = GameObject.Find("LaunchSegment(Clone)"); // Tên mặc định khi instantiate
        if (existingLaunch != null)
        {
            Debug.LogWarning("Found leftover launch segment in scene! Destroying it.");
            Destroy(existingLaunch);
        }
    }

    private void SpawnLaunchSegment()
    {
        if (launchSegmentInstance != null)
        {
            Debug.LogWarning("Launch segment already exists! Skipping spawn.");
            return; // Ngăn sinh lần thứ hai
        }

        launchSegmentInstance = Instantiate(config.launchSegmentPrefab, Vector3.zero, Quaternion.identity, transform);
        launchSegmentInstance.SetActive(true);
        activeSegments.Add(launchSegmentInstance);
        spawnedSegmentCount++;

        coinManager?.SpawnCoins(launchSegmentInstance.transform);
    }

    private void SpawnNextMiddleSegment(Transform previousSpawnPoint)
    {
        GameObject segment = segmentPool.Get();
        segment.transform.position = previousSpawnPoint.position;
        segment.transform.rotation = previousSpawnPoint.rotation;
        segment.SetActive(true);
        activeSegments.Add(segment);
        spawnedSegmentCount++;

        coinManager?.SpawnCoins(segment.transform);
        
        // Sinh skill circle tại segment chia hết cho 5
        skillGenerator.TrySpawnSkill(segment, spawnedSegmentCount);
    }

    private void SpawnFinishSegment(Transform previousSpawnPoint)
    {
        GameObject finishSegment = Instantiate(config.finishSegmentPrefab, previousSpawnPoint.position, previousSpawnPoint.rotation, transform);
        finishSegment.SetActive(true);
        activeSegments.Add(finishSegment);
        spawnedSegmentCount++;

       coinManager?.SpawnCoins(finishSegment.transform);
    }

    private Transform GetLastSpawnPoint()
    {
        GameObject lastSegment = activeSegments[activeSegments.Count - 1];
        return lastSegment.transform.Find("SpawnPoint");
    }

    private GameObject InstantiateMiddleSegment()
    {
        int index = Random.Range(0, config.middleSegmentPrefabs.Length); // Luôn ngẫu nhiên vì seed đã được set
        GameObject segment = Instantiate(config.middleSegmentPrefabs[index], Vector3.zero, Quaternion.identity);
        segment.SetActive(false);
        return segment;
    }

    private void SpawnPlayer()
    {
        if (launchSegmentInstance == null)
        {
            Debug.LogError("Launch segment not spawned yet!");
            return;
        }

        // Tìm child "Launch" trong đoạn đầu
        Transform launchPos = launchSegmentInstance.transform.Find("Launch");
        if (launchPos == null)
        {
            Debug.LogWarning("Child 'Launch' not found in launchSegmentPrefab, using SpawnPoint instead.");
            launchPos = launchSegmentInstance.transform.Find("SpawnPoint"); // Fallback nếu không có "Launch"
        }

        if (launchPos == null)
        {
            Debug.LogError("No 'Launch' or 'SpawnPoint' found in launchSegmentPrefab! Spawning at default position.");
            launchPos = launchSegmentInstance.transform; // Dùng vị trí gốc của đoạn đầu nếu không tìm thấy
        }

        Vector3 spawnPosition = launchPos.position;
        GameObject playerInstance = Instantiate(config.playerPrefab, spawnPosition + new Vector3(0,5,0), Quaternion.identity, transform);
        playerInstance.tag = "Player";
        player = playerInstance.transform;
        FindFirstObjectByType<CameraManager>()?.SetTarget(player); // Camera sẽ được set trong OnSceneLoaded, nhưng để chắc chắn
        Debug.LogError("Player spawned: " + playerInstance.name);
    }

    private void SpawnSkillCircle(Transform segment)
    {
        GameObject skillCircle = Instantiate(config.skillCirclePrefab, segment);
        skillCircle.transform.localPosition = new Vector3(15f, levelHeight / 2, 0);
    }

    private int CalculateSegmentsPerLevel(int level)
    {
        return level + 2;
    }

}

