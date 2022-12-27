using DG.Tweening;
using System;
using UnityEngine;
using UnityEngine.UI;

public class UI_FullScreenFade : UnitySingleton_D<UI_FullScreenFade>
{
    public Image BlackFade;
    public Text Gameover;
    public DOTweenAnimation[] MovieBlack;
    private Action gameoverCallback;

    public void SetMovieMode(bool enable)
    {
        foreach (var item in MovieBlack)
        {
            if (enable)
                item.DOPlayForward();
            else
                item.DOPlayBackwards();
        }
    }

    public void BlackIn(float duration)
    {
        if (DOTween.IsTweening(BlackFade))
            return;
        BlackFade.DOFade(1, duration);
    }

    public void BlackOut(float duration)
    {
        if (DOTween.IsTweening(BlackFade))
            return;
        BlackFade.DOFade(0, duration);
    }

    public void BlackAuto(float FadeTime, float BlackTime, Action OnCompelete = null)
    {
        if (DOTween.IsTweening(BlackFade))
            return;
        BlackFade.DOFade(1, FadeTime)
            .OnComplete(() =>
            {
                BlackFade.DOFade(0, FadeTime)
                    .SetDelay(BlackTime)
                    .OnComplete(() => { OnCompelete?.Invoke(); });
            });
    }

    public void GameoverIn(float duration)
    {
        if (DOTween.IsTweening(Gameover))
            return;
        Gameover.DOFade(1, duration);
    }

    public void GameoverOut(float duration)
    {
        if (DOTween.IsTweening(Gameover))
            return;
        Gameover.DOFade(0, duration);
    }
}