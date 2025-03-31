using System.Collections.Generic;
using UnityEngine;

public class LevelGenerator : MonoBehaviour, ILevelGenerator
{
    [SerializeField] private ObstacleConfig config;
    [SerializeField] private float levelWidth = 30f;
    [SerializeField] private float levelHeight = 10f;
    [SerializeField] private float levelDepth = 0f;
    [SerializeField] private int minObstacles = 2;
    [SerializeField] private int maxObstacles = 5;
    [SerializeField] private float safeZoneRadius = 2f;

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Vector3 startPos;
    private Vector3 goalPos;
    private float tunnelHeightBottom;

    // Khởi tạo interface
    private ISegmentGenerator IsegmentGenerator;
    private IObstacleSpawner IobstacleSpawner;
    private IPathChecker IpathChecker;
    private ILevelVisualizer IlevelVisualizer;

    private void Awake()
    {
        IsegmentGenerator = new SegmentGenerator();
        IobstacleSpawner = new ObstacleSpawner(config, transform, spawnedObjects, safeZoneRadius);
        IpathChecker = new PathChecker();
        IlevelVisualizer = new LevelVisualizer();
    }

    public void GenerateLevel(int levelNumber, int seed)
    {
        Debug.Log("GenerateLevel is created");
        ClearLevel();

        // Khóa chuỗi ngẫu nhiên bằng seed
        UnityEngine.Random.InitState(seed);

        // Tính số vật cản dựa trên level
        int obstacleCount = Mathf.RoundToInt(Mathf.Lerp(minObstacles, maxObstacles, levelNumber / 10f));
        obstacleCount = Mathf.Clamp(obstacleCount, minObstacles, maxObstacles);

        // Chia level thành các đoạn
        List<Segment> segments = IsegmentGenerator.GenerateSegments(levelNumber, levelWidth, levelHeight, levelDepth);

        // Lấy độ cao sàn đường hầm từ đoạn đầu tiên
        tunnelHeightBottom = UnityEngine.Random.Range(0f, levelHeight * 0.2f); // Sàn đường hầm (0%-30% chiều cao)

        // Điều chỉnh startPos và goalPos để nằm trên sàn
        startPos = segments[0].start;
        startPos.y = tunnelHeightBottom;
        goalPos = segments[segments.Count - 1].end;
        goalPos.y = tunnelHeightBottom;

        // Tính offset dựa trên chiều cao của các đối tượng (nếu cần)
        float startHeight = GetObjectHeight(config.startPrefab);
        float goalHeight = GetObjectHeight(config.goalPrefab);
        float playerHeight = GetObjectHeight(config.playerPrefab);

        // Điều chỉnh y để phần dưới của đối tượng nằm trên sàn
        startPos.y += startHeight / 2f;
        goalPos.y += goalHeight / 2f;

        // Đặt điểm xuất phát và hạ cánh
        GameObject startInstance = Instantiate(config.startPrefab, Vector3.zero, Quaternion.identity, transform);
        GameObject playerInstance = Instantiate(config.playerPrefab, startPos, Quaternion.identity, transform);
        GameObject goal = Instantiate(config.goalPrefab, goalPos, Quaternion.identity, transform);
        startInstance.transform.position = new Vector3(
            startInstance.transform.position.x,
            startPos.y + playerHeight / 2f + 20f,
            startInstance.transform.position.z
        );
        playerInstance.transform.position = startInstance.transform.position;
        goal.transform.position = new Vector3(
            goal.transform.position.x,
            goalPos.y + playerHeight / 2f + 20f,
            goalPos.z
        );
        Debug.Log("playerInstance spawned: " + (playerInstance != null ? playerInstance.name : "Null"));


        // Kiểm tra config có được gán không
        Debug.Log("Config: " + (config != null ? "Assigned" : "Null"));

        // Lưu các instance vào spawnedObjects
        spawnedObjects.Add(goal);
        spawnedObjects.Add(startInstance);
        spawnedObjects.Add(playerInstance);

        Debug.Log("Created: " + playerInstance.name + " with tag: " + playerInstance.tag);

        // Sinh vật cản cho từng đoạn
        foreach (Segment segment in segments)
        {
            // Bước 1: Sinh tường ngang liên tục để tạo đường hầm
            IobstacleSpawner.SpawnTunnelWalls(segment, levelHeight, tunnelHeightBottom);

            // Bước 2: Sinh các vật cản khác (tường dọc, bẫy laser, v.v.)
            int additionalObstacles = obstacleCount / segments.Count;
            int attempts = 0;
            int maxAttempts = 20;
            for (int i = 0; i < additionalObstacles && attempts < maxAttempts; i++)
            {
                if (IobstacleSpawner.SpawnAdditionalObstacle(segment, levelHeight))
                {
                    if (!IpathChecker.IsPathClear(segment.start, segment.end, levelHeight, levelDepth))
                    {
                        Destroy(spawnedObjects[spawnedObjects.Count - 1]);
                        spawnedObjects.RemoveAt(spawnedObjects.Count - 1);
                        i--;
                    }
                }
                attempts++;
            }
        }

        IlevelVisualizer.VisualizeSegments(segments);
        Debug.Log($"Generated Level {levelNumber} with Seed {seed} and {spawnedObjects.Count - 1} obstacles");
    }

    private float GetObjectHeight(GameObject prefab)
    {
        // Lấy chiều cao của đối tượng dựa trên Renderer hoặc Collider
        Renderer renderer = prefab.GetComponentInChildren<Renderer>();
        if (renderer != null)
        {
            return renderer.bounds.size.y;
        }

        Collider collider = prefab.GetComponentInChildren<Collider>();
        if (collider != null)
        {
            return collider.bounds.size.y;
        }

        // Nếu không tìm thấy Renderer hoặc Collider, trả về giá trị mặc định
        return 1f;
    }

    private void ClearLevel()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();
        Debug.Log("Cleared Level");
    }

    private void OnDestroy()
    {
        ClearLevel();
    }
}

