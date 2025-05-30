using UnityEngine;

[CreateAssetMenu(fileName = "Rifle",menuName = "WeaponStrategy/Rife")]
public class RifleWeapon : WeaponStrategy
{
    public override void Fire(Transform firePoint)
    {
        base.Fire(firePoint);
        //var projectile =  Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
        var projectile =  ObjectPool.Instance.SpawnFromPool(ProjectileConfig.projectilePoolType,
        firePoint.position,
        firePoint.rotation);
        var projectileComponent = projectile.GetComponent<Projectile>();
        projectileComponent.Initialize(ProjectileConfig, firePoint);
        projectileComponent.SpawnMuzzle(firePoint.forward);
    }
}
