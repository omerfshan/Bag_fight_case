using UnityEngine;

public class Player_item : MonoBehaviour
{
    public SpriteRenderer sr;   // Oyuncunun tuttuğu item sprite'ı
    public float attackDamage;  // Oyuncunun vuracağı damage

    public void Load(ItemDataSO data)
    {
        attackDamage = data.AttackDamage;

        if (sr != null)
            sr.sprite = data.Sprite;
    }
}
