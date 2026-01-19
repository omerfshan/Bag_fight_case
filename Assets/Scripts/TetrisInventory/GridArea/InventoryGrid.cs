using UnityEngine;

public class InventoryGrid : MonoBehaviour
{
    public int gridWidth = 8;
    public int gridHeight = 6;
    public float cellSize = 120f;
    public GameObject cell_Layout;
    private RectTransform rect;
    public CellUI[] cells;
    public CellUI[,] cellUIs;

   
    private GridInitializer initializer;
    private GridPositionCalculator posCalc;
    private GridOccupancy occupancy;
    private GridPlacementValidator validator;
    private GridAreaPainter painter;
    private GridHoverResetter hoverReset;

    void Awake()
    {
        rect = GetComponent<RectTransform>();

        
        initializer = new GridInitializer();
        posCalc = new GridPositionCalculator();
        occupancy = new GridOccupancy();
        validator = new GridPlacementValidator();
        painter = new GridAreaPainter();
        hoverReset = new GridHoverResetter();
    }

    void Start()
    {
       
        initializer.InitializeGrid(this);
    }

    public void FillCell(int x, int y)
    {
        occupancy.FillCell(this, x, y);
    }

    public void EmptyCell(int x, int y)
    {
        occupancy.EmptyCell(this, x, y);
    }

    public bool IsCellFilled(int x, int y)
    {
        return occupancy.IsCellFilled(this, x, y);
    }

    public bool ScreenToGrid(Vector2 screenPos, out int gx, out int gy)
    {
        return posCalc.ScreenToGrid(this, screenPos, out gx, out gy);
    }

    public Vector2 GridToPos(int gx, int gy, int w, int h)
    {
        return posCalc.GridToPos(this, gx, gy, w, h);
    }

    public bool CanPlace(int gx, int gy, InventoryGridItemController item)
    {
        return validator.CanPlace(this, gx, gy, item);
    }

    public void FillArea(int gx, int gy, InventoryGridItemController item)
    {
        painter.FillArea(this, gx, gy, item);
    }

    public void ClearArea(int gx, int gy, InventoryGridItemController item)
    {
        painter.ClearArea(this, gx, gy, item);
    }

    public void ClearAllHover()
    {
        hoverReset.ClearAllHover(this);
    }

    public void HighlightArea(int gx, int gy, InventoryGridItemController item)
    {
        painter.HighlightArea(this, gx, gy, item);
    }
}
