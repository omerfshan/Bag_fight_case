using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;
using UnityEngine.UI;

public class InventoryGridItemController: MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
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
    

    private InventoryManager inv;
    [SerializeField] private ItemDataSO itemProperty;
    [SerializeField] private Image cooldownFill;
    // Cooldown
    private bool isOnCooldown = false;
    private float currentCooldown = 0;
    private Coroutine cooldownRoutine;

public bool isReadyToFire = false;
private Transform originalParent;
private Vector2 originalAnchoredPos;
private TrashArea trashArea;
private bool isOnTrash = false;
private Image itemImage; 
private Color originalItemColor;
private Color originalFillColor;
    // =========================
    // COOLDOWN FONKSIYONLARI
    // =========================
   

public void StartCooldown()
{
    if (itemProperty == null) return;
    if (isDragging) return;
    if (lastGX == -1 || lastGY == -1) return;

    if (currentCooldown <= 0f)   // tamamen bitmişse yeni başlat
    {
        currentCooldown = itemProperty.CoolDown;

        if (cooldownFill != null)
            cooldownFill.fillAmount = 0f;  // başta boş olsun

        cooldownRoutine = StartCoroutine(CooldownRoutine());
    }
}

private void PulseEffect()
{
    rect.DOKill(); // eski animler çakışmasın

    Sequence seq = DOTween.Sequence();

    seq.Append(rect.DOScale(originalScale * 1.12f, 0.15f).SetEase(Ease.OutBack))
       .Append(rect.DOScale(originalScale, 0.15f).SetEase(Ease.InBack));
}


 private IEnumerator CooldownRoutine()
{
    isOnCooldown = true;
    float maxCooldown = itemProperty.CoolDown;

    // Dolma phase
    while (currentCooldown > 0f)
    {
        currentCooldown -= Time.deltaTime;

        if (cooldownFill != null)
            cooldownFill.fillAmount = 1f - (currentCooldown / maxCooldown);

        yield return null;
    }

    // Buraya geldi → cooldown %100 doldu  
    cooldownFill.fillAmount = 1f;

    // 🔥 FULL OLDUĞU ANDA PULSE ÇAK
    PulseEffect();

    // 🔥 Pulse bitene kadar hafif gecikme (anim süresi 0.3sn)
    yield return new WaitForSeconds(0.3f);

    // Artık gerçekten atışa hazır
    isReadyToFire = true;

    // Listeye ekle (eğer yoksa)
    if (!inv.inventory_Items.Contains(this))
        inv.AddItem(this);

    isOnCooldown = false;
    cooldownRoutine = null;
}



public void OnFiredBySpawner()
{
    isReadyToFire = false;

    if (cooldownFill != null)
        cooldownFill.fillAmount = 0f;

    currentCooldown = itemProperty.CoolDown;

    if (cooldownRoutine != null)
        StopCoroutine(cooldownRoutine);

    cooldownRoutine = StartCoroutine(CooldownRoutine());
}





private void PauseCooldown()
{
    if (!isOnCooldown) return;

    // Coroutine durdur
    if (cooldownRoutine != null)
    {
        StopCoroutine(cooldownRoutine);
        cooldownRoutine = null;
    }

    isOnCooldown = true; // Duruyor ama bitmedi
}
private void ResumeCooldown()
{
    if (isDragging) return;
    if (!isOnCooldown) return;
    if (currentCooldown <= 0f) return;

    cooldownRoutine = StartCoroutine(CooldownRoutine());
}




    // =========================
    // UNITY
    // =========================
    void Awake()
    {
        rect = GetComponent<RectTransform>();
        canvas = GameObject.FindWithTag("MainCanvas").GetComponent<Canvas>();
        originalScale = rect.localScale;
        if (cooldownFill != null)
            cooldownFill.fillAmount = 1f;
        inv = FindAnyObjectByType<InventoryManager>();
        grid=FindAnyObjectByType<InventoryGrid>();
        itemImage = GetComponent<Image>();
        trashArea = FindAnyObjectByType<TrashArea>();
       
        if (itemImage != null)
            originalItemColor = itemImage.color;

        if (cooldownFill != null)
    originalFillColor = cooldownFill.color;

    }

    // void Update()
    // {
    //     if (isDragging && Input.GetKeyDown(KeyCode.R))
    //         RotateItem();
    // }


    // =========================
    // DRAG EVENTLERI
    // =========================
   public void OnBeginDrag(PointerEventData eventData)
{
    isDragging = true;
  originalParent = transform.parent;
    originalAnchoredPos = rect.anchoredPosition;
  PauseCooldown();

    if (inv != null)
        inv.RemoveItem(this);

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

    if (inv != null)
        inv.RemoveItem(this);

    // ================================
    // ⭐ GRID ÜSTÜNDE Mİ KONTROL
    // ================================
    RectTransform gridRect = grid.GetComponent<RectTransform>();

    bool overGrid = RectTransformUtility.RectangleContainsScreenPoint(
        gridRect,
        eventData.position,
        canvas.worldCamera
    );

    if (overGrid)
    {
        if (grid.ScreenToGrid(eventData.position, out int gx, out int gy))
        {
            int targetGX = gx - (width / 2);
            int targetGY = gy - (height / 2);
            grid.HighlightArea(targetGX, targetGY, this);
        }
    }
    else
    {
        // ⭐ Grid dışına çıkınca hücreleri eski haline döndür
        grid.ClearAllHover();
    }

    // ================================
    // ÇÖP ALANI KONTROLÜ
    // ================================
    if (trashArea != null)
    {
        RectTransform trashRect = trashArea.GetComponent<RectTransform>();

        bool overTrash = RectTransformUtility.RectangleContainsScreenPoint(
            trashRect,
            eventData.position,
            canvas.worldCamera
        );

        if (overTrash && !isOnTrash)
        {
            isOnTrash = true;

            trashArea.SetOpen();

            Color trashColor = new Color(1f, 0.3f, 0.3f, 0.7f);

            if (itemImage != null)
                itemImage.color = trashColor;

            if (cooldownFill != null)
                cooldownFill.color = trashColor;
        }
        else if (!overTrash && isOnTrash)
        {
            isOnTrash = false;

            trashArea.SetClose();

            if (itemImage != null)
                itemImage.color = originalItemColor;

            if (cooldownFill != null)
                cooldownFill.color = originalFillColor;
        }
    }
}


    public void OnEndDrag(PointerEventData eventData)
    {
        if (isOnTrash)
        {
            isOnTrash = false;

            trashArea.SetClose();

            // ⭐ Eğer item UI slot'tan geldiyse → slot'u boşalt
            if (TryGetComponent<UISlotInfo>(out var info))
            {
                FindAnyObjectByType<UIitemSpawner>().MarkSlotEmpty(info.slotIndex);
                Destroy(info);   // bu component'i sil
            }

            // 🔥 Item kırmızı kalsın ve yok olsun
            rect.DOScale(0f, 0.2f).SetEase(Ease.InBack).OnComplete(() =>
            {
                Destroy(gameObject);
            });

            return;
        }




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

                if (inv != null)
                    inv.AddItem(this);

                // 🔥 UI slot boşalt (Yeni ekleme)
                if (TryGetComponent<UISlotInfo>(out var info))
                {
                    FindAnyObjectByType<UIitemSpawner>().MarkSlotEmpty(info.slotIndex);
                    Destroy(info);
                }

                // Cooldown
                if (currentCooldown <= 0f)
                    StartCooldown();
                else
                    ResumeCooldown();

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
    // GRIDDEYDİ → grid pozisyonuna dön
    if (lastGX != -1 && lastGY != -1)
    {
        Vector3 worldPos = rect.TransformPoint(Vector3.zero);
        transform.SetParent(grid.transform, false);

        Vector2 localInGrid = grid.transform.InverseTransformPoint(worldPos);
        rect.anchoredPosition = localInGrid;

        Vector2 targetPos = grid.GridToPos(lastGX, lastGY, width, height);
        rect.DOAnchorPos(targetPos, 0.2f).SetEase(Ease.OutQuad);
        rect.DOScale(originalScale, 0.15f);

        grid.FillArea(lastGX, lastGY, this);

        // 🔥 EKLENECEK TEK KISIM
        if (currentCooldown <= 0f)
            StartCooldown();
        else
            ResumeCooldown();

        return;
    }

    // GRIDDE DEĞİLDİ → UI'YA dön
    transform.SetParent(originalParent, false);
    rect.DOAnchorPos(originalAnchoredPos, 0.2f);
    rect.DOScale(originalScale, 0.15f);
}





    public bool IsCellInShape(int localX, int localY)
    {
        int index = localY * width + localX;
        if (index >= 0 && index < shape.Length)
            return shape[index] == 1;

        return false;
    }

    // private void RotateItem()
    // {
    //     int[] newShape = new int[shape.Length];

    //     for (int y = 0; y < height; y++)
    //     {
    //         for (int x = 0; x < width; x++)
    //         {
    //             int newX = (height - 1) - y;
    //             int newY = x;
    //             newShape[newY * height + newX] = shape[y * width + x];
    //         }
    //     }

    //     shape = newShape;

    //     int temp = width;
    //     width = height;
    //     height = temp;

    //     rect.DORotate(rect.eulerAngles + new Vector3(0, 0, -90), 0.2f);

    //     if (grid.ScreenToGrid(Input.mousePosition, out int gx, out int gy))
    //     {
    //         int targetGX = gx - (width / 2);
    //         int targetGY = gy - (height / 2);
    //         grid.HighlightArea(targetGX, targetGY, this);
    //     }
    // }
    public ItemDataSO GetData()
{
    return itemProperty;
}
public void LoadData(InventoryItemSO so)
{
    itemProperty = so.ItemProperty;

    width = so.Width;
    height = so.Height;
    shape = so.Shape;

    // Sprite
    Image img;
    if (TryGetComponent<Image>(out img))
        img.sprite = so.ItemProperty.Sprite;

    // SIZE
    rect.sizeDelta = so.UISize;
    if (img != null)
        img.rectTransform.sizeDelta = so.UISize;

    // ============================
    // ROTATION (SADECE ITEM VE ANA İMAGE)
    // ============================
    Quaternion rot = Quaternion.Euler(so.Rotation);

    rect.localRotation = rot;           
    if (img != null)
        img.rectTransform.localRotation = rot;

    // ============================
    // 🔥 COOLDOWN FILL → SAKIN ROTATE ETME
    // ============================
    if (cooldownFill != null)
    {
        cooldownFill.sprite = img.sprite;
        cooldownFill.rectTransform.sizeDelta = so.UISize;

        // Rotasyon sıfırlanır → böylece yamulmaz
        cooldownFill.rectTransform.localRotation = Quaternion.identity;
    }

    currentCooldown = 0f;
    isOnCooldown = false;
    lastGX = -1;
    lastGY = -1;

    rect.localScale = originalScale;
}




}
