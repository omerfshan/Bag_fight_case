using UnityEngine;
using System.Collections;
using DG.Tweening;

public class ItemCooldownHandler
{
    private readonly InventoryGridItemController item;

    public ItemCooldownHandler(InventoryGridItemController controller)
    {
        item = controller;
    }

    // =====================================================
    //  START COOLDOWN
    // =====================================================
    public void StartCooldown()
    {
        if (item.itemProperty == null) return;
        if (item.isDragging) return;
        if (item.lastGX == -1 || item.lastGY == -1) return;

        if (item.currentCooldown <= 0f)
        {
            item.currentCooldown = item.itemProperty.CoolDown;

            if (item.cooldownFill != null)
                item.cooldownFill.fillAmount = 0f;

            item.cooldownRoutine = item.StartCoroutine(CooldownRoutine());
        }
    }

    // =====================================================
    //  FIRE SONRASI COOLDOWN
    // =====================================================
    public void OnFiredBySpawner()
    {
        item.isReadyToFire = false;

        if (item.cooldownFill != null)
            item.cooldownFill.fillAmount = 0f;

        item.currentCooldown = item.itemProperty.CoolDown;

        if (item.cooldownRoutine != null)
            item.StopCoroutine(item.cooldownRoutine);

        item.cooldownRoutine = item.StartCoroutine(CooldownRoutine());
    }

    // =====================================================
    // COOLDOWN DURDURMA
    // =====================================================
    public void PauseCooldown()
    {
        if (!item.isOnCooldown) return;

        if (item.cooldownRoutine != null)
        {
            item.StopCoroutine(item.cooldownRoutine);
            item.cooldownRoutine = null;
        }

        item.isOnCooldown = true;
    }

    // =====================================================
    // COOLDOWN DEVAM
    // =====================================================
    public void ResumeCooldown()
    {
        if (item.isDragging) return;
        if (!item.isOnCooldown) return;
        if (item.currentCooldown <= 0f) return;

        item.cooldownRoutine = item.StartCoroutine(CooldownRoutine());
    }

    // =====================================================
    // COOLDOWN ANA ROUTINE (ORİJİNAL)
    // =====================================================
    private IEnumerator CooldownRoutine()
    {
        item.isOnCooldown = true;

        float maxCooldown = item.itemProperty.CoolDown;

        // Dolma fazı
        while (item.currentCooldown > 0f)
        {
            item.currentCooldown -= Time.deltaTime;

            if (item.cooldownFill != null)
                item.cooldownFill.fillAmount = 1f - (item.currentCooldown / maxCooldown);

            yield return null;
        }

        item.cooldownFill.fillAmount = 1f;

        // Pulse efekti
        PulseEffect();

        yield return new WaitForSeconds(0.3f);

        // artık atışa hazır
        item.isReadyToFire = true;

        // listeye ekle
        if (!item.inv.inventory_Items.Contains(item))
            item.inv.AddItem(item);

        item.isOnCooldown = false;
        item.cooldownRoutine = null;
    }

    // =====================================================
    // PULSE EFEKTİ (ORİJİNAL)
    // =====================================================
    private void PulseEffect()
    {
        item.rect.DOKill();

        Sequence seq = DOTween.Sequence();

        seq.Append(item.rect.DOScale(item.originalScale * 1.12f, 0.15f)
            .SetEase(Ease.OutBack))
           .Append(item.rect.DOScale(item.originalScale, 0.15f)
            .SetEase(Ease.InBack));
    }
}
