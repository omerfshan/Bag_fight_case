using UnityEngine;
using System.Collections;

public class Spawner : MonoBehaviour
{
    [SerializeField] private InventoryManager invSystem;
    [SerializeField] private Animator anim;
    [SerializeField] private string AttackID = "Attack";
    [SerializeField] private Player_item prefab;

    private void OnEnable()
    {
        invSystem.OnItemAdded += HandleItemAdded;
    }

    private void OnDisable()
    {
        invSystem.OnItemAdded -= HandleItemAdded;
    }

    private void HandleItemAdded(InventoryGridItemController invItem)
    {
        ItemDataSO data = invItem.GetData();

        Enemy[] enemies = FindObjectsOfType<Enemy>();

        foreach (Enemy enemy in enemies)
        {
            if (enemy.is_ready)
            {
                Player_item bullet = Instantiate(prefab, transform.position, Quaternion.identity);

                bullet.Load(data);
                bullet.SetTarget(enemy.transform);

                // 🔥 ANİMASYON → ANINDA BAŞLA
                StartCoroutine(PlayAttackAnimation());

                Debug.Log("Item fırlatıldı → " + enemy.name);
            }
        }

        invSystem.RemoveItem(invItem);
    }

    private IEnumerator PlayAttackAnimation()
    {
        // 🔥 Attack animasyonunu başlat
        anim.SetBool(AttackID, true);

        // 🔥 Attack animasyonunun klip uzunluğunu al (gecikmesiz, %100 doğru)
        float clipLength = GetAnimationLength(anim, AttackID);

        // Klip bulunamadıysa fallback
        if (clipLength <= 0f) clipLength = 0.3f;

        // 🔥 Animasyon süresi kadar bekle
        yield return new WaitForSeconds(clipLength);

        // 🔥 Animasyonu kapat
        anim.SetBool(AttackID, false);
    }

    private float GetAnimationLength(Animator animator, string stateName)
    {
        foreach (AnimationClip clip in animator.runtimeAnimatorController.animationClips)
        {
            if (clip.name == stateName)
                return clip.length; // 🔥 gerçek süre
        }

        return -1f; // bulunamazsa
    }
}
