using UnityEngine;

[CreateAssetMenu(fileName = "EnemyConfig", menuName = "Enemy/EnemyConfig")]
public class EnemyConfig : ScriptableObject
{
    [Header("Movement Settings")]
    [SerializeField] private float moveSpeed = 1.0f;
    [SerializeField] private float turnSpeed = 10f;
    [SerializeField] private float acceleration = 8f;

    [Header("Combat Settings")]
    [SerializeField] private int maxHealth = 100;
    [SerializeField] private int attackDamage = 10;
    [SerializeField] private float detectionRange = 10f;
    [SerializeField] private float attackRange = 2f;
    [SerializeField] private float attackCooldown = 1.5f;
    [SerializeField] private float deathDelay = 2f;


    public int MaxHealth => maxHealth;
    public float MoveSpeed => moveSpeed;
    public float DetectionRange => detectionRange;
    public float AttackRange => attackRange;
    public float AttackCooldown => attackCooldown;
    public int AttackDamage => attackDamage;
    public float DeathDelay => deathDelay;
    public float TurnSpeed => turnSpeed;
    public float Acceleration => acceleration;
}
