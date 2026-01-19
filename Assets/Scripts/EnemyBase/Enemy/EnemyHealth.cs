using System.Collections;
using UnityEngine;

public class EnemyHealth
{
    private readonly Enemy enemy;
    private readonly EnemyDeath deathHandler;

    public EnemyHealth(Enemy enemyRef, EnemyDeath deathRef)
    {
        enemy = enemyRef;
        deathHandler = deathRef;
    }

    public void Initialize()
    {
        enemy.currentHealth = enemy.maxHealth;

        if (enemy.damageText != null)
            enemy.damageText.gameObject.SetActive(false);

        if (enemy.barFill != null)
            enemy.barFill.fillAmount = 1f;
    }

    public void TakeDamage(float dmg)
    {
        if (enemy.is_dead) return;

        enemy.currentHealth -= dmg;
        if (enemy.currentHealth < 0) enemy.currentHealth = 0;

        UpdateHealthBar();
        PlayHitEffects();
        enemy.StartCoroutine(DamageRoutine(dmg));

        SoundManager.Instance.EnemyHurtSound();

        if (enemy.currentHealth <= 0)
            deathHandler.Die();
    }

    private void UpdateHealthBar()
    {
        if (enemy.barFill != null)
            enemy.barFill.fillAmount = enemy.currentHealth / enemy.maxHealth;
    }

    private IEnumerator DamageRoutine(float dmg)
    {
        enemy.anim.SetBool(enemy.HurtID, true);

        if (enemy.damageText != null)
        {
            enemy.damageText.gameObject.SetActive(true);
            enemy.damageText.text = "-" + dmg;
        }

        yield return new WaitForEndOfFrame();

        enemy.anim.SetBool(enemy.HurtID, false);
        yield return new WaitForSeconds(0.1f);

        if (enemy.damageText != null)
            enemy.damageText.gameObject.SetActive(false);
    }
    private void PlayHitEffects()
    {
        if (enemy.HitEffect == null) return;

        foreach (var fx in enemy.HitEffect)
        {
            if (fx != null)
                fx.Play();
        }
    }
}
