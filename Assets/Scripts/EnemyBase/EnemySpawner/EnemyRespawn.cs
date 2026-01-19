public class EnemyRespawn
{
    private readonly EnemySpawner _spawner;
    private readonly EnemyFactory _factory;
    public int respawnIndex;

    public EnemyRespawn(EnemySpawner spawner, EnemyFactory factory)
    {
        _spawner = spawner;
        _factory = factory;
    }
}
