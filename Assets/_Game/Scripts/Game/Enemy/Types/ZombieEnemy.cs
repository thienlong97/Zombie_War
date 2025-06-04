using UnityEngine;

public class ZombieEnemy : EnemyBase, IPooledObject
{
    private const float MAX_DISTANCE = 20.0f;

    protected override void Awake()
    {
        base.Awake();
    }

    protected override void Update()
    {
        base.Update();
        UpdateDistance();
    }

    protected override void FixedUpdate()
    {
        base.FixedUpdate();
    }

    public void OnObjectSpawn()
    {
       
    }

    public void OnObjectReturn()
    {
   
    }

    protected override void Die()
    {        
        base.Die();
       // ObjectPool.Instance.ReturnToPool(PoolType.Zombie, gameObject);
    }

    private void UpdateDistance()
    {
        if (PlayerTransform != null && Vector3.Distance(transform.position, PlayerTransform.position) > MAX_DISTANCE)
        {
            Agent.enabled = false;
            EventBusManager.Instance.Publish(EventType.EnemyOutOfRange, this);
            ObjectPool.Instance.ReturnToPool(PoolType.Zombie, gameObject);
        }
    }

    // Override TakeDamage to add zombie-specific behavior
    public override void TakeDamage(int damage)
    {
        // Play zombie hurt sound
        //AudioManager.Instance.PlaySound("ZombieHurt");
        
        base.TakeDamage(damage);
    }
} 