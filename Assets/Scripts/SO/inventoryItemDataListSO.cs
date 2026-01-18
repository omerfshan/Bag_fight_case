using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "InventoryItemDataList", menuName = "ScriptableObjects/InventoryItemDataList")]
public class InventoryItemDataListSO : ScriptableObject
{
    [Header("Oyun İçindeki Tüm Item Verileri")]
    public List<InventoryItemSO> inventoryItems = new List<InventoryItemSO>();
}
