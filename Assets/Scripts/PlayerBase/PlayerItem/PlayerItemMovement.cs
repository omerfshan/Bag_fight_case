using UnityEngine;
using DG.Tweening;

public class PlayerItemMovement
{
    private readonly Player_item item;

    private Tween moveTween;
    private Tween rotateTween;

    public PlayerItemMovement(Player_item itemRef)
    {
        item = itemRef;
    }

    public void StartTweenToTarget()
    {
        if (item.target == null) return;

        float distance = Vector3.Distance(item.transform.position, item.target.position);
        float duration = distance / item.speed;

        if (item.data.IsDiagonalThrow)
        {
            float jumpPower = 2f;

            moveTween = item.transform
                .DOJump(item.target.position, jumpPower, 1, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    item.TryHitTarget();
                    Object.Destroy(item.gameObject);
                });
        }
        else
        {
            moveTween = item.transform
                .DOMove(item.target.position, duration)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    item.TryHitTarget();
                    Object.Destroy(item.gameObject);
                });
        }

        rotateTween = item.transform
             .DORotate(new Vector3(0, 0, -360), 0.15f, RotateMode.FastBeyond360)
            .SetLoops(-1)
            .SetEase(Ease.Linear);
    }

    public void KillTweens()
    {
        rotateTween?.Kill();
        moveTween?.Kill();
    }
}
