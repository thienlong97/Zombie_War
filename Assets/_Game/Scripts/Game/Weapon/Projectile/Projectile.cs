using UnityEngine;
using System.Threading.Tasks;
using System.Threading;

public class Projectile : MonoBehaviour , IPooledObject
{
    [SerializeField] private PoolType _muzzleVFXPoolType = PoolType.MuzzleVFX;
    [SerializeField] private PoolType _hitVFXPoolType = PoolType.HitVFX;
    [SerializeField] private Rigidbody _rigidbody;

    private ProjectileConfig _projectileConfig;
    private Transform _firePoint;
    private CancellationTokenSource _cancellationTokenSource;
    private bool _isCollision;

    public void OnObjectSpawn()
    {
        _rigidbody.linearVelocity = Vector3.zero;
        _isCollision = false;
    }

    public void OnObjectReturn()
    {
        _rigidbody.linearVelocity = Vector3.zero;
        _cancellationTokenSource?.Cancel();
 
    }

    private void OnEnable()
    {
        
    }

    public void OnDestroy()
    {
        _cancellationTokenSource?.Cancel();
    }

    public void Initialize(ProjectileConfig config, Transform firePoint)
    {
        _projectileConfig = config;
        _firePoint = firePoint;

        _cancellationTokenSource = new CancellationTokenSource();
        DestroyAfterTime(_projectileConfig.lifetime, _cancellationTokenSource.Token);
    }

    private void FixedUpdate()
    {
        UpdatePhysics();
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (_isCollision) return;  // Skip if already returning to pool
        
        _isCollision = true;
        SpawnHitVfx(collision);
        CheckHit(collision);
        Destroy();
    }

    private void UpdatePhysics()
    {
        _rigidbody.linearVelocity = transform.forward * _projectileConfig.speed;
    }

    private void CheckHit(Collision collision)
    {
        IDamageable damageable = collision.gameObject.GetComponent<IDamageable>();
        damageable?.TakeDamage(_projectileConfig.damage);
    }

    private void SpawnHitVfx(Collision collision)
    {
        ContactPoint contact = collision.contacts[0];
        var hitVfx = ObjectPool.Instance.SpawnFromPool(_hitVFXPoolType,contact.point,Quaternion.identity);
        hitVfx.transform.rotation = Quaternion.LookRotation(-transform.forward);
        DestroyParticleSystem(hitVfx,_hitVFXPoolType);
    }

    public void SpawnMuzzle(Vector3 direction)
    {
        var muzzleVFX = ObjectPool.Instance.SpawnFromPool(_muzzleVFXPoolType, transform.position, Quaternion.identity);
        muzzleVFX.transform.rotation = Quaternion.LookRotation(-direction);
        muzzleVFX.transform.forward = direction;
        DestroyParticleSystem(muzzleVFX,_muzzleVFXPoolType);
    }

    public void Destroy()
    {            
        _cancellationTokenSource?.Cancel();
        ObjectPool.Instance.ReturnToPool(PoolType.Bullet, gameObject);
    }

    private async void DestroyParticleSystem(GameObject vfx,PoolType poolType)
    {
        var ps = vfx. GetComponent<ParticleSystem>();
        if (vfx == null) vfx.GetComponentInChildren<ParticleSystem>();
        await Task.Delay((int)(ps.main.duration * 1000));
        ObjectPool.Instance.ReturnToPool(poolType,vfx);
    }

   public async void DestroyAfterTime(float time, CancellationToken token)
    {
        try
        {
            await Task.Delay((int)(time * 1000), token);
            if (!token.IsCancellationRequested)
            {
                ObjectPool.Instance.ReturnToPool(PoolType.Bullet, gameObject);
            }
        }
        catch (TaskCanceledException)
        {
           
        }
    }
}
