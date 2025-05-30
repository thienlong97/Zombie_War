using UnityEngine;

public class PlayerWeaponStateFactory 
{
    private PlayerWeaponController _controller;

    public PlayerWeaponStateFactory(PlayerWeaponController controller)
    {
        _controller = controller;
    }

    public BaseState<PlayerWeaponController, PlayerWeaponStateFactory> IdleState => new PlayerWeaponIdleState(_controller, this);
    public BaseState<PlayerWeaponController, PlayerWeaponStateFactory> AttackState => new PlayerWeaponAttackState(_controller, this);
    public BaseState<PlayerWeaponController, PlayerWeaponStateFactory> EquipState => new PlayerWeaponEquipState(_controller, this);

}
