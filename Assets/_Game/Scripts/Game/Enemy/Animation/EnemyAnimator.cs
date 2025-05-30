using UnityEngine;

public class EnemyAnimator
{ 
    private readonly Animator _animator;

    public EnemyAnimator(Animator animator)
    {
        _animator = animator;
    }

    private static readonly int HitHash = Animator.StringToHash("Hit");
    private static readonly int AttackHash = Animator.StringToHash("Attack");
    private static readonly int DieHash = Animator.StringToHash("Die");
    private static readonly int RunHash = Animator.StringToHash("Run");

    public void PlayAnimation_Run(bool isRun) => _animator.SetBool(RunHash, isRun);
    public void PlayAnimation_Hit() => _animator.SetTrigger(HitHash);   
    public void PlayAnimation_Attack() => _animator.SetTrigger(AttackHash);
    public void PlayAnimation_Die() => _animator.SetTrigger(DieHash);
}
