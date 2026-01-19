public class GridPlacementValidator
{
    public bool CanPlace(InventoryGrid grid, int gx, int gy, InventoryGridItemController item)
    {
        if (gx < 0 || gy < 0) return false;
        if (gx + item.width > grid.gridWidth || gy + item.height > grid.gridHeight) return false;

        for (int x = 0; x < item.width; x++)
        {
            for (int y = 0; y < item.height; y++)
            {
                if (item.IsCellInShape(x, y))
                {
                    if (grid.cellUIs[gx + x, gy + y].is_filled)
                        return false;
                }
            }
        }
        return true;
    }
}
