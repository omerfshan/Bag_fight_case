using UnityEngine;

public class GridAreaPainter
{
    public void FillArea(InventoryGrid grid, int gx, int gy, InventoryGridItemController item)
    {
        for (int x = 0; x < item.width; x++)
        {
            for (int y = 0; y < item.height; y++)
            {
                if (item.IsCellInShape(x, y))
                {
                    grid.cellUIs[gx + x, gy + y].SetFilled();
                    grid.cellUIs[gx + x, gy + y].is_filled = true;
                }
            }
        }
    }

    public void ClearArea(InventoryGrid grid, int gx, int gy, InventoryGridItemController item)
    {
        for (int x = 0; x < item.width; x++)
        {
            for (int y = 0; y < item.height; y++)
            {
                if (item.IsCellInShape(x, y))
                {
                    int tx = gx + x;
                    int ty = gy + y;

                    if (tx >= 0 && tx < grid.gridWidth && ty >= 0 && ty < grid.gridHeight)
                    {
                        grid.cellUIs[tx, ty].SetEmpty();
                        grid.cellUIs[tx, ty].is_filled = false;
                    }
                }
            }
        }
    }

    public void HighlightArea(InventoryGrid grid, int gx, int gy, InventoryGridItemController item)
    {
        grid.ClearAllHover();
        bool canPlace = grid.CanPlace(gx, gy, item);

        for (int x = 0; x < item.width; x++)
        {
            for (int y = 0; y < item.height; y++)
            {
                if (!item.IsCellInShape(x, y))
                    continue;

                int tx = gx + x;
                int ty = gy + y;

                if (tx < 0 || tx >= grid.gridWidth || ty < 0 || ty >= grid.gridHeight)
                    continue;

                if (canPlace)
                    grid.cellUIs[tx, ty].SetValid();
                else
                    grid.cellUIs[tx, ty].SetInvalid();
            }
        }
    }
}
