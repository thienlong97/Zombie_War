using UnityEngine;
using UnityEngine.AI;
using System;

/// <summary>
/// Base class for all enemy entities in the game.
/// </summary>

[RequireComponent(typeof(NavMeshAgent))]
public abstract class EnemyBase : MonoBehaviour, IDamageable
{
    [Header("Components")]
    [SerializeField] protected EnemyConfig enemyConfig;
    [SerializeField] protected Animator animator;

    protected EnemyAnimator enemyAnimator;
    protected EnemyStateMachine stateMachine;
    protected NavMeshAgent agent;
    protected Transform playerTransform;

    // Properties for states to access
    public EnemyConfig EnemyConfig => enemyConfig;
    public EnemyAnimator EnemyAnimator => enemyAnimator;
    public NavMeshAgent Agent => agent;
    public Transform PlayerTransform => playerTransform;

    /// <summary>
    /// Current health of the enemy
    /// </summary>
    public int CurrentHealth { get; private set; }

    /// <summary>
    /// Event triggered when the enemy's health changes
    /// </summary>
    public event Action<int> OnHealthChanged;

    /// <summary>
    /// Event triggered when the enemy dies
    /// </summary>
    public event Action OnDeath;

    protected virtual void Awake()
    {
        // Get components
        if (animator == null)
        {
            Debug.LogError($"Animator is not assigned on {gameObject.name}");
            return;
        }

        // Validate configuration
        if (enemyConfig == null)
        {
            Debug.LogError($"EnemyConfig is not assigned on {gameObject.name}");
            enabled = false;
            return;
        }
        
        agent = GetComponent<NavMeshAgent>();
        enemyAnimator = new EnemyAnimator(animator);
        
        // Find player
        playerTransform = GameObject.FindGameObjectWithTag("Player")?.transform;
        

        // Setup state machine
        InitializeStateMachine();
    }

    protected virtual void OnEnable()
    {   
        InitializeConfig();
        Agent.enabled = true;
        stateMachine.Initialize(EnemyStateType.Idle);      
    }

    protected virtual void OnDisable()
    {
        Agent.enabled = false;
    }

    protected virtual void Start()
    {
       
    }

    protected virtual void Update()
    {
        stateMachine?.Update();
    }

    protected virtual void FixedUpdate()
    {
        stateMachine?.FixedUpdate();
    }

    /// <summary>
    /// Applies damage to the enemy and handles death if health reaches zero
    /// </summary>
    /// <param name="damage">Amount of damage to apply</param>
    public virtual void TakeDamage(int damage)
    {
        if(CurrentHealth <= 0) return;

        if (damage < 0)
        {
            Debug.LogWarning($"Attempted to apply negative damage to {gameObject.name}");
            return;
        }

        var enemyHitData = new EnemyHitData(damage, transform.position);
        EventBusManager.Instance.Publish(EventType.EnemyHit, enemyHitData);

        enemyAnimator.PlayAnimation_Hit();
        CurrentHealth = Mathf.Max(0, CurrentHealth - damage);
        OnHealthChanged?.Invoke(CurrentHealth);

        if (CurrentHealth <= 0)
        {
            Die();
        }
    }

    /// <summary>
    /// Handles the enemy's death
    /// </summary>
    protected virtual void Die()
    {      
        OnDeath?.Invoke();
        stateMachine.ChangeState(EnemyStateType.Die);
    }

    protected virtual void InitializeConfig()
    {
        agent.speed = enemyConfig.MoveSpeed;
        agent.angularSpeed = enemyConfig.TurnSpeed;
        agent.acceleration = enemyConfig.Acceleration;
        agent.stoppingDistance = enemyConfig.AttackRange * 0.9f;

        CurrentHealth = enemyConfig.MaxHealth;
    }

    protected virtual void InitializeStateMachine()
    {
        stateMachine = new EnemyStateMachine();
        
        // Add states
        stateMachine.AddState(EnemyStateType.Idle, new EnemyIdleState(stateMachine, this));
        stateMachine.AddState(EnemyStateType.Chase, new EnemyChaseState(stateMachine, this));
        stateMachine.AddState(EnemyStateType.Attack, new EnemyAttackState(stateMachine, this));
        stateMachine.AddState(EnemyStateType.Die, new EnemyDeathState(stateMachine, this));
    }
}
