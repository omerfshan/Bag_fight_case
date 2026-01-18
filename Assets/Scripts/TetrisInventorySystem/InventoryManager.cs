using UnityEngine;
using System.Collections.Generic;
using System; // List için gerekli

public class InventoryManager : MonoBehaviour
{
    // Dinamik liste kullanımı snap/remove işlemleri için daha iyidir
    public List<InventoryGridItemController> inventory_Items = new List<InventoryGridItemController>();
     public Action<InventoryGridItemController> OnItemAdded;
public void AddItem(InventoryGridItemController item)
{
    inventory_Items.Add(item);
    Debug.Log(item.name + " envanter listesine eklendi.");

    // 🔥 Event tetikle → Spawner bunu yakalayacak
    OnItemAdded?.Invoke(item);
}


public void RemoveItem(InventoryGridItemController item)
    {
       
        
            inventory_Items.Remove(item);
            Debug.Log(item.name + " envanter listesinden çıkarıldı.");
        
    }

   
    
}