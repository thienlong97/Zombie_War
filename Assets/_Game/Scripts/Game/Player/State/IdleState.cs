using UnityEngine;

public class IdleState : BaseState<PlayerController, PlayerStateFactory>
{
    public IdleState(PlayerController playerCtr, PlayerStateFactory playerStateFactory) : base(playerCtr, playerStateFactory)
    {
        _controller = playerCtr;
        _factory = playerStateFactory;
    }

    public override void Enter()
    {
        Debug.Log($"{nameof(IdleState)} Enter");
        _controller._playerAnimator.SetMoving(false);
    }

    public override void Update()
    {
        CheckSwitchState();
    }

    public override void FixedUpdate()
    {
       
    }

    public override void Exit()
    {
      
    }

    public override void CheckSwitchState()
    {
        if (_controller.PlayerInputHandler.IsDragging )
        {
            _controller.ChangeState(_factory.MoveState);
        }
    }


}

