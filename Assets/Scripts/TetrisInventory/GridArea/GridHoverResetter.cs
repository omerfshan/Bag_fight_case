public class GridHoverResetter
{
    public void ClearAllHover(InventoryGrid grid)
    {
        for (int y = 0; y < grid.gridHeight; y++)
        {
            for (int x = 0; x < grid.gridWidth; x++)
            {
                if (grid.cellUIs[x, y].is_filled)
                    grid.cellUIs[x, y].SetFilled();
                else
                    grid.cellUIs[x, y].SetEmpty();
            }
        }
    }
}
