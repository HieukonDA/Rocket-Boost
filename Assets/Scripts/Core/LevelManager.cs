using System;
using UnityEngine;
using UnityEngine.SceneManagement;
using Unity.Cinemachine;

public class LevelManager : MonoBehaviour, ILevelManager 
{
    public static LevelManager Instance { get; private set; }

    [SerializeField] private GameObject levelGeneratorPrefab;
    [SerializeField] private Animator transitionAnim; // Prefab của Player

    // thiet lap interface
    private ILevelGenerator IlevelGenerator;
    private ICameraManager IcameraManager;
    private LevelDataManager levelDataManager;

    private int currentLevel = 1; // Mặc định là 0, sẽ được cập nhật khi load level
    private int currentSeed; 
    private Transform playerTransform;

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Xóa PlayerPrefs khi chạy trong Editor (chỉ để debug)
        if (Debug.isDebugBuild)
        {
            PlayerPrefs.DeleteAll();
            PlayerPrefs.Save();
            Debug.Log("PlayerPrefs đã được xóa để bắt đầu từ Level 1");
        }

        levelDataManager = new LevelDataManager();
        levelDataManager.LoadLevelData(out currentLevel, out currentSeed, out Vector3 lastPlayerPosition, out bool isDead);
        levelDataManager.SetLastPlayerPosition(lastPlayerPosition);
        levelDataManager.SetIsDead(isDead);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }

    private void OnSceneLoaded(Scene scene, LoadSceneMode mode)
    {
        // setting camera
        IcameraManager = (ICameraManager)GameObject.FindFirstObjectByType<CameraManager>();
        if (currentLevel > 3)
        {
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                playerTransform = player.transform;
                IcameraManager?.SetTarget(playerTransform);
            }
            else
            {
                playerTransform = null;
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
        Debug.Log("LoadLevel called for Level " + currentLevel + " at time: " + Time.time);
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
        if (IlevelGenerator == null)
        {
            GameObject generatorObj = Instantiate(levelGeneratorPrefab, transform);
            IlevelGenerator = generatorObj.GetComponent<ILevelGenerator>();
            Debug.Log("generatorObj spawned: " + (generatorObj != null ? generatorObj.name : "Null"));
            Debug.Log("IlevelGenerator initialized: " + (IlevelGenerator != null ? "Yes" : "No"));
        }

        //nếu chưa có seed mới thì tạo seed ngẫu nhiên
        if (currentSeed == 0) 
        {
            currentSeed = (int)System.DateTime.Now.Ticks; 
        }
        
        IlevelGenerator.GenerateLevel(currentLevel - 3, currentSeed); // Level 4 = 1, Level 5 = 2,...

        // Lưu lại thông tin level
        GameObject player = GameObject.FindWithTag("Player");
        if (player != null)
        {
            player.GetComponent<Movement>().SetPosition(levelDataManager.GetIsDead() ? levelDataManager.GetLastPlayerPosition() : Vector3.zero);
            player.GetComponent<Movement>().enabled = true;
        }

        levelDataManager.SaveLevelData(currentLevel, currentSeed, levelDataManager.GetLastPlayerPosition(), levelDataManager.GetIsDead());
    }

        public void RestartLevel()
        {
            Debug.Log("RestartLevel called at time: " + Time.time);
            if (currentLevel <= 3)
            {
                LoadLevel(); // Reload scene tĩnh
            }
            else
            {
                GenerateProceduralLevel(); // Tái tạo level với seed hiện tại
                levelDataManager.SetLastPlayerPosition(Vector3.zero);
                levelDataManager.SetIsDead(false);
                HUDManager.Instance?.ControllActiveScoreText(true);
                CoinManager.Instance.ResetCoinCount();
                HUDManager.Instance.UpdateScore(0);
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

    public void OnPlayerDeath(Vector3 deathPosition)
    {
        if (currentLevel > 3)
        {
            levelDataManager.SetIsDead(true);
            levelDataManager.SetLastPlayerPosition(deathPosition);
            GameObject player = GameObject.FindWithTag("Player");
            if (player != null)
            {
                player.GetComponent<Movement>().enabled = false;
            }
            levelDataManager.SaveLevelData(currentLevel, currentSeed, deathPosition, true);
            
        }
        MainUI.Instance.ShowGameOver(HUDManager.Instance.GetScore());
    }

    public void NextLevel()
    {
        Debug.Log($"NextLevel called - Moving from Level {currentLevel} to {currentLevel + 1}");
        currentLevel++;
        if (currentLevel > 3)
        {
            currentSeed = (int)System.DateTime.Now.Ticks; // Seed mới cho level procedural tiếp theo
            levelDataManager.SetLastPlayerPosition(Vector3.zero);
            levelDataManager.SetIsDead(false);
            HUDManager.Instance.SetLevelText(currentLevel);
        }
        transitionAnim.SetTrigger("end");
        LoadLevel();
        transitionAnim.SetTrigger("start");
    }

    public void OnLevelComplete()
    {
        Debug.Log($"OnLevelComplete called - Level {currentLevel} completed");
        NextLevel();
    }
}
