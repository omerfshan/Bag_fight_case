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
    
    public float dragScale = 1.15f;
    public InventoryGrid grid;   
      public int width = 1;   
    public int height = 2;  
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rect.localScale;
    }

   public void OnBeginDrag(PointerEventData eventData)
{
    originalPos = rect.anchoredPosition;
    rect.localScale = originalScale * dragScale;

    // Eski yer varsa, item artık oradan kaldırılıyor → hücreleri BOŞALT
    if (lastGX != -1)
    {
        grid.ClearArea(lastGX, lastGY, width, height);
    }
}


    public void OnDrag(PointerEventData eventData)
    {
        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            canvas.transform as RectTransform,
            eventData.position,
            canvas.worldCamera,
            out Vector2 localPoint
        );

        rect.anchoredPosition = localPoint;
    }

   public void OnEndDrag(PointerEventData eventData)
{
    rect.localScale = originalScale;

    if (grid.ScreenToGrid(eventData.position, out int gx, out int gy))
    {
        if (grid.CanPlace(gx, gy, width, height))
        {
            // SNAP
            rect.anchoredPosition = grid.GridToPos(gx, gy);

            // Yeni alanı doldur
            grid.FillArea(gx, gy, width, height);

            // Item’in yeni pozisyonunu kaydet
            lastGX = gx;
            lastGY = gy;
        }
        else
        {
            // Yeni yere sığmadı → eski yerine dön
            rect.anchoredPosition = originalPos;

            // Eski yerini tekrar doldur
            if (lastGX != -1)
                grid.FillArea(lastGX, lastGY, width, height);
        }
    }
    else
    {
        // Grid dışı bırakıldı → eski yerine dön
        rect.anchoredPosition = originalPos;

        if (lastGX != -1)
            grid.FillArea(lastGX, lastGY, width, height);
    }
}


}