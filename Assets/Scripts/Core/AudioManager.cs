using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
    }

    public List<Sound> sounds;
    private Dictionary<string, AudioClip> soundDictionary;

    [SerializeField] private AudioSource audioSourceMusic;
    [SerializeField] private AudioSource audioSourceButton;
    [SerializeField] private int sfxSourceCount = 5;
    private List<AudioSource> audioSourceSFX;
    private int currentSFXSourceIndex = 0;
    private float sfxVolume = 1.0f;
    

    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;

        InitSounds();
        InitSFXSources();
        DontDestroyOnLoad(gameObject);
    }

    private void InitSFXSources()
    {
        audioSourceSFX = new List<AudioSource>() ;
        for (int i = 0; i < sfxSourceCount; i++)
        {
            AudioSource source = gameObject.AddComponent<AudioSource>();
            source.volume = sfxVolume; // Đặt volume mặc định
            source.spatialBlend = 0.0f; // 2D sound (không bị ảnh hưởng bởi khoảng cách)
            source.pitch = 1.0f; // Đặt pitch mặc định là 1
            audioSourceSFX.Add(source);
        }
    }

    private void InitSounds()
    {
        soundDictionary = new Dictionary<string, AudioClip>();
        foreach (Sound sound in sounds)
        {
            soundDictionary[sound.name] = sound.clip;
        }
    }

    public void PlaySound(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            AudioSource audioSourceSFX = GetNextAvailableSFXSource();

           // Reset các thuộc tính trước khi phát
            audioSourceSFX.Stop();
            audioSourceSFX.volume = sfxVolume;       // Volume tối đa
            audioSourceSFX.spatialBlend = 0.0f; // 2D sound
            audioSourceSFX.pitch = 1.0f;        // Tốc độ chuẩn

            audioSourceSFX.PlayOneShot(soundDictionary[soundName]);
            Debug.Log("Playing " + soundName + " with duration: " + soundDictionary[soundName].length);
        }
        else
        {
            Debug.LogWarning("Sound " + soundName + " not found");
        }
    }

    private AudioSource GetNextAvailableSFXSource()
    {
        foreach (AudioSource audioSource in audioSourceSFX)
        {
            if (!audioSource.isPlaying)
            {
                return audioSource;
            }
        }

        // Nếu tất cả đang phát, dùng AudioSource tiếp theo trong vòng lặp
        Debug.LogWarning("All SFX AudioSources are busy. Consider increasing sfxSourceCount.");
        currentSFXSourceIndex = (currentSFXSourceIndex + 1) % audioSourceSFX.Count;
        return audioSourceSFX[currentSFXSourceIndex];
    }

    public void StopSound()
    {
        foreach (AudioSource audioSource in audioSourceSFX)
        {
            audioSource.Stop();
        }
    }

    public void StopMusic()
    {
        audioSourceMusic.Stop();
    }

    public bool IsSoundPlaying()
    {
        foreach (AudioSource audioSource in audioSourceSFX)
        {
            if (audioSource.isPlaying)
            {
                return true;
            }
        }
        return false;
    }

    public void PlayMusic(string soundName)
    {
        if (soundDictionary.ContainsKey(soundName))
        {
            audioSourceMusic.clip = soundDictionary[soundName];
            audioSourceMusic.loop = true;
            audioSourceMusic.Play();
        }
        else
        {
            Debug.LogWarning("Sound " + soundName + " not found");
        }
    }

    public void SetMusicVolume(float volume)
    {
        audioSourceMusic.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxVolume = Mathf.Clamp01(volume);
        foreach (AudioSource audioSourceSFX in audioSourceSFX)
        {
            audioSourceSFX.volume = sfxVolume;
        }
        Debug.Log("SFX volume set to " + sfxVolume);
    }

    public void  GetSoundVolume()
    {
        Debug.Log("volume sound" + (int)(audioSourceSFX[0].volume * 100));
    }

    // audio for button clip beacause this system not nomal active
    public void PlaySoundButton()
    {
        audioSourceButton.PlayOneShot(audioSourceButton.clip);
    }

}
