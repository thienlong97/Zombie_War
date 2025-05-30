using UnityEngine;
using System;

[CreateAssetMenu(fileName = "EnemyLevelConfig", menuName = "Game/Enemy/Level Config")]
public class EnemyLevelConfig : ScriptableObject
{
    [Serializable]
    public class EnemyWave
    {
        public GameObject enemyPrefab;
        public int count;
        public float spawnDelay = 1f;
        public bool isBoss;
    }

    [Header("Level Settings")]
    public int levelNumber = 1;
    public float timeBetweenWaves = 5f;
    public GameObject navMeshMapPrefab;
    public float minSpawnDistanceFromPlayer = 10f;
    public float maxSpawnDistanceFromPlayer = 20f;

    [Header("Enemy Waves")]
    public EnemyWave[] waves;

    [Header("Level Complete Conditions")]
    public bool requireBossDefeat = true;
    public int totalEnemiesRequired;
} 