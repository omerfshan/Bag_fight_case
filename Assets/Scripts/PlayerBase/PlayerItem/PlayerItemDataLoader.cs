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

        if (item.rendererRef != null)
            item.rendererRef.sprite = item.data.Sprite;

        item.transform.localScale = item.data.Size;
    }
}
