using System.Collections.Generic;
using UnityEngine;

public class ObstacleSpawner : IObstacleSpawner
{
    private readonly ObstacleConfig config;
    private readonly Transform parent;
    private readonly List<GameObject> spawnedObjects;
    private readonly float safeZoneRadius;

    public ObstacleSpawner(ObstacleConfig config, Transform parent, List<GameObject> spawnedObjects, float safeZoneRadius)
    {
        this.config = config;
        this.parent = parent;
        this.spawnedObjects = spawnedObjects;
        this.safeZoneRadius = safeZoneRadius;
    }

    public void SpawnTunnelWalls(Segment segment, float levelHeight, float tunnelHeightBottom)
    {
        float minX = Mathf.Min(segment.start.x, segment.end.x) + safeZoneRadius;
        float maxX = Mathf.Max(segment.start.x, segment.end.x) - safeZoneRadius;

        float segmentLengthX = Mathf.Abs(segment.end.x - segment.start.x);
        int wallCount = Mathf.Max(2, Mathf.FloorToInt(segmentLengthX / 5f));
        float stepX = segmentLengthX / (wallCount + 1);

        float tunnelHeightTop = UnityEngine.Random.Range(levelHeight * 0.7f, levelHeight);

        for (int i = 1; i <= wallCount; i++)
        {
            float xPos = minX + stepX * i;

            Vector3 topPosition = new Vector3(xPos, tunnelHeightTop, segment.start.z);
            GameObject topWall = Object.Instantiate(config.wallHorizontalPrefab, topPosition, Quaternion.identity, parent);
            spawnedObjects.Add(topWall);

            Vector3 bottomPosition = new Vector3(xPos, tunnelHeightBottom, segment.start.z);
            GameObject bottomWall = Object.Instantiate(config.wallHorizontalPrefab, bottomPosition, Quaternion.identity, parent);
            spawnedObjects.Add(bottomWall);
        }
    }

    public bool SpawnAdditionalObstacle(Segment segment, float levelHeight)
    {
        int obstacleType = UnityEngine.Random.Range(0, 3);
        Vector3 position;
        GameObject obstacle;

        float minX = Mathf.Min(segment.start.x, segment.end.x) + safeZoneRadius;
        float maxX = Mathf.Max(segment.start.x, segment.end.x) - safeZoneRadius;
        float minY = 0f;
        float maxY = levelHeight;

        float midX = (segment.start.x + segment.end.x) / 2f;
        float midY = (segment.start.y + segment.end.y) / 2f;
        float segmentLengthX = Mathf.Abs(segment.end.x - segment.start.x);
        float segmentLengthY = Mathf.Abs(segment.end.y - segment.end.y);
        float midZoneX = segmentLengthX * 0.1f;
        float midZoneY = segmentLengthY * 0.1f;

        switch (obstacleType)
        {
            case 0:
                position = new Vector3(
                    UnityEngine.Random.Range(minX, maxX),
                    UnityEngine.Random.Range(0f, levelHeight - 2f),
                    segment.start.z
                );

                if (segment.direction == "Horizontal")
                {
                    if (Mathf.Abs(position.x - midX) < midZoneX)
                    {
                        return false;
                    }
                }
                else if (segment.direction == "Vertical")
                {
                    if (Mathf.Abs(position.y - midY) < midZoneY)
                    {
                        return false;
                    }
                }
                else
                {
                    if (Mathf.Abs(position.x - midX) < midZoneX && Mathf.Abs(position.y - midY) < midZoneY)
                    {
                        return false;
                    }
                }

                obstacle = Object.Instantiate(config.wallVerticalPrefab, position, Quaternion.identity, parent);
                break;

            case 1:
                position = new Vector3(
                    UnityEngine.Random.Range(minX, maxX),
                    UnityEngine.Random.Range(2f, levelHeight - 2f),
                    segment.start.z
                );
                obstacle = Object.Instantiate(config.rotatingDoorPrefab, position, Quaternion.identity, parent);
                break;

            case 2:
                position = new Vector3(
                    UnityEngine.Random.Range(minX, maxX),
                    UnityEngine.Random.Range(1f, levelHeight - 1f),
                    segment.start.z
                );
                obstacle = Object.Instantiate(config.laserTrapPrefab, position, Quaternion.identity, parent);
                break;

            default:
                return false;
        }

        spawnedObjects.Add(obstacle);
        return true;
    }
}