using UnityEngine;

[CreateAssetMenu(fileName = "ShotGun", menuName = "WeaponStrategy/ShotGun")]
public class ShotGunWeapon : WeaponStrategy
{
    [SerializeField] private int _bulletCount = 3;
    private const float spreadAngle = 15.0f;

    public override void Fire(Transform firePoint)
    {
        base.Fire(firePoint);
        for (int i = 0; i < _bulletCount; i++)
        {
            //var projectile = Instantiate(projectilePrefab, firePoint.position, firePoint.rotation);
            var projectile =  ObjectPool.Instance.SpawnFromPool(ProjectileConfig.projectilePoolType,
           
            firePoint.position,
            firePoint.rotation);
            var projectileComponent = projectile.GetComponent<Projectile>();
            projectile.transform.Rotate(0.0f, GetRotateAngleY(i), 0.0f);
            projectileComponent.Initialize(ProjectileConfig, firePoint);


            if (i == 0) projectileComponent.SpawnMuzzle(firePoint.forward);
        }
    }

    private float GetRotateAngleY(int index)
    {
        float half = _bulletCount / 2;
        float t = (float)index / (float)_bulletCount;
        float minAngle = -half * spreadAngle;
        float maxAngle = half * spreadAngle;
        float angleY = Mathf.Lerp(minAngle, maxAngle, t);
        return angleY;
    }
}
