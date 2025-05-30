using UnityEngine;

public struct EnemyHitData
{
    public float Damage { get; private set; }
    public Vector3 HitPoint { get; private set; }

    public EnemyHitData(float damage, Vector3 hitPoint )
    {
        Damage = damage;
        HitPoint = hitPoint;
    }
} 