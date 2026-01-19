using UnityEngine;
using System.Collections;

public class SpawnAttackExecutor
{
    private readonly PlayerSpawner spawner;
    private readonly Animator animator;
    private readonly string attackID;
    private readonly Player_item prefab;

    public SpawnAttackExecutor(PlayerSpawner owner, Animator anim, string attackName, Player_item bulletPrefab)
    {
        spawner = owner;
        animator = anim;
        attackID = attackName;
        prefab = bulletPrefab;
    }

    public IEnumerator FireItem(InventoryGridItemController invItem, Transform target)
    {
        ItemDataSO data = invItem.GetData();

        Player_item bullet = Object.Instantiate(prefab, spawner.transform.position, Quaternion.identity);

        SoundManager.Instance.ThrowItemSound();

        bullet.Load(data);
        bullet.SetTarget(target);

        yield return PlayAttackAnimation();
    }

    public IEnumerator PlayAttackAnimation()
    {
        if (animator == null) yield break;

        animator.SetBool(attackID, true);

        float clipLength = GetAnimationLength(animator, attackID);
        if (clipLength <= 0f) clipLength = 0.3f;

        yield return new WaitForSeconds(clipLength);

        animator.SetBool(attackID, false);
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
    public void SpawnBullet(InventoryGridItemController invItem, Transform target)
    {
        ItemDataSO data = invItem.GetData();

        Player_item bullet = Object.Instantiate(prefab,
            spawner.transform.position,
            Quaternion.identity);

        SoundManager.Instance.ThrowItemSound();

        bullet.Load(data);
        bullet.SetTarget(target);
    }



}
