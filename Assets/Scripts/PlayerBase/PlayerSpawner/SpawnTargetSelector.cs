public class SpawnTargetSelector
{
    private readonly SpawnEnemyQueue queue;

    public SpawnTargetSelector(SpawnEnemyQueue q)
    {
        queue = q;
    }

    public Enemy GetNextEnemy()
    {
        foreach (var e in queue.GetAll())
        {
            if (e != null && e.is_ready && !e.is_dead)
                return e;
        }

        return null; 
    }
}
