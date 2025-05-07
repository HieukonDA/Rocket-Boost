using System;
using System.Collections.Generic;
using Unity.Mathematics;
using UnityEngine;

public class BackgroundManager : MonoBehaviour, IBackgroundManager
{
    // public static BackgroundManager Instance { get; private set; }
    [SerializeField] private GameObject[] backgroundPrefabs;
    [SerializeField] private float spawnDistance = 50f; // Khoảng cách spawn background tiếp theo
    [SerializeField] private float despawnDistance = 100f; // Khoảng cách để recycle background
    [SerializeField] private float backgroundDepth = 10f; // Độ sâu Z của background (xa hơn segment)
    [SerializeField] private float moveSpeed = 5f;
    [SerializeField] private int maxActiveBackgrounds = 5;
    [SerializeField] private string spawnPointName = "SpawnPoint";

    private ObjectPool<GameObject> backgroundPool;
    private List<GameObject> activeBackgrounds = new List<GameObject>();
    private Transform player;
    private Vector3 lastSpawnPosition;
    private bool isSpawning;

    private void Awake()
    {
        // if (Instance != null)
        // {
        //     Destroy(gameObject);
        //     return;
        // }
        // Instance = this;
        // DontDestroyOnLoad(gameObject);

        backgroundPool = new ObjectPool<GameObject>(() => InstantiateBackground(),backgroundPrefabs.Length, transform);
        player = GameObject.FindGameObjectWithTag("Player").transform;
        lastSpawnPosition = new Vector3(player.position.x , 0, backgroundDepth); // Bắt đầu từ trái player

        // Spawn background ban đầu để che phủ tầm nhìn
        InitializeBackgrounds();
    }

    private void InitializeBackgrounds()
    {
        float playerX = player.position.x;
        isSpawning = true;
        while (lastSpawnPosition.x < playerX + spawnDistance && activeBackgrounds.Count < maxActiveBackgrounds)
        {
            SpawnNextBackground();
        }
        isSpawning = false;
    }

    private void Update()
    {
        if (player == null) return;

        ManageBackgrounds();
        // MoveBackgrounds();
    }
    
    private void ManageBackgrounds()
    {
        float playerX = player.position.x;
        Debug.Log($"BackgroundManager: Player X = {playerX}, Last Spawn Position X = {lastSpawnPosition.x}");

        //sinh backlgorund mới nếu player gần đến vị trí sinh background tiếp theo
        if(lastSpawnPosition.x < playerX +spawnDistance)
        {
            SpawnNextBackground();
        }

        //tai su dung background cu
        for(int i = activeBackgrounds.Count-1; i >= 0; i--)
        {
            if (activeBackgrounds[i].transform.position.x < playerX - despawnDistance)
            {
                backgroundPool.Return(activeBackgrounds[i]);
                activeBackgrounds.RemoveAt(i);
            }
        }
    }

    private void SpawnNextBackground()
    {
        GameObject bg = backgroundPool.Get();
        bg.SetActive(true);
        bg.transform.position = lastSpawnPosition; // Đặt background tại lastSpawnPosition
        activeBackgrounds.Add(bg);

        // Tìm spawn point trong background để tính vị trí tiếp theo
        Transform spawnPoint = bg.transform.Find(spawnPointName);
        if (spawnPoint != null)
        {
            // Dùng vị trí world space của spawn point làm lastSpawnPosition
            lastSpawnPosition = new Vector3(spawnPoint.position.x, 0, backgroundDepth);
            Debug.Log($"BackgroundManager: Spawned {bg.name} at {bg.transform.position}, next spawn at {lastSpawnPosition}");
        }
        else
        {
            // Fallback nếu không tìm thấy spawn point
            lastSpawnPosition = new Vector3(lastSpawnPosition.x + 50f, 0, backgroundDepth);
            Debug.LogWarning($"Background {bg.name} missing SpawnPoint. Using default offset: 50.");
        }
    }

    private float GetBackgroundWidth(GameObject bg)
    {
        MeshFilter meshFilter = bg.GetComponent<MeshFilter>();
        if (meshFilter != null && meshFilter.sharedMesh != null)
        {
            float width = meshFilter.sharedMesh.bounds.size.x * bg.transform.localScale.x;
            if (width > 0.1f) return width;
        }
        Debug.LogWarning($"Background {bg.name} has invalid mesh width. Using default width: 50.");
        return 50f;
    }

    private void MoveBackgrounds()
    {
        foreach (GameObject bg in activeBackgrounds)
        {
            bg.transform.position += Vector3.left * moveSpeed * Time.deltaTime;
        }
    }

    private GameObject InstantiateBackground()
    {
        int index = UnityEngine.Random.Range(0, backgroundPrefabs.Length); // Chọn ngẫu nhiên prefab
        GameObject bg = Instantiate(backgroundPrefabs[index], Vector3.zero, Quaternion.Euler(0, 90, 0));
        bg.SetActive(false);
        return bg;
    }

    public void ResetBackgrounds()
    {
        foreach (GameObject bg in activeBackgrounds)
        {
            backgroundPool.Return(bg);
        }
        activeBackgrounds.Clear();
        lastSpawnPosition = new Vector3(0, 0, backgroundDepth);
        SpawnNextBackground(); // Spawn cái đầu tiên sau khi reset
    }
}    