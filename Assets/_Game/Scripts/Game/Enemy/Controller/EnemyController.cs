using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

public class EnemyController : GenericSingleton<EnemyController>
{
    private const int MAX_ACTIVE_ENEMIES = 60;

    [Header("Level Configuration")]
    [SerializeField] private EnemyLevelConfig[] levelConfigs;
    [SerializeField] private Transform playerTransform;

    private EnemyLevelConfig currentLevelConfig;
    private GameObject currentNavMeshMap;
    private int currentWaveIndex = 0;
    private int totalEnemiesSpawned = 0;
    private int totalEnemiesDefeated = 0;
    private int activeEnemiesCount = 0;
    private bool isBossDefeated = false;
    private bool isSpawning = false;
    private Coroutine spawnCoroutine;

    private void OnEnable()
    {
        EventBusManager.Instance.Subscribe(EventType.PlayerDied, OnPlayerDied);
        EventBusManager.Instance.Subscribe(EventType.EnemyDied, OnEnemyDefeated);
        EventBusManager.Instance.Subscribe(EventType.EnemyOutOfRange, OnEnemyOutOfRange);
        EventBusManager.Instance.Subscribe(EventType.EnemyDied, CheckLevelComplete);
    }

    private void OnDisable()
    {
        EventBusManager.Instance.Unsubscribe(EventType.PlayerDied, OnPlayerDied);
        EventBusManager.Instance.Unsubscribe(EventType.EnemyDied, OnEnemyDefeated);
        EventBusManager.Instance.Unsubscribe(EventType.EnemyOutOfRange, OnEnemyOutOfRange);
        EventBusManager.Instance.Unsubscribe(EventType.EnemyDied, CheckLevelComplete);
    }

    private void Start()
    {
        StartLevel(0);
    }

    public void StartLevel(int levelIndex)
    {
        if (levelIndex < 0 || levelIndex >= levelConfigs.Length)
        {
            Debug.LogError($"Invalid level index: {levelIndex}");
            return;
        }

        StopCurrentLevel();
        SetupNewLevel(levelIndex);
        StartSpawning();
    }

    private void SetupNewLevel(int levelIndex)
    {
        currentLevelConfig = levelConfigs[levelIndex];
        SpawnNavMeshMap();
        ResetLevelStats();
    }

    private void SpawnNavMeshMap()
    {
        if (currentNavMeshMap != null)
            Destroy(currentNavMeshMap);

        if (currentLevelConfig.navMeshMapPrefab != null)
        {
            currentNavMeshMap = Instantiate(currentLevelConfig.navMeshMapPrefab);
        }
    }

    private void ResetLevelStats()
    {
        currentWaveIndex = 0;
        totalEnemiesSpawned = 0;
        totalEnemiesDefeated = 0;
        activeEnemiesCount = 0;
        isBossDefeated = false;
        isSpawning = false;
    }

    private void StartSpawning()
    {
        if (isSpawning)
            return;

        isSpawning = true;
        spawnCoroutine = StartCoroutine(SpawnWaves());
    }

    private IEnumerator SpawnWaves()
    {
        while (currentWaveIndex < currentLevelConfig.waves.Length)
        {
            var wave = currentLevelConfig.waves[currentWaveIndex];
            totalEnemiesSpawned = totalEnemiesDefeated = 0;
            activeEnemiesCount = 0;
            // Keep spawning until we reach total wave count
            while (totalEnemiesDefeated < wave.count)
            {
                // Only check max active enemies before spawning
                if (activeEnemiesCount >= MAX_ACTIVE_ENEMIES)
                {
                    yield return new WaitForSeconds(1f); // Wait a bit before trying again
                    continue;
                }
              
                if(totalEnemiesSpawned < wave.count)
                {
                    SpawnEnemy(wave);
                    totalEnemiesSpawned++;
                    activeEnemiesCount++;
                    yield return new WaitForSeconds(wave.spawnDelay);
                }

                yield return null;
            }

            currentWaveIndex++;
            if (currentWaveIndex < currentLevelConfig.waves.Length)
                yield return new WaitForSeconds(currentLevelConfig.timeBetweenWaves);
        }
    }

    private void SpawnEnemy(EnemyLevelConfig.EnemyWave wave)
    {
        Vector3 spawnPosition = GetRandomSpawnPosition();
        if (spawnPosition != Vector3.zero)
        {
            var enemy = ObjectPool.Instance.SpawnFromPool(PoolType.Zombie, spawnPosition, Quaternion.identity);
            var enemyComponent = enemy.GetComponent<ZombieEnemy>();
            if (enemyComponent != null)
            {
               // enemyComponent.Initialize(wave.isBoss);
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (playerTransform == null) return Vector3.zero;

        for (int i = 0; i < 30; i++) // Try 30 times to find a valid position
        {
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(currentLevelConfig.minSpawnDistanceFromPlayer, 
                                             currentLevelConfig.maxSpawnDistanceFromPlayer);
            
            Vector3 offset = new Vector3(randomCircle.x, 0, randomCircle.y) * randomDistance;
            Vector3 targetPosition = playerTransform.position + offset;

            // Sample the NavMesh nearest position
            NavMeshHit hit;
            if (NavMesh.SamplePosition(targetPosition, out hit, 5f, NavMesh.AllAreas))
            {
                return hit.position;
            }
        }

        Debug.LogWarning("Could not find valid spawn position on NavMesh");
        return Vector3.zero;
    }

    private void OnPlayerDied(object data)
    {
        StopCurrentLevel();
    }

    private void OnEnemyDefeated(object data)
    {
        if (data is EnemyBase enemy)
        {         
            totalEnemiesDefeated++;
            activeEnemiesCount--;
            // Only count towards level progress if it's a ZombieEnemy and within range
            // The ZombieEnemy class will handle this check internally and only send the event if within range
            //if (enemy.IsBoss)
                //isBossDefeated = true;
        }
    }

    private void OnEnemyOutOfRange(object data)
    {
        if (data is EnemyBase enemy)
        {
            totalEnemiesSpawned--;
            activeEnemiesCount--;
        }
    }

    private void CheckLevelComplete(object data)
    {
        bool lastWave = currentWaveIndex >= currentLevelConfig.waves.Length - 1;
        bool enoughEnemiesDefeated = totalEnemiesDefeated >= totalEnemiesSpawned;
        if (lastWave && enoughEnemiesDefeated)
        {
            EventBusManager.Instance.Publish(EventType.LevelCompleted);
            StopCurrentLevel();
        }
    }

    private void StopCurrentLevel()
    {
        isSpawning = false;
        if (spawnCoroutine != null)
            StopCoroutine(spawnCoroutine);
    }

    public void SetPlayer(Transform player)
    {
        playerTransform = player;
    }
} 