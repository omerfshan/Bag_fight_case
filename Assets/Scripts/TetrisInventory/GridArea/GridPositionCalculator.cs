using UnityEngine;

public class GridPositionCalculator
{
    public bool ScreenToGrid(InventoryGrid grid, Vector2 screenPos, out int gx, out int gy)
    {
        Canvas parentCanvas = grid.GetComponentInParent<Canvas>();
        Camera cam = (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : parentCanvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            grid.GetComponent<RectTransform>(),
            screenPos,
            cam,
            out var localPos
        );

        float startX = -(grid.gridWidth * grid.cellSize) / 2f;
        float startY = (grid.gridHeight * grid.cellSize) / 2f;

        float px = localPos.x - startX;
        float py = startY - localPos.y;

        gx = Mathf.FloorToInt(px / grid.cellSize);
        gy = Mathf.FloorToInt(py / grid.cellSize);

        return gx >= 0 && gy >= 0 && gx < grid.gridWidth && gy < grid.gridHeight;
    }

    public Vector2 GridToPos(InventoryGrid grid, int gx, int gy, int w, int h)
    {
        float startX = -(grid.gridWidth * grid.cellSize) / 2f;
        float startY = (grid.gridHeight * grid.cellSize) / 2f;

        float px = startX + (gx * grid.cellSize) + (w * grid.cellSize * 0.5f);
        float py = startY - (gy * grid.cellSize) - (h * grid.cellSize * 0.5f);

        return new Vector2(px, py);
    }
}
