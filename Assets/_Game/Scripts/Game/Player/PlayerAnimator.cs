using UnityEngine;

public class PlayerAnimator
{
    private static readonly int VelocityXHash = Animator.StringToHash("VelocityX");
    private static readonly int VelocityZHash = Animator.StringToHash("VelocityZ");
    private static readonly int IsAimingHash  = Animator.StringToHash("IsAiming");
    private static readonly int IsMovingHash  = Animator.StringToHash("IsMoving");
    private static readonly int FireHash      = Animator.StringToHash("Fire");
    private static readonly int DeadHash      = Animator.StringToHash("Dead");
    private static readonly int RifleGrabHash = Animator.StringToHash("Grab");

    private readonly Animator _animator;
    public PlayerAnimator(Animator animator)
    {
        _animator = animator;
    }

    public void SetVelocity(Vector3 velocity)
    {
        _animator.SetFloat(VelocityXHash, velocity.x);
        _animator.SetFloat(VelocityZHash, velocity.z);
    }

    public void SetAiming(bool isAiming) => _animator.SetBool(IsAimingHash, isAiming);
    public void SetMoving(bool isMoving) => _animator.SetBool(IsMovingHash, isMoving);
    public void PlayAnimation_Fire() => _animator.SetTrigger(FireHash);
    public void PlayAnimation_Dead() => _animator.SetTrigger(DeadHash);
    public void PlayAnimation_RifleGrab() => _animator.SetTrigger(RifleGrabHash);
}
