using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.SceneManagement;

public class MovementFeedbackPlayer
{
    private readonly IAudioManager audioManager;
    private readonly ParticleSystem mainEngineParticles;
    private readonly ParticleSystem leftEngineParticles;
    private readonly ParticleSystem rightEngineParticles;

    public MovementFeedbackPlayer(IAudioManager audioManager, ParticleSystem mainEngineParticles, ParticleSystem leftEngineParticles, ParticleSystem rightEngineParticles)
    {
        this.audioManager = audioManager;
        this.mainEngineParticles = mainEngineParticles;
        this.leftEngineParticles = leftEngineParticles;
        this.rightEngineParticles = rightEngineParticles;
    }

    public void PlayThrustFeedback()
    {
        if (!audioManager.IsSoundPlaying() && SceneManager.GetActiveScene().name != "Level1") 
        {
            audioManager.PlaySound("thrust");
        }
        if (!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
        }
    }

    public void StopThrustFeedback()
    {
        if (audioManager == null)
        {
            Debug.LogWarning("Stopthrustfeedback : audioManager is null, cannot stop sound");
        }
        if (audioManager.IsSoundPlaying())
        {
            audioManager.StopSound();
            Debug.LogWarning("Stopthrustfeedback : Stopping thrust sound");
        }
        mainEngineParticles.Stop();
    }

    public void PlayRightRotationFeedback()
    {
        if (!leftEngineParticles.isPlaying)
        {
            leftEngineParticles.Stop();
            rightEngineParticles.Play();
        }
    }

    public void PlayLeftRotationFeedback()
    {
        if (!rightEngineParticles.isPlaying)
        {
            rightEngineParticles.Stop();
            leftEngineParticles.Play();
        }
    }

    public void StopRotationFeedback()
    {
        leftEngineParticles.Stop();
        rightEngineParticles.Stop();
    }
}