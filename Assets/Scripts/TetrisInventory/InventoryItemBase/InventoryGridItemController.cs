using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

public class InventoryGridItemController :
    MonoBehaviour,
    IBeginDragHandler,
    IDragHandler,
    IEndDragHandler
{
    
    public RectTransform rect;
    public Canvas canvas;
    public InventoryGrid grid;
    public InventoryManager inv;
    public TrashArea trashArea;
    public Image cooldownFill;
    public Image itemImage;
    public int[] shape;
    public int width = 1;
    public int height = 1;
    public float dragScale = 1.15f;
    public bool isReadyToFire = false;
    public ItemDataSO itemProperty;
    public Vector3 lastWorldPos;
    public Vector3 originalScale;
    public Vector2 originalAnchoredPos;
    public Transform originalParent;
    public Vector2 originalPos;
    public bool isDragging = false;
    public bool isOnCooldown = false;
    public bool isOnTrash = false;
    public int lastGX = -1;
    public int lastGY = -1;
    public float currentCooldown = 0;
    public Coroutine cooldownRoutine;
    public UIitemSpawner spawnerRef;
    public Color originalItemColor;
    public Color originalFillColor;
    private ItemDragHandler dragHandler;
    private ItemCooldownHandler cooldownHandler;
    private ItemPlacementHandler placementHandler;
    private ItemTrashHandler trashHandler;
    private ItemHighlightHandler highlightHandler;
    private ItemVisualHandler visualHandler;
    private ItemDataLoader dataLoader;

    void Awake()
    {
        
        rect = GetComponent<RectTransform>();
        canvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
        originalScale = rect.localScale;
        inv = FindAnyObjectByType<InventoryManager>();
        grid = FindAnyObjectByType<InventoryGrid>();
        trashArea = FindAnyObjectByType<TrashArea>();
        itemImage = GetComponent<Image>();
        spawnerRef = FindAnyObjectByType<UIitemSpawner>();
        if (itemImage != null)
            originalItemColor = itemImage.color;

        if (cooldownFill != null)
            originalFillColor = cooldownFill.color;

        if (cooldownFill != null)
            cooldownFill.fillAmount = 1f;

        dragHandler = new ItemDragHandler(this);
        cooldownHandler = new ItemCooldownHandler(this);
        placementHandler = new ItemPlacementHandler(this);
        trashHandler = new ItemTrashHandler(this);
        highlightHandler = new ItemHighlightHandler(this);
        visualHandler = new ItemVisualHandler(this);
        dataLoader = new ItemDataLoader(this);
    }

   

    public void StartCooldown() => cooldownHandler.StartCooldown();
    public void OnFiredBySpawner() => cooldownHandler.OnFiredBySpawner();
    public void PauseCooldown() => cooldownHandler.PauseCooldown();
    public void ResumeCooldown() => cooldownHandler.ResumeCooldown();
    public ItemDataSO GetData() => itemProperty;
    public void LoadData(InventoryItemSO so) => dataLoader.LoadData(so);

  

    public void OnBeginDrag(PointerEventData eventData)
    {
        dragHandler.OnBeginDrag(eventData);
    }

    public void OnDrag(PointerEventData eventData)
    {
        dragHandler.OnDrag(eventData);
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        dragHandler.OnEndDrag(eventData);
    }

   
    public bool IsCellInShape(int x, int y)
    {
        int index = y * width + x;
        if (index >= 0 && index < shape.Length)
            return shape[index] == 1;

        return false;
    }

    public void ReturnToOriginal()
    {
        dragHandler.ReturnToOriginal();
    }
}
