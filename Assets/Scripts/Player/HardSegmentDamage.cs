using UnityEngine;

public class HardSegmentDamage : SegmentDamage
{
    [SerializeField] private int extraDamage = 2;

    public override int GetDamage() => damage + extraDamage;
}