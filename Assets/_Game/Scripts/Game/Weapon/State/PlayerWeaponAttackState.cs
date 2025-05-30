using UnityEngine;

public class PlayerWeaponAttackState : BaseState<PlayerWeaponController,PlayerWeaponStateFactory>
{
    private float _fireTimer;

    public PlayerWeaponAttackState(PlayerWeaponController controller, PlayerWeaponStateFactory factory) : base(controller, factory)
    {
        _controller = controller;
        _factory = factory;
    }

    public override void Enter()
    {
        Debug.Log($"{nameof(PlayerWeaponAttackState)} Enter");
        _fireTimer = _controller.WeaponStrategy.FireRate;
    }

    public override void Exit()
    {

    }

    public override void FixedUpdate()
    {

    }

    public override void Update()
    {
        UpdateFire();
    }

    public override void CheckSwitchState()
    {

    }

    private void UpdateFire()
    {
        _fireTimer += Time.deltaTime;
        if (_fireTimer >= _controller.WeaponStrategy.FireRate)
        {
            _controller.PlayerAnimator.PlayAnimation_Fire();
            _controller.WeaponStrategy.Fire(_controller.FirePoint);
            _fireTimer = 0;
        }
    }
}
