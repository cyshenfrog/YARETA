using DG.Tweening;
using UltEvents;
using UnityEngine;
using UnityEngine.UI;

[RequireComponent(typeof(Image))]
public class DoTweenFillamount : MonoBehaviour
{
    public bool PlayOnEnable;
    public bool RewindOnDisable;
    public Ease ease = Ease.OutQuad;
    public float To;
    public float duration;
    public float delay;
    public LoopType loopType = LoopType.Restart;
    public int loops = 1;
    public bool Relative;
    public UltEvent OnCompelete;
    public UltEvent OnRewind;
    private Image img;
    private float initValue;

    private void OnEnable()
    {
        if (PlayOnEnable)
            DoFill();
    }

    private void OnDisable()
    {
        if (RewindOnDisable)
        {
            img.fillAmount = initValue;
            OnRewind.Invoke();
        }
    }

    // Use this for initialization
    private void Awake()
    {
        img = GetComponent<Image>();
        initValue = img.fillAmount;
    }

    public void DoFill()
    {
        img.DOFillAmount(To, duration)
            .SetLoops(loops, loopType)
            .SetDelay(delay)
            .SetEase(ease)
            .SetRelative(Relative)
            .OnComplete(() => { OnCompelete.Invoke(); });
    }

    public void DoReverse(float duration)
    {
        img.DOFillAmount(initValue, duration)
            .SetLoops(loops, loopType)
            .SetDelay(delay)
            .SetEase(ease)
            .SetRelative(Relative)
            .OnComplete(() => { OnCompelete.Invoke(); });
    }
}