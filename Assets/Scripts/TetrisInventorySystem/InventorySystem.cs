using UnityEngine;
using System.Collections.Generic;
using System; // List için gerekli

public class InventorySystem : MonoBehaviour
{
    // Dinamik liste kullanımı snap/remove işlemleri için daha iyidir
    public List<SimpleDragItem> inventory_Items = new List<SimpleDragItem>();
     public Action<SimpleDragItem> OnItemAdded;
public void AddItem(SimpleDragItem item)
{
    inventory_Items.Add(item);
    Debug.Log(item.name + " envanter listesine eklendi.");

    // 🔥 Event tetikle → Spawner bunu yakalayacak
    OnItemAdded?.Invoke(item);
}


public void RemoveItem(SimpleDragItem item)
    {
       
        
            inventory_Items.Remove(item);
            Debug.Log(item.name + " envanter listesinden çıkarıldı.");
        
    }

   
    
}