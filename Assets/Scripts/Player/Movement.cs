using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.SceneManagement;

public class Movement : MonoBehaviour
{
    [SerializeField] private InputAction thrust;
    [SerializeField] private InputAction rotation;
    [SerializeField] private float thrustStrength = 1f;
    [SerializeField] private float rotationStrength = 1f;
    [SerializeField] private ParticleSystem mainEngineParticles;
    [SerializeField] private ParticleSystem leftEngineParticles;
    [SerializeField] private ParticleSystem rightEngineParticles;

    [SerializeField] private float figureEightSpeed = 2f; // Tốc độ quỹ đạo số 8
    [SerializeField] private float figureEightWidth = 5f; // Chiều rộng số 8 (trục X)
    [SerializeField] private float figureEightHeight = 3f; // Chiều cao số 8 (trục Y)

    private Rigidbody rb;
    private IAudioManager audioManager;
    private MovementFeedbackPlayer feedbackPlayer;
    private float figureEightTime;

    private void Awake()
    {
        rb = GetComponent<Rigidbody>();
        audioManager = AudioManager.Instance;
        feedbackPlayer = new MovementFeedbackPlayer(audioManager, mainEngineParticles, leftEngineParticles, rightEngineParticles);
    }

    private void OnEnable()
    {
        thrust.Enable();
        rotation.Enable();
    }

    private void FixedUpdate()
    {
        ProcessThrust();
        ProcessRotation();
        if (SceneManager.GetActiveScene().name == "Level1")
        {
            ProcessThrustInFigureEight();
        }
    }

    private void ProcessThrust()
    {
        if (thrust.IsPressed())
        {
            StartThrusting();
        }
        else
        {
            StopThrusting();
        }
    }

    private void StartThrusting()
    {
        rb.AddRelativeForce(Vector3.up * thrustStrength * Time.fixedDeltaTime);
        feedbackPlayer.PlayThrustFeedback();
    }

    private void StopThrusting()
    {
        feedbackPlayer.StopThrustFeedback();
    }

    

    private void ProcessRotation()
    {
        float rotationInput = rotation.ReadValue<float>();
        if (rotationInput < 0)
        {
            RotateRight();
        }
        else if (rotationInput > 0)
        {
            RotateLeft();
        }
        else
        {
            StopRotating();
        }
    }

    private void ProcessThrustInFigureEight()
    {
        // Tăng thời gian cho quỹ đạo
        figureEightTime += Time.fixedDeltaTime * figureEightSpeed;

        // Tính toán vận tốc theo quỹ đạo số 8
        float xVelocity = figureEightWidth * Mathf.Cos(figureEightTime); // Sin cho trục X
        float yVelocity = figureEightHeight * Mathf.Sin(2f * figureEightTime); // Sin(2t) cho trục Y (số 8)

        // Áp dụng vận tốc
        rb.linearVelocity = new Vector3(xVelocity, yVelocity, 0f);

        // Xoay player theo hướng vận tốc
        if (Mathf.Abs(xVelocity) > 0.01f || Mathf.Abs(yVelocity) > 0.01f)
        {
            float angle = Mathf.Atan2(yVelocity, xVelocity) * Mathf.Rad2Deg - 90f; // Góc theo vận tốc
            transform.rotation = Quaternion.Euler(0, 0, angle);
        }

        // Hiệu ứng hạt
        if (!mainEngineParticles.isPlaying)
        {
            mainEngineParticles.Play();
            feedbackPlayer.PlayThrustFeedback();
        }
    }

    public void SetPosition(Vector3 position)
    {
        transform.position = position;
    }

    private void RotateRight()
    {
        ApplyRotation(rotationStrength);
        feedbackPlayer.PlayRightRotationFeedback();
    }

    private void RotateLeft()
    {
        ApplyRotation(-rotationStrength);
        feedbackPlayer.PlayLeftRotationFeedback();
    }

    private void StopRotating()
    {
        feedbackPlayer.StopRotationFeedback();
    }

    private void ApplyRotation(float RotationStrength)
    {
        transform.Rotate(0, 0, RotationStrength * Time.fixedDeltaTime);
    }
}
