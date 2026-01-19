public class GridOccupancy
{
    public void FillCell(InventoryGrid grid, int x, int y)
    {
        grid.cellUIs[x, y].SetFilled();
        grid.cellUIs[x, y].is_filled = true;
    }

    public void EmptyCell(InventoryGrid grid, int x, int y)
    {
        grid.cellUIs[x, y].SetEmpty();
        grid.cellUIs[x, y].is_filled = false;
    }

    public bool IsCellFilled(InventoryGrid grid, int x, int y)
    {
        return grid.cellUIs[x, y].is_filled;
    }
}
