using UnityEngine;
using Unity.Cinemachine;
using UnityEngine.SceneManagement;

public class CameraManager : MonoBehaviour
{
    private CinemachineCamera cinemachineCam;

    void Awake()
    {
        cinemachineCam = GetComponent<CinemachineCamera>();
        Debug.Log("CameraManager created");
        if(cinemachineCam == null)
        {
            Debug.LogError("CinemachineCamera not found!");
            Destroy(gameObject);
            return;
        }

        int currentIndex = SceneManager.GetActiveScene().buildIndex;
        if (currentIndex == 2)
        {
            DontDestroyOnLoad(gameObject);
        }
    }

    public void SetTarget(Transform target)
    {
        cinemachineCam.Follow = target;
    }
}
