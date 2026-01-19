using DG.Tweening;
using UnityEngine;

public static class PulseEffect
{
    public static void Play(RectTransform rt)
    {
        rt.localScale = Vector3.zero;

        Sequence seq = DOTween.Sequence();

        seq.Append(rt.DOScale(1.15f, 0.22f).SetEase(Ease.OutBack))
           .Append(rt.DOScale(1f, 0.20f).SetEase(Ease.InOutQuad));
    }
}
