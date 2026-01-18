using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    [SerializeField] private InventoryManager invSystem;
    [SerializeField] private Animator anim;
    [SerializeField] private string AttackID = "Attack";
    [SerializeField] private Player_item prefab;

    private bool isAttacking = false;

    void Update()
{
    if (isAttacking) return;

    foreach (var item in invSystem.inventory_Items)
    {
        if (item.isReadyToFire)
        {
            StartCoroutine(FireItemCoroutine(item));
            break;
        }
    }
}


   private IEnumerator FireItemCoroutine(InventoryGridItemController invItem)
{
    isAttacking = true;

    ItemDataSO data = invItem.GetData();

    // ⭐ EN YAKIN READY ENEMY BUL
    Enemy targetEnemy = null;
    float closestDistance = Mathf.Infinity;
    Vector3 myPos = transform.position;

    foreach (Enemy enemy in FindObjectsOfType<Enemy>())
    {
        if (!enemy.is_ready)
            continue;

        float dist = Vector3.Distance(myPos, enemy.transform.position);

        if (dist < closestDistance)
        {
            closestDistance = dist;
            targetEnemy = enemy;
        }
    }

    // 🔥 Eğer target bulunduysa ateş et
    if (targetEnemy != null)
    {
        Player_item bullet = Instantiate(prefab, transform.position, Quaternion.identity);
        bullet.Load(data);
        bullet.SetTarget(targetEnemy.transform);

        StartCoroutine(PlayAttackAnimation());
    }

    // 🔥 Itemı listeden kaldır
    invSystem.RemoveItem(invItem);

    // 🔥 Cooldown sıfırla
    invItem.OnFiredBySpawner();

    // 🔥 Küçük global delay (0.12 saniye)
    yield return new WaitForSeconds(0.12f);

    isAttacking = false;
}




    private IEnumerator PlayAttackAnimation()
    {
        anim.SetBool(AttackID, true);

        float clipLength = GetAnimationLength(anim, AttackID);
        if (clipLength <= 0f) clipLength = 0.3f;

        yield return new WaitForSeconds(clipLength);

        anim.SetBool(AttackID, false);
    }

    private float GetAnimationLength(Animator animator, string stateName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == stateName)
                return clip.length;
        }
        return -1f;
    }
}
