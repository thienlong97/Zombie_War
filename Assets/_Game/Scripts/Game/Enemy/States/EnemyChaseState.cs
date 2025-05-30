using UnityEngine;

public class EnemyChaseState : EnemyStateBase
{
    public EnemyChaseState(EnemyStateMachine stateMachine, EnemyBase enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        enemy.EnemyAnimator.PlayAnimation_Run(true);
    }

    public override void Update()
    {
        if (enemy.PlayerTransform == null)
        {
            stateMachine.ChangeState(EnemyStateType.Idle);
            return;
        }

        if (!IsInDetectionRange())
        {
            stateMachine.ChangeState(EnemyStateType.Idle);
            return;
        }

        if (IsInAttackRange())
        {
            stateMachine.ChangeState(EnemyStateType.Attack);
            return;
        }

        // Update chase position
        enemy.Agent.SetDestination(enemy.PlayerTransform.position);
    }

    public override void Exit()
    {
        enemy.Agent.ResetPath();
    }
} 