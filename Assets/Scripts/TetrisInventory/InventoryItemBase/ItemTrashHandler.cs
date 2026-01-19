using UnityEngine;
using DG.Tweening;

public class ItemTrashHandler
{
    private readonly InventoryGridItemController item;

    public ItemTrashHandler(InventoryGridItemController controller)
    {
        item = controller;
    }

 
    public void HandleTrashHover(Vector2 pointerPos)
    {
        if (item.trashArea == null)
            return;

        RectTransform trashRect = item.trashArea.GetComponent<RectTransform>();

        bool overTrash = RectTransformUtility.RectangleContainsScreenPoint(
            trashRect,
            pointerPos,
            item.canvas.worldCamera
        );

        
        if (overTrash && !item.isOnTrash)
        {
            item.isOnTrash = true;

            item.trashArea.SetOpen();

            Color trashColor = new Color(1f, 0.3f, 0.3f, 0.7f);

            if (item.itemImage != null)
                item.itemImage.color = trashColor;

            if (item.cooldownFill != null)
                item.cooldownFill.color = trashColor;

            return;
        }

       
        if (!overTrash && item.isOnTrash)
        {
            item.isOnTrash = false;

            item.trashArea.SetClose();

            if (item.itemImage != null)
                item.itemImage.color = item.originalItemColor;

            if (item.cooldownFill != null)
                item.cooldownFill.color = item.originalFillColor;
        }
    }

  
    public bool TryDeleteOnTrash()
    {
        if (!item.isOnTrash)
            return false;

        item.isOnTrash = false;
        item.trashArea.SetClose();
        SoundManager.Instance.TrashSound();

     
        if (item.TryGetComponent<UISlotInfo>(out var info))
        {
            item.spawnerRef.MarkSlotEmpty(info.slotIndex);
            Object.Destroy(info);
        }

     
        item.rect.DOScale(0f, 0.2f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                Object.Destroy(item.gameObject);
            });

        return true;
    }
}
