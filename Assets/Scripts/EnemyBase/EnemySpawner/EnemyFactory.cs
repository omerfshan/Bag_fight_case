using UnityEngine;

public class EnemyFactory
{
    private readonly Enemy _prefab;
    private readonly Transform _spawnPoint;
    private readonly Transform[] _targetPoints;
    private readonly EnemySpawner _spawner;

    public EnemyFactory(Enemy prefab, Transform spawnPoint, Transform[] targetPoints, EnemySpawner spawner)
    {
        _prefab = prefab;
        _spawnPoint = spawnPoint;
        _targetPoints = targetPoints;
        _spawner = spawner;
    }

    public Enemy SpawnEnemy(int index)
    {
        Enemy newEnemy = Object.Instantiate(_prefab, _spawnPoint.position, Quaternion.identity);

        // Orijinal koddaki Init → değişmedi
        newEnemy.Init(_spawner, index, _targetPoints);

        return newEnemy;
    }
}
