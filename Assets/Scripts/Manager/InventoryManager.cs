using UnityEngine;
using System.Collections.Generic;
using System; 

public class InventoryManager : MonoBehaviour
{  
    public List<InventoryGridItemController> inventory_Items = new List<InventoryGridItemController>();
    public Action<InventoryGridItemController> OnItemAdded;
    public void AddItem(InventoryGridItemController item)
    {
        inventory_Items.Add(item);
        Debug.Log(item.name + " envanter listesine eklendi.");
        OnItemAdded?.Invoke(item);
    }


    public void RemoveItem(InventoryGridItemController item)
        {
            inventory_Items.Remove(item);
            Debug.Log(item.name + " envanter listesinden çıkarıldı.");
        }

   
    
}