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

    void Awake()
    {
        rect = GetComponent<RectTransform>();
    }

    void Start()
    {
       
        

        cellUIs = new CellUI[gridWidth, gridHeight];

        int index = 0;

        for (int y = 0; y < gridHeight; y++)
        {
            for (int x = 0; x < gridWidth; x++)
            {
                CellUI c = cells[index];
                c.SetEmpty();        
                c.is_filled = false;  
                cellUIs[x, y] = c;
                index++;
            }
        }
    }
       public void FillCell(int x, int y)
    {
        cellUIs[x, y].SetFilled();
        cellUIs[x, y].is_filled = true;
    }

    
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
    // Grid'in bağlı olduğu canvası bul
    Canvas parentCanvas = GetComponentInParent<Canvas>();
    Camera cam = (parentCanvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : parentCanvas.worldCamera;

    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        rect,
        screenPos,
        cam, // Null yerine render moduna göre kamera gelmeli
        out var localPos
    );

    // Mevcut hesaplaman doğru, aynen devam:
    float startX = -(gridWidth * cellSize) / 2f;
    float startY =  (gridHeight * cellSize) / 2f;

    float px = localPos.x - startX;
    float py = startY - localPos.y;

    gx = Mathf.FloorToInt(px / cellSize);
    gy = Mathf.FloorToInt(py / cellSize);

    return gx >= 0 && gy >= 0 && gx < gridWidth && gy < gridHeight;
}

    // Grid hücresini UI pozisyonuna çevir
// gx, gy: grid koordinatı | w, h: eşyanın hücre cinsinden boyutu
 public Vector2 GridToPos(int gx, int gy, int w, int h)
{
    // Grid'in sol üstünden (0,0 hücresi) hesaplamaya başla
    float startX = -(gridWidth * cellSize) / 2f;
    float startY =  (gridHeight * cellSize) / 2f;

    // Hücrenin merkezine oturması için 0.5f ekliyoruz
    float px = startX + (gx * cellSize) + (w * cellSize * 0.5f);
    float py = startY - (gy * cellSize) - (h * cellSize * 0.5f);

    return new Vector2(px, py);
}
public bool CanPlace(int gx, int gy, SimpleDragItem item)
{
    if (gx < 0 || gy < 0) return false;
    if (gx + item.width > gridWidth || gy + item.height > gridHeight) return false;

    for (int x = 0; x < item.width; x++)
    {
        for (int y = 0; y < item.height; y++)
        {
            // EŞYANIN O HÜCRESİ DOLUYSA grid kontrolü yap
            if (item.IsCellInShape(x, y))
            {
                if (cellUIs[gx + x, gy + y].is_filled)
                    return false;
            }
        }
    }
    return true;
}

public void FillArea(int gx, int gy, SimpleDragItem item)
{
    for (int x = 0; x < item.width; x++)
    {
        for (int y = 0; y < item.height; y++)
        {
            if (item.IsCellInShape(x, y))
            {
                cellUIs[gx + x, gy + y].SetFilled();
                cellUIs[gx + x, gy + y].is_filled = true;
            }
        }
    }
}
public void ClearArea(int gx, int gy, SimpleDragItem item)
{
    for (int x = 0; x < item.width; x++)
    {
        for (int y = 0; y < item.height; y++)
        {
            // Sadece eşyanın şekline (shape) dahil olan hücreleri boşalt
            if (item.IsCellInShape(x, y))
            {
                // Koordinatların grid sınırları içinde olduğunu kontrol etmekte fayda var
                int targetX = gx + x;
                int targetY = gy + y;

                if (targetX >= 0 && targetX < gridWidth && targetY >= 0 && targetY < gridHeight)
                {
                    cellUIs[targetX, targetY].SetEmpty();
                    cellUIs[targetX, targetY].is_filled = false;
                }
            }
        }
    }
}

public void ClearAllHover()
{
    for (int y = 0; y < gridHeight; y++)
    {
        for (int x = 0; x < gridWidth; x++)
        {
    
            if (cellUIs[x, y].is_filled)
                cellUIs[x, y].SetFilled();
            else
                cellUIs[x, y].SetEmpty();
        }
    }
}


public void HighlightArea(int gx, int gy, SimpleDragItem item)
{
    ClearAllHover(); 
    bool canPlace = CanPlace(gx, gy, item);
    Color highlightColor = canPlace ? Color.green : Color.red;

    for (int x = 0; x < item.width; x++)
    {
        for (int y = 0; y < item.height; y++)
        {
            if (item.IsCellInShape(x, y))
            {
                int tx = gx + x;
                int ty = gy + y;

                if (tx >= 0 && tx < gridWidth && ty >= 0 && ty < gridHeight)
                {
                    cellUIs[tx, ty].img.color = highlightColor;
                }
            }
        }
    }
}


}
