public interface IObstacleSpawner
{
    void SpawnTunnelWalls(Segment segment, float levelHeight, float tunnelHeightBottom);
    bool SpawnAdditionalObstacle(Segment segment, float levelHeight);
}