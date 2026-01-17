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
    
    if (isDragging && Input.GetKeyDown(KeyCode.R))
    {
        RotateItem();
    }
}
 public void OnBeginDrag(PointerEventData eventData)
{
    isDragging = true;
    originalPos = rect.anchoredPosition;
    rect.localScale = originalScale * dragScale;

    if (lastGX != -1)
    {
        grid.ClearArea(lastGX, lastGY, this);
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

  
    rect.Rotate(0, 0, -90);

    
    if (grid.ScreenToGrid(Input.mousePosition, out int gx, out int gy))
    {
        int targetGX = gx - (width / 2);
        int targetGY = gy - (height / 2);
        grid.HighlightArea(targetGX, targetGY, this);
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

   
    if (grid.ScreenToGrid(eventData.position, out int gx, out int gy))
    {
        int targetGX = gx - (width / 2);
        int targetGY = gy - (height / 2);
        grid.HighlightArea(targetGX, targetGY, this);
    }
    else
    {
        grid.ClearAllHover(); 
    }
}
public void OnEndDrag(PointerEventData eventData)
{
    isDragging = false; 
    grid.ClearAllHover();
    rect.localScale = originalScale;

    if (grid.ScreenToGrid(eventData.position, out int gx, out int gy))
    {
        int targetGX = gx - (width / 2);
        int targetGY = gy - (height / 2);

       
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


private void ReturnToOriginal()
{
    rect.anchoredPosition = originalPos;
    
    if (lastGX != -1) grid.FillArea(lastGX, lastGY, this);
}


}