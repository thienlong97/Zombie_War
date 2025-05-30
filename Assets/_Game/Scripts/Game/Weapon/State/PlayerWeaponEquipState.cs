using System.Threading.Tasks;
using UnityEngine;

public class PlayerWeaponEquipState : BaseState<PlayerWeaponController, PlayerWeaponStateFactory>
{
    private const int DelayTimeToEquip   = 250; // milliseconds
    private const int TimeToEndAnimation = 300; // milliseconds

    public PlayerWeaponEquipState(PlayerWeaponController controller, PlayerWeaponStateFactory factory) : base(controller, factory)
    {
        _controller = controller;
        _factory = factory;
    }

    public override void CheckSwitchState()
    {

    }

    public override void Enter()
    {
        Debug.Log($"{nameof(PlayerWeaponEquipState)} Enter");
        EquipAnimation();
    }

    public override void Exit()
    {

    }

    public override void FixedUpdate()
    {

    }

    public override void Update()
    {

    }

    private async void EquipAnimation()
    {
        EventBusManager.Instance.Publish(EventType.PlayerGrabWeapon, GameConfig.Instance.grabWeaponSfx);
        _controller.PlayerAnimator.PlayAnimation_RifleGrab();
        await Task.Delay(DelayTimeToEquip);
        _controller.EquipNextWeapon();
        await Task.Delay(TimeToEndAnimation);
        _controller.ChangeState(_factory.IdleState);
    }
}
