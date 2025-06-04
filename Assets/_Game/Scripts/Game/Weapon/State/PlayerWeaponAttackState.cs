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
        UpdateRotation();
    }

    public override void Update()
    {
        UpdateFire();
        CheckSwitchState();
    }

    public override void CheckSwitchState()
    {
        float weaponRange = GameConfig.Instance.PlayerConfig.WeaponRange;
        if (!EnemyController.Instance.HasEnemyInRange(_controller.transform.position, weaponRange))
        {
            _controller.ChangeState(_factory.IdleState);
        }
    }

    private void UpdateFire()
    {
        _fireTimer += Time.deltaTime;
        if (_fireTimer >= _controller.WeaponStrategy.FireRate)
        {
            _controller.PlayerAnimator.PlayAnimation_Fire();
            _controller.WeaponStrategy.Fire(_controller.FirePoint);
            
            // Add camera shake
            CinemachineShake.Instance.ShakeCameraLight();
            
            _fireTimer = 0;
        }
    }

    private void UpdateRotation()
    {
        float weaponRange = GameConfig.Instance.PlayerConfig.WeaponRange;
        float rotationSpeed = GameConfig.Instance.PlayerConfig.WeaponRotationSpeed;
        Transform nearestEnemy = EnemyController.Instance.GetNearestEnemyInRange(_controller.transform.position, weaponRange);
        if (nearestEnemy != null)
        {
            Vector3 targetDir = nearestEnemy.position - _controller.transform.position;
            targetDir.y = 0; // Keep rotation only in horizontal plane
            Quaternion targetRotation = Quaternion.LookRotation(targetDir);
            _controller.transform.rotation = Quaternion.Lerp(_controller.transform.rotation, targetRotation, Time.deltaTime * rotationSpeed);
        }
    }
}
