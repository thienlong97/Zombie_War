using UnityEngine;

[CreateAssetMenu(fileName = "NewProjectile", menuName = "Projectiles/Projectile Config")]
public class ProjectileConfig : ScriptableObject
{
    [Header("Prefab & Visual")]
    public PoolType projectilePoolType = PoolType.Bullet;

    [Header("Stats")]
    public float speed = 10f;
    public float lifetime = 4f;
    public int damage = 10;
}
