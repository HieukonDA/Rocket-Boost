using System;
using System.Data;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float levelLoadDelay = 2f;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem crashParticles;

    private ILevelManager levelManager;
    private IAudioManager audioManager;
    private CollisionFeedbackPlayer feedbackPlayer;
    private DebugHandler debugHandler;
    private bool isControllable = true;

    private void Awake()
    {
        levelManager = LevelManager.Instance;
        audioManager = AudioManager.Instance;
        feedbackPlayer = new CollisionFeedbackPlayer(audioManager, successParticles, crashParticles);
        debugHandler = new DebugHandler(levelManager);
    }

    void Update()
    {
        debugHandler.Update();
    }

    private void OnCollisionEnter(Collision other)
    {
        Debug.Log("Collision detected with: " + other.gameObject.name + " at time: " + Time.time);
        if (!isControllable || !debugHandler.IsCollisionable()) return;

        switch (other.gameObject.tag)
        {
            case "Friendly":
                Debug.Log("This thing is friendly");
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            default:
                StartCrashSequence();
                break;
        }
    }

    private void StartSuccessSequence()
    {
        isControllable = false;
        feedbackPlayer.PlaySuccessFeedback();
        GetComponent<Movement>().enabled = false;
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    private void StartCrashSequence()
    {
        isControllable = false;
        feedbackPlayer.PlayCrashFeedback();
        levelManager.OnPlayerDeath(transform.position);
        GetComponent<Movement>().enabled = false;
        Invoke("ReloadLevel", 2f);
    }

    private void ReloadLevel()
    {
        levelManager.RestartLevel();
    }

    private void LoadNextLevel()
    {
        levelManager.OnLevelComplete();
    }
}
