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
        EventSystem existingEventSystem = GameObject.FindFirstObjectByType<EventSystem>();
        if (existingEventSystem != null && existingEventSystem.gameObject != gameObject)
        {
            Destroy(existingEventSystem.gameObject);
        }
        if (!GetComponent<EventSystem>())
        {
            gameObject.AddComponent<EventSystem>();
            gameObject.AddComponent<StandaloneInputModule>();
        }
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
        musicSetting.onValueChanged.AddListener((float value) => AudioManager.Instance.SetMusicVolume(value));
        sfxSetting.onValueChanged.AddListener((float value) => AudioManager.Instance.SetSFXVolume(value));
    }

    private void XButtonClicked()
    {
        settingPanel.gameObject.SetActive(false);
        AudioManager.Instance.PlaySoundButton();
        Debug.Log("X Button clicked - Attempting to play ClickButton");
    }

    private void SettingButtonClicked()
    {    
        settingPanel.gameObject.SetActive(true);
        AudioManager.Instance.PlaySoundButton();
        Debug.Log("X Button clicked - Attempting to play ClickButton");
    }



    

}
