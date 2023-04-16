using System;
using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;
using UnityEngine.Events;

public class UI_ButtonBlink : MonoBehaviour
{
    public Actions Buttons;
    public bool SelfUpdate = true;
    public const float Duration = 0.1f;
    public Image Icon;
    public Image Fill;
    public Image Head;
    public Text Name;
    public Text HoldText;
    public bool Hold;

    public void SetHold(bool b)
    {
        if (Hold == b)
            return;
        Hold = b;
        HoldText.gameObject.SetActive(b);
    }

    public Action OnBlinkFinish;
    public UnityEvent Event;
    private Vector2 original;

    private void Start()
    {
        enabled = SelfUpdate;
        if (HoldText)
            HoldText.gameObject.SetActive(Hold);
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameInput.GetButtonDown(Buttons))
        {
            Press();
        }
        if (Hold)
        {
            if (GameInput.GetButtonUp(Buttons))
            {
                Init();
            }
        }
    }

    public void Press()
    {
        if (Hold)
        {
            Fill.DOFillAmount(1, 0.8f)
                .SetEase(Ease.Linear)
                .OnComplete(() =>
                {
                    Invoke();
                });
        }
        else
        {
            Fill.fillAmount = 1;
            Delay.Instance.Wait(Duration, () =>
            {
                Invoke();
            });
        }
        if (Icon)
            Icon.color = Color.black;
        if (Head)
            Head.color = Color.black;
        if (Name)
            Name.color = Color.black;
        if (HoldText)
            HoldText.color = Color.black;
    }

    public void Init()
    {
        Fill.DOKill();
        Fill.fillAmount = 0;
        if (Icon)
            Icon.color = Color.white;
        if (Head)
            Head.color = Color.white;
        if (Name)
            Name.color = Color.white;
        if (HoldText)
            HoldText.color = Color.white;
    }

    private void Invoke()
    {
        Event.Invoke();
        OnBlinkFinish?.Invoke();
        OnBlinkFinish = null;
        Init();
    }
}