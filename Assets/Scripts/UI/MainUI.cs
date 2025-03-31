using System;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class MainUI : MonoBehaviour
{
    public static MainUI Instance { get; private set; }

    [SerializeField] private Slider musicSetting;
    [SerializeField] private Slider sfxSetting;
    private Transform setting;
    private Transform settingPanel;
    private Transform X_button;

    private AudioManager audioManager;
    private EventSystemManager eventSystemManager;

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

    void Start()
    {
        setting = transform.Find("SettingButton");
        settingPanel = transform.Find("SettingPanel");
        X_button = settingPanel.Find("X_button");

        Button settingButton = setting.GetComponent<Button>();
        Button XButton = X_button.GetComponent<Button>();

        XButton.onClick.AddListener(XButtonClicked);
        settingButton.onClick.AddListener(SettingButtonClicked);
        musicSetting.onValueChanged.AddListener((float value) => audioManager.SetMusicVolume(value));
        sfxSetting.onValueChanged.AddListener((float value) => audioManager.SetSFXVolume(value));
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



    

}
