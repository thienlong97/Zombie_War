using UnityEngine;
using UnityEngine.AI;
using System.Collections;
using System.Threading.Tasks;
using System.Collections.Generic;

public class EnemyController : GenericSingleton<EnemyController>
{
    private const int MAX_ACTIVE_ENEMIES = 60;
    private const float WEAPON_RANGE = 10f; // Default weapon range
    private const float HEIGHT_THRESHOLD = 0.2f; // Maximum height difference allowed

    [Header("Level Configuration")]
    [SerializeField] private EnemyLevelConfig[] levelConfigs;
    [SerializeField] private Transform playerTransform;

    private List<EnemyBase> activeEnemies = new List<EnemyBase>();
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
                activeEnemies.Add(enemyComponent);
            }
        }
    }

    private Vector3 GetRandomSpawnPosition()
    {
        if (playerTransform == null) return Vector3.zero;

        const float MAX_SLOPE_ANGLE = 45f; // Maximum slope angle in degrees
        const float MIN_CLEARANCE_HEIGHT = 2f; // Minimum space needed above spawn point
        const float RAYCAST_HEIGHT = 5f; // Height to start the clearance check from

        for (int i = 0; i < 30; i++) // Try 30 times to find a valid position
        {
            // Get random direction in circle around player
            Vector2 randomCircle = Random.insideUnitCircle.normalized;
            float randomDistance = Random.Range(currentLevelConfig.minSpawnDistanceFromPlayer, 
                                             currentLevelConfig.maxSpawnDistanceFromPlayer);
            
            // Calculate target position in XZ plane
            Vector3 offset = new Vector3(randomCircle.x, 0, randomCircle.y) * randomDistance;
            Vector3 targetPosition = playerTransform.position + offset;

            // Sample the NavMesh nearest position with larger search area
            NavMeshHit hit;
            float searchRadius = 10f;
            if (NavMesh.SamplePosition(targetPosition, out hit, searchRadius, NavMesh.AllAreas))
            {
                // Check slope angle
                if (Vector3.Angle(hit.normal, Vector3.up) > MAX_SLOPE_ANGLE)
                {
                    continue; // Skip if slope is too steep
                }

                // Check for clearance above the point
                Vector3 checkStart = hit.position + Vector3.up * RAYCAST_HEIGHT;
                RaycastHit clearanceHit;
                if (Physics.Raycast(checkStart, Vector3.down, out clearanceHit, RAYCAST_HEIGHT))
                {
                    float clearance = RAYCAST_HEIGHT - clearanceHit.distance;
                    if (clearance < MIN_CLEARANCE_HEIGHT)
                    {
                        continue; // Skip if not enough clearance
                    }
                }

                // Check if the found position is within our desired spawn range
                float distanceToPlayer = Vector3.Distance(new Vector3(hit.position.x, 0, hit.position.z), 
                                                        new Vector3(playerTransform.position.x, 0, playerTransform.position.z));
                
                if (distanceToPlayer >= currentLevelConfig.minSpawnDistanceFromPlayer && 
                    distanceToPlayer <= currentLevelConfig.maxSpawnDistanceFromPlayer)
                {
                    // Additional ground validation
                    RaycastHit groundHit;
                    Vector3 spawnPoint = hit.position + Vector3.up * 0.1f; // Slight offset to ensure ray hits ground
                    if (Physics.Raycast(spawnPoint, Vector3.down, out groundHit, 0.3f))
                    {
                        // Use the ground hit point with a small offset
                        return groundHit.point + Vector3.up * 0.1f;
                    }
                    
                    return hit.position;
                }
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
            activeEnemies.Remove(enemy);
        }
    }

    private void OnEnemyOutOfRange(object data)
    {
        if (data is EnemyBase enemy)
        {
            totalEnemiesSpawned--;
            activeEnemiesCount--;
            activeEnemies.Remove(enemy);
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

    public bool HasEnemyInRange(Vector3 position, float range = WEAPON_RANGE)
    {
        foreach (var enemy in activeEnemies)
        {
            if (enemy == null) continue;

            Vector3 enemyPos = enemy.transform.position;
            float heightDiff = Mathf.Abs(enemyPos.y - position.y);
            
            if (heightDiff <= HEIGHT_THRESHOLD)
            {
                // Project positions to XZ plane for horizontal distance check
                Vector3 positionXZ = new Vector3(position.x, 0, position.z);
                Vector3 enemyXZ = new Vector3(enemyPos.x, 0, enemyPos.z);
                
                if (Vector3.Distance(positionXZ, enemyXZ) <= range)
                {
                    return true;
                }
            }
        }
        return false;
    }

    public Transform GetNearestEnemyInRange(Vector3 position, float range = WEAPON_RANGE)
    {
        Transform nearestEnemy = null;
        float nearestDistance = float.MaxValue;

        foreach (var enemy in activeEnemies)
        {
            if (enemy == null) continue;

            Vector3 enemyPos = enemy.transform.position;
            float heightDiff = Mathf.Abs(enemyPos.y - position.y);
            
            if (heightDiff <= HEIGHT_THRESHOLD)
            {
                // Project positions to XZ plane for horizontal distance check
                Vector3 positionXZ = new Vector3(position.x, 0, position.z);
                Vector3 enemyXZ = new Vector3(enemyPos.x, 0, enemyPos.z);
                
                float distance = Vector3.Distance(positionXZ, enemyXZ);
                if (distance <= range && distance < nearestDistance)
                {
                    nearestDistance = distance;
                    nearestEnemy = enemy.transform;
                }
            }
        }

        return nearestEnemy;
    }
} 