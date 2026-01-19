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
     
        OnItemAdded?.Invoke(item);
    }


    public void RemoveItem(InventoryGridItemController item)
        {
            inventory_Items.Remove(item);
         
        }

   
    
}