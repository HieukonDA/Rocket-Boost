public interface IAudioManager
{
    void PlaySound(string soundName);
    void StopSound();
    void PlayMusic(string soundName);
    void StopMusic();
    void SetMusicVolume(float volume);
    void SetSFXVolume(float volume);
    bool IsSoundPlaying();
    void PlaySoundButton();
    void PlaySoundCoin();
}