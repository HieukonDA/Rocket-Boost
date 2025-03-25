using UnityEngine;

public interface ILevelGenerator 
{
    void GenerateLevel(int levelNumber, int seed);
}
