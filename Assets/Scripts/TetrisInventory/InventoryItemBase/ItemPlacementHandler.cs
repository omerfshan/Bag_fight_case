using UnityEngine;
using DG.Tweening;
using System.Collections;

public class ItemPlacementHandler
{
    private readonly InventoryGridItemController item;

    public ItemPlacementHandler(InventoryGridItemController controller)
    {
        item = controller;
    }

    
    public void PlaceItem(int gx, int gy)
    {
        item.lastWorldPos = item.transform.position;
        item.transform.SetParent(item.grid.transform, true);

        Vector2 targetPos = item.grid.GridToPos(gx, gy, item.width, item.height);

        
        item.StartCoroutine(SmoothMove(targetPos, 0.15f));

        item.grid.FillArea(gx, gy, item);

        item.lastGX = gx;
        item.lastGY = gy;

     
        item.rect.DOScale(item.originalScale, 0.15f);

        SoundManager.Instance.ItemPlaceSound();

        if (item.cooldownFill != null)
            item.cooldownFill.fillAmount = 1f;

        if (item.inv != null)
            item.inv.AddItem(item);

        if (item.TryGetComponent<UISlotInfo>(out var info))
        {
            item.spawnerRef.MarkSlotEmpty(info.slotIndex);
            Object.Destroy(info);
        }

        if (item.currentCooldown <= 0f)
            item.StartCooldown();
        else
            item.ResumeCooldown();
    }

   
    public void ReturnToOriginalGridPosition()
    {
        if (item.lastGX != -1 && item.lastGY != -1)
        {
            Vector3 worldPos = item.rect.TransformPoint(Vector3.zero);

            item.transform.SetParent(item.grid.transform, false);

            Vector2 localInGrid = item.grid.transform.InverseTransformPoint(worldPos);
            item.rect.anchoredPosition = localInGrid;

            Vector2 targetPos = item.grid.GridToPos(
                item.lastGX,
                item.lastGY,
                item.width,
                item.height
            );

          
            item.StartCoroutine(SmoothMove(targetPos, 0.2f));

            item.rect.DOScale(item.originalScale, 0.15f);

            item.grid.FillArea(item.lastGX, item.lastGY, item);

            if (item.currentCooldown <= 0f)
                item.StartCooldown();
            else
                item.ResumeCooldown();

            return;
        }

        ReturnToOriginalSlotPosition();
    }

   
    private void ReturnToOriginalSlotPosition()
    {
        item.transform.SetParent(item.originalParent, true);

        item.StartCoroutine(SmoothMove(item.originalAnchoredPos, 0.2f));

        item.rect.DOScale(item.originalScale, 0.15f);
    }

  
    private IEnumerator SmoothMove(Vector2 target, float duration)
    {
        Vector2 start = item.rect.anchoredPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / duration;
            item.rect.anchoredPosition = Vector2.Lerp(start, target, t);
            yield return null;
        }

        item.rect.anchoredPosition = target;
    }
}
