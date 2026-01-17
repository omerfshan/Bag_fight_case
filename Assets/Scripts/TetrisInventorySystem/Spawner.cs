using UnityEngine;

public class Spawner : MonoBehaviour
{
    [SerializeField] private InventorySystem invSystem;
    public Player_item prefab;

    void Start()
    {
        // 🔥 Envantere bir şey eklenir eklenmez otomatik spawn
        invSystem.OnItemAdded += HandleItemAdded;
    }

    private void HandleItemAdded(SimpleDragItem invItem)
    {
        ItemDataSO data = invItem.GetData();

        // Prefab oluştur
        Player_item playerItem = Instantiate(prefab, transform.position, Quaternion.identity);

        // Player silahına item ver
        playerItem.Load(data);

        Debug.Log("Spawn: " + data.name + " Damage: " + data.AttackDamage);

        // 🔥 ENVANTERDEN SİL
        invSystem.RemoveItem(invItem);
    }
}
