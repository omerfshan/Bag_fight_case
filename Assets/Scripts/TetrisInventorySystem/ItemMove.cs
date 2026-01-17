using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening; // DOTween kütüphanesini eklemeyi unutma!

public class SimpleDragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    private RectTransform rect;
    private Canvas canvas;
    private Vector2 originalPos;
    private Vector3 originalScale;
    private int lastGX = -1;
    private int lastGY = -1;
    private bool isDragging = false;
    private Tween scaleTween;

    public float dragScale = 1.15f;
    public InventoryGrid grid;

    [Header("Item Shape (1=Full, 0=Empty)")]
    public int[] shape;
    public int width = 1;
    public int height = 1;

    // Detay 3: Eşya istatistikleri
    [Header("Item Stats")]
    public float damage = 10f;
    public float cooldownDuration = 2f;
    private InventorySystem invSystem;
    [SerializeField] private ItemDataSO itemProperty;

    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rect.localScale;
         invSystem=FindAnyObjectByType<InventorySystem>();
    }

    void Update()
    {
        if (isDragging && Input.GetKeyDown(KeyCode.R))
        {
            RotateItem();
        }
    }

   public void OnBeginDrag(PointerEventData eventData)
{
    isDragging = true;
      if (invSystem != null) invSystem.RemoveItem(this);
 
    transform.SetParent(canvas.transform); 

    originalPos = rect.anchoredPosition;

    scaleTween?.Kill();
    scaleTween = rect.DOScale(originalScale * dragScale, 0.2f).SetEase(Ease.OutBack);

    if (lastGX != -1)
    {
        grid.ClearArea(lastGX, lastGY, this);
    }

    rect.SetAsLastSibling();
}

  public void OnDrag(PointerEventData eventData)
{
    Camera cam = (canvas.renderMode == RenderMode.ScreenSpaceOverlay) ? null : canvas.worldCamera;

   
    RectTransformUtility.ScreenPointToLocalPointInRectangle(
        transform.parent as RectTransform, 
        eventData.position,
        cam,
        out Vector2 localPoint
    );
    rect.anchoredPosition = localPoint;
  if (invSystem != null) invSystem.RemoveItem(this);
 
    if (grid.ScreenToGrid(eventData.position, out int gx, out int gy))
    {
        int targetGX = gx - (width / 2);
        int targetGY = gy - (height / 2);
        grid.HighlightArea(targetGX, targetGY, this);
    }
}

public void OnEndDrag(PointerEventData eventData)
{
    isDragging = false;
    grid.ClearAllHover();
    rect.DOKill();

    if (grid.ScreenToGrid(eventData.position, out int gx, out int gy))
    {
      
        int targetGX = gx - Mathf.FloorToInt(width / 2f);
        int targetGY = gy - Mathf.FloorToInt(height / 2f);

        if (grid.CanPlace(targetGX, targetGY, this))
        {
         
            transform.SetParent(grid.transform, true); 
            
            Vector2 targetPos = grid.GridToPos(targetGX, targetGY, width, height);
            if (invSystem != null) invSystem.AddItem(this);
         
            rect.DOAnchorPos(targetPos, 0.15f).SetEase(Ease.OutQuint);

            grid.FillArea(targetGX, targetGY, this);
            lastGX = targetGX;
            lastGY = targetGY;
            
            rect.DOScale(originalScale, 0.15f);
        }
        else { ReturnToOriginal(); }
    }
    else { ReturnToOriginal(); }
}

    private void ReturnToOriginal()
    {
      
        rect.DOAnchorPos(originalPos, 0.3f).SetEase(Ease.OutBounce);

        if (lastGX != -1)
        {
            grid.FillArea(lastGX, lastGY, this);
        }
    }

    public bool IsCellInShape(int localX, int localY)
    {
        int index = localY * width + localX;
        if (index >= 0 && index < shape.Length)
        {
            return shape[index] == 1;
        }
        return false;
    }

    private void RotateItem()
    {
        int[] newShape = new int[shape.Length];
        for (int y = 0; y < height; y++)
        {
            for (int x = 0; x < width; x++)
            {
                int newX = (height - 1) - y;
                int newY = x;
                newShape[newY * height + newX] = shape[y * width + x];
            }
        }

        shape = newShape;
        int temp = width;
        width = height;
        height = temp;

      
        rect.DORotate(rect.eulerAngles + new Vector3(0, 0, -90), 0.2f);

        if (grid.ScreenToGrid(Input.mousePosition, out int gx, out int gy))
        {
            int targetGX = gx - (width / 2);
            int targetGY = gy - (height / 2);
            grid.HighlightArea(targetGX, targetGY, this);
        }
    }
}