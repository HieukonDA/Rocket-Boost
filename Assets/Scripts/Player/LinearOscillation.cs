using UnityEngine;

public class LinearOscillation : IMovementPattern
{
    private readonly float speed;

    public LinearOscillation(float speed)
    {
        this.speed = speed;
    }

    public Vector3 CalculatePosition(Vector3 startPosition, Vector3 endPosition, float time)
    {
        float movementFactor = Mathf.PingPong(time * speed, 1f);
        return Vector3.Lerp(startPosition, endPosition, movementFactor);
    }
}