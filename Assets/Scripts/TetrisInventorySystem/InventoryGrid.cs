using UnityEngine;

public class InventoryGrid : MonoBehaviour
{
    public int gridWidth = 8;
    public int gridHeight = 6;
    public float cellSize = 120f;

    private RectTransform rect;

    // --- EK KISIM ---
    public CellUI[,] cellUIs;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Start()
    {
        // Tüm CellUI'ları çekiyoruz
        CellUI[] all = GetComponentsInChildren<CellUI>();

        cellUIs = new CellUI[gridWidth, gridHeight];

        int index = 0;

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                CellUI c = all[index];
                c.SetEmpty();         // başlangıç → boş
                c.is_filled = false;  // doldurulmamış
                cellUIs[x, y] = c;
                index++;
            }
        }
    }
    // --- EK BİTİŞ ---

    // Hücreyi doldur
    public void FillCell(int x, int y)
    {
        cellUIs[x, y].SetFilled();
        cellUIs[x, y].is_filled = true;
    }

    // Hücreyi boşalt
    public void EmptyCell(int x, int y)
    {
        cellUIs[x, y].SetEmpty();
        cellUIs[x, y].is_filled = false;
    }

    // Hücre dolu mu?
    public bool IsCellFilled(int x, int y)
    {
        return cellUIs[x, y].is_filled;
    }

    // Ekran pozisyonunu grid hücresine çevir
    public bool ScreenToGrid(Vector2 screenPos, out int gx, out int gy)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            rect,
            screenPos,
            null,
            out var localPos
        );

        float startX = -(gridWidth * cellSize) / 2f;
        float startY =  (gridHeight * cellSize) / 2f;

        float px = localPos.x - startX;
        float py = startY - localPos.y;

        gx = Mathf.FloorToInt(px / cellSize);
        gy = Mathf.FloorToInt(py / cellSize);

        return gx >= 0 && gy >= 0 && gx < gridWidth && gy < gridHeight;
    }

    // Grid hücresini UI pozisyonuna çevir
    public Vector2 GridToPos(int gx, int gy)
    {
        float startX = -(gridWidth * cellSize) / 2f + cellSize / 2f;
        float startY =  (gridHeight * cellSize) / 2f - cellSize / 2f;

        float px = startX + gx * cellSize;
        float py = startY - gy * cellSize;

        return new Vector2(px, py);
    }
    public bool CanPlace(int gx, int gy, int w, int h)
{
    if (gx < 0 || gy < 0) return false;
    if (gx + w > gridWidth) return false;
    if (gy + h > gridHeight) return false;

    for (int x = 0; x < w; x++)
    {
        for (int y = 0; y < h; y++)
        {
            if (cellUIs[gx + x, gy + y].is_filled)
                return false;
        }
    }

    return true;
}
public void FillArea(int gx, int gy, int w, int h)
{
    for (int x = 0; x < w; x++)
    {
        for (int y = 0; y < h; y++)
        {
            cellUIs[gx + x, gy + y].SetFilled();
            cellUIs[gx + x, gy + y].is_filled = true;
        }
    }
}
public void ClearArea(int gx, int gy, int w, int h)
{
    for (int x = 0; x < w; x++)
    {
        for (int y = 0; y < h; y++)
        {
            cellUIs[gx + x, gy + y].SetEmpty();
            cellUIs[gx + x, gy + y].is_filled = false;
        }
    }
}


}
