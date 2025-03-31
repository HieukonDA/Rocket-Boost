using UnityEngine;

public class CollisionFeedbackPlayer
{
    private readonly IAudioManager audioManager;
    private readonly ParticleSystem successParticles;
    private readonly ParticleSystem crashParticles;

    public CollisionFeedbackPlayer(IAudioManager audioManager, ParticleSystem successParticles, ParticleSystem crashParticles)
    {
        this.audioManager = audioManager;
        this.successParticles = successParticles;
        this.crashParticles = crashParticles;
    }

    public void PlaySuccessFeedback()
    {
        audioManager.StopSound();
        audioManager.PlaySound("success");
        successParticles.Play();
    }

    public void PlayCrashFeedback()
    {
        audioManager.StopSound();
        audioManager.PlaySound("crash");
        crashParticles.Play();
    }
}