using UnityEngine;

#if UNITY_EDITOR
[RequireComponent(typeof(EnemyBase))]
public class EnemyRangeVisualizer : MonoBehaviour
{
    [Header("Visualization Settings")]
    [SerializeField] private Color detectionRangeColor = Color.yellow;
    [SerializeField] private Color attackRangeColor = Color.red;
    [SerializeField] private bool showDetectionRange = true;
    [SerializeField] private bool showAttackRange = true;

    private EnemyBase enemy;
    private EnemyConfig config;

    private void Awake()
    {
        enemy = GetComponent<EnemyBase>();
        config = enemy.EnemyConfig;
    }

    private void OnDrawGizmosSelected()
    {
        // If not initialized in play mode, try to get references
        if (enemy == null)
        {
            enemy = GetComponent<EnemyBase>();
            if (enemy == null) return;
            config = enemy.EnemyConfig;
            if (config == null) return;
        }

        if (showDetectionRange)
        {
            Gizmos.color = detectionRangeColor;
            Gizmos.DrawWireSphere(transform.position, config.DetectionRange);
        }

        if (showAttackRange)
        {
            Gizmos.color = attackRangeColor;
            Gizmos.DrawWireSphere(transform.position, config.AttackRange);
        }
    }
}
#endif 