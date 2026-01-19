using UnityEngine;

public class ItemFactory
{
    private readonly InventoryItemDataListSO itemDatabase;
    private readonly InventoryGridItemController prefab;

    public ItemFactory(InventoryItemDataListSO db, InventoryGridItemController pf)
    {
        itemDatabase = db;
        prefab = pf;
    }

    public InventoryGridItemController CreateRandomItem()
    {
        InventoryItemSO randomSO =
            itemDatabase.inventoryItems[Random.Range(0, itemDatabase.inventoryItems.Count)];

        InventoryGridItemController newItem = Object.Instantiate(prefab);
        newItem.LoadData(randomSO);

        return newItem;
    }
}
