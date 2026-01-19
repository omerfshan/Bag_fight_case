using UnityEngine;

public class PlayerItemDataLoader
{
    private readonly Player_item item;

    public PlayerItemDataLoader(Player_item itemRef)
    {
        item = itemRef;
    }

    public void Load(ItemDataSO newData)
    {
        item.data = newData;

        // Sprite yükle
        if (item.rendererRef != null)
            item.rendererRef.sprite = item.data.Sprite;

        // Boyut yükle
        item.transform.localScale = item.data.Size;
    }
}
