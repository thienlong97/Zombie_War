using UnityEngine;

public class PlayerStateFactory
{
    private PlayerController _playerCtr;

    public PlayerStateFactory(PlayerController playerCtr)
    {
        _playerCtr = playerCtr;
    }

    public BaseState<PlayerController, PlayerStateFactory> IdleState => new IdleState(_playerCtr, this);
    public BaseState<PlayerController, PlayerStateFactory> MoveState => new MoveState(_playerCtr, this);
}
