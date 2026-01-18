using UnityEngine;

public class UIitemSpawner : MonoBehaviour
{
    [Header("Tüm itemlerin bulunduğu veri listesi")]
    public InventoryItemDataListSO itemDatabase;

    [Header("Canvas üzerindeki 3 slot (Transform)")]
    public  RectTransform[] spawnSlots;

    [Header("Spawn edilecek Item UI Prefab")]
    public InventoryGridItemController itemPrefab;

    // Hangi slotun dolu olduğunu takip eder
    private InventoryGridItemController[] slotItems;

    private void Awake()
    {
        slotItems = new InventoryGridItemController[spawnSlots.Length];
    }

    private void Start()
    {
        SpawnInitialItems();
    }

    // ================================
    //  BAŞTA 3 TANE ITEM SPAWN
    // ================================
    private void SpawnInitialItems()
    {
        for (int i = 0; i < spawnSlots.Length; i++)
        {
            if (slotItems[i] == null)
                SpawnItemToSlot(i);
        }
    }

    // ================================
    //  TEK SLOT'A ITEM KOYMA
    // ================================
  private void SpawnItemToSlot(int index)
{
    // Rastgele item seç
    InventoryItemSO randomSO =
        itemDatabase.inventoryItems[Random.Range(0, itemDatabase.inventoryItems.Count)];

    // Prefab oluştur (parent belirtmeden)
    InventoryGridItemController itemUI = Instantiate(itemPrefab);

    // Doğru UI parentlama
    itemUI.transform.SetParent(spawnSlots[index], true);

    // Slotun ortasına oturması için
    RectTransform rt = itemUI.GetComponent<RectTransform>();
    rt.anchorMin = new Vector2(0.5f, 0.5f);
    rt.anchorMax = new Vector2(0.5f, 0.5f);
    rt.anchoredPosition = Vector2.zero;
    rt.localScale = Vector3.one;

    // Veriyi yükle
    itemUI.LoadData(randomSO);

    // Slot bilgisini ekle
    UISlotInfo info = itemUI.gameObject.AddComponent<UISlotInfo>();
    info.slotIndex = index;

    // Slotu dolu işaretle
    slotItems[index] = itemUI;
}


    // ================================
    //  ITEM GRID'E ALININCA SLOTTAN ÇIKART
    // ================================
    public void MarkSlotEmpty(int index)
    {
        slotItems[index] = null;

        // Tüm slotlar boşsa → yeniden 3 item spawn
        if (AllSlotsEmpty())
            SpawnInitialItems();
    }

    private bool AllSlotsEmpty()
    {
        for (int i = 0; i < slotItems.Length; i++)
            if (slotItems[i] != null)
                return false;

        return true;
    }
}
