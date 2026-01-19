using UnityEngine;

public class GridInitializer
{
    public void InitializeGrid(InventoryGrid grid)
    {
        grid.cellUIs = new CellUI[grid.gridWidth, grid.gridHeight];

        int index = 0;

        for (int y = 0; y < grid.gridHeight; y++)
        {
            for (int x = 0; x < grid.gridWidth; x++)
            {
                CellUI c = grid.cells[index];
                c.SetEmpty();
                c.is_filled = false;
                grid.cellUIs[x, y] = c;
                index++;
            }
        }
    }
}
