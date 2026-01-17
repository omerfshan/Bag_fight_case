using UnityEngine;
using System.Collections.Generic; // List için gerekli

public class InventorySystem : MonoBehaviour
{
    // Dinamik liste kullanımı snap/remove işlemleri için daha iyidir
    public List<SimpleDragItem> inventory_Items = new List<SimpleDragItem>();

    public void AddItem(SimpleDragItem item)
    {
        if (!inventory_Items.Contains(item))
        {
            inventory_Items.Add(item);
            Debug.Log(item.name + " envanter listesine eklendi.");
        }
    }

    public void RemoveItem(SimpleDragItem item)
    {
        if (inventory_Items.Contains(item))
        {
            inventory_Items.Remove(item);
            Debug.Log(item.name + " envanter listesinden çıkarıldı.");
        }
    }
}