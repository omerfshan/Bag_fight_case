using UnityEngine;

public class ItemHighlightHandler
{
    private readonly InventoryGridItemController item;

    public ItemHighlightHandler(InventoryGridItemController controller)
    {
        item = controller;
    }

    // =====================================================
    // GRID ÜZERİNDE HOVER HIGHLIGHT
    // =====================================================
    public void HandleHighlight(Vector2 pointerPos)
    {
        RectTransform gridRect = item.grid.GetComponent<RectTransform>();

        bool overGrid = RectTransformUtility.RectangleContainsScreenPoint(
            gridRect,
            pointerPos,
            item.canvas.worldCamera
        );

        if (overGrid)
        {
            if (item.grid.ScreenToGrid(pointerPos, out int gx, out int gy))
            {
                int targetGX = gx - (item.width / 2);
                int targetGY = gy - (item.height / 2);

                item.grid.HighlightArea(targetGX, targetGY, item);
            }
        }
        else
        {
            item.grid.ClearAllHover();
        }
    }
}
