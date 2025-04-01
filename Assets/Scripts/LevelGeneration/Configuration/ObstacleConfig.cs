using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleConfig", menuName = "Configs/ObstacleConfig", order = 1)]
public class ObstacleConfig : ScriptableObject
{
    public GameObject playerPrefab;
    public GameObject launchSegmentPrefab; // Đoạn đầu cố định
    public GameObject finishSegmentPrefab; // Đoạn cuối cố định
    public GameObject[] middleSegmentPrefabs; // Các đoạn giữa ngẫu nhiên
    public GameObject skillCirclePrefab;
}