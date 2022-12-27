using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using RootMotion.FinalIK;
using UnityEngine;

public enum PlayerIK
{
    Body,
    RightHand,
    LeftHand,
    RightFoot,
    LeftFoot,
    Lenth,
}

public enum IKReachType
{
    Position,
    Roataion,
    Both
}

public class Player_IKManager : UnitySingleton_D<Player_IKManager>
{
    public FullBodyBipedIK BodyIK;
    public LookAtIK LookIK;
    public InteractionSystem interactionSystem;
    public Animator IKAnimator;
    public Transform TwoHandHoldPosition;
    public Transform RightHandRef;
    public Transform LeftHandRef;
    public Transform RightFootRef;
    public Transform LeftFootRef;
    public Transform BodyRef;
    public Transform HeadRef;
    public float pickUpTime = 0.3f;
    private float holdWeight, holdWeightVel;
    private Vector3 pickUpPosition;
    private Quaternion pickUpRotation;

    private Tween[] simpleIKTween = new Tween[(int)PlayerIK.Lenth];
    private IKEffector[] simpleIKeffector = new IKEffector[(int)PlayerIK.Lenth];

    private void Start()
    {
        for (int i = 0; i < (int)PlayerIK.Lenth; i++)
        {
            IKRegistion((PlayerIK)i);
        }
    }

    // Are we currently holding the object?
    private bool holding
    {
        get
        {
            return interactionSystem.IsPaused(FullBodyBipedEffector.LeftHand);
        }
    }

    public IKEffector GetIKEffector(PlayerIK TargetIK)
    {
        return simpleIKeffector[(int)TargetIK];
    }

    public void StartLooking(Vector3 pos)
    {
        HeadRef.position = pos;
        StartLooking();
    }

    public void StartLooking(Transform target)
    {
        LookIK.solver.target = target;
        LookIK.solver.IKPositionWeight = 1;
    }

    public void StartLooking()
    {
        LookIK.solver.target = HeadRef;
        LookIK.solver.IKPositionWeight = 1;
    }

    public void StopLooking()
    {
        DOTween.To(() => LookIK.solver.IKPositionWeight, x => LookIK.solver.IKPositionWeight = x, 0, 1);
        //LookIK.solver.IKPositionWeight = 0;
    }

    public void RotatePivot(Transform obj, Transform pivot, float duration = .2f, float delay = 0, Ease ease = Ease.Linear)
    {
        float dist = (TwoHandHoldPosition.position - obj.position).magnitude;

        Vector3 AimPoint = TwoHandHoldPosition.position + transform.forward * dist;
        AimPoint -= AimPoint.y * Vector3.up;
        Vector3 dir = AimPoint - TwoHandHoldPosition.position;
        pivot.DORotateQuaternion(Quaternion.LookRotation(dir), duration)
            .SetDelay(delay)
            .SetEase(ease);
    }

    public void RotatePivot(Transform obj, Transform pivot)
    {
        float dist = (TwoHandHoldPosition.position - obj.position).magnitude;

        Vector3 AimPoint = TwoHandHoldPosition.position + transform.forward * dist;
        AimPoint -= AimPoint.y * Vector3.up;
        Vector3 dir = AimPoint - TwoHandHoldPosition.position;
        pivot.rotation = Quaternion.LookRotation(dir);
    }

    public void PlaySimpleIK(PlayerIK type, float blendTime = 1, Ease easeType = Ease.Linear, IKReachType reachType = IKReachType.Both)
    {
        switch (type)
        {
            case PlayerIK.Body:
                BodyRef.position = BodyIK.references.pelvis.position;
                PlaySimpleIK(BodyRef, type, blendTime, easeType, reachType);
                break;

            case PlayerIK.RightHand:
                RightHandRef.position = BodyIK.references.rightHand.position;
                PlaySimpleIK(RightHandRef, type, blendTime, easeType, reachType);
                break;

            case PlayerIK.LeftHand:
                LeftHandRef.position = BodyIK.references.leftHand.position;
                PlaySimpleIK(LeftHandRef, type, blendTime, easeType, reachType);
                break;

            case PlayerIK.RightFoot:
                RightFootRef.position = BodyIK.references.rightFoot.position;
                PlaySimpleIK(RightFootRef, type, blendTime, easeType, reachType);
                break;

            case PlayerIK.LeftFoot:
                LeftFootRef.transform.position = BodyIK.references.leftFoot.position;
                PlaySimpleIK(LeftFootRef, type, blendTime, easeType, reachType);
                break;

            default:
                return;
        }
    }

    public void PlaySimpleIK(Transform target, PlayerIK type, float blendTime = 1, Ease easeType = Ease.Linear, IKReachType reachType = IKReachType.Both, float level = 1)
    {
        StopAllCoroutines();
        simpleIKeffector[(int)type].target = target;
        simpleIKTween[(int)type].Kill();

        switch (reachType)
        {
            case IKReachType.Position:
                simpleIKTween[(int)type] = DOTween.To(() => simpleIKeffector[(int)type].positionWeight, x => simpleIKeffector[(int)type].positionWeight = x, level, blendTime)
                    .SetEase(easeType);
                break;

            case IKReachType.Roataion:
                simpleIKTween[(int)type] = DOTween.To(() => simpleIKeffector[(int)type].rotationWeight, x => simpleIKeffector[(int)type].rotationWeight = x, level, blendTime)
                    .SetEase(easeType);
                break;

            case IKReachType.Both:
            default:
                DOTween.To(() => simpleIKeffector[(int)type].rotationWeight, x => simpleIKeffector[(int)type].rotationWeight = x, level, blendTime)
                    .SetEase(easeType);
                simpleIKTween[(int)type] = DOTween.To(() => simpleIKeffector[(int)type].positionWeight, x => simpleIKeffector[(int)type].positionWeight = x, level, blendTime)
                    .SetEase(easeType);
                break;
        }
    }

    private void IKRegistion(PlayerIK type)
    {
        switch (type)
        {
            case PlayerIK.Body:
                simpleIKeffector[(int)type] = BodyIK.solver.bodyEffector;
                break;

            case PlayerIK.RightHand:
                simpleIKeffector[(int)type] = BodyIK.solver.rightHandEffector;
                break;

            case PlayerIK.LeftHand:
                simpleIKeffector[(int)type] = BodyIK.solver.leftHandEffector;
                break;

            case PlayerIK.RightFoot:
                simpleIKeffector[(int)type] = BodyIK.solver.rightFootEffector;
                break;

            case PlayerIK.LeftFoot:
                simpleIKeffector[(int)type] = BodyIK.solver.leftFootEffector;
                break;

            default:
                return;
        }
    }

    public void ResumeSimpleIK(PlayerIK type, float blendTime = 1, Ease easeType = Ease.Linear, Action onFinish = null)
    {
        StartCoroutine(_ResumeSimpleIK(type, blendTime, easeType, onFinish));
    }

    public IEnumerator _ResumeSimpleIK(PlayerIK type, float blendTime = 1, Ease easeType = Ease.Linear, Action onFinish = null)
    {
        simpleIKTween[(int)type].Kill();
        DOTween.To(() => simpleIKeffector[(int)type].rotationWeight, x => simpleIKeffector[(int)type].rotationWeight = x, 0, blendTime)
            .SetEase(easeType);
        simpleIKTween[(int)type] = DOTween.To(() => simpleIKeffector[(int)type].positionWeight, x => simpleIKeffector[(int)type].positionWeight = x, 0, blendTime)
            .SetEase(easeType);
        yield return new WaitForSeconds(blendTime);
        simpleIKeffector[(int)type].target = null;
        onFinish?.Invoke();
    }

    public void PlayTwoHandIK(InteractionObject obj, Action onCompelete = null, bool take = true)
    {
        StopAllCoroutines();
        if (!holding)
        {
            StartCoroutine(_PlayTwoHandIK(obj, onCompelete, take));
        }
        else
        {
            ResumeTwoHandIK();
        }
    }

    public IEnumerator _PlayTwoHandIK(InteractionObject obj, Action onCompelete = null, bool take = true)
    {
        interactionSystem.StartInteraction(FullBodyBipedEffector.LeftHand, obj, false);
        interactionSystem.StartInteraction(FullBodyBipedEffector.RightHand, obj, false);
        yield return new WaitForSeconds(interactionSystem.fadeInTime);
        if (take)
        {
            obj.transform.DOKill();
            obj.transform.DOMove(TwoHandHoldPosition.position, interactionSystem.fadeInTime);
        }

        yield return new WaitForSeconds(interactionSystem.fadeInTime);

        onCompelete?.Invoke();
    }

    public IEnumerator _IK_Resume(Action onCompelete = null)
    {
        interactionSystem.StopAll();
        interactionSystem.ResumeAll();
        yield return new WaitForSeconds(0.2f);
        interactionSystem.StopAll();
        yield return new WaitForSeconds(0.3f);
        onCompelete?.Invoke();
    }

    public void ResumeTwoHandIK(Action onCompelete = null)
    {
        StopAllCoroutines();
        StartCoroutine(_IK_Resume(onCompelete));
    }

    public void StopTwoHandIK()
    {
        interactionSystem.StopAll();
    }
}