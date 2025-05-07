using UnityEngine;

public class CollisionHandleSkill : MonoBehaviour
{
    void OnCollisionEnter(Collision other)
    {
        switch(other.gameObject.tag)
        {
            case "Player":              
                break;
            default:
                break;
        }
        
    }
}
