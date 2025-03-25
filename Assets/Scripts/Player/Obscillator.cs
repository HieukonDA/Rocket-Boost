using UnityEngine;

public class Obscillator : MonoBehaviour
{
    
    [SerializeField] private Vector3 movementVector;
    [SerializeField] private float speed = 1.0f;

    private Vector3 startPosition;
    private Vector3 endPosition;
    private float movementFactor;

    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        startPosition = transform.position;
        endPosition = startPosition + movementVector;
    }

    // Update is called once per frame
    void Update()
    {
        movementFactor = Mathf.PingPong(Time.time * speed, 1f);
        transform.position = Vector3.Lerp(startPosition, endPosition, movementFactor);
    }
}
