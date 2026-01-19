using System.Collections;
using UnityEngine;

public class UIitemSpawner : MonoBehaviour
{
    [Header("Item veri listesi")]
    public InventoryItemDataListSO itemDatabase;

    [Header("3 adet UI slotu")]
    public RectTransform[] spawnSlots;

    [Header("UI Prefab")]
    public InventoryGridItemController itemPrefab;

    private SlotController slotController;
    private ItemFactory itemFactory;

    private void Awake()
    {
        slotController = new SlotController(spawnSlots.Length);
        itemFactory = new ItemFactory(itemDatabase, itemPrefab);
    }

    private void Start()
    {
        StartCoroutine(SpawnWithDelay());
    }

    private IEnumerator SpawnWithDelay()
    {
        yield return new WaitForSeconds(1f); 
        SpawnInitialItems();
    }

    private void SpawnInitialItems()
    {
        for (int i = 0; i < spawnSlots.Length; i++)
        {
            if (slotController.IsSlotEmpty(i))
                SpawnItemToSlot(i);
        }
    }

    private void SpawnItemToSlot(int index)
    {
        InventoryGridItemController itemUI = itemFactory.CreateRandomItem();
        SoundManager.Instance.ItemSpawnSound();

        itemUI.transform.SetParent(spawnSlots[index], true);

        RectTransform rt = itemUI.GetComponent<RectTransform>();
        rt.anchorMin = new Vector2(0.5f, 0.5f);
        rt.anchorMax = new Vector2(0.5f, 0.5f);
        rt.anchoredPosition = Vector2.zero;
        rt.localScale = Vector3.one;

        UISlotInfo info = itemUI.gameObject.AddComponent<UISlotInfo>();
        info.slotIndex = index;

        slotController.SetSlot(index, itemUI);

        PulseEffect.Play(rt);
    }

    public void MarkSlotEmpty(int index)
    {
        slotController.ClearSlot(index);

        if (slotController.AllEmpty())
            SpawnInitialItems();
    }
}
