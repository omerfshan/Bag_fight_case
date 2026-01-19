using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private InventoryManager invSystem;
    [SerializeField] private Animator anim;
    [SerializeField] private string AttackID = "Attack";
    [SerializeField] private Player_item prefab;

    private bool isAttacking = false;

    // === SOLID Yardımcı sınıflar ===
    private SpawnEnemyQueue enemyQueueHandler;
    private SpawnTargetSelector targetSelector;
    private SpawnAttackExecutor attackExecutor;

    void Awake()
    {
        // SOLID sınıfları initialize ediyoruz
        enemyQueueHandler = new SpawnEnemyQueue();
        targetSelector = new SpawnTargetSelector(enemyQueueHandler);
        attackExecutor = new SpawnAttackExecutor(this, anim, AttackID, prefab);
    }

    // 🔥 Enemy doğunca
    public void RegisterEnemy(Enemy enemy)
    {
        enemyQueueHandler.Register(enemy);
    }

    // 🔥 Enemy ölünce
    public void UnregisterEnemy(Enemy enemy)
    {
        enemyQueueHandler.Unregister(enemy);
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

        // ⭐ sıradaki enemy (davranış aynı)
        Enemy target = targetSelector.GetNextEnemy();

        if (target != null)
        {
            yield return attackExecutor.FireItem(invItem, target.transform);

            // Item listeden kaldır
            invSystem.RemoveItem(invItem);

            // Cooldown tetikle
            invItem.OnFiredBySpawner();
        }

        yield return new WaitForSeconds(0.12f);
        isAttacking = false;
    }
}
