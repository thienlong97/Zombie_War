using UnityEngine;

public class EnemyDeathState : EnemyStateBase
{
    public EnemyDeathState(EnemyStateMachine stateMachine, EnemyBase enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        enemy.EnemyAnimator.PlayAnimation_Die();
        enemy.Agent.enabled = false;
        
        // Notify any listeners about the death
        EventBusManager.Instance.Publish(EventType.EnemyDied, null);
        
        // Destroy the enemy after animation
        ObjectPool.Instance.ReturnToPool(PoolType.Zombie, enemy.gameObject);
    }
} 