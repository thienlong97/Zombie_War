using UnityEngine;

public class EnemyAttackState : EnemyStateBase
{
    private float nextAttackTime;

    public EnemyAttackState(EnemyStateMachine stateMachine, EnemyBase enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        enemy.EnemyAnimator.PlayAnimation_Run(false);
        nextAttackTime = 0f;
    }

    public override void Update()
    {
        if (!IsInAttackRange())
        {
            stateMachine.ChangeState(EnemyStateType.Chase);
            return;
        }

        if (Time.time >= nextAttackTime)
        {
            PerformAttack();
        }

        FaceTarget();
    }

    private void PerformAttack()
    {
        enemy.EnemyAnimator.PlayAnimation_Attack();

        if (enemy.PlayerTransform.TryGetComponent<IDamageable>(out var damageable))
        {
            damageable.TakeDamage(enemy.EnemyConfig.AttackDamage);
        }

        nextAttackTime = Time.time + enemy.EnemyConfig.AttackCooldown;
    }
} 