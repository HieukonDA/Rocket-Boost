using UnityEngine;

[CreateAssetMenu(fileName = "ObstacleConfig", menuName = "Configs/ObstacleConfig", order = 1)]
public class ObstacleConfig : ScriptableObject
{
    public GameObject playerPrefab;
    public GameObject startPrefab;
    public GameObject goalPrefab;
    public GameObject wallHorizontalPrefab;
    public GameObject wallVerticalPrefab;
    public GameObject rotatingDoorPrefab;
    public GameObject laserTrapPrefab;
}