using UnityEngine;
using DG.Tweening;

public class Player_item : MonoBehaviour
{
    private ItemDataSO data;
    private Transform target;

    public float speed = 5f;
    private SpriteRenderer _renderer;

    private Tween moveTween;
    private Tween rotateTween;

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    public void Load(ItemDataSO newData)
    {
        data = newData;
        _renderer.sprite = data.Sprite;
    }

    public void SetTarget(Transform t)
    {
        target = t;
        StartTweenToTarget();
    }

    private void StartTweenToTarget()
    {
        if (target == null) return;

        float distance = Vector3.Distance(transform.position, target.position);
        float duration = distance / speed;
        float jumpPower = 2f;

        moveTween = transform
            .DOJump(target.position, jumpPower, 1, duration)
            .SetEase(Ease.Linear);

        rotateTween = transform
            .DORotate(new Vector3(0, 0, -360), 0.3f, RotateMode.FastBeyond360)
            .SetLoops(-1)
            .SetEase(Ease.Linear);
    }

    // 🔥 ÇARPIŞMA BURADA
    private void OnTriggerEnter2D(Collider2D other)
    {
        // Diğer objede Enemy var mı?
        Enemy enemy = other.GetComponent<Enemy>();
        if (enemy != null)
        {
            Debug.Log("Düşmana çarptı! Damage: " + data.AttackDamage);

            enemy.TakeDamage(data.AttackDamage);

            rotateTween?.Kill();
            moveTween?.Kill();

            Destroy(gameObject);
        }
    }

    void OnDestroy()
    {
        rotateTween?.Kill();
        moveTween?.Kill();
    }
}
