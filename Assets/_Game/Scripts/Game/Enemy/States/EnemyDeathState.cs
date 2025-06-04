using UnityEngine;

public class EnemyDeathState : EnemyStateBase
{
    public EnemyDeathState(EnemyStateMachine stateMachine, EnemyBase enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        enemy.EnemyEffectHandler.OnDissolveComplete += OnDissolveEffectComplete;
        enemy.EnemyEffectHandler.PlayDeathEffect();
        enemy.EnemyAnimator.PlayAnimation_Die();
        enemy.Agent.enabled = false;
        enemy.Collider.enabled = false;
        // Notify any listeners about the death
        EventBusManager.Instance.Publish(EventType.EnemyDied, null);

    }

    private void OnDissolveEffectComplete()
    {
        enemy.EnemyEffectHandler.OnDissolveComplete -= OnDissolveEffectComplete;
        ObjectPool.Instance.ReturnToPool(PoolType.Zombie, enemy.gameObject);
    }
} 