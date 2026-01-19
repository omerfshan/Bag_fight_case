using UnityEngine;
using UnityEngine.UI;

public class ItemDataLoader
{
    private readonly InventoryGridItemController item;

    public ItemDataLoader(InventoryGridItemController controller)
    {
        item = controller;
    }

 
    public void LoadData(InventoryItemSO so)
    {
       
        item.itemProperty = so.ItemProperty;

        item.width = so.Width;
        item.height = so.Height;
        item.shape = so.Shape;

     
        Image img;
        if (item.TryGetComponent<Image>(out img))
            img.sprite = so.ItemProperty.Sprite;

      
        item.rect.sizeDelta = so.UISize;

        if (img != null)
            img.rectTransform.sizeDelta = so.UISize;

     
        Quaternion rot = Quaternion.Euler(so.Rotation);

        item.rect.localRotation = rot;

        if (img != null)
            img.rectTransform.localRotation = rot;

     
        if (item.cooldownFill != null)
        {
            item.cooldownFill.sprite = img.sprite;
            item.cooldownFill.rectTransform.sizeDelta = so.UISize;

           
            item.cooldownFill.rectTransform.localRotation = Quaternion.identity;
        }

  
        item.currentCooldown = 0f;
        item.isOnCooldown = false;

        item.lastGX = -1;
        item.lastGY = -1;

        item.rect.localScale = item.originalScale;
    }
}
