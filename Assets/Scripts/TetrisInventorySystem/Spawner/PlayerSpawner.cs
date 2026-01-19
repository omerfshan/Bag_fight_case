using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class Spawner : MonoBehaviour
{
    [SerializeField] private InventoryManager invSystem;
    [SerializeField] private Animator anim;
    [SerializeField] private string AttackID = "Attack";
    [SerializeField] private Player_item prefab;

    private bool isAttacking = false;

    // 🔥 Enemy kuyruğu: yeni gelen hep SONUNA eklenir
    private readonly List<Enemy> enemyQueue = new List<Enemy>();

    // 🔥 Enemy doğunca çağrılacak
    public void RegisterEnemy(Enemy enemy)
    {
        if (enemy == null) return;
        if (!enemyQueue.Contains(enemy))
            enemyQueue.Add(enemy);   // HER ZAMAN EN SONA
    }

    // 🔥 Enemy ölünce çağrılacak
    public void UnregisterEnemy(Enemy enemy)
    {
        if (enemy == null) return;
        enemyQueue.Remove(enemy);
    }

    // 🔥 Sıradaki hedef: listenin başı
    private Enemy GetNextEnemy()
    {
        // null veya ölüleri temizle
        enemyQueue.RemoveAll(e => e == null || e.is_dead);

        if (enemyQueue.Count == 0)
            return null;

        return enemyQueue[0]; // HER ZAMAN BAŞTAKİ
    }

    void Update()
    {
        if (isAttacking) return;

        foreach (var item in invSystem.inventory_Items)
        {
            if (item.isReadyToFire)
            {
                StartCoroutine(FireItemCoroutine(item));
                break;
            }
        }
    }

    private IEnumerator FireItemCoroutine(InventoryGridItemController invItem)
    {
        isAttacking = true;

        ItemDataSO data = invItem.GetData();

        // ⭐ Artık en yakın arama YOK → sıradaki enemy’i al
        Enemy targetEnemy = GetNextEnemy();

        if (targetEnemy != null)
        {
            Player_item bullet = Instantiate(prefab, transform.position, Quaternion.identity);
            SoundManager.Instance.ThrowItemSound();
            bullet.Load(data);
            bullet.SetTarget(targetEnemy.transform); // Player_item Transform bekliyor
            StartCoroutine(PlayAttackAnimation());
        }

        // Item listeden kaldır
        invSystem.RemoveItem(invItem);

        // Cooldown tetikle
        invItem.OnFiredBySpawner();

        yield return new WaitForSeconds(0.12f);
        isAttacking = false;
    }

    private IEnumerator PlayAttackAnimation()
    {
        if (anim == null)
            yield break;

        anim.SetBool(AttackID, true);

        float clipLength = GetAnimationLength(anim, AttackID);
        if (clipLength <= 0f) clipLength = 0.3f;

        yield return new WaitForSeconds(clipLength);

        anim.SetBool(AttackID, false);
    }

    private float GetAnimationLength(Animator animator, string stateName)
    {
        if (animator == null || animator.runtimeAnimatorController == null)
            return -1f;

        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == stateName)
                return clip.length;
        }
        return -1f;
    }
}
