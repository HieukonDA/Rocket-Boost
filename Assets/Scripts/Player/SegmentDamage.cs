using UnityEngine;

public class SegmentDamage : MonoBehaviour, ISegmentQuantity
{
    [SerializeField] protected int damage = 1;
    [SerializeField] protected int coins = 5;

    public virtual int GetDamage() => damage;
    public virtual int GetCoins() => coins;
}