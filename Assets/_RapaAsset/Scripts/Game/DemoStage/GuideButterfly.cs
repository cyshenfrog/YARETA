using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.VFX;
using DG.Tweening;

[RequireComponent(typeof(VisualEffect))]
public class GuideButterfly : MonoBehaviour
{
    private VisualEffect VFX;
    public DOTweenAnimation[] TweenAnimation;

    private void Start()
    {
        VFX = GetComponent<VisualEffect>();
    }

    public void FlyAway()
    {
        DOTween.To(() => VFX.GetFloat("Alpha"), x => VFX.SetFloat("Alpha", x), .2f, 6f)
            .SetDelay(1);
        foreach (var item in TweenAnimation)
        {
            item.DOPlay();
        }
    }
}