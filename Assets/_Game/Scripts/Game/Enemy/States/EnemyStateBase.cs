using UnityEngine;

public abstract class EnemyStateBase
{
    protected readonly EnemyStateMachine stateMachine;
    protected readonly EnemyBase enemy;

    protected EnemyStateBase(EnemyStateMachine stateMachine, EnemyBase enemy)
    {
        this.stateMachine = stateMachine;
        this.enemy = enemy;
    }

    public virtual void Enter() { }
    public virtual void Exit() { }
    public virtual void Update() { }
    public virtual void FixedUpdate() { }

    protected bool IsInAttackRange()
    {
        if (enemy.PlayerTransform == null) return false;
        return Vector3.Distance(enemy.transform.position, enemy.PlayerTransform.position) <= enemy.EnemyConfig.AttackRange;
    }

    protected bool IsInDetectionRange()
    {
        if (enemy.PlayerTransform == null) return false;
        return Vector3.Distance(enemy.transform.position, enemy.PlayerTransform.position) <= enemy.EnemyConfig.DetectionRange;
    }

    protected void FaceTarget()
    {
        if (enemy.PlayerTransform == null) return;
        Vector3 direction = (enemy.PlayerTransform.position - enemy.transform.position).normalized;
        enemy.transform.rotation = Quaternion.LookRotation(direction);
    }
} 