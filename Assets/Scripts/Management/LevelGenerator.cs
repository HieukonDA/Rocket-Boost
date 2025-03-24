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

    public void GenerateLevel(int levelNumber, int seed)
    {
        ClearLevel();

        // Khóa chuỗi ngẫu nhiên bằng seed
        UnityEngine.Random.InitState(seed);

        // Tính số vật cản dựa trên level
        int obstacleCount = Mathf.RoundToInt(Mathf.Lerp(minObstacles, maxObstacles, levelNumber / 10f));
        obstacleCount = Mathf.Clamp(obstacleCount, minObstacles, maxObstacles);

        // Đặt điểm xuất phát và hạ cánh
        GameObject startInstance = Instantiate(startPrefab, Vector3.zero, Quaternion.identity, transform);
        Debug.Log("startPrefab position : " + startPrefab.transform.position);
        startPos = startInstance.transform.position;
        goalPos = new Vector3(levelWidth - 2f, 0f, levelDepth);

        GameObject playerInstance = Instantiate(playerPrefab, startPos , Quaternion.identity, transform);
        GameObject goal = Instantiate(goalPrefab, goalPos, Quaternion.identity, transform);
        Debug.Log("playerPrefab position : " + playerPrefab.transform.position);

        
        // Lưu các instance vào spawnedObjects
        spawnedObjects.Add(goal);
        spawnedObjects.Add(startInstance);
        spawnedObjects.Add(playerInstance);

        // Sinh vật cản ngẫu nhiên với seed
        int attempts = 0;
        int maxAttempts = 20; // Giới hạn thử để tránh vòng lặp vô hạn
        for (int i = 0; i < obstacleCount && attempts < maxAttempts; i++)
        {
            if (SpawnRandomObstacle(startPos.x + safeZoneRadius, goalPos.x - safeZoneRadius))
            {
                if (!IsPathClear(startPos, goalPos))
                {
                    // Nếu chặn đường, xóa vật cản vừa thêm
                    Destroy(spawnedObjects[spawnedObjects.Count - 1]);
                    spawnedObjects.RemoveAt(spawnedObjects.Count - 1);
                    i--; // Thử lại
                }
            }
            attempts++;
        }

        Debug.Log($"Generated Level {levelNumber} with Seed {seed} and {spawnedObjects.Count - 1} obstacles");
    }

    private bool SpawnRandomObstacle(float minX, float maxX)
    {
        int obstacleType = UnityEngine.Random.Range(0, 4); // Loại vật cản ngẫu nhiên dựa trên seed
        Vector3 position;
        GameObject obstacle;

        switch (obstacleType)
        {
            case 0: // Tường ngang
                position = new Vector3(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(1f, levelHeight - 1f), levelDepth);
                obstacle = Instantiate(wallHorizontalPrefab, position, Quaternion.identity, transform);
                break;
            case 1: // Tường dọc
                position = new Vector3(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(0f, levelHeight - 2f), levelDepth);
                obstacle = Instantiate(wallVerticalPrefab, position, Quaternion.identity, transform);
                break;
            case 2: // Cửa xoay
                position = new Vector3(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(2f, levelHeight - 2f), levelDepth);
                obstacle = Instantiate(rotatingDoorPrefab, position, Quaternion.identity, transform);
                break;
            case 3: // Bẫy laser
                position = new Vector3(UnityEngine.Random.Range(minX, maxX), UnityEngine.Random.Range(1f, levelHeight - 1f), levelDepth);
                obstacle = Instantiate(laserTrapPrefab, position, Quaternion.identity, transform);
                break;
            default:
                return false;
        }
        spawnedObjects.Add(obstacle);
        return true;
    }

    private bool IsPathClear(Vector3 start, Vector3 end)
    {
        // Kiểm tra đường thẳng từ Start đến Goal trong không gian 3D
        RaycastHit hit;
        if (Physics.Raycast(start, end - start, out hit, Vector3.Distance(start, end)))
        {
            if (hit.collider != null && !hit.collider.CompareTag("Finish"))
            {
                return false; // Có vật cản chặn đường
            }
        }

        // Kiểm tra đường cong (bay lên giữa level)
        Vector3 midPoint = new Vector3((start.x + end.x) / 2f, levelHeight / 2f, levelDepth);
        if (Physics.Raycast(start, midPoint - start, out hit, Vector3.Distance(start, midPoint)) ||
            Physics.Raycast(midPoint, end - midPoint, out hit, Vector3.Distance(midPoint, end)))
        {
            if (hit.collider != null && !hit.collider.CompareTag("Finish"))
            {
                return false;
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

    private void Destroy()
    {
        ClearLevel();
    }
}


