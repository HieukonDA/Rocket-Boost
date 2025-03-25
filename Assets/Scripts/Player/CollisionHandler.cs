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

    AudioSource audioSource;

    bool isControllable = true;
    bool isCollisionable = true;


    void Update()
    {
        RespondToDebugKeys();
    }

    private void RespondToDebugKeys()
    {
        if (Debug.isDebugBuild)
        {
            if (Input.GetKeyDown(KeyCode.L))
            {
                LoadNextLevel();
            }
            else if (Keyboard.current.cKey.wasPressedThisFrame)
            {
                isCollisionable = !isCollisionable;
            }
        }
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!isControllable || !isCollisionable) return;

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

    void StartSuccessSequence()
    {
        isControllable = false;
        //audio
        AudioManager.Instance.StopSound();
        AudioManager.Instance.PlaySound("success");

        //particles
        successParticles.Play();
        GetComponent<Movement>().enabled = false;
        Invoke("LoadNextLevel", levelLoadDelay);
    }

    void StartCrashSequence()
    {
        isControllable = false;

        //audio
        AudioManager.Instance.StopSound();
        AudioManager.Instance.PlaySound("crash");

        //particles
        crashParticles.Play();

        //save data when player died
        LevelManager.Instance.OnPlayerDeath(transform.position);

        GetComponent<Movement>().enabled = false;
        Invoke("ReloadLevel", 2f);  
    }

    void ReloadLevel()
    {
        int currentSceneIndex = SceneManager.GetActiveScene().buildIndex;
        LevelManager.Instance.RestartLevel();
    }

    void LoadNextLevel()
    {
        LevelManager.Instance.OnLevelComplete();
    }
}
