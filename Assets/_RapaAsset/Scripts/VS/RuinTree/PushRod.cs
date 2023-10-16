using DG.Tweening;
using RootMotion.FinalIK;
using System;
using UnityEngine;

public class PushRod : MonoBehaviour
{
    public Transform StandPosBack;
    public Transform StandPosFront;
    public InteractionObject HandBack;
    public InteractionObject HandFront;
    public Action<bool> OnPush;
    public Action OnStop;
    private bool isFront;
    private bool ready;
    private float hSpeed;
    private Tween hSpeedBlend;

    private void Update()
    {
        if (!ready)
            return;
        if (GameInput.GetButtonDown(Actions.Interact))
            Stop();
        if (GameInput.GetButtonDown(Actions.Move))
        {
            if (hSpeedBlend.IsActive())
                hSpeedBlend.Kill();
            hSpeed = 0;
            hSpeedBlend = DOTween.To(() => hSpeed, x => hSpeed = x, 0.1f, 0.2f);
            OnPush?.Invoke(isFront);
        }
        else if (GameInput.GetButtonUp(Actions.Move))
        {
            if (hSpeedBlend.IsActive())
                hSpeedBlend.Kill();
            hSpeedBlend = DOTween.To(() => hSpeed, x => hSpeed = x, 0, 0.2f);
            OnStop?.Invoke();
        }

        Player.Instance.Anim.SetFloat("Speed", hSpeed, .1f, Time.deltaTime);
    }

    public void SetFront(bool front)
    {
        isFront = front;
    }

    public void Ready()
    {
        Player.Instance.WalkTo(isFront ? StandPosFront.position : StandPosBack.position, then);
        void then()
        {
            Player.Instance.transform.parent = transform;
            Player.Instance.transform.DOLookAt(transform.position, 0.5f, AxisConstraint.Y);
            Player_IKManager.Instance.PlayTwoHandIK(isFront ? HandFront : HandBack, null, false);
            ready = true;
        }
    }

    private void Stop()
    {
        ready = false;
        Player.Instance.transform.parent = null;
        Player_IKManager.Instance.ResumeTwoHandIK();
        Player.Instance.Status = PlayerStatus.Moving;
    }
}