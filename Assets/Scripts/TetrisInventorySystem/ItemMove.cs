using UnityEngine;
using UnityEngine.EventSystems;

public class SimpleDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rect;
    private Canvas canvas;
    private Vector2 originalPos;
    private Vector3 originalScale;
    private int lastGX = -1;
    private int lastGY = -1;
    private bool isDragging = false;
    public float dragScale = 1.15f;
    public InventoryGrid grid;   
    [Header("Item Shape (1=Full, 0=Empty)")]
    // Örn: 2x3'lük bir balta için bu diziyi Inspector'dan dolduracağız
    public int[] shape; 
    public int width = 1;
    public int height = 1;
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rect.localScale;
    }
void Update()
{
    // Sadece sürüklerken ve R tuşuna basıldığında çalış
    if (isDragging && Input.GetKeyDown(KeyCode.R))
    {
        RotateItem();
    }
}
 public void OnBeginDrag(PointerEventData eventData)
{
    isDragging = true; // Sürükleme başladı
    originalPos = rect.anchoredPosition;
    rect.localScale = originalScale * dragScale;

    if (lastGX != -1)
    {
        grid.ClearArea(lastGX, lastGY, this);
    }
}
public bool IsCellInShape(int localX, int localY)
    {
        // Tek boyutlu diziyi 2D gibi okuyoruz
        int index = localY * width + localX;
        if (index >= 0 && index < shape.Length)
        {
            return shape[index] == 1;
        }
        return false;
    }
private void RotateItem()
{
    // 1. Şekil matrisini (shape) 90 derece döndür
    int[] newShape = new int[shape.Length];
    for (int y = 0; y < height; y++)
    {
        for (int x = 0; x < width; x++)
        {
            // 90 derece döndürme formülü: yeni_x = eski_y, yeni_y = (genişlik - 1) - eski_x
            int newX = (height - 1) - y;
            int newY = x;
            newShape[newY * height + newX] = shape[y * width + x];
        }
    }

    // 2. Değerleri güncelle
    shape = newShape;
    int temp = width;
    width = height;
    height = temp;

    // 3. Görseli (RectTransform) 90 derece döndür
    rect.Rotate(0, 0, -90);

    // 4. Hover efektini hemen güncelle ki oyuncu sığıp sığmadığını anında görsün
    if (grid.ScreenToGrid(Input.mousePosition, out int gx, out int gy))
    {
        int targetGX = gx - (width / 2);
        int targetGY = gy - (height / 2);
        grid.HighlightArea(targetGX, targetGY, this);
    }
}
   public void OnDrag(PointerEventData eventData)
{
    // Mevcut sürükleme kodu
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        canvas.transform as RectTransform,
        eventData.position,
        canvas.worldCamera,
        out Vector2 localPoint
    );
    rect.anchoredPosition = localPoint;

    // --- HOVER EFEKTİ KISMI ---
    if (grid.ScreenToGrid(eventData.position, out int gx, out int gy))
    {
        int targetGX = gx - (width / 2);
        int targetGY = gy - (height / 2);
        grid.HighlightArea(targetGX, targetGY, this);
    }
    else
    {
        grid.ClearAllHover(); // Grid dışındaysak efekt kalsın
    }
}
public void OnEndDrag(PointerEventData eventData)
{
    isDragging = false; // Sürükleme bitti
    grid.ClearAllHover();
    rect.localScale = originalScale;

    if (grid.ScreenToGrid(eventData.position, out int gx, out int gy))
    {
        int targetGX = gx - (width / 2);
        int targetGY = gy - (height / 2);

        // Grid'e "Bu eşyanın şekline bakarak kontrol et" diyoruz
        if (grid.CanPlace(targetGX, targetGY, this))
        {
            rect.anchoredPosition = grid.GridToPos(targetGX, targetGY, width, height);

            grid.FillArea(targetGX, targetGY, this);
            lastGX = targetGX;
            lastGY = targetGY;
        }
        else
        {
            ReturnToOriginal();
        }
    }
    else
    {
        ReturnToOriginal();
    }
}

// Kod tekrarını önlemek için küçük bir yardımcı metod
private void ReturnToOriginal()
{
    rect.anchoredPosition = originalPos;
    // Geri dönerken de şekil maskesini kullanarak doldur
    if (lastGX != -1) grid.FillArea(lastGX, lastGY, this);
}


}