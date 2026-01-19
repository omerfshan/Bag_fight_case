using UnityEngine;
using UnityEngine.UI;

public class ItemDataLoader
{
    private readonly InventoryGridItemController item;

    public ItemDataLoader(InventoryGridItemController controller)
    {
        item = controller;
    }

    // =====================================================
    //    SENİN ORİJİNAL LoadData(...) METHOD'UN
    // =====================================================
    public void LoadData(InventoryItemSO so)
    {
        // Item Property
        item.itemProperty = so.ItemProperty;

        item.width = so.Width;
        item.height = so.Height;
        item.shape = so.Shape;

        // ============================
        // Sprite yükleme
        // ============================
        Image img;
        if (item.TryGetComponent<Image>(out img))
            img.sprite = so.ItemProperty.Sprite;

        // ============================
        // SIZE ayarı
        // ============================
        item.rect.sizeDelta = so.UISize;

        if (img != null)
            img.rectTransform.sizeDelta = so.UISize;

        // ============================
        // ROTATION
        // (Senin orijinal kodun — bire bir)
        // ============================
        Quaternion rot = Quaternion.Euler(so.Rotation);

        item.rect.localRotation = rot;

        if (img != null)
            img.rectTransform.localRotation = rot;

        // ============================
        // COOLDOWN FILL  ROTATE ETME!
        // (Senin orijinal fix’in)
        // ============================
        if (item.cooldownFill != null)
        {
            item.cooldownFill.sprite = img.sprite;
            item.cooldownFill.rectTransform.sizeDelta = so.UISize;

            // cooldown fill yamulmasın diye her zaman sıfır derece
            item.cooldownFill.rectTransform.localRotation = Quaternion.identity;
        }

        // ============================
        // RESET STATE
        // ============================
        item.currentCooldown = 0f;
        item.isOnCooldown = false;

        item.lastGX = -1;
        item.lastGY = -1;

        item.rect.localScale = item.originalScale;
    }
}
