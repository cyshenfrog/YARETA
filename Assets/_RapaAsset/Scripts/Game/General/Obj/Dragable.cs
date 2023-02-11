using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UltEvents;

public enum DragPose
{
    Left,
    Right,
    Front
}

[RequireComponent(typeof(Tool_TransformFollow))]
public class Dragable : MonoBehaviour
{
    public float TurnSpeed = 0.3f;
    public float MoveSpeed = 0.2f;
    public DragPose DragPose;
    public float BreakForce = 500;
    public Transform LeftHandAnchor;
    public Transform RightHandAnchor;
    public Transform LookPos;
    public Transform StandPos;
    public GameObject[] Model;
    public CameraMode CamMode = CameraMode.Default;
    public MoveMode MoveMode = MoveMode.Aimming;
    public UltEvent OnDragStart;
    public UltEvent OnDragFinish;
    public bool Lock;
    public bool CanSpringAndJump;
    private HingeJoint joint;
    private Tool_TransformFollow follower;
    private bool enable;

    private void Start()
    {
        joint = GetComponent<HingeJoint>();
        follower = GetComponent<Tool_TransformFollow>();
        switch (DragPose)
        {
            case DragPose.Left:
                follower.Target = Player.Instance.LeftDragPos;
                break;

            case DragPose.Right:
                follower.Target = Player.Instance.RightDragPos;
                break;

            case DragPose.Front:
                follower.Target = Player.Instance.FrontDragPos;
                break;

            default:
                break;
        }
    }

    private void Update()
    {
        if (!enable)
            return;
        if (GameInput.GetButtonDown(Actions.Interact))
        {
            FinishDrag();
        }
        if (!joint)
            return;
        if (joint.currentForce.magnitude > BreakForce)
        {
            FinishDrag(0);
        }
    }

    public void StartDrag()
    {
        if (Lock)
            return;
        GetComponentInChildren<Interactable>().enabled = false;
        foreach (var item in Model)
        {
            item.layer = 12;
        }
        Player.Instance.MoveMode = MoveMode.Walking;
        Player.Instance.WalkTo(StandPos, () =>
        {
            Player.Instance.Status = PlayerStatus.Moving;
            Player.Instance.MoveMode = MoveMode;
            Player.Instance.MoveSpeed = MoveSpeed;
            Player.Instance.TurnSpeed = TurnSpeed;
            Player_IKManager.Instance.PlaySimpleIK(LeftHandAnchor, PlayerIK.LeftHand);
            Player_IKManager.Instance.PlaySimpleIK(RightHandAnchor, PlayerIK.RightHand);
            Player_IKManager.Instance.StartLooking(LookPos);
            Player.Instance.transform.DOLookAt(StandPos.position + StandPos.forward, 0.5f, AxisConstraint.Y)
                .OnComplete(() =>
                {
                    StartCoroutine(delay());

                    IEnumerator delay()
                    {
                        yield return new WaitForSeconds(0.5f);
                        UI_General.Instance.ShowActionUI(ButtonAction.PutDown);
                        CameraMain.Instance.SetCameraMode(CamMode);
                        follower.enabled = true;
                        enable = true;
                    }
                    OnDragStart.Invoke();
                });
        });
    }

    public void FinishDrag(float duration = 1)
    {
        GetComponentInChildren<Interactable>().enabled = true;

        UI_General.Instance.CloseActionUI(ButtonAction.PutDown);
        foreach (var item in Model)
        {
            item.layer = 0;
        }
        Player.Instance.Status = PlayerStatus.Wait;
        Player.Instance.MoveMode = MoveMode.Normal;
        Player_IKManager.Instance.ResumeSimpleIK(PlayerIK.LeftHand, duration);
        Player_IKManager.Instance.ResumeSimpleIK(PlayerIK.RightHand, duration);
        Player_IKManager.Instance.StopLooking();
        CameraMain.Instance.SetCameraMode(CameraMode.Default);
        follower.enabled = false;
        enable = false;
        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(0.5f);
            Player.Instance.Status = PlayerStatus.Moving;
            Player.Instance.MoveMode = MoveMode.Normal;
            Player.Instance.MoveSpeed = 1;
            Player.Instance.TurnSpeed = 3;
        }
        OnDragFinish.Invoke();
    }
}