using System.Collections;
using UnityEngine;

public class EnemyDeath
{
    private readonly Enemy enemy;
    private bool hasDied;

    public EnemyDeath(Enemy enemyRef)
    {
        enemy = enemyRef;
    }

    public void Die()
    {
        if (enemy.is_dead || hasDied) return;

        enemy.is_dead = true;
        hasDied = true;

        SoundManager.Instance.EnemyDieSound();
        enemy.StartCoroutine(DeathSequence());
    }

    private IEnumerator DeathSequence()
    {
        if (enemy.HealthParent != null) enemy.HealthParent.SetActive(false);
        if (enemy.TextParent != null) enemy.TextParent.SetActive(false);

        if (enemy.rendererRef != null) enemy.rendererRef.enabled = false;

        foreach (var p in enemy.DiedEffect)
        {
            if (p != null)
                p.Play();
        }

        if (enemy.attackSpawner != null)
            enemy.attackSpawner.UnregisterEnemy(enemy);

        if (enemy.spawner != null)
            enemy.spawner.OnEnemyDied(enemy.spawnIndex);

        yield return new WaitForSeconds(1f);

        Object.Destroy(enemy.gameObject);
    }
    

}
