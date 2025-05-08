using UnityEngine;

public class CollisionHandler : MonoBehaviour
{
    [SerializeField] float delay = 2f;
    [SerializeField] ParticleSystem successParticles;
    [SerializeField] ParticleSystem crashParticles;

    private IAudioManager audioManager;
    private ILevelManager levelManager;
    private CollisionFeedbackPlayer feedbackPlayer;
    private DebugHandler debugHandler;
    private CoinManager coinManager;

    private bool isControllable = true;

    private void Awake()
    {
        levelManager = LevelManager.Instance;
        audioManager = AudioManager.Instance;
        feedbackPlayer = new CollisionFeedbackPlayer(audioManager, successParticles, crashParticles);
        debugHandler = new DebugHandler(levelManager);
        coinManager = CoinManager.Instance; // Lấy CoinManager từ scene
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
            default:
                Debug.LogError($"CollisionHandler: Collision with {other.gameObject.name} detected.");
                if (SkillManager.Instance != null && SkillManager.Instance.IsShieldActive())
                {
                    Debug.Log("CollisionHandler: Shield active, ignoring obstacle collision.");
                    return; // Bỏ qua va chạm với obstacle
                }
                StartCrashSequence();
                break;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!isControllable || !debugHandler.IsCollisionable()) return;

        switch (other.gameObject.tag)
        {
            case "Coin":
                HandleCoinCollection(other.gameObject);
                break;
            case "Shield":
                // Nhặt skill shield
                Debug.Log("CollisionHandler: Shield skill collected!");
                HandleShieldCollection(other);
                break;
            default:
                break;
        }
    }

    private void HandleShieldCollection(Collider other)
    {
        if (SkillManager.Instance != null)
        {
            SkillManager.Instance.ActivateSkill("Shield", other.transform.position, transform);
            HUDManager.Instance.GetImageTimer().gameObject.SetActive(true); // Hiện thanh thời gian
            Debug.Log("CollisionHandler: Activated Shield skill.");
            other.gameObject.SetActive(false); // Deactive SkillCircle sau khi nhặt
        }
        else
        {
            Debug.LogWarning("CollisionHandler: SkillManager.Instance is null! Cannot activate skill.");
        }
    }

    private void HandleCoinCollection(GameObject coin)
    {
        if (coin.activeSelf)
        {
            coin.SetActive(false);
            feedbackPlayer.PlayCoinFeedback();
            CoinManager.Instance.CollectCoin(); // Tăng coinCount trong CoinManager
            HUDManager.Instance.UpdateScore(CoinManager.Instance.GetCoinCount()); // Lấy từ CoinManager
            Debug.Log($"CollisionHandler: Coin collected at {coin.transform.position}. Total coins: {CoinManager.Instance.GetCoinCount()}");
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
