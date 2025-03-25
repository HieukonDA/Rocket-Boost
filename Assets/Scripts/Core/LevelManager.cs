using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class LevelManager : MonoBehaviour
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private LevelGenerator levelGenerator;
    private int currentLevel = 1;
    private int currentSeed; // Seed hiện tại của level
    private Vector3 lastPlayerPosition; // Vị trí người chơi khi chết
    private bool isDead; // Trạng thái chết

    private Transform playerTransform; // Tham chiếu đến Player
    private CameraManager cameraManager;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);
        Debug.Log("LevelManager created");

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // Tìm CinemachineCamera trong scene
        CinemachineCamera camera = GameObject.FindFirstObjectByType<CinemachineCamera>();
        if (camera != null)
        {
            cameraManager = camera.GetComponent<CameraManager>();
            if (cameraManager == null)
            {
                camera.gameObject.AddComponent<CameraManager>();
                cameraManager = camera.GetComponent<CameraManager>();
            }
        }

        // Chỉ gán Follow Target trong Generator scene (ProceduralLevel)
        if (currentLevel > 3)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                if (cameraManager != null)
                {
                    cameraManager.SetTarget(playerTransform);
                }
            }
            else
            {
                playerTransform = null; // Đặt lại để tìm trong Update
            }
        }
    }

    private void Update()
    {
        // Chỉ tìm và gán Player trong Generator scene
        if (currentLevel > 3)
        {
            if (playerTransform == null || playerTransform.gameObject == null)
            {
                GameObject player = GameObject.FindWithTag("Player");
                if (player != null)
                {
                    playerTransform = player.transform;
                    playerTransform.GetComponent<Movement>().enabled = !isDead;
                    if (cameraManager != null)
                    {
                        cameraManager.SetTarget(playerTransform);
                    }
                }
            }
        }
    }

    void Start()
    {
        Debug.Log("LevelManager started with Current Level " + currentLevel);
        LoadLevel();
    }

    
    private void LoadLevel()
    {
        if (currentLevel <= 3)
        {
            // Load scene tĩnh
            SceneManager.LoadScene("Level" + currentLevel);
        }
        else
        {
            // Load scene procedural và sinh level tự động
            SceneManager.LoadScene("ProceduralLevel");
            GenerateProceduralLevel();
        }
        Debug.Log($"Loading Level {currentLevel}");
    }

    
    private void GenerateProceduralLevel()
    {
        Debug.Log("Generating Procedural Level...");
        

        Debug.Log($"Generating Procedural Level {currentLevel} with Seed {currentSeed}");
        if (currentSeed == 0) // Nếu chưa có seed (level mới)
        {
            currentSeed = (int)System.DateTime.Now.Ticks; // Seed ngẫu nhiên
        }
        if (levelGenerator == null)
        {
            Debug.LogError("LevelGenerator is null! Please assign it in the Inspector.");
            return;
        }
        levelGenerator.GenerateLevel(currentLevel - 3, currentSeed); // Level 4 = 1, Level 5 = 2,...
        SaveLevelData();

        GameObject player = GameObject.FindWithTag("Player");
        // Đặt lại vị trí người chơi
        if (player != null)
        {
            Debug.Log($"Player found, setting position to: {(isDead ? lastPlayerPosition : Vector3.zero)}");
            player.GetComponent<Movement>().SetPosition(isDead ? lastPlayerPosition : Vector3.zero);
            player.GetComponent<Movement>().enabled = true;
        }
        else
        {
            Debug.LogWarning("Player not found in scene!");
        }
        Debug.Log("generated position player" + player.transform.position);
    }

    
    public void RestartLevel()
    {
        if (currentLevel <= 3)
        {
            LoadLevel(); // Reload scene tĩnh
        }
        else
        {
            GenerateProceduralLevel(); // Tái tạo level với seed hiện tại
            lastPlayerPosition = Vector3.zero; // Reset về Start
            isDead = false;
        }
    }

    
    public void ResumeFromDeath()
    {
        if (currentLevel <= 3)
        {
            LoadLevel(); // Reload scene tĩnh, không lưu vị trí chết
        }
        else
        {
            GenerateProceduralLevel(); // Tái tạo level, giữ vị trí chết
        }
    }

    
    public void NextLevel()
    {
        Debug.Log($"NextLevel called - Moving from Level {currentLevel} to {currentLevel + 1}");
        currentLevel++;
        if (currentLevel > 3)
        {
            currentSeed = (int)System.DateTime.Now.Ticks; // Seed mới cho level procedural tiếp theo
            lastPlayerPosition = Vector3.zero; // Reset vị trí
            isDead = false;
        }
        LoadLevel();
    }

    // Gọi khi người chơi chết
    public void OnPlayerDeath(Vector3 deathPosition)
    {
        if (currentLevel > 3) // Chỉ lưu trạng thái cho procedural levels
        {
            isDead = true;
            lastPlayerPosition = deathPosition;
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                Movement movement = player.GetComponent<Movement>();
                if (movement != null)
                {
                    movement.enabled = false; // Tắt Movement
                }
                else
                {
                    Debug.LogWarning("Player does not have Movement component!");
                }
            }
            SaveLevelData();
        }
        // Hiển thị UI Restart/Resume (tùy bạn thiết kế)
    }

    // Gọi khi hoàn thành level
    
    public void OnLevelComplete()
    {
        Debug.Log($"OnLevelComplete called - Level {currentLevel} completed");
        NextLevel();
    }

    private void SaveLevelData()
    {
        PlayerPrefs.SetInt("CurrentLevel", currentLevel);
        if (currentLevel > 3) // Chỉ lưu seed và trạng thái cho procedural levels
        {
            PlayerPrefs.SetInt("CurrentSeed", currentSeed);
            PlayerPrefs.SetFloat("PlayerX", lastPlayerPosition.x);
            PlayerPrefs.SetFloat("PlayerY", lastPlayerPosition.y);
            PlayerPrefs.SetFloat("PlayerZ", lastPlayerPosition.z);
            PlayerPrefs.SetInt("IsDead", isDead ? 1 : 0);
        }
        PlayerPrefs.Save();
    }

    private void LoadLevelData()
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
    }
}
