using UnityEngine;
using DG.Tweening;

public class ItemVisualHandler
{
    private readonly InventoryGridItemController item;

    public ItemVisualHandler(InventoryGridItemController controller)
    {
        item = controller;
    }

    public void ScaleTo(Vector3 targetScale, float duration, Ease ease = Ease.OutBack)
    {
        item.rect.DOKill();
        item.rect.DOScale(targetScale, duration).SetEase(ease);
    }

    public void ResetScale(float duration = 0.15f)
    {
        item.rect.DOKill();
        item.rect.DOScale(item.originalScale, duration).SetEase(Ease.OutBack);
    }

    public void KillTweens()
    {
        item.rect.DOKill();
    }

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
