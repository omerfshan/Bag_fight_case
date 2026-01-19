using UnityEngine;
using DG.Tweening;

public class ItemVisualHandler
{
    private readonly InventoryGridItemController item;

    public ItemVisualHandler(InventoryGridItemController controller)
    {
        item = controller;
    }

    // =====================================================
    //  SCALE DEĞİŞTİRME
    // =====================================================
    public void ScaleTo(Vector3 targetScale, float duration, Ease ease = Ease.OutBack)
    {
        item.rect.DOKill();
        item.rect.DOScale(targetScale, duration).SetEase(ease);
    }

    // =====================================================
    //  ORİJİNAL SCALE’E DÖN
    // =====================================================
    public void ResetScale(float duration = 0.15f)
    {
        item.rect.DOKill();
        item.rect.DOScale(item.originalScale, duration).SetEase(Ease.OutBack);
    }

    // =====================================================
    //  TWEENLERİ TEMİZLE
    // =====================================================
    public void KillTweens()
    {
        item.rect.DOKill();
    }

    // =====================================================
    //  RENK AYARI (GENEL)
    // =====================================================
    public void SetItemColor(Color c)
    {
        if (item.itemImage != null)
            item.itemImage.color = c;

        if (item.cooldownFill != null)
            item.cooldownFill.color = c;
    }

    public void ResetItemColor()
    {
        if (item.itemImage != null)
            item.itemImage.color = item.originalItemColor;

        if (item.cooldownFill != null)
            item.cooldownFill.color = item.originalFillColor;
    }
}
