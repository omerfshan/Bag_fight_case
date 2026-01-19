using System.Collections.Generic;
using UnityEngine;

public class SpawnEnemyQueue
{
    private readonly List<Enemy> enemyQueue = new List<Enemy>();

    public void Register(Enemy e)
    {
        if (e != null && !enemyQueue.Contains(e))
            enemyQueue.Add(e);
    }

    public void Unregister(Enemy e)
    {
        if (e != null)
            enemyQueue.Remove(e);
    }
    public List<Enemy> GetAll()
    {
        enemyQueue.RemoveAll(e => e == null || e.is_dead);
        return enemyQueue;
    }

    public Enemy GetFirst()
    {
        enemyQueue.RemoveAll(e => e == null || e.is_dead);

        if (enemyQueue.Count == 0)
            return null;

        return enemyQueue[0];
    }
}
