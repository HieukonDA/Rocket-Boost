using System;
using System.Data;
using UnityEditor.SearchService;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float delay = 2f;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem crashParticles;

    private IAudioManager audioManager;
    private ILevelManager levelManager;
    private CollisionFeedbackPlayer feedbackPlayer;
    private DebugHandler debugHandler;
    private SkillHandler skillHandler;

    private bool isControllable = true;
    private int health = 3;
    private int coinsCollected = 0;

    private void Awake()
    {
        levelManager = LevelManager.Instance;
        audioManager = AudioManager.Instance;
        feedbackPlayer = new CollisionFeedbackPlayer(audioManager, successParticles, crashParticles);
        debugHandler = new DebugHandler(levelManager);
        // skillHandler = gameObject.AddComponent<SkillHandler>();
    }

    void Update()
    {
        debugHandler.Update();
    }

    private void OnCollisionEnter(Collision other)
    {
        if (!isControllable || !debugHandler.IsCollisionable()) return;

        switch (other.gameObject.tag)
        {
            case "Start":
                break;
            case "Friendly":
                break;
            case "Finish":
                StartSuccessSequence();
                break;
            case "SkillCircle":
                skillHandler.ApplySkill(other.gameObject);
                break;
            default:
                StartCrashSequence();
                break;
        }       

        ISegmentQuantity segment = other.gameObject.GetComponentInParent<ISegmentQuantity>();
        if (segment != null)
        {
            coinsCollected += segment.GetCoins();
            HUDManager.Instance.UpdateScore(coinsCollected);
            if (!skillHandler.HasShield())
            {
                health -= skillHandler.HasPierce() ? 0 : segment.GetDamage();
                if (health <= 0) StartCrashSequence();
            }
        }
    }

    private void StartCrashSequence()
    {
        isControllable = false;
        feedbackPlayer.PlayCrashFeedback();
        GetComponent<Movement>().enabled = false;
        Invoke("OnPlayerDeath", delay);
    }

    private void StartSuccessSequence()
    {
        isControllable = false;
        feedbackPlayer.PlaySuccessFeedback();
        GetComponent<Movement>().enabled = false;
        Invoke("OnLevelComplete", delay);
    }

    private void OnPlayerDeath()
    {
        levelManager.OnPlayerDeath(transform.position);
    }

    private void OnLevelComplete()
    {
        levelManager.OnLevelComplete();
    }
}    
