using UnityEngine;
using UnityEngine.EventSystems;
using DG.Tweening;
using System.Collections;

public class ItemDragHandler
{
    private readonly InventoryGridItemController item;

    public ItemDragHandler(InventoryGridItemController controller)
    {
        item = controller;
    }

   
    public void OnBeginDrag(PointerEventData eventData)
    {
        item.isDragging = true;
        SoundManager.Instance.ItemPickSound();

        item.originalParent = item.transform.parent;
        item.originalAnchoredPos = item.rect.anchoredPosition;

        item.PauseCooldown();

        if (item.inv != null)
            item.inv.RemoveItem(item);

        item.transform.SetParent(item.canvas.transform);

        item.originalPos = item.rect.anchoredPosition;

        item.rect.DOKill();
        item.rect.DOScale(item.originalScale * item.dragScale, 0.2f)
            .SetEase(DG.Tweening.Ease.OutBack);

        if (item.lastGX != -1)
            item.grid.ClearArea(item.lastGX, item.lastGY, item);

        item.rect.SetAsLastSibling();
    }

    
    public void OnDrag(PointerEventData eventData)
    {
        Camera cam = (item.canvas.renderMode == RenderMode.ScreenSpaceOverlay)
            ? null
            : item.canvas.worldCamera;

        RectTransformUtility.ScreenPointToLocalPointInRectangle(
            item.transform.parent as RectTransform,
            eventData.position,
            cam,
            out Vector2 localPoint
        );

        item.rect.anchoredPosition = localPoint;

        if (item.inv != null)
            item.inv.RemoveItem(item);

        RectTransform gridRect = item.grid.GetComponent<RectTransform>();

        bool overGrid = RectTransformUtility.RectangleContainsScreenPoint(
            gridRect,
            eventData.position,
            item.canvas.worldCamera
        );

        if (overGrid)
        {
            if (item.grid.ScreenToGrid(eventData.position, out int gx, out int gy))
            {
                int targetGX = gx - (item.width / 2);
                int targetGY = gy - (item.height / 2);

                item.grid.HighlightArea(targetGX, targetGY, item);
            }
        }
        else
        {
            item.grid.ClearAllHover();
        }

       
        if (item.trashArea != null)
        {
            RectTransform trashRect = item.trashArea.GetComponent<RectTransform>();

            bool overTrash = RectTransformUtility.RectangleContainsScreenPoint(
                trashRect,
                eventData.position,
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
            }
            else if (!overTrash && item.isOnTrash)
            {
                item.isOnTrash = false;

                item.trashArea.SetClose();

                if (item.itemImage != null)
                    item.itemImage.color = item.originalItemColor;

                if (item.cooldownFill != null)
                    item.cooldownFill.color = item.originalFillColor;
            }
        }
    }

   
    public void OnEndDrag(PointerEventData eventData)
    {
        if (item.isOnTrash)
        {
            HandleTrashDelete();
            return;
        }

        item.isDragging = false;

        item.grid.ClearAllHover();
        item.rect.DOKill();

     
        if (item.grid.ScreenToGrid(eventData.position, out int gx, out int gy))
        {
            int targetGX = gx - Mathf.FloorToInt(item.width / 2f);
            int targetGY = gy - Mathf.FloorToInt(item.height / 2f);

            if (item.grid.CanPlace(targetGX, targetGY, item))
            {
                PlaceItem(targetGX, targetGY);
                return;
            }
        }

       
        item.ReturnToOriginal();
    }

   
    private void PlaceItem(int gx, int gy)
    {
        item.transform.SetParent(item.grid.transform, true);

        Vector2 targetPos = item.grid.GridToPos(gx, gy, item.width, item.height);

        item.StartCoroutine(SmoothPlace(targetPos));

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

        // 🔥 Eğer şu an cooldown içindeyse (daha önce ateş etmiş ve durdurulmuşsa)
        // sadece kaldığı yerden devam ettir.
        if (item.currentCooldown > 0f && item.isOnCooldown)
        {
            item.ResumeCooldown();
        }
        else
        {
            // ❗ Hiç cooldown yoksa (yeni item) → direkt ready olsun.
            item.isReadyToFire = true;
        }
    }



   
    private void HandleTrashDelete()
    {
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
    }

    
   public void ReturnToOriginal()
    {
        item.isDragging = false;

        // GRİDE GERİ DÖNÜŞ
        if (item.lastGX != -1 && item.lastGY != -1)
        {
            Vector2 screenPos = RectTransformUtility.WorldToScreenPoint(item.canvas.worldCamera, item.rect.position);

            RectTransform gridRect = item.grid.GetComponent<RectTransform>();
            RectTransformUtility.ScreenPointToLocalPointInRectangle(
                gridRect,
                screenPos,
                item.canvas.worldCamera,
                out Vector2 localPos
            );

            item.transform.SetParent(item.grid.transform, false);
            item.rect.anchoredPosition = localPos;

            Vector2 targetPos = item.grid.GridToPos(item.lastGX, item.lastGY, item.width, item.height);

        
            item.StartCoroutine(SmoothReturn(targetPos));

            item.rect.DOScale(item.originalScale, 0.15f);

            item.grid.FillArea(item.lastGX, item.lastGY, item);

            if (item.currentCooldown <= 0f)
                item.StartCooldown();
            else
                item.ResumeCooldown();

            return;
        }


        item.transform.SetParent(item.originalParent, true);

    
        item.StartCoroutine(SmoothReturn(item.originalAnchoredPos));

        item.rect.DOScale(item.originalScale, 0.15f);
    }

    private IEnumerator SmoothReturn(Vector2 targetPos, float time = 0.18f)
    {
        Vector2 start = item.rect.anchoredPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / time;
            item.rect.anchoredPosition = Vector2.Lerp(start, targetPos, t);
            yield return null;
        }

        item.rect.anchoredPosition = targetPos;
    }
    private IEnumerator SmoothPlace(Vector2 targetPos, float time = 0.13f)
    {
        Vector2 start = item.rect.anchoredPosition;
        float t = 0f;

        while (t < 1f)
        {
            t += Time.deltaTime / time;
            item.rect.anchoredPosition = Vector2.Lerp(start, targetPos, t);
            yield return null;
        }

        item.rect.anchoredPosition = targetPos;
    }


}
