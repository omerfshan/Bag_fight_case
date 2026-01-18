using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private InventoryManager invSystem;
    public Player_item prefab;

    void Start()
    {
        invSystem.OnItemAdded += HandleItemAdded;
    }

    private void HandleItemAdded(SimpleDragItem invItem)
    {
        ItemDataSO data = invItem.GetData();

        Enemy[] enemies = FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            if (enemy.is_ready)
            {
                // Spawn noktası → spawner’ın olduğu yer
                Player_item bullet = Instantiate(prefab, transform.position, Quaternion.identity);

                bullet.Load(data);              // item datası
                bullet.SetTarget(enemy.transform); // hedef düşman

                Debug.Log("Item mermi gibi fırlatıldı → " + enemy.name);
            }
        }

        invSystem.RemoveItem(invItem); // envanterden sil
    }
}
