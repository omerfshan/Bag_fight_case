using System.Collections;
using UnityEngine;

public class EnemyMovement
{
    private readonly Enemy enemy;
    private float _footStepTimer = 0f;

    public EnemyMovement(Enemy enemyRef)
    {
        enemy = enemyRef;
    }

    public IEnumerator MoveCoroutine()
    {
        Transform target = enemy.setPlace[enemy.spawnIndex];
        enemy.anim.SetBool(enemy.WalkID, true);

        while (Vector3.Distance(enemy.transform.position, target.position) > enemy.reachDistance)
        {
            _footStepTimer += Time.deltaTime;

            if (_footStepTimer >= enemy.footStepInterval)
            {
                _footStepTimer = 0f;
                SoundManager.Instance.EnemyFootSound();
            }

            enemy.transform.position = Vector3.MoveTowards(
                enemy.transform.position,
                target.position,
                enemy.speed * Time.deltaTime
            );

            yield return null;
        }

        enemy.anim.SetBool(enemy.WalkID, false);
        enemy.is_ready = true;
    }
}