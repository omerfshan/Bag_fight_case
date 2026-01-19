using UnityEngine;

public class SlotController
{
    private readonly InventoryGridItemController[] slotItems;

    public SlotController(int slotCount)
    {
        slotItems = new InventoryGridItemController[slotCount];
    }

    public bool IsSlotEmpty(int index) => slotItems[index] == null;

    public void SetSlot(int index, InventoryGridItemController item)
    {
        slotItems[index] = item;
    }

    public void ClearSlot(int index)
    {
        slotItems[index] = null;
    }

    public bool AllEmpty()
    {
        foreach (var item in slotItems)
            if (item != null)
                return false;

        return true;
    }
}
