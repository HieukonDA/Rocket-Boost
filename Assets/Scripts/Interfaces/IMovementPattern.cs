using UnityEngine;

public interface IMovementPattern
{
    Vector3 CalculatePosition(Vector3 startPosition, Vector3 endPosition, float time);
}