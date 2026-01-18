using UnityEngine;

public class EnemySpawner : MonoBehaviour
{
    [Header("Enemy Settings")]
    [SerializeField] private Enemy enemyPrefab;

    [Header("Spawn Position (enemy burada doğacak)")]
    [SerializeField] private Transform spawnPoint;

    [Header("4 Adet Hedef Noktası")]
    [SerializeField] private Transform[] targetPoints;

    private Enemy[] activeEnemies; // 4 enemy tutacağız
    private int _respawnIndex;
    void Start()
    {
        activeEnemies = new Enemy[targetPoints.Length];

        // ilk spawn
        for (int i = 0; i < targetPoints.Length; i++)
            SpawnEnemy(i);
    }

   public void SpawnEnemy(int index)
{
    Enemy newEnemy = Instantiate(enemyPrefab, spawnPoint.position, Quaternion.identity);

    
    newEnemy.Init(this, index, targetPoints);

    activeEnemies[index] = newEnemy;
}

   public void OnEnemyDied(int index)
{
    Invoke(nameof(RespawnDelayed), 0.5f);
    _respawnIndex = index;  
}



private void RespawnDelayed()
{
    SpawnEnemy(_respawnIndex);
}

}
