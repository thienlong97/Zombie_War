using System.IO.Pipes;
using UnityEngine;

public class MoveState : BaseState<PlayerController,PlayerStateFactory>
{
    private const float RotatePlayerWhenRunning = 20.0f;

    public MoveState(PlayerController playerCtr,PlayerStateFactory playerStateFactory) : base(playerCtr, playerStateFactory)
    {
        _controller = playerCtr;
        _factory = playerStateFactory;
    }

    public override void Enter()
    {
        Debug.Log($"{nameof(MoveState)} Enter");
        _controller._playerAnimator.SetMoving(true);
    }
    public override void Update()
    {
  
    }

    public override void FixedUpdate()
    {  
        UpdateMove();
        UpdateRotate();    
        CheckSwitchState(); 
    }

    public override void Exit()
    {
        _controller._playerAnimator.SetMoving(false);
    }

    public override void CheckSwitchState()
    {
        if (!_controller.PlayerInputHandler.IsDragging)
        {
            _controller.ChangeState(_factory.IdleState);
        }
    }

    private void UpdateMove()
    {
        float walkSpeed = GameConfig.Instance.PlayerConfig.WalkSpeed;
        float runSpeed = GameConfig.Instance.PlayerConfig.RunSpeed;
        float moveSpeed = _controller.IsHaveEnemyInRange ? walkSpeed : runSpeed;
        Vector2 joystickDirection = _controller.PlayerInputHandler.JoystickDirection;
        Vector3 moveDirection = new Vector3(joystickDirection.x, 0, joystickDirection.y);
        Vector3 velocityXZ = moveDirection.sqrMagnitude * moveDirection.normalized * moveSpeed;
        Vector3 velocity = velocityXZ;
        velocity.y = _controller.RigidBody.linearVelocity.y;
        _controller.RigidBody.linearVelocity = velocity;

        Vector3 InverseDirection = _controller.transform.InverseTransformDirection(moveDirection);
        _controller._playerAnimator.SetVelocity(InverseDirection);
    }

    private void UpdateRotate()
    {
        if (_controller.IsHaveEnemyInRange) return;

        Vector3 direction = new Vector3(_controller.RigidBody.linearVelocity.x, 0, _controller.RigidBody.linearVelocity.z);
        Quaternion curQua = _controller.transform.rotation;
        Quaternion tarQua = Quaternion.LookRotation(direction);
        _controller.transform.rotation = Quaternion.Lerp(curQua, tarQua,
            Time.deltaTime * RotatePlayerWhenRunning);
    }
}

