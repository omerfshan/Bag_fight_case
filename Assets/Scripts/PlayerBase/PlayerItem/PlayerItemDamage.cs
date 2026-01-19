using UnityEngine;

public class PlayerItemDamage
{
    private readonly Player_item item;

    public PlayerItemDamage(Player_item itemRef)
    {
        item = itemRef;
    }

    public void TryHitTarget()
    {
        if (item.target == null) return;

        float hitDist = Vector3.Distance(item.transform.position, item.target.position);

        if (hitDist < 0.5f)
        {
            Enemy enemy = item.target.GetComponent<Enemy>();
            if (enemy != null)
                enemy.TakeDamage(item.data.AttackDamage);
        }
    }
}
