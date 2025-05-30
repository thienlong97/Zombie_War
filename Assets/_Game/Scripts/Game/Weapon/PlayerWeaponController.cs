using UnityEngine;
using System.Threading.Tasks;

public class PlayerWeaponController : WeaponBase
{
    [SerializeField] private PlayerController _playerCtr;
    [SerializeField] private Transform _firePoint;
    [SerializeField] private GameObject[] weaponsHolder;

    private WeaponStrategy _weaponStrategy;
    public WeaponStrategy WeaponStrategy => _weaponStrategy;
    public PlayerAnimator PlayerAnimator => _playerCtr._playerAnimator;
    public Transform FirePoint => _firePoint;

    private WeaponStrategy[] weaponStrategyConfig;
    private PlayerWeaponStateFactory _stateFactory;
    private BaseState<PlayerWeaponController, PlayerWeaponStateFactory> _currentState;

    private int _currentWeaponIndex;

    public void Start()
    {
        Initialize();
    }

    public void Update()
    {
        _currentState?.Update();
    }

    private void FixedUpdate()
    {
        _currentState?.FixedUpdate();
    }

    public void Initialize()
    {
        weaponStrategyConfig = GameConfig.Instance.WeaponStrategy;
        EquipWeapon(0);

        _stateFactory = new PlayerWeaponStateFactory(this);
        ChangeState(_stateFactory.IdleState);
    }

    public void ChangeState(BaseState<PlayerWeaponController, PlayerWeaponStateFactory> newState)
    {
        if (_currentState != null && _currentState.GetType() == newState.GetType()) return;
        _currentState?.Exit();
        _currentState = newState;
        _currentState.Enter();
    }

    public void OnClick_ChangeWeapon()
    {
        ChangeState(_stateFactory.EquipState);
    }

    public void EquipNextWeapon()
    {
        _currentWeaponIndex++;
        if(_currentWeaponIndex >= weaponStrategyConfig.Length) _currentWeaponIndex = 0;
        EquipWeapon(_currentWeaponIndex);
    }

    public void EquipWeapon(int index)
    {
        _currentWeaponIndex = index;
        ActiveWeaponHolder(index);
        SetWeaponStrategy(weaponStrategyConfig[index]);
    }

    private void ActiveWeaponHolder(int index)
    {
        for(int i = 0; i < weaponsHolder.Length; i++) weaponsHolder[i].SetActive(false);
        weaponsHolder[index].SetActive(true);
    }

    public void SetWeaponStrategy(WeaponStrategy weaponStrategy)
    {
        _weaponStrategy = weaponStrategy;
        _weaponStrategy.Initialize();
    }
}
