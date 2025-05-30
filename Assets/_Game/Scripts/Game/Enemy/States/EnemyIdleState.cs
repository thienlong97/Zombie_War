public class EnemyIdleState : EnemyStateBase
{
    public EnemyIdleState(EnemyStateMachine stateMachine, EnemyBase enemy) : base(stateMachine, enemy) { }

    public override void Enter()
    {
        enemy.EnemyAnimator.PlayAnimation_Run(false);
    }

    public override void Update()
    {
        if (IsInDetectionRange())
        {
            stateMachine.ChangeState(EnemyStateType.Chase);
        }
    }
} 