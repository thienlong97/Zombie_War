using UnityEngine;

[RequireComponent (typeof(PlayerWeaponController))]
public class PlayerController : MonoBehaviour
{
    public bool IsHaveEnemyInRange;
    public Transform enemyTarget;

    [Header("References")]
    [SerializeField] private Rigidbody _rigidbody;
    [SerializeField] private Animator _animator;
    [SerializeField] private PlayerInputHandler _playerInputHandler;

    public Rigidbody RigidBody => _rigidbody;
    public PlayerInputHandler PlayerInputHandler => _playerInputHandler;
    public PlayerAnimator _playerAnimator { get; private set; }
    private PlayerStateFactory _playerStateFactory;
    private BaseState<PlayerController,PlayerStateFactory> _currentState;

    private void Awake()
    {
        _playerStateFactory = new PlayerStateFactory(this);
        _playerAnimator = new PlayerAnimator(_animator);
    }

    private void Start()
    {
        ChangeState(_playerStateFactory.IdleState);
    }

    private void Update()
    {
        _currentState?.Update();

        //if (enemyTarget != null)
        //{
        //    Vector3 targetDir = enemyTarget.position - transform.position;
        //    transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(targetDir), Time.deltaTime * 20.0f);
        //}
    }

    private void FixedUpdate()
    {
        _currentState?.FixedUpdate();
    }

    public void ChangeState(BaseState<PlayerController,PlayerStateFactory> newState)
    {
        if (_currentState != null && _currentState.GetType() == newState.GetType()) return;

        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }
}
