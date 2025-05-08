using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour, ICameraManager
{
    private CinemachineCamera cinemachineCam;

    void Awake()
    {
        cinemachineCam = GetComponent<CinemachineCamera>();
        if (cinemachineCam == null)
        {
            Debug.LogError("CinemachineCamera not found!");
            Destroy(gameObject);
            return;
        }
    }

    public void SetTarget(Transform target)
    {
        Debug.Log("Camera target set to: " + (target != null ? target.name : "Null"));
        cinemachineCam.Follow = target;
    }
}
