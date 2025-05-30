using UnityEngine;
using System.Threading.Tasks;

public abstract class WeaponStrategy : ScriptableObject
{
    [SerializeField] private ProjectileConfig projectileConfig;
    [SerializeField] private AudioClip soundFx;
    [SerializeField] protected GameObject projectilePrefab;
    [SerializeField] private float fireRate = 0.5f;

    public int Damage => projectileConfig.damage;
    public float FireRate => fireRate;
    public ProjectileConfig ProjectileConfig => projectileConfig;
    public AudioClip SoundFx => soundFx;

    public virtual void Initialize()
    {

    }

    public virtual void Fire(Transform firePoint)
    {
        EventBusManager.Instance.Publish(EventType.PlayerFired, soundFx);
    }

}
