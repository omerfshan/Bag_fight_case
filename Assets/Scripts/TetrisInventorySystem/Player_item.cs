using UnityEngine;
using DG.Tweening;

public class Player_item : MonoBehaviour
{
    private ItemDataSO data;
    private Transform target;

    public float speed = 5f;
    public float lifetime = 2f;

    private SpriteRenderer _renderer;

    private Tween moveTween;
    private Tween rotateTween;

    private bool hasHit = false; // bir kere hasar için

    void Awake()
    {
        _renderer = GetComponent<SpriteRenderer>();
    }

    void Start()
    {
        // güvenlik için 2 saniye sonra yok et
        Destroy(gameObject, lifetime);
    }

    public void Load(ItemDataSO newData)
    {
        data = newData;

        // ⭐ Sprite yükle
        _renderer.sprite = data.Sprite;

        // ⭐ BOYUTU AYARLA
        transform.localScale = data.Size;
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

        // 🔥 EĞİK mi NORMAL mi atanacak?
        if (data.IsDiagonalThrow)
        {
            // ========== EĞİK (JUMP) ATIŞ ==========
            float jumpPower = 2f;

            moveTween = transform
                .DOJump(target.position, jumpPower, 1, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    TryHitTarget();
                    Destroy(gameObject);
                });
        }
        else
        {
            // ========== DÜZ ATIŞ ==========
            moveTween = transform
                .DOMove(target.position, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    TryHitTarget();
                    Destroy(gameObject);
                });
        }

        // döndürme animasyonu aynı kalabilir
        rotateTween = transform
            .DORotate(new Vector3(0, 0, -360), 0.3f, RotateMode.FastBeyond360)
            .SetLoops(-1)
            .SetEase(Ease.Linear);
    }


    private void TryHitTarget()
    {
        if (hasHit) return;
        if (target == null) return;

        float hitDist = Vector3.Distance(transform.position, target.position);

        // hedefe yeterince yakınsa hasar ver
        if (hitDist < 0.5f)
        {
            hasHit = true;

            Enemy enemy = target.GetComponent<Enemy>();
            if (enemy != null)
            {
                enemy.TakeDamage(data.AttackDamage);
                Debug.Log("Target enemy'e damage vuruldu! Damage: " + data.AttackDamage);
            }
        }
    }

    void OnDestroy()
    {
        rotateTween?.Kill();
        moveTween?.Kill();
    }
}
