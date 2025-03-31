using System.Collections.Generic;
using UnityEngine;

public class SFXPlayer
{
    private readonly List<AudioSource> audioSourceSFX;
    private int currentSFXSourceIndex = 0;
    private float sfxVolume = 1.0f;

    public SFXPlayer(GameObject parent, int sfxSourceCount)
    {
        audioSourceSFX = new List<AudioSource>();
        for (int i = 0; i < sfxSourceCount; i++)
        {
            AudioSource source = parent.AddComponent<AudioSource>();
            source.volume = sfxVolume;
            source.spatialBlend = 0.0f;
            source.pitch = 1.0f;
            audioSourceSFX.Add(source);
        }
    }

    public void PlaySound(AudioClip clip)
    {
        audioSourceSFX.RemoveAll(source => source == null); // Xóa trước khi tìm
        AudioSource audioSource = GetNextAvailableSFXSource();
        audioSource.Stop();
        audioSource.volume = sfxVolume;
        audioSource.spatialBlend = 0.0f;
        audioSource.pitch = 1.0f;
        audioSource.PlayOneShot(clip);
    }

    public void StopSound()
    {
        audioSourceSFX.RemoveAll(source => source == null); // Xóa trước khi tìm
        foreach (AudioSource audioSource in audioSourceSFX)
        {
            audioSource.Stop();
        }
    }

    public bool IsSoundPlaying()
    {
        audioSourceSFX.RemoveAll(source => source == null); // Xóa các tham chiếu null
        foreach (AudioSource audioSource in audioSourceSFX)
        {
            if (audioSource != null && audioSource.isPlaying)
            {
                return true;
            }
        }
        return false;
    }

    public void SetSFXVolume(float volume)
    {
        audioSourceSFX.RemoveAll(source => source == null); // Xóa trước khi tìm
        sfxVolume = Mathf.Clamp01(volume);
        foreach (AudioSource audioSource in audioSourceSFX)
        {
            audioSource.volume = sfxVolume;
        }
    }

    private AudioSource GetNextAvailableSFXSource()
    {
        audioSourceSFX.RemoveAll(source => source == null); // Xóa trước khi tìm
        foreach (AudioSource audioSource in audioSourceSFX)
        {
            if (!audioSource.isPlaying)
            {
                return audioSource;
            }
        }

        currentSFXSourceIndex = (currentSFXSourceIndex + 1) % audioSourceSFX.Count;
        return audioSourceSFX[currentSFXSourceIndex];
    }
}