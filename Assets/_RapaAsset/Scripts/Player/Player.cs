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
    RocketJump
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
    public DynamicBone MLMDynamic;
    public Transform MLM;
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
    public float RocketJumpSpeed = 5;
    public float RocketFallSpeed = 1;
    public float RocketJumpMaxTime = 5;

    #endregion Setting

    #region StateControl

    public bool CanScan;
    private bool running;
    private bool facingWall;

    private bool grounded; private bool Grounded
    {
        get { return grounded; }
        set
        {
            if ((grounded != value) && value)
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
        Anim.applyRootMotion = status == PlayerStatus.Wait;
        PlayerTrigger.enabled = status == PlayerStatus.Moving;
        characterController.enabled = status == PlayerStatus.Moving || status == PlayerStatus.Wait;
        switch (status)
        {
            case PlayerStatus.Climbing:
                if (!speedTween.IsActive())
                    Anim.SetFloat("Speed", 0);
                Anim.SetBool("isSprinting", false);
                useGravity = false;
                break;

            case PlayerStatus.Static:
                if (!speedTween.IsActive())
                    Anim.SetFloat("Speed", 0);
                Anim.SetBool("isSprinting", false);
                break;

            case PlayerStatus.Wait:
                if (!speedTween.IsActive())
                    Anim.SetFloat("Speed", 0);
                Anim.SetBool("isSprinting", false);
                break;

            case PlayerStatus.Moving:
                Anim.SetBool("IsClimbing", false);
                useGravity = true;
                break;
        }
    }

    public Action OnJump;

    #endregion StateControl

    #region CalculateTemp

    private const float MAX_MOVE_SPEED = 4f;
    private const int GRAVITY = 12;
    private float vSpeed;
    private float hSpeed;
    private float rotationDiff;
    private float climbAccuValue;
    private RaycastHit climbHitInfo;
    private Quaternion targetRotation;
    private Quaternion freeRotation;
    private Tween speedTween;
    private Tween crouchTween;
    private Tween initFacing;
    private Tween hSpeedBlend;
    private Tween vSpeedBlend;
    private Vector3 targetDirection;
    private Vector3 forward;
    private Vector3 right;
    private RaycastHit groundHit;
    private bool useGravity = true;
    private float rocketJumpTime;

    #endregion CalculateTemp

    public virtual void Update()
    {
        // Always execute
        GravityUpdate();
        DefaultAnimUpdate();

        // Execute by status
        switch (Status)
        {
            case PlayerStatus.Wait:
                break;

            case PlayerStatus.Moving:
                GroundCheck();
                ClimbOnCheck();
                MovementControl();
                UpdateRotation();
                ActionControl();
                LocomoationAnimUpdate();
                break;

            case PlayerStatus.Static:
                break;

            case PlayerStatus.Climbing:
                ClimbOffCheck();
                ClimbControl(GameInput.Move);
                break;

            default:
                break;
        }
    }

    #region Locomotion

    private Vector3 RemoveY(Vector3 vector)
    {
        return new Vector3(vector.x, 0, vector.z);
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
        if (!useGravity)
            return;
        if (vSpeed > -10)
        {
            vSpeed -= GRAVITY * Time.deltaTime;
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
                    SetRunning(!running);
                if (GameInput.GetButton(Actions.Run) && GameInput.GetButton(Actions.Move))
                    SetRunning(true);
                else
                    SetRunning(false);

                if (grounded)
                {
                    if (GameInput.GetButtonDown(Actions.Jump))
                    {
                        rocketJumpTime = 0;
                        Jump();
                    }
                }
                else
                {
                    if (GameInput.GetButton(Actions.Jump))
                    {
                        rocketJumpTime += Time.deltaTime;
                        if (rocketJumpTime > .4f)
                        {
                            rocketJumpTime = 0;
                            RocketJump();
                        }
                    }
                    if (GameInput.GetButtonDown(Actions.Jump))
                    {
                        RocketJump();
                    }
                }

                break;

            case MoveMode.RocketJump:
                if (GameInput.GetButtonDown(Actions.Jump))
                    EndRocketFall();
                if (GameInput.GetButtonUp(Actions.Jump))
                    RocketFall();
                break;

            case MoveMode.Walking:
            case MoveMode.Aimming:
            default:
                break;
        }

        if (GameInput.GetButtonDown(Actions.Move))
        {
            if (hSpeedBlend.IsActive())
                hSpeedBlend.Kill();
            hSpeed = 0;
            hSpeedBlend = DOTween.To(() => hSpeed, x => hSpeed = x, MAX_MOVE_SPEED, 0.2f);
        }
        else if (GameInput.GetButtonUp(Actions.Move))
        {
            if (hSpeedBlend.IsActive())
                hSpeedBlend.Kill();
            hSpeedBlend = DOTween.To(() => hSpeed, x => hSpeed = x, 0, 0.2f);
        }

        if (characterController.enabled)
            characterController.Move((GameInput.MovementCameraSpace.normalized * hSpeed * MoveSpeed + Vector3.up * vSpeed) * Time.deltaTime);
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
        if (GameInput.GetButtonDown(Actions.Move))
        {
            initFacing = transform.DOLookAt(transform.position + GameInput.MovementCameraSpace, 0.2f, AxisConstraint.Y);
            return;
        }
        if (FacingTarget)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(FacingTarget.position - transform.position, Vector3.up), Time.deltaTime * 5);
        else
        {
            CalcTargetDirection();
            if (GameInput.GetButton(Actions.Move) && targetDirection.magnitude > 0.1f)
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

    private void SetRunning(bool run)
    {
        running = run;
        MoveSpeed = run ? 1.5f : 1;
    }

    private void OnGround()
    {
        if (moveMode == MoveMode.RocketJump)
            EndRocketFall();
        if (Status == PlayerStatus.Climbing)
            Status = PlayerStatus.Moving;
    }

    private void RocketJump()
    {
        MoveSpeed = 0;
        moveMode = MoveMode.RocketJump;
        if (vSpeedBlend.IsActive())
            vSpeedBlend.Kill();
        vSpeed = 0;
        useGravity = false;
        vSpeedBlend = DOTween.To(() => vSpeed, x => vSpeed = x, RocketJumpSpeed, 0.5f);

        PullMLM();
        Delay.Instance.Wait(RocketJumpMaxTime, RocketFall);
    }

    private void RocketFall()
    {
        MoveSpeed = 0.5f;
        if (vSpeedBlend.IsActive())
            vSpeedBlend.Kill();
        vSpeedBlend = DOTween.To(() => vSpeed, x => vSpeed = x, -1, 1f);
    }

    private void EndRocketFall()
    {
        MoveSpeed = 1f;
        moveMode = MoveMode.Normal;
        useGravity = true;
        ReleaseMLM();
    }

    private void PullMLM()
    {
        MLMDynamic.enabled = false;
        MLM.DOKill();
        MLM.DOLocalMoveY(-1f, 1f)
            .SetRelative(true);
        //Rope.Instance.SetRopeLength(1);
        Player_IKManager.Instance.PlaySimpleIK(MLM, PlayerIK.LeftHand);
    }

    private void ReleaseMLM()
    {
        Player_IKManager.Instance.ResumeSimpleIK(PlayerIK.LeftHand);
        MLM.DOKill();
        MLM.DOLocalMoveY(1, 1f)
            .SetRelative(true)
            .OnComplete(() => MLMDynamic.enabled = true);
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
        //if (GameInput.GetButton(Actions.Move))
        vSpeed = JumpPower * (running ? 1.2f : 1);
        OnJump?.Invoke();
    }

    public void OnJumpFinish()
    {
    }

    #endregion Locomotion

    #region Climbing

    private void ClimbControl(Vector2 input)
    {
        if (Physics.Raycast(transform.position, transform.forward, out climbHitInfo, .6f, 1 << 0, QueryTriggerInteraction.Ignore))
        {
            transform.position = climbHitInfo.point + climbHitInfo.normal * 0.05f;
            transform.forward = Vector3.Lerp(transform.forward,
                -climbHitInfo.normal,
                10f * Time.fixedDeltaTime);
        }
        transform.Translate(input * Time.deltaTime, Space.Self);
    }

    private void AccumulateClimbingAction()
    {
        climbAccuValue += Time.deltaTime;
        if (climbAccuValue >= .5f)
        {
            climbAccuValue = 0;
            StartClimbing();
        }
    }

    private void ClimbOnCheck()
    {
        //爬牆檢測
        facingWall = Physics.Raycast(transform.position + transform.up, RemoveY(transform.forward), out climbHitInfo, 1, 1 << 0, QueryTriggerInteraction.Ignore);
        Debug.DrawRay(transform.position + transform.up * 1.6f, RemoveY(transform.forward), Color.red);
        if (facingWall && GameInput.GetButton(Actions.Move))
        {
            //if (Vector3.Angle(climbHitInfo.normal, Vector3.up) > 45)
            AccumulateClimbingAction();
        }
        else
            climbAccuValue = 0;
    }

    private void ClimbOffCheck()
    {
        facingWall = Physics.Raycast(transform.position + transform.up * 1.6f, RemoveY(transform.forward), out _, .6f, 1 << 0, QueryTriggerInteraction.Ignore);
        if (Physics.Raycast(transform.position + transform.up, Vector3.down, out groundHit, .25f, 1 << 0, QueryTriggerInteraction.Ignore))
        {
            if (Vector3.Angle(groundHit.normal, Vector3.up) < 45)
                EndClimbing(groundHit.point);
        }
        if (!facingWall)
        {
            EndClimbing(transform.position + Vector3.up * 2);
        }
    }

    private void StartClimbing()
    {
        Anim.SetBool("IsClimbing", true);
        Delay.Instance.Wait(0.1f, () => { vSpeed = 6; });
        Delay.Instance.Wait(0.2f, () => { Status = PlayerStatus.Climbing; });
    }

    private void EndClimbing(Vector3 landingPos)
    {
        Anim.SetBool("IsClimbing", false);
        Status = PlayerStatus.Wait;
        transform.DOMove(landingPos, 0.5f)
            .OnComplete(() =>
            {
                Status = PlayerStatus.Moving;
            });
    }

    #endregion Climbing

    #region Animation

    private void DefaultAnimUpdate()
    {
        Anim.SetFloat("MoveX", GameInput.Move.x);
        Anim.SetFloat("MoveY", GameInput.Move.y);
        Anim.SetBool("Grounded", Grounded);
        Anim.SetBool("FacingWall", facingWall);
    }

    private void LocomoationAnimUpdate()
    {
        Anim.SetFloat("Speed", hSpeed / MAX_MOVE_SPEED * MoveSpeed, .1f, Time.deltaTime);

        if (GameInput.GetButton(Actions.Move))
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
        Anim.SetBool("isSprinting", false);
        MoveMode = MoveMode.Walking;
    }

    public void EndCrouch()
    {
        crouchTween.Kill();
        crouchTween = DOTween.To(() => Anim.GetFloat("CrouchLevel"), x => Anim.SetFloat("CrouchLevel", x), 0, 1f);
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