using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class AudioManager : MonoBehaviour, IAudioManager
{
    public static AudioManager Instance { get; private set; }

    [System.Serializable]
    public class Sound
    {
        public string name;
        public AudioClip clip;
    }

    [SerializeField] private List<Sound> sounds;
    [SerializeField] private AudioSource audioSourceMusic;
    [SerializeField] private AudioSource audioSourceButton;
    [SerializeField] private AudioSource audioSourceCoin;
    [SerializeField] private int sfxSourceCount = 5;
    
    private SFXPlayer sfxPlayer;
    private Dictionary<string, AudioClip> soundDictionary;
    


    private void Awake()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }
        Instance = this;
        DontDestroyOnLoad(gameObject);

        InitSounds();
        sfxPlayer = new SFXPlayer(gameObject, sfxSourceCount);
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
            sfxPlayer.PlaySound(soundDictionary[soundName]);
        }
        else
        {
            Debug.LogWarning($"Sound {soundName} not found");
        }
    }

    public void StopSound()
    {
        sfxPlayer.StopSound();
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
            Debug.LogWarning($"Sound {soundName} not found");
        }
    }

    public void StopMusic()
    {
        audioSourceMusic.Stop();
    }

    public bool IsSoundPlaying()
    {
        if (sfxPlayer == null)
    {
        Debug.LogWarning("SFXPlayer is null, reinitializing...");
        sfxPlayer = new SFXPlayer(gameObject, sfxSourceCount);
    }
        return sfxPlayer.IsSoundPlaying();
    }

    public void SetMusicVolume(float volume)
    {
        audioSourceMusic.volume = volume;
    }

    public void SetSFXVolume(float volume)
    {
        sfxPlayer.SetSFXVolume(volume);
    }

    // audio for button clip beacause this system not nomal active
    public void PlaySoundButton()
    {
        audioSourceButton.PlayOneShot(audioSourceButton.clip);
    }

    public void PlaySoundCoin()
    {
        audioSourceCoin.PlayOneShot(audioSourceCoin.clip);
    }
}
