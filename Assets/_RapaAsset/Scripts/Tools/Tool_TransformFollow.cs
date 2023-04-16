using UnityEngine;
using NaughtyAttributes;
using DG.Tweening;

public enum SmoothMoveType
{
    Lerp,
    SmoothDamp,
}

public enum UpdateType
{
    Update,
    LateUpdate,
    FixedUpdate
}

public class Tool_TransformFollow : MonoBehaviour
{
    public UpdateType UpdateType;
    public SmoothMoveType SmoothType;

    [SerializeField]
    private bool _FollowPlayer;

    public bool FollowPlayer
    {
        get { return _FollowPlayer; }
        set
        {
            _FollowPlayer = value;
            if (value)
            {
                Target = Player.Instance.transform;
            }
        }
    }

    [HideIf("FollowPlayer")]
    public Transform Target;

    public bool Snap = true;
    public bool LookAtTarget;

    [BoxGroup("Follow Speed")]
    [HideIf("Snap")]
    public float SmoothMoveTime = 1;

    [BoxGroup("Follow Speed")]
    [HideIf("Snap")]
    public float SmoothRotTime = 1;

    public bool FollowPos;

    [BoxGroup("Position Follow")]
    [ShowIf("FollowPos")]
    public bool FollowPos_X = true;

    [BoxGroup("Position Follow")]
    [ShowIf("FollowPos")]
    public bool FollowPos_Y = true;

    [BoxGroup("Position Follow")]
    [ShowIf("FollowPos")]
    public bool FollowPos_Z = true;

    [BoxGroup("Position Follow")]
    [ShowIf("FollowPos")]
    public bool LocalShift;

    [BoxGroup("Position Follow")]
    [ShowIf("FollowPos")]
    public Vector3 PosOffset;

    public bool FollowRot;
    private Vector3 speed;

    public Transform SetTarget
    { set { Target = value; } }
    public bool SetFollowPos
    { set { FollowPos = value; } }
    public bool SetFollowRot
    { set { FollowRot = value; } }
    public bool SetLocalShift
    { set { LocalShift = value; } }
    public bool SetSnap
    { set { Snap = value; } }

    private Tween mt;

    public float SetSmoothMoveTime
    {
        set
        {
            mt = DOTween.To(() => SmoothMoveTime, x => SmoothMoveTime = x, value, 1);
        }
    }

    private Tween rt;

    public float SetSmoothRotTime
    {
        set
        {
            rt = DOTween.To(() => SmoothRotTime, x => SmoothRotTime = x, value, 1);
        }
    }

    public Vector3 SetPosOffset
    { set { PosOffset = value; } }

    private void Start()
    {
        if (FollowPlayer)
        {
            Target = Player.Instance.transform;
        }
    }

    private void FixedUpdate()
    {
        if (UpdateType != global::UpdateType.FixedUpdate)
            return;
        SmoothUpdate();
    }

    private void Update()
    {
        if (UpdateType != global::UpdateType.LateUpdate)
            return;
        SmoothUpdate();
    }

    private void LateUpdate()
    {
        if (UpdateType != global::UpdateType.Update)
            return;
        SmoothUpdate();
    }

    // Update is called once per frame
    private void SmoothUpdate()
    {
        if (!Target)
            return;
        if (!FollowPos && !FollowRot)
            return;
        if (FollowPos)
        {
            switch (SmoothType)
            {
                case SmoothMoveType.Lerp:
                    transform.position = Snap ? TargetPos : Vector3.Slerp(transform.position, TargetPos, Time.deltaTime / SmoothMoveTime);
                    break;

                case SmoothMoveType.SmoothDamp:
                    transform.position = Snap ? TargetPos : Vector3.SmoothDamp(transform.position, TargetPos, ref speed, SmoothMoveTime);
                    break;

                default:
                    break;
            }
        }
        if (LookAtTarget)
        {
            transform.rotation = Quaternion.LookRotation(TargetPos - transform.position);
        }
        else if (FollowRot)
        {
            transform.rotation = Snap ? Target.rotation : Quaternion.Slerp(transform.rotation, Target.rotation, Time.deltaTime / SmoothRotTime);
        }
    }

    private Vector3 targetPos;

    public Vector3 TargetPos
    {
        get
        {
            if (LocalShift)
                targetPos = Target.TransformPoint(Target.localPosition + PosOffset);
            else
                targetPos = Target.position + PosOffset;
            targetPos.Set(FollowPos_X ? targetPos.x : transform.position.x, FollowPos_Y ? targetPos.y : transform.position.y, FollowPos_Z ? targetPos.z : transform.position.z);

            return targetPos;
        }
    }
}