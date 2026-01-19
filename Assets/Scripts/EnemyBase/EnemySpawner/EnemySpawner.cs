using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private Enemy enemyPrefab;

    [Header("Spawn Position (enemy burada doğacak)")]
    [SerializeField] private Transform spawnPoint;

    [Header("4 Adet Hedef Noktası")]
    [SerializeField] private Transform[] targetPoints;

    private Enemy[] activeEnemies;

    private EnemyFactory _factory;
    private EnemyRespawn _respawn;

    void Awake()
    {
        _factory = new EnemyFactory(enemyPrefab, spawnPoint, targetPoints, this);
        _respawn = new EnemyRespawn(this, _factory);
    }

    void Start()
    {
        activeEnemies = new Enemy[targetPoints.Length];

        for (int i = 0; i < targetPoints.Length; i++)
            activeEnemies[i] = _factory.SpawnEnemy(i);
    }

    public void OnEnemyDied(int index)
    {
        // Invoke burada olacak → ÇALIŞMASI GARANTİ
        Invoke(nameof(RespawnDelayed), 0.5f);
        _respawn.respawnIndex = index;
    }

    private void RespawnDelayed()
    {
        Enemy enemy = _factory.SpawnEnemy(_respawn.respawnIndex);
        activeEnemies[_respawn.respawnIndex] = enemy;
    }
}
