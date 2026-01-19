using UnityEngine;
using System.Collections;

public class PlayerSpawner : MonoBehaviour
{
    [SerializeField] private InventoryManager invSystem;
    [SerializeField] private Animator anim;
    [SerializeField] private string AttackID = "Attack";
    [SerializeField] private Player_item prefab;
    private bool globalDelay = false;
   

    
    private SpawnEnemyQueue enemyQueueHandler;
    private SpawnTargetSelector targetSelector;
    private SpawnAttackExecutor attackExecutor;

    void Awake()
    {
      
        enemyQueueHandler = new SpawnEnemyQueue();
        targetSelector = new SpawnTargetSelector(enemyQueueHandler);
        attackExecutor = new SpawnAttackExecutor(this, anim, AttackID, prefab);
    }

  
    public void RegisterEnemy(Enemy enemy)
    {
        enemyQueueHandler.Register(enemy);
    }

   
    public void UnregisterEnemy(Enemy enemy)
    {
        enemyQueueHandler.Unregister(enemy);
    }

   private void Update()
   {
        if (globalDelay) return;  

        invSystem.inventory_Items.RemoveAll(x => x == null);

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
        globalDelay = true; 

        yield return new WaitForSeconds(0.12f);  

        invItem.isReadyToFire = false;
        invSystem.RemoveItem(invItem);
        invItem.OnFiredBySpawner();

        Enemy target = targetSelector.GetNextEnemy();

        if (target != null)
        {
            attackExecutor.SpawnBullet(invItem, target.transform);
            StartCoroutine(attackExecutor.PlayAttackAnimation());
        }

        yield return new WaitForSeconds(0.05f);

        globalDelay = false;    
    }


}
