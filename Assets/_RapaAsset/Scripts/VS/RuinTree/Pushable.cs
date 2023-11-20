using DG.Tweening;
using RootMotion.FinalIK;
using System;
using UnityEngine;

public class Pushable : MonoBehaviour
{
    public Transform StandPos;
    public InteractionObject HandAnchor;
    public Action OnPush;
    public Action OnStop;
    public GameObject PushingCam;
    private bool ready;
    private float hSpeed;
    private Tween hSpeedBlend;

    private void Update()
    {
        if (!ready)
            return;
        if (GameInput.Keyboard.GetKeyDown(KeyCode.F2))
        {
            PushingCam.SetActive(!PushingCam.activeSelf);
        }
        if (GameInput.GetButtonDown(Actions.Interact))
            Stop();
        if (GameInput.GetButtonDown(Actions.Move))
        {
            if (hSpeedBlend.IsActive())
                hSpeedBlend.Kill();
            hSpeed = 0;
            hSpeedBlend = DOTween.To(() => hSpeed, x => hSpeed = x, 0.1f, 0.2f);
            OnPush?.Invoke();
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

    public void Ready()
    {
        Player.Instance.WalkTo(StandPos, then);
        void then()
        {
            Player.Instance.transform.parent = transform;
            Player.Instance.transform.DOLookAt(transform.position, 0.5f, AxisConstraint.Y);
            Player_IKManager.Instance.PlayTwoHandIK(HandAnchor, null, false);
            ready = true;
        }
    }

    public void Stop()
    {
        if (!ready)
            return;
        if (hSpeedBlend.IsActive())
            hSpeedBlend.Kill();

        Player.Instance.Anim.SetFloat("Speed", 0);
        ready = false;
        Player.Instance.transform.parent = null;
        Player_IKManager.Instance.ResumeTwoHandIK();
        Player.Instance.Status = PlayerStatus.Moving;
        PushingCam.SetActive(false);
    }
}