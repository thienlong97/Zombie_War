using UnityEngine;

public class PlayerWeaponIdleState : BaseState<PlayerWeaponController,PlayerWeaponStateFactory>
{
    public PlayerWeaponIdleState(PlayerWeaponController controller, PlayerWeaponStateFactory factory) : base(controller, factory)
    {
        _controller = controller;
        _factory = factory;
    }

    public override void Enter()
    {
        Debug.Log($"{nameof(PlayerWeaponIdleState)} Enter");
    }

    public override void Exit()
    {
       
    }

    public override void FixedUpdate()
    {
        
    }

    public override void Update()
    {
        CheckSwitchState();
    }
    
    public override void CheckSwitchState()
    {
        float weaponRange = GameConfig.Instance.PlayerConfig.WeaponRange;
        if (EnemyController.Instance.HasEnemyInRange(_controller.transform.position, weaponRange))
        {
            _controller.ChangeState(_factory.AttackState);
        }
    }
}
