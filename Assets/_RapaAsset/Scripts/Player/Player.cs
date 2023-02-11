using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;

public enum PlayerStatus
{
    Wait,
    Moving,
    Static,
    Climbing
}

public enum MoveMode
{
    Normal,
    Aimming,
    Walking,
}

[RequireComponent(typeof(CharacterController))]
public class Player : UnitySingleton_D<Player>
{
    #region Compenents

    public static Transform FacingTarget;
    public CharacterController characterController;
    public Animator Anim;
    public GameObject[] Scanners;
    public GameObject Rock;
    public GameObject Egg;
    public GameObject Hammer;
    public GameObject DrawModel;
    public GameObject FPCam;
    public GameObject Model;
    public Transform LeftDragPos;
    public Transform RightDragPos;
    public Transform FrontDragPos;
    public PlayerTrigger PlayerTrigger;
    [SerializeField] private GameObject Pad;
    [SerializeField] private Transform MenuHand;
    [SerializeField] private Transform DrawingHand;
    [SerializeField] private ParticleSystem WalkFX;
    [SerializeField] private GameObject DrownFX;

    protected Transform ScanHandTracker_R
    { get { return Player_IKManager.Instance.RightHandRef; } }

    protected Transform ScanHandTracker_L
    { get { return Player_IKManager.Instance.LeftHandRef; } }

    #endregion Compenents

    #region Setting

    public float MoveSpeed = 1;
    public float TurnSpeed = 10f;
    public float AirControl = 1;
    public float AirDrag = 2.5f;
    public float JumpPower = 5;
    public float RocketJumpPower;

    #endregion Setting

    #region StateControl

    public bool IsJumping;
    private bool running;

    private bool grounded; private bool Grounded
    {
        get { return grounded; }
        set
        {
            if (grounded != value)
                OnGround();
            grounded = value;
        }
    }

    public bool PuzzleMode;

    private MoveMode moveMode; public MoveMode MoveMode
    {
        get { return moveMode; }
        set
        {
            moveMode = value;
            switch (value)
            {
                case MoveMode.Aimming:
                    Anim.SetBool("Aiming", true);
                    Anim.SetBool("isSprinting", false);
                    break;

                case MoveMode.Walking:
                    Anim.SetBool("isSprinting", false);
                    break;

                default:
                    Anim.SetBool("Aiming", false);
                    break;
            }
        }
    }

    [SerializeField] private PlayerStatus status = PlayerStatus.Wait;

    public PlayerStatus Status

    {
        get => status;
        set
        {
            if (value == status)
                return;
            status = value;
            OnStateChanged(status);
        }
    }

    public virtual void OnStateChanged(PlayerStatus status)
    {
        switch (status)
        {
            case PlayerStatus.Climbing:
            case PlayerStatus.Static:
            case PlayerStatus.Wait:
                PlayerTrigger.enabled = false;
                if (!speedTween.IsActive())
                    Anim.SetFloat("Speed", 0);
                Anim.SetBool("isSprinting", false);
                break;

            case PlayerStatus.Moving:
                PlayerTrigger.enabled = true;
                //PlayerTrigger.Rescan();
                break;
        }
        if (status == PlayerStatus.Static || status == PlayerStatus.Climbing)
        {
            characterController.enabled = false;
        }
        else
        {
            characterController.enabled = true;
        }
    }

    #endregion StateControl

    #region CalculateTemp

    private const float MAX_MOVE_SPEED = 4f;
    private float fallVelocity;
    private float moveVelocity;
    private float rotationDiff;
    private RaycastHit climbHitInfo;
    private Quaternion targetRotation;
    private Quaternion freeRotation;
    private Tween speedTween;
    private Tween crouchTween;
    private Tween initFacing;
    private Tween hSpeedBlend;
    private Vector3 targetDirection;
    private Vector3 forward;
    private Vector3 right;

    #endregion CalculateTemp

    #region PlayerControl

    public virtual void Update()
    {
        // Always execute
        GroundCheck();
        GravityUpdate();
        DefaultAnimUpdate();

        // Execute by status
        switch (Status)
        {
            case PlayerStatus.Wait:
                break;

            case PlayerStatus.Moving:
                ClimbCheck();
                MovementControl();
                UpdateRotation();
                ActionControl();
                LocomoationAnimUpdate();
                break;

            case PlayerStatus.Static:
                break;

            case PlayerStatus.Climbing:
                ClimbControl(GameInput.Move);
                break;

            default:
                break;
        }
    }

    private void ClimbControl(Vector2 input)
    {
        //// Check walls in a cross pattern
        //Vector3 offset = transform.TransformDirection(Vector2.one * 0.5f);
        //Vector3 checkDirection = Vector3.zero;
        //int k = 0;
        //for (int i = 0; i < 4; i++)
        //{
        //    RaycastHit checkHit;
        //    if (Physics.Raycast(transform.position + offset,
        //                        transform.forward,
        //                        out checkHit))
        //    {
        //        checkDirection += checkHit.normal;
        //        k++;
        //    }
        //    // Rotate Offset by 90 degrees
        //    offset = Quaternion.AngleAxis(90f, transform.forward) * offset;
        //}
        //checkDirection /= k;

        //float dot = Vector3.Dot(transform.forward, -climbHitInfo.normal);

        //transform.position = climbHitInfo.point + climbHitInfo.normal * 0.05f;
        transform.forward = Vector3.Lerp(transform.forward,
            -climbHitInfo.normal,
            10f * Time.fixedDeltaTime);
        //(-input.x * transform.right + input.y * transform.up)
        transform.Translate(input * Time.deltaTime, Space.Self);
    }

    private void ClimbCheck()
    {
        //爬牆檢測
        if (Physics.Raycast(transform.position, transform.forward, out climbHitInfo, .6f, 1 << 0, QueryTriggerInteraction.Ignore))
        {
            if (Vector3.Angle(climbHitInfo.normal, Vector3.up) < 45)
                Status = PlayerStatus.Moving;
            else
            {
                Status = PlayerStatus.Climbing;
                IsJumping = false;
            }
        }
        else if (Status == PlayerStatus.Climbing)
            Status = PlayerStatus.Moving;
    }

    private void GroundCheck()
    {
        //落地檢測
        Grounded = Physics.SphereCast(transform.position + Vector3.up * 0.6f, 0.5f, Vector3.down, out _, 0.5f, 1 << 0, QueryTriggerInteraction.Ignore);

        //落地特效、動畫
        WalkFX.enableEmission = Grounded;
    }

    private void GravityUpdate()
    {
        if (!Grounded && !IsJumping && fallVelocity > -10)
        {
            fallVelocity -= (IsJumping ? 0 : 10) * Time.deltaTime;
        }

        //if (IsJumping)
        //{
        //    VirticleVelocity -= Vector3.up * currentDrag * Time.deltaTime;
        //    if (VirticleVelocity.y < 0)
        //        currentDrag = Mathf.Lerp(currentDrag, AirDrag, Time.deltaTime * 4);
        //}
    }

    private void MovementControl()
    {
        switch (MoveMode)
        {
            case MoveMode.Normal:
                if (GameInput.GetButtonDown(Actions.ToggleRun))
                    running = !running;
                if (GameInput.GetButtonDown(Actions.Run))
                    running = true;
                else if (GameInput.GetButtonUp(Actions.Run))
                    running = false;
                if (GameInput.GetButtonDown(Actions.Jump))
                    Jump();
                break;

            case MoveMode.Walking:
            case MoveMode.Aimming:
            default:
                break;
        }

        if (GameInput.GetButtonDown(Actions.Move))
        {
            initFacing = transform.DOLookAt(transform.position + GameInput.MovementCameraSpace, 0.2f, AxisConstraint.Y);
            if (hSpeedBlend.IsActive())
                hSpeedBlend.Kill();
            moveVelocity = 0;
            hSpeedBlend = DOTween.To(() => moveVelocity, x => moveVelocity = x, MAX_MOVE_SPEED, 0.2f);
        }
        else if (GameInput.GetButtonUp(Actions.Move))
        {
            if (hSpeedBlend.IsActive())
                hSpeedBlend.Kill();
            hSpeedBlend = DOTween.To(() => moveVelocity, x => moveVelocity = x, 0, 0.2f);
        }

        if (characterController.enabled)
            characterController.Move((GameInput.MovementCameraSpace.normalized * moveVelocity * (running && Grounded ? 2 : 1) + Vector3.up * fallVelocity) * MoveSpeed * Time.deltaTime);
    }

    private void ActionControl()
    {
        //Air and grounded

        if (GameRef.CarringObj)
        {
            if (GameInput.GetButtonDown(Actions.Interact) && !GameRef.CarringObj.DontDrop)
            {
                GameRef.CarringObj.Drop();
                return;
            }
        }

        //Ground only

        if (!Grounded)
            return;

        if (PlayerTrigger.NearestObj)
        {
            if (GameInput.GetButtonDown(PlayerTrigger.NearestObj.InteractButton))
                PlayerTrigger.Interact();
        }

        if (SaveDataManager.TutorialPassed)
        {
            if (GameInput.GetButtonDown(Actions.Menu) && !PuzzleMode)
                OpenMenu();
            if (GameInput.GetButtonDown(Actions.DrawMode))
                DrawingMode.Instance.StartDrawingMode();
        }
    }

    private void UpdateRotation()
    {
        if (initFacing.IsActive() || MoveMode == MoveMode.Aimming)
            return;

        if (FacingTarget)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(FacingTarget.position - transform.position, Vector3.up), Time.deltaTime * 5);
        else
        {
            CalcTargetDirection();
            if (GameInput.IsMove && targetDirection.magnitude > 0.1f)
            {
                freeRotation = Quaternion.LookRotation(targetDirection.normalized, transform.up);
                rotationDiff = freeRotation.eulerAngles.y - transform.eulerAngles.y;
                targetRotation = Quaternion.Euler(new Vector3(0, rotationDiff != 0 ? freeRotation.eulerAngles.y : transform.eulerAngles.y, 0));
                transform.rotation = Quaternion.Slerp(transform.rotation, targetRotation, TurnSpeed * Time.deltaTime);
                if (MathF.Abs(rotationDiff) > 5)
                    Anim.SetFloat("Direction", IsClockwise(transform.rotation, targetRotation, Vector3.up) ? 1 : -1, .2f, Time.deltaTime);
                else
                    Anim.SetFloat("Direction", 0, .1f, Time.deltaTime);
            }
            else
                Anim.SetFloat("Direction", 0, .1f, Time.deltaTime);
        }
    }

    private void CalcTargetDirection()
    {
        right = GameRef.MainCam.transform.TransformDirection(Vector3.right);
        forward = GameRef.MainCam.transform.TransformDirection(Vector3.forward);
        forward.y = 0;

        targetDirection = GameInput.Move.x * right + GameInput.Move.y * forward;
    }

    private void OnGround()
    {
        if (Status == PlayerStatus.Climbing)
            Status = PlayerStatus.Moving;
    }

    public void OpenMenu()
    {
        StartCoroutine(_OpenMenu());
    }

    private IEnumerator _OpenMenu()
    {
        Player_IKManager.Instance.LeftHandRef.localPosition = new Vector3(0, 1, 0.2f);
        PadUp();
        enabled = false;
        Status = PlayerStatus.Wait;
        yield return new WaitForSeconds(.2f);
        UI_ScanMechine.Instance.Open(() =>
        {
            enabled = true;
            Status = PlayerStatus.Moving;
            PadDown();
        });
    }

    private void Jump()
    {
        Anim.SetTrigger("Jump");
        if (GameInput.IsMove)
            fallVelocity = JumpPower;
    }

    public void OnJumpFinish()
    {
        IsJumping = false;
    }

    #endregion PlayerControl

    #region Animation

    private void DefaultAnimUpdate()
    {
        Anim.SetBool("Grounded", Grounded);
    }

    private void LocomoationAnimUpdate()
    {
        Anim.SetFloat("Speed", moveVelocity / MAX_MOVE_SPEED * MoveSpeed, .1f, Time.deltaTime);
        Anim.SetFloat("MoveX", GameInput.Move.x);
        Anim.SetFloat("MoveY", GameInput.Move.y);

        if (GameInput.IsMove)
            Anim.SetBool("isSprinting", running);
        else
            Anim.SetBool("isSprinting", false);
    }

    public void RightHandDrawing()
    {
        DrawingHand.DOLocalMove(new Vector3(-.22f, 0, -.05f), .2f)
            .SetRelative(true)
            .OnComplete(() =>
            {
                DrawingHand.DOLocalMove(new Vector3(.05f, 0, .05f), .1f)
                    .SetEase(Ease.Linear)
                    .SetLoops(4, LoopType.Yoyo)
                    .SetRelative(true)
                    .OnComplete(() =>
                    {
                        DrawingHand.DOLocalMove(new Vector3(.22f, 0, .05f), .2f)
                            .SetRelative(true);
                    });
            });
        //Player_IKManager.Instance.PlaySimpleIK(DrawingHand, PlayerIK.RightHand, .5f);
        //StartCoroutine(d());

        //IEnumerator d()
        //{
        //    yield return new WaitForSeconds(.3f);
        //    DrawingHand.DOLocalMoveX(-.2f, .2f)
        //        .SetLoops(4, LoopType.Yoyo)
        //        .SetRelative(true);
        //    yield return new WaitForSeconds(.7f);
        //    Player_IKManager.Instance.ResumeSimpleIK(PlayerIK.RightHand, .5f);
        //}
    }

    public void PadUp()
    {
        Pad.SetActive(true);
        Player_IKManager.Instance.PlaySimpleIK(MenuHand, PlayerIK.LeftHand, .5f);
    }

    public void PadDown()
    {
        Player_IKManager.Instance.ResumeSimpleIK(PlayerIK.LeftHand, .5f, Ease.Linear, () =>
        {
            Pad.SetActive(false);
        });
    }

    public void OnDeepWater()
    {
        if (Status == PlayerStatus.Wait)
            return;
        if (!Grounded)
            Drown();
        else
            WalkBack();
    }

    private void Drown()
    {
        Status = PlayerStatus.Wait;
        SEManager.Instance.PlaySystemSE(UnityEngine.Random.value > 0.5f ? SystemSE.進水1 : SystemSE.進水2);
        Anim.SetTrigger("Drown");
        DrownFX.SetActive(false);
        DrownFX.SetActive(true);
        StartCoroutine(d());

        IEnumerator d()
        {
            yield return new WaitForSeconds(.5f);
            Die(() => { Anim.SetTrigger("Resume"); });
        }
    }

    public void Die(Action OnBlack = null, bool ShowGameover = false)
    {
        Status = PlayerStatus.Static;
        UI_FullScreenFade.Instance.BlackIn(1);
        StartCoroutine(d());
        IEnumerator d()
        {
            yield return new WaitForSeconds(1f);
            if (ShowGameover)
                UI_FullScreenFade.Instance.GameoverIn(1);
            OnBlack?.Invoke();
            transform.position = PrototypeMain.Instance.RespawnPos;
            //transform.SetPositionAndRotation(PrototypeMain.Instance.CheckPoints[PlayerPrefs.GetInt("CheckPoint", 1)].position, PrototypeMain.Instance.CheckPoints[PlayerPrefs.GetInt("CheckPoint", 1)].rotation);

            if (ShowGameover)
            {
                yield return new WaitForSeconds(1.5f);
                UI_FullScreenFade.Instance.GameoverOut(1);
            }
            yield return new WaitForSeconds(1f);
            SEManager.Instance.ResetWalkSE();
            UI_FullScreenFade.Instance.BlackOut(1);
            CameraMain.Instance.Recenter(2f);
            Status = PlayerStatus.Moving;
        }
    }

    public void WalkBack(float speed = 0.2f, float duration = 1)
    {
        StartCoroutine(_WalkBack());

        IEnumerator _WalkBack()
        {
            Anim.SetBool("isSprinting", false);
            MoveMode = MoveMode.Walking;
            Status = PlayerStatus.Wait;
            Anim.SetFloat("Speed", -speed);
            yield return new WaitForSeconds(duration);
            Status = PlayerStatus.Moving;
            MoveMode = MoveMode.Normal;
        }
    }

    public void WalkTo(Transform target)
    {
        WalkTo(target.position, null);
    }

    public void WalkTo(Transform target, Action onCompelete = null)
    {
        WalkTo(target.position, onCompelete);
    }

    public void WalkTo(Vector3 targetPos, Action onCompelete = null, bool keepHeight = true)
    {
        Status = PlayerStatus.Static;
        transform.DOKill();
        if (keepHeight)
            targetPos.y = transform.position.y;
        if (Vector3.Dot(transform.forward, targetPos - transform.position) > 0)
        {
            transform.DOLookAt(targetPos, 0.5f, AxisConstraint.Y);
            Anim.SetFloat("Speed", 0.2f);
        }
        else
        {
            transform.DOLookAt(transform.position * 2 - targetPos, 0.5f, AxisConstraint.Y);
            Anim.SetFloat("Speed", -0.2f);
        }
        transform.DOMove(targetPos, (targetPos - transform.position).magnitude * 0.6f)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                transform.position = targetPos;
                LerpSpeed(0);
                onCompelete?.Invoke();
            });
    }

    public void LerpWalkTo(Transform target, float Speed = 0.2f)
    {
        LerpWalkTo(target.position, null, Speed);
    }

    public void LerpWalkTo(Transform target, Action onCompelete = null, float Speed = 0.2f)
    {
        LerpWalkTo(target.position, onCompelete, Speed);
    }

    public void LerpWalkTo(Vector3 targetPos, Action onCompelete = null, float Speed = 0.2f)
    {
        Status = PlayerStatus.Wait;

        StartCoroutine(delay());

        transform.DOKill();
        IEnumerator delay()
        {
            yield return null;
            if (Vector3.Dot(transform.forward, targetPos - transform.position) > 0)
            {
                StartCoroutine(_WalkTo(targetPos, targetPos, onCompelete));
                Anim.SetFloat("Speed", Speed);
            }
            else
            {
                StartCoroutine(_WalkTo(targetPos, transform.position * 2 - targetPos, onCompelete));
                Anim.SetFloat("Speed", -Speed);
            }

            IEnumerator _WalkTo(Vector3 _targetPos, Vector3 _lookPos, Action _onCompelete = null)
            {
                float speed = 5 - (_targetPos - transform.position).magnitude;
                Vector3 direction;
                Quaternion toRotation;
                while ((_targetPos - transform.position).magnitude > 1f)
                {
                    direction = _lookPos - transform.position;
                    toRotation = Quaternion.LookRotation(direction, Vector3.up);
                    transform.rotation = Quaternion.Lerp(transform.rotation, toRotation, speed * Time.deltaTime);
                    yield return null;
                }

                LerpSpeed(0, 1, 0, _onCompelete);
            }
        }
    }

    public void TakeRodAnim()
    {
        Anim.SetTrigger("Equip");
    }

    public void LerpSpeed(float to, float duration = 1, float delay = 0, Action onCompelete = null)
    {
        speedTween.Kill();
        speedTween = DOTween.To(() => Anim.GetFloat("Speed"), x => Anim.SetFloat("Speed", x), to, duration)
            .SetDelay(delay)
            .OnComplete(() => onCompelete?.Invoke());
    }

    public void StartCrouch()
    {
        crouchTween.Kill();
        crouchTween = DOTween.To(() => Anim.GetFloat("CrouchLevel"), x => Anim.SetFloat("CrouchLevel", x), 1, 1f);
        CameraMain.Instance.SetCameraMode(CameraMode.Aim);
        Anim.SetBool("isSprinting", false);
        MoveMode = MoveMode.Walking;
    }

    public void EndCrouch()
    {
        crouchTween.Kill();
        crouchTween = DOTween.To(() => Anim.GetFloat("CrouchLevel"), x => Anim.SetFloat("CrouchLevel", x), 0, 1f);
        CameraMain.Instance.SetCameraMode(CameraMode.Default);
        MoveMode = MoveMode.Normal;
    }

    public virtual void StartScan()
    {
    }

    public virtual void StopScan()
    {
    }

    public void SlowLookAt(Transform Target, float Duration, float Delay)
    {
        Status = PlayerStatus.Wait;
        transform.DOLookAt(Target.position, Duration, AxisConstraint.Y)
            .SetDelay(Delay)
            .OnComplete(() =>
            {
                Status = PlayerStatus.Moving;
            });
    }

    public void BackFall()
    {
        Status = PlayerStatus.Wait;
        Anim.SetTrigger("GooseBack");
        Delay.Instance.Wait(4, d);
        void d()
        {
            Status = PlayerStatus.Moving;
        }
    }

    #endregion Animation

    #region CalculateFunction

    private bool IsClockwise(Quaternion from, Quaternion to, Vector3 up)
    {
        Vector3 fromDir = from * Vector3.forward;
        Vector3 toDir = to * Vector3.forward;
        Vector3 cross = Vector3.Cross(fromDir, toDir);
        return Vector3.Dot(cross, up) > 0;
    }

    #endregion CalculateFunction
}