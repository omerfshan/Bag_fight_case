using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

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

    private InventorySystem invSystem;
    [SerializeField] private ItemDataSO itemProperty;
    [SerializeField] private Image cooldownFill;
    // Cooldown
    private bool isOnCooldown = false;
    private float currentCooldown = 0;
    private Coroutine cooldownRoutine;


    // =========================
    // COOLDOWN FONKSIYONLARI
    // =========================
    public void ResetCooldown()
    {
        if (cooldownRoutine != null)
        {
            StopCoroutine(cooldownRoutine);
            cooldownRoutine = null;
        }

        isOnCooldown = false;
        currentCooldown = 0;
    if (cooldownFill != null)
        cooldownFill.fillAmount = 0f;

        Debug.Log($"{name} cooldown sıfırlandı.");
    }

  public void StartCooldown()
{
    if (itemProperty == null) return;

    // 🔥 DRAG ESNASINDA COOLDOWN BAŞLAMASIN
    if (isDragging) return;

    // 🔥 sadece grid üzerindeyse cooldown başlasın
    if (lastGX == -1 || lastGY == -1)
        return;

    if (!isOnCooldown)
        cooldownRoutine = StartCoroutine(CooldownRoutine());
}


   private IEnumerator CooldownRoutine()
{
    isOnCooldown = true;
    currentCooldown = itemProperty.CoolDown;

    float maxCooldown = currentCooldown;

    Debug.Log($"{name} cooldown başladı: {currentCooldown}");

    if (cooldownFill != null)
        cooldownFill.fillAmount = 0f;

    while (currentCooldown > 0)
    {
        currentCooldown -= Time.deltaTime;

        if (cooldownFill != null)
            cooldownFill.fillAmount = 1f - (currentCooldown / maxCooldown);

        yield return null;
    }

    isOnCooldown = false;
    cooldownRoutine = null;

    Debug.Log($"{name} cooldown bitti → Inventory’e ekleniyor");

    if (cooldownFill != null)
        cooldownFill.fillAmount = 1f;

    // Listeye Ekle
    if (invSystem != null)
        invSystem.AddItem(this);

    // Küçük gecikme
    yield return new WaitForSeconds(0.1f);

    if (cooldownFill != null)
        cooldownFill.fillAmount = 0f;

    // 🔥 GRID ÜZERİNDE DURDUĞU SÜRECE TEKRAR BAŞLAT!
    if (lastGX != -1 && lastGY != -1)
        StartCooldown();
}




    // =========================
    // UNITY
    // =========================
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GetComponentInParent<Canvas>();
        originalScale = rect.localScale;
        if (cooldownFill != null)
            cooldownFill.fillAmount = 1f;
        invSystem = FindAnyObjectByType<InventorySystem>();
    }

    void Update()
    {
        if (isDragging && Input.GetKeyDown(KeyCode.R))
            RotateItem();
    }


    // =========================
    // DRAG EVENTLERI
    // =========================
   public void OnBeginDrag(PointerEventData eventData)
{
    isDragging = true;

    ResetCooldown();

    if (invSystem != null)
        invSystem.RemoveItem(this);

    transform.SetParent(canvas.transform);

    originalPos = rect.anchoredPosition;

    scaleTween?.Kill();
    scaleTween = rect.DOScale(originalScale * dragScale, 0.2f).SetEase(Ease.OutBack);

    if (lastGX != -1)
        grid.ClearArea(lastGX, lastGY, this);

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

        if (invSystem != null)
            invSystem.RemoveItem(this);

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
                rect.DOAnchorPos(targetPos, 0.15f).SetEase(Ease.OutQuint);

                grid.FillArea(targetGX, targetGY, this);

                lastGX = targetGX;
                lastGY = targetGY;

                rect.DOScale(originalScale, 0.15f);

                if (cooldownFill != null)
                    cooldownFill.fillAmount = 1f;

                // 🔥 İTEMİ ANINDA ENVANTERE EKLE
                if (invSystem != null)
                    invSystem.AddItem(this);
                // Cooldown başlat
                StartCooldown();

                return;
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


    // =========================
    // HELPER FONKSIYONLAR
    // =========================
    private void ReturnToOriginal()
{
    rect.DOAnchorPos(originalPos, 0.3f).SetEase(Ease.OutBounce);

    if (lastGX != -1 && lastGY != -1)
    {
        grid.FillArea(lastGX, lastGY, this);

        // 🔥 GRID’de durduğu sürece cooldown hep devam etsin
        StartCooldown();
    }
}


    public bool IsCellInShape(int localX, int localY)
    {
        int index = localY * width + localX;
        if (index >= 0 && index < shape.Length)
            return shape[index] == 1;

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
    public ItemDataSO GetData()
{
    return itemProperty;
}

}
