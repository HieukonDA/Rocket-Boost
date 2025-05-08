using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using UnityEditor;

public class MainUI : MonoBehaviour
{
    public static MainUI Instance { get; private set; }
    [SerializeField] private Slider musicSetting;
    [SerializeField] private Slider sfxSetting;
    [SerializeField] private Text finalScoreText;
    private Transform setting;
    private Transform settingPanel;
    private Transform gameOverPanel;
    private Transform X_button;
    private Transform restartButton;
    private Transform starting;
    private Transform Score;
    private Transform quitButton;
    private Transform pauseButton;
    private Transform resumeButton;
    private Transform restartButtonInSetting;
    private Transform soundPanelButton;
    private Transform soundPanel;
    private Transform buttonSettingPanel;
    private Transform Level;


    private AudioManager audioManager;
    private EventSystemManager eventSystemManager;

    private bool switchPanel = false;

    void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(gameObject);

        // Kiểm tra và giữ EventSystem duy nhất
        audioManager = AudioManager.Instance;
        eventSystemManager = new EventSystemManager(gameObject);
    }

    private void Start()
    {
        setting = transform.Find("SettingButton");
        starting = transform.Find("PlayingButton");
        gameOverPanel = transform.Find("GameOverPanel");
        restartButton = gameOverPanel.Find("RestartButton");
        settingPanel = transform.Find("SettingPanel");
        soundPanelButton = settingPanel.Find("SoundPanelButton");
        soundPanel = settingPanel.Find("SoundSetting");
        buttonSettingPanel = settingPanel.Find("ButtonSetting");
        X_button = settingPanel.Find("X_button");
        quitButton = buttonSettingPanel.Find("QuitButton");
        pauseButton = buttonSettingPanel.Find("PauseButton");
        resumeButton = buttonSettingPanel.Find("ResumeButton");
        restartButtonInSetting = buttonSettingPanel.Find("RestartButton");
        Score = transform.Find("Score");
        Level = transform.Find("Level");

        Button settingButton = setting.GetComponent<Button>();
        Button XButton = X_button.GetComponent<Button>();
        Button restartBtn = restartButton.GetComponent<Button>();
        Button startBtn = starting.GetComponent<Button>();
        Button quitBtn = quitButton.GetComponent<Button>();
        Button pauseBtn = pauseButton.GetComponent<Button>();
        Button resumeBtn = resumeButton.GetComponent<Button>();
        Button restartBtnInSetting = restartButtonInSetting.GetComponent<Button>();
        Button soundPanelBtn = soundPanelButton.GetComponent<Button>();

        restartBtn.onClick.AddListener(OnRestartButton);
        XButton.onClick.AddListener(XButtonClicked);
        settingButton.onClick.AddListener(SettingButtonClicked);
        startBtn.onClick.AddListener(() => { StartGame(); });
        quitBtn.onClick.AddListener(() => { QuitGame(); });
        pauseBtn.onClick.AddListener(() => { PauseGame(); });
        resumeBtn.onClick.AddListener(() => { ResumeGame(); });
        restartBtnInSetting.onClick.AddListener(() => { OnRestartButton(); });
        soundPanelBtn.onClick.AddListener(() => { ShowSoundPanel(); });
        musicSetting.onValueChanged.AddListener((float value) => audioManager.SetMusicVolume(value));
        sfxSetting.onValueChanged.AddListener((float value) => audioManager.SetSFXVolume(value));
    }

    
    private void Update()
    {
        if (gameOverPanel.gameObject.activeInHierarchy && Input.GetKeyDown(KeyCode.R))
        {
            OnRestartButton();
        }

        if ( Input.GetKeyDown(KeyCode.P))
        {
            if (gameOverPanel.gameObject.activeInHierarchy)
            {
                return; // Không cho phép tạm dừng khi game over
            }
            if (Time.timeScale == 1f)
            {
                PauseGame();
            }
            else
            {
                ResumeGame();
            }
        }

        GameObject Player = GameObject.FindGameObjectWithTag("Player");
        if (Player != null)
        {
            Movement playerController = Player.GetComponent<Movement>();
            if (playerController != null)
            {
                // MovePlayerInFigureEight(Player);
            }
        }
    }

    private void StartGame()
    {
        gameOverPanel.gameObject.SetActive(false);
        settingPanel.gameObject.SetActive(false);
        starting.gameObject.SetActive(false); 
        GameObject.Find("Welcome").gameObject.SetActive(false);
        audioManager.PlaySoundButton();
        Debug.Log("Start Game - Attempting to play ClickButton");

        // Bắt đầu chơi game
        SceneManager.LoadScene("ProceduralLevel"); // Thay đổi tên scene nếu cần
        setting.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f); // Đặt kích thước về 0 để ẩn nút bắt đầu
        setting.GetComponent<RectTransform>().anchoredPosition = new Vector2(-350, -160);
        settingPanel.transform.localScale = new Vector3(0.7f, 0.7f, 0.7f); // Đặt kích thước về 1 để hiện nút bắt đầu
        settingPanel.GetComponent<RectTransform>().anchoredPosition = new Vector2(-233, -60);

        Score.gameObject.SetActive(true); // Ẩn điểm số    
        Level.gameObject.SetActive(true); // Ẩn level

        switchPanel = false;
        Time.timeScale = 1f;
    }

    private void PauseGame()
    {
        audioManager.PlaySoundButton();
        LevelManager.Instance.OnPaused();
    }

    public void ResumeGame()
    {
        audioManager.PlaySoundButton();
        LevelManager.Instance.OnResume();
    }

    public void ShowSoundPanel()
    {
        switchPanel = !switchPanel;
        if (switchPanel)
        {
            soundPanel.gameObject.SetActive(true);
            buttonSettingPanel.gameObject.SetActive(false);
            audioManager.PlaySoundButton();
            Debug.Log("Show Sound Panel - Attempting to play ClickButton");
        }
        else
        {
            soundPanel.gameObject.SetActive(false);
            buttonSettingPanel.gameObject.SetActive(true);
            audioManager.PlaySoundButton();
            Debug.Log("Hide Sound Panel - Attempting to play ClickButton");
        }
    }

    private void QuitGame()
    {
        audioManager.PlaySoundButton();
        Debug.Log("QuitGame called");

        #if UNITY_EDITOR
        EditorApplication.isPlaying = false; // Dừng Play Mode trong Editor
        #elif UNITY_WEBGL
        Debug.Log("Quit not supported on WebGL. Redirecting to main menu.");
        SceneManager.LoadScene("MainMenu"); // Fallback cho WebGL
        #else
        Application.Quit(); // Thoát ứng dụng trên standalone/mobile
        #endif
    }


    private void XButtonClicked()
    {
        settingPanel.gameObject.SetActive(false);
        audioManager.PlaySoundButton();
        Debug.Log("X Button clicked - Attempting to play ClickButton");
    }

    private void SettingButtonClicked()
    {    
        settingPanel.gameObject.SetActive(true);
        audioManager.PlaySoundButton();
        Debug.Log("X Button clicked - Attempting to play ClickButton");
    }

    public void ShowGameOver(int score)
    {
        gameOverPanel.gameObject.SetActive(true);
        finalScoreText.text = score.ToString(); // Cập nhật điểm cuối
        HUDManager.Instance.ControllActiveScoreText(false); // Ẩn HUD
    }

    public void OnRestartButton()
    {
        gameOverPanel.gameObject.SetActive(false);
        HUDManager.Instance.gameObject.SetActive(true); // Hiện lại HUD
        audioManager.PlaySoundButton();
        LevelManager.Instance.RestartLevel();
    }

}
