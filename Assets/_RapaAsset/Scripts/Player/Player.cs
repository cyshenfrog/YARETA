using System;
using System.Collections;
using DG.Tweening;
using Lean.Pool;
using UnityEngine;
using NaughtyAttributes;

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
    OneDirAndTurn,
}

[RequireComponent(typeof(CharacterController))]
public class Player : UnitySingleton_D<Player>
{
    #region Fields/properties

    // Component
    [BoxGroup("Component")]
    [Space]
    public CharacterController characterController;

    [BoxGroup("Component")]
    public Animator Anim;

    [BoxGroup("Component")]
    public GameObject[] Scanners;

    [BoxGroup("Component")]
    public GameObject Pad;

    [BoxGroup("Component")]
    public GameObject Rock;

    [BoxGroup("Component")]
    public GameObject Egg;

    [BoxGroup("Component")]
    public GameObject Hammer;

    [BoxGroup("Component")]
    public GameObject DrawModel;

    [BoxGroup("Component")]
    public LeanGameObjectPool RodPool;

    [BoxGroup("Component")]
    public GameObject FPCam;

    [BoxGroup("Component")]
    public GameObject Model;

    [BoxGroup("Component")]
    public Transform MenuHand;

    [BoxGroup("Component")]
    public Transform DrawingHand;

    [BoxGroup("Component")]
    public Transform LeftDragPos;

    [BoxGroup("Component")]
    public Transform RightDragPos;

    [BoxGroup("Component")]
    public Transform FrontDragPos;

    [BoxGroup("Component")]
    public PlayerTrigger PlayerTrigger;

    [BoxGroup("Component")]
    public ParticleSystem WalkFX;

    [BoxGroup("Component")]
    public GameObject DrownFX;

    protected Transform ScanHandTracker_R
    { get { return Player_IKManager.Instance.RightHandRef; } }

    protected Transform ScanHandTracker_L
    { get { return Player_IKManager.Instance.LeftHandRef; } }

    // Setting

    [BoxGroup("Setting")]
    [Space]
    public float turnSpeed = 10f;

    [BoxGroup("Setting")]
    public float MoveSpeed = 1;

    [BoxGroup("Setting")]
    public float AirMoveSpeed = 1;

    [BoxGroup("Setting")]
    public float JumpPower = 5;

    [BoxGroup("Setting")]
    public float RocketJumpPower;

    [BoxGroup("Setting")]
    public float AirDrag = 2.5f;

    [BoxGroup("Setting")]
    public bool OneDirMode;

    [BoxGroup("Setting")]
    public bool PuzzleMode;

    [BoxGroup("Setting")]
    public float DirectionShift;

    // CalculateTemp

    private Tween t;
    private Vector3 targetDirection;
    private Vector3 onAirVelocity = Vector3.down * 5;
    private Vector3 forward;
    private Vector3 right;

    private Quaternion freeRotation;
    private RaycastHit hitInfo;
    private RaycastHit climbHitInfo;
    private float currentDrag;
    private float rotationDiff;
    private float speed;
    private float velocity;
    private bool grounded => Physics.SphereCast(transform.position + Vector3.up * 0.6f, 0.5f, Vector3.down, out hitInfo, 0.5f, 1 << 0, QueryTriggerInteraction.Ignore);

    // State Control

    [HideInInspector]
    public Transform LookTarget;

    public bool IsJumping;

    [SerializeField]
    [BoxGroup("Status")]
    [Space]
    private MoveMode moveMode;

    public MoveMode MoveMode
    {
        get { return moveMode; }
        set
        {
            moveMode = value;
            switch (value)
            {
                case MoveMode.OneDirAndTurn:
                    OneDirMode = true;
                    break;

                default:
                    OneDirMode = false;
                    break;
            }
        }
    }

    private PlayerStatus laststatus = PlayerStatus.Moving;

    [SerializeField]
    [BoxGroup("Status")]
    private PlayerStatus status = PlayerStatus.Wait;

    public PlayerStatus Status
    {
        get => status;
        set
        {
            if (value == status)
                return;
            laststatus = status;
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
                if (!Slowdown)
                    Anim.SetFloat("Speed", 0);
                Anim.SetBool("isSprinting", false);
                break;

            case PlayerStatus.Moving:
                Slowdown = false;
                PlayerTrigger.enabled = true;
                //PlayerTrigger.Rescan();
                break;
        }
        if (status == PlayerStatus.Static || status == PlayerStatus.Climbing)
        {
            Anim.applyRootMotion = false;
            characterController.enabled = false;
        }
        else
        {
            Anim.applyRootMotion = true;
            characterController.enabled = true;
        }
    }

    public bool Slowdown;
    //private bool canSpring;

    public bool CanSpringAndJump = true;
    private bool AutoRunning;

    //{
    //    get { return canSpring; }
    //    set
    //    {
    //        canSpring = value;
    //        Anim.SetBool("isSprinting", value);
    //    }
    //}

    #endregion Fields/properties

    /////////////////////////////////////
    //            Control              //
    /////////////////////////////////////
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

        float dot = Vector3.Dot(transform.forward, -climbHitInfo.normal);

        //transform.position = climbHitInfo.point + climbHitInfo.normal * 0.05f;
        transform.forward = Vector3.Lerp(transform.forward,
            -climbHitInfo.normal,
            10f * Time.fixedDeltaTime);
        //(-input.x * transform.right + input.y * transform.up)
        transform.Translate(input * Time.deltaTime, Space.Self);
    }

    public virtual void Update()
    {
        AirMovementAndCheck();
        if (Slowdown)
            Anim.SetFloat("Speed", 0, 0.5f, Time.deltaTime);
        if (Status == PlayerStatus.Climbing)
            ClimbControl(GameInput.Move);
        if (Status != PlayerStatus.Moving)
            return;
        //Basic Section
        MovementUpdate();
        if (GameRef.CarringObj)
        {
            if (GameInput.GetButtonDown(Actions.Interact) && !GameRef.CarringObj.DontDrop)
            {
                GameRef.CarringObj.Drop();
                return;
            }
        }
        if (!grounded)
            return;
        if (GameInput.GetButtonDown(Actions.Menu) && !PuzzleMode && SaveDataManager.TutorialPassed)
            OpenMenu();
        if (GameInput.GetButtonDown(Actions.DrawMode) && SaveDataManager.TutorialPassed)
            DrawingMode.Instance.StartDrawingMode();
        if (PlayerTrigger.NearestObj)
        {
            if (GameInput.GetButtonDown(PlayerTrigger.NearestObj.InteractButton)) PlayerTrigger.Interact();
        }
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

    private void MovementUpdate()
    {
        if (OneDirMode)
        {
            speed = GameInput.Move.y + GameInput.Move.x;
            speed = Mathf.Clamp(speed, -1f, 1f);
            speed *= MoveSpeed;
        }
        else
        {
            speed = Mathf.Abs(GameInput.Move.x) + Mathf.Abs(GameInput.Move.y);
            speed = Mathf.Clamp(speed, 0f, 1f);
            speed *= MoveSpeed;
        }

        speed = Mathf.SmoothDamp(Anim.GetFloat("Speed"), speed, ref velocity, 0.1f);
        Anim.SetFloat("Speed", speed);

        //Anim.SetFloat("Direction", GameInput.Move.x);
        if (!OneDirMode && CanSpringAndJump && GameInput.IsMove)
        {
            if (GameInput.GetButtonDown(Actions.ToggleRun))
                AutoRunning = !AutoRunning;
            if (GameInput.GetButton(Actions.Run) || AutoRunning)
                Anim.SetBool("isSprinting", true);
            else
                Anim.SetBool("isSprinting", false);
        }
        else
        {
            AutoRunning = false;
            Anim.SetBool("isSprinting", false);
        }

        if (grounded && !IsJumping && CanSpringAndJump)
        {
            if (GameInput.GetButtonDown(Actions.Jump))
            {
                Jump();
            }
        }

        if (LookTarget)
            transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(LookTarget.position - transform.position, Vector3.up), Time.deltaTime * 5);
        else
        {
            UpdateTargetDirection();
            if (GameInput.IsMove && targetDirection.magnitude > 0.1f)
            {
                freeRotation = Quaternion.LookRotation(targetDirection.normalized, transform.up);
                rotationDiff = freeRotation.eulerAngles.y - transform.eulerAngles.y;
                Vector3 euler = new Vector3(0, rotationDiff != 0 ? freeRotation.eulerAngles.y : transform.eulerAngles.y, 0);

                transform.rotation = Quaternion.Slerp(transform.rotation, Quaternion.Euler(euler), turnSpeed * Time.deltaTime);
            }
        }
    }

    private void Jump()
    {
        Anim.SetTrigger("Jump");
        currentDrag = AirDrag;
        if (GameInput.IsMove)
            onAirVelocity = Vector3.up * JumpPower;
    }

    public virtual void UpdateTargetDirection()
    {
        forward = GameRef.MainCam.transform.TransformDirection(Vector3.forward);
        forward.y = 0;
        right = GameRef.MainCam.transform.TransformDirection(Vector3.right);

        targetDirection = (GameInput.Move.x + DirectionShift) * right + (OneDirMode ? 0 : GameInput.Move.y) * forward;
    }

    private void AirMovementAndCheck()
    {
        WalkFX.enableEmission = grounded;
        Anim.SetBool("Grounded", grounded);
        if (!grounded)
        {
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

            onAirVelocity = Vector3.Lerp(onAirVelocity, (GameInput.MovementCameraSpace + Vector3.down * GameInput.MovementCameraSpace.y) * AirMoveSpeed + Vector3.down * (IsJumping ? 0 : 10), Time.deltaTime);
        }
        else if (!IsJumping)
        {
            if (Status == PlayerStatus.Climbing)
                Status = PlayerStatus.Moving;
            onAirVelocity = Vector3.Lerp(onAirVelocity, Vector3.down, Time.deltaTime * 5);
        }

        if (IsJumping)
        {
            onAirVelocity -= Vector3.up * currentDrag * Time.deltaTime;
            if (onAirVelocity.y < 0)
            {
                currentDrag = Mathf.Lerp(currentDrag, AirDrag, Time.deltaTime * 4);
            }
        }
        if (characterController.enabled)
            characterController.Move(onAirVelocity * Time.deltaTime);
    }

    public void OnJumpFinish()
    {
        IsJumping = false;
    }

    /////////////////////////////////////
    //          Animation              //
    /////////////////////////////////////

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
        if (!grounded)
            Drown();
        else
            WalkBack();
    }

    public void Drown()
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
            CanSpringAndJump = false;
            Status = PlayerStatus.Wait;
            Anim.SetFloat("Speed", -speed);
            yield return new WaitForSeconds(duration);
            Status = PlayerStatus.Moving;
            CanSpringAndJump = true;
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
        t.Kill();
        t = DOTween.To(() => Anim.GetFloat("Speed"), x => Anim.SetFloat("Speed", x), to, duration)
            .SetDelay(delay)
            .OnComplete(() => onCompelete?.Invoke());
    }

    public void StartCrouch()
    {
        t.Kill();
        t = DOTween.To(() => Anim.GetFloat("CrouchLevel"), x => Anim.SetFloat("CrouchLevel", x), 1, 1f);
        CameraMain.Instance.SetCameraMode(CameraMode.Aim);
        Anim.SetBool("isSprinting", false);
        CanSpringAndJump = false;
    }

    public void EndCrouch()
    {
        t.Kill();
        t = DOTween.To(() => Anim.GetFloat("CrouchLevel"), x => Anim.SetFloat("CrouchLevel", x), 0, 1f);
        CameraMain.Instance.SetCameraMode(CameraMode.Default);
        CanSpringAndJump = true;
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
}