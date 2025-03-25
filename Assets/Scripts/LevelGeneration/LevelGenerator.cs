using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.PlayerLoop;

public class LevelGenerator : MonoBehaviour
{
    [SerializeField] private GameObject playerPrefab;         // Tên lửa 3D
    [SerializeField] private GameObject startPrefab;          // Điểm bat dau 3D
    [SerializeField] private GameObject goalPrefab;          // Điểm hạ cánh 3D
    [SerializeField] private GameObject wallHorizontalPrefab;// Tường ngang 3D
    [SerializeField] private GameObject wallVerticalPrefab;  // Tường dọc 3D
    [SerializeField] private GameObject rotatingDoorPrefab;  // Cửa xoay 3D
    [SerializeField] private GameObject laserTrapPrefab;     // Bẫy laser 3D

    [SerializeField] private float levelWidth = 30f;  // Chiều dài level (trục X)
    [SerializeField] private float levelHeight = 10f; // Chiều cao level (trục Y)
    [SerializeField] private float levelDepth = 0f;   // Độ sâu (trục Z, cố định vì góc nhìn XY)
    [SerializeField] private int minObstacles = 2;    // Số vật cản tối thiểu
    [SerializeField] private int maxObstacles = 5;    // Số vật cản tối đa
    [SerializeField] private float safeZoneRadius = 2f; // Khoảng cách an toàn quanh Start/Goal

    private List<GameObject> spawnedObjects = new List<GameObject>();
    private Vector3 startPos;
    private Vector3 goalPos;
    private float tunnelHeightBottom;

    //chia đường hầm thành các segment
    private struct Segment
    {
        public Vector3 start;
        public Vector3 end;
        public string direction; // "Horizontal", "Vertical", "Diagonal"
    }

    public void GenerateLevel(int levelNumber, int seed)
    {
        ClearLevel();

        // Khóa chuỗi ngẫu nhiên bằng seed
        UnityEngine.Random.InitState(seed);

        // Tính số vật cản dựa trên level
        int obstacleCount = Mathf.RoundToInt(Mathf.Lerp(minObstacles, maxObstacles, levelNumber / 10f));
        obstacleCount = Mathf.Clamp(obstacleCount, minObstacles, maxObstacles);

        // Chia level thành các đoạn
        List<Segment> segments = GenerateSegments(levelNumber);

        // Lấy độ cao sàn đường hầm từ đoạn đầu tiên
        tunnelHeightBottom = UnityEngine.Random.Range(0f, levelHeight * 0.2f); // Sàn đường hầm (0%-30% chiều cao)

        // Điều chỉnh startPos và goalPos để nằm trên sàn
        startPos = segments[0].start;
        startPos.y = tunnelHeightBottom; // Đặt trên sàn đường hầm
        goalPos = segments[segments.Count - 1].end;
        goalPos.y = tunnelHeightBottom; // Đặt trên sàn đường hầm

        // Tính offset dựa trên chiều cao của các đối tượng (nếu cần)
        float startHeight = GetObjectHeight(startPrefab);
        float goalHeight = GetObjectHeight(goalPrefab);
        float playerHeight = GetObjectHeight(playerPrefab);

        // Điều chỉnh y để phần dưới của đối tượng nằm trên sàn
        startPos.y += startHeight / 2f;
        goalPos.y += goalHeight / 2f;

        // Đặt điểm xuất phát và hạ cánh
        GameObject startInstance = Instantiate(startPrefab, Vector3.zero, Quaternion.identity, transform);
        GameObject playerInstance = Instantiate(playerPrefab, startPos , Quaternion.identity, transform);
        GameObject goal = Instantiate(goalPrefab, goalPos, Quaternion.identity, transform);
        playerInstance.transform.position = new Vector3(
            playerInstance.transform.position.x,
            startPos.y + playerHeight / 2f + 9f, // Đảm bảo player nằm trên sàn
            playerInstance.transform.position.z
        );

        
        // Lưu các instance vào spawnedObjects
        spawnedObjects.Add(goal);
        spawnedObjects.Add(startInstance);
        spawnedObjects.Add(playerInstance);

        // Sinh vật cản cho từng đoạn
        foreach (Segment segment in segments)
        {
            // Bước 1: Sinh tường ngang liên tục để tạo đường hầm
            SpawnTunnelWalls(segment);

            // Bước 2: Sinh các vật cản khác (tường dọc, bẫy laser, v.v.)
            int additionalObstacles = obstacleCount / segments.Count;
            int attempts = 0;
            int maxAttempts = 20;
            for (int i = 0; i < additionalObstacles && attempts < maxAttempts; i++)
            {
                if (SpawnAdditionalObstacleInSegment(segment))
                {
                    if (!IsPathClear(segment.start, segment.end))
                    {
                        Destroy(spawnedObjects[spawnedObjects.Count - 1]);
                        spawnedObjects.RemoveAt(spawnedObjects.Count - 1);
                        i--;
                    }
                }
                attempts++;
            }
        }

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

    private bool SpawnAdditionalObstacleInSegment(Segment segment)
    {
        int obstacleType = UnityEngine.Random.Range(0, 3); // 0: Tường dọc, 1: Cửa xoay, 2: Bẫy laser
        Vector3 position;
        GameObject obstacle;

        float minX = Mathf.Min(segment.start.x, segment.end.x) + safeZoneRadius;
        float maxX = Mathf.Max(segment.start.x, segment.end.x) - safeZoneRadius;
        float minY = 0f;
        float maxY = levelHeight;

        float midX = (segment.start.x + segment.end.x) / 2f;
        float midY = (segment.start.y + segment.end.y) / 2f;
        float segmentLengthX = Mathf.Abs(segment.end.x - segment.start.x);
        float segmentLengthY = Mathf.Abs(segment.end.y - segment.end.y);
        float midZoneX = segmentLengthX * 0.1f;
        float midZoneY = segmentLengthY * 0.1f;

        switch (obstacleType)
        {
            case 0: // Tường dọc
                position = new Vector3(
                    UnityEngine.Random.Range(minX, maxX),
                    UnityEngine.Random.Range(0f, levelHeight - 2f),
                    levelDepth
                );

                if (segment.direction == "Horizontal")
                {
                    if (Mathf.Abs(position.x - midX) < midZoneX)
                    {
                        return false;
                    }
                }
                else if (segment.direction == "Vertical")
                {
                    if (Mathf.Abs(position.y - midY) < midZoneY)
                    {
                        return false;
                    }
                }
                else // Diagonal
                {
                    if (Mathf.Abs(position.x - midX) < midZoneX && Mathf.Abs(position.y - midY) < midZoneY)
                    {
                        return false;
                    }
                }

                obstacle = Instantiate(wallVerticalPrefab, position, Quaternion.identity, transform);
                break;

            case 1: // Cửa xoay
                position = new Vector3(
                    UnityEngine.Random.Range(minX, maxX),
                    UnityEngine.Random.Range(2f, levelHeight - 2f),
                    levelDepth
                );
                obstacle = Instantiate(rotatingDoorPrefab, position, Quaternion.identity, transform);
                break;

            case 2: // Bẫy laser
                position = new Vector3(
                    UnityEngine.Random.Range(minX, maxX),
                    UnityEngine.Random.Range(1f, levelHeight - 1f),
                    levelDepth
                );
                obstacle = Instantiate(laserTrapPrefab, position, Quaternion.identity, transform);
                break;

            default:
                return false;
        }

        spawnedObjects.Add(obstacle);
        return true;
    }

    private void SpawnTunnelWalls(Segment segment)
    {
        float minX = Mathf.Min(segment.start.x, segment.end.x) + safeZoneRadius;
        float maxX = Mathf.Max(segment.start.x, segment.end.x) - safeZoneRadius;

        // Tính chiều dài đoạn trên trục X (đối với đoạn ngang hoặc chéo)
        float segmentLengthX = Mathf.Abs(segment.end.x - segment.start.x);
        // Số lượng tường ngang cần sinh (đặt cách đều nhau)
        int wallCount = Mathf.Max(2, Mathf.FloorToInt(segmentLengthX / 5f)); // Mỗi 5 đơn vị sinh 1 cặp tường
        float stepX = segmentLengthX / (wallCount + 1);

        // Chọn độ cao cố định cho tường ngang (để tạo đường hầm)
        float tunnelHeightTop = UnityEngine.Random.Range(levelHeight * 0.7f, levelHeight); // Mái
        // float tunnelHeightBottom = UnityEngine.Random.Range(0f, levelHeight * 0.3f); // Sàn

        // Sinh các cặp tường ngang (mái và sàn) cách đều nhau
        for (int i = 1; i <= wallCount; i++)
        {
            float xPos = minX + stepX * i;

            // Sinh tường ngang ở trên (mái)
            Vector3 topPosition = new Vector3(xPos, tunnelHeightTop, levelDepth);
            GameObject topWall = Instantiate(wallHorizontalPrefab, topPosition, Quaternion.identity, transform);
            spawnedObjects.Add(topWall);

            // Sinh tường ngang ở dưới (sàn)
            Vector3 bottomPosition = new Vector3(xPos, tunnelHeightBottom, levelDepth);
            GameObject bottomWall = Instantiate(wallHorizontalPrefab, bottomPosition, Quaternion.identity, transform);
            spawnedObjects.Add(bottomWall);
        }
    }

    private List<Segment> GenerateSegments(int levelNumber)
    {
        List<Segment> segments = new List<Segment>();
        Vector3 currentPos = Vector3.zero; // Bắt đầu từ (0, 0, 0)

        // Quyết định số đoạn dựa trên levelNumber
        int segmentCount = Mathf.Clamp(levelNumber, 1, 3); // Ví dụ: Level 1: 1 đoạn, Level 2: 2 đoạn, Level 3+: 3 đoạn

        for (int i = 0; i < segmentCount; i++)
        {
            Segment segment = new Segment();
            segment.start = currentPos;

            // Quyết định hướng của đoạn (ưu tiên ngang)
            string direction = "Horizontal";
            if (i > 0) // Đoạn đầu tiên luôn ngang
            {
                int randomDirection = UnityEngine.Random.Range(0, 3);
                if (randomDirection == 1) direction = "Vertical";
                else if (randomDirection == 2) direction = "Diagonal";
            }

            if (direction == "Horizontal")
            {
                float segmentLength = levelWidth / segmentCount;
                segment.end = new Vector3(currentPos.x + segmentLength, currentPos.y, levelDepth);
            }
            else if (direction == "Vertical")
            {
                float segmentLength = levelHeight / 2f; // Đoạn dọc ngắn hơn
                segment.end = new Vector3(currentPos.x, currentPos.y + segmentLength, levelDepth);
            }
            else // Diagonal
            {
                float segmentLengthX = levelWidth / (segmentCount * 2f);
                float segmentLengthY = levelHeight / (segmentCount * 2f);
                segment.end = new Vector3(currentPos.x + segmentLengthX, currentPos.y + segmentLengthY, levelDepth);
            }

            segment.direction = direction;
            segments.Add(segment);
            currentPos = segment.end; // Điểm kết thúc của đoạn này là điểm bắt đầu của đoạn tiếp theo

            // Log thông tin về đoạn
            Debug.Log($"Segment {i + 1}: Direction = {segment.direction}, Start = {segment.start}, End = {segment.end}");
        }

        // Đảm bảo đoạn cuối kết thúc gần biên phải của level
        Segment lastSegment = segments[segments.Count - 1];
        lastSegment.end = new Vector3(levelWidth - 2f, lastSegment.end.y, levelDepth);
        segments[segments.Count - 1] = lastSegment;

        // Vẽ các đoạn trong Unity Editor để dễ hình dung
        for (int i = 0; i < segments.Count; i++)
        {
            Color color = i == 0 ? Color.green : (i == segments.Count - 1 ? Color.red : Color.yellow);
            Debug.DrawLine(segments[i].start, segments[i].end, color, 30f); // Vẽ 30 giây
        }

        return segments;
    }

    private bool IsPathClear(Vector3 start, Vector3 end)
    {
        RaycastHit hit;
        if (Physics.Raycast(start, end - start, out hit, Vector3.Distance(start, end)))
        {
            if (hit.collider != null && !hit.collider.CompareTag("Finish"))
            {
                return false;
            }
        }

        // Kiểm tra đường cong với nhiều độ cao
        float[] heights = { levelHeight / 4f, levelHeight / 2f, 3 * levelHeight / 4f };
        foreach (float height in heights)
        {
            Vector3 midPoint = new Vector3((start.x + end.x) / 2f, height, levelDepth);
            if (!Physics.Raycast(start, midPoint - start, out hit, Vector3.Distance(start, midPoint)) &&
                !Physics.Raycast(midPoint, end - midPoint, out hit, Vector3.Distance(midPoint, end)))
            {
                return true;
            }
            if (hit.collider != null && !hit.collider.CompareTag("Finish"))
            {
                continue;
            }
        }

        return true;
    }

    private void ClearLevel()
    {
        foreach (GameObject obj in spawnedObjects)
        {
            if (obj != null) Destroy(obj);
        }
        spawnedObjects.Clear();
    }

    private void OnDestroy()
    {
        ClearLevel();
    }
}


