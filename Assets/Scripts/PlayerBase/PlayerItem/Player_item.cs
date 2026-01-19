using UnityEngine;
using DG.Tweening;

public class Player_item : MonoBehaviour
{
    public ItemDataSO data;
    public Transform target;

    public float speed = 5f;
    public float lifetime = 2f;

    public SpriteRenderer rendererRef;

    private bool hasHit = false;

    // SOLID bileşenler
    public PlayerItemMovement moveHandler;
    public PlayerItemDamage damageHandler;
    public PlayerItemDataLoader dataLoader;

    void Awake()
    {
        rendererRef = GetComponent<SpriteRenderer>();

        moveHandler = new PlayerItemMovement(this);
        damageHandler = new PlayerItemDamage(this);
        dataLoader = new PlayerItemDataLoader(this);
    }

    void Start()
    {
        Destroy(gameObject, lifetime);
    }

    public void Load(ItemDataSO newData)
    {
        dataLoader.Load(newData);
    }

    public void SetTarget(Transform t)
    {
        target = t;
        moveHandler.StartTweenToTarget();
    }

    public void TryHitTarget()
    {
        damageHandler.TryHitTarget();
    }

    void OnDestroy()
    {
        moveHandler.KillTweens();
    }
}
