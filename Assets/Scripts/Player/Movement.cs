using UnityEditor;
using UnityEngine;
using UnityEngine.InputSystem;

public class Movement : MonoBehaviour
{
    [SerializeField] private InputAction thrust;
    [SerializeField] private InputAction rotation;
    [SerializeField] private float thrustStrength = 1f;
    [SerializeField] private float rotationStrength = 1f;
    [SerializeField] private ParticleSystem mainEngineParticles;
    [SerializeField] private ParticleSystem leftEngineParticles;
    [SerializeField] private ParticleSystem rightEngineParticles;

    private Rigidbody rb;
    private IAudioManager audioManager;
    private MovementFeedbackPlayer feedbackPlayer;

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
        rb.freezeRotation = true;
        transform.Rotate(Vector3.forward * RotationStrength * Time.fixedDeltaTime);
        rb.freezeRotation = false;
    }
}
