using System.Collections;
using DG.Tweening;
using RootMotion.FinalIK;
using UnityEngine;
using UnityEngine.Events;

public class OneTwoThreeWood : UnitySingleton<OneTwoThreeWood>
{
    public Transform Neck;

    //public Transform PlayerLookAt;
    public float WarningValue;

    public float WarningValue2;
    public GameObject CuriousFX;
    public GameObject GotYouFX;
    public GameObject EndingCam1;
    public GameObject EndingCam2;
    public Animator BirdAnimator;
    public Transform BirdModel;
    public Transform FlyTracker;
    public LookAtIK LookIK;
    public DOTweenAnimation WindFXAnim;
    public ParticleSystem WindFX;
    public ParticleSystem CameraWindFX;
    public Material GressMat;
    public DOTweenAnimation[] WalkTweens;
    public UnityEvent EndingEvent;

    private bool IsPlayerFacingStation
    {
        get
        {
            return Vector3.Angle(Player.Instance.transform.forward, BirdModel.position - Player.Instance.transform.position) < 60;
        }
    }

    private bool looking;
    private bool playerIn;
    private Coroutine lookingCoroutine;
    private Coroutine calmDownCoroutine;

    private void Start()
    {
        GressMat.SetFloat("_WindGustStrength", 0);
        GressMat.SetFloat("_WindGustTint", 0);
    }

    public void Init()
    {
        BirdAnimator.speed = 1;
        CuriousFX.SetActive(false);
        looking = false;
        WarningValue = WarningValue2 = 0;
        Neck.DOKill();
        Neck.DOLocalRotate(new Vector3(0, 0, 22.884f), 1);
    }

    private void Update()
    {
        if (GotYouFX.activeSelf || Player.Instance.Status != PlayerStatus.Moving || !playerIn)
            return;
        if (looking && GameInput.Move.magnitude > 0.1f)
        {
            WarningValue2 += 60 * Time.deltaTime / (Player.Instance.transform.position - BirdModel.position).magnitude;
            if (WarningValue2 >= 2)
            {
                if (IsPlayerFacingStation)
                    GotYou();
            }
        }
        if (WarningValue >= 10)
            return;
        if (WarningValue >= 0)
        {
            WarningValue -= Time.deltaTime / 2;
        }
        if (GameInput.Move.magnitude > 0.1f)
        {
            WarningValue += 60 * Time.deltaTime / (Player.Instance.transform.position - BirdModel.position).magnitude;
            if (WarningValue >= 10)
            {
                Look();
            }
        }
    }

    private void Look()
    {
        if (lookingCoroutine != null)
            StopCoroutine(lookingCoroutine);
        lookingCoroutine = StartCoroutine(_Look());
    }

    private IEnumerator _Look()
    {
        BirdAnimator.speed = 0;
        CuriousFX.SetActive(true);
        foreach (var item in WalkTweens)
        {
            item.DOPause();
        }
        BirdModel.DOKill();
        BirdModel.DOLookAt(Player.Instance.transform.position + Player.Instance.transform.right * 5, 1f, AxisConstraint.Y);
        Neck.DOKill();
        Neck.DOLocalRotate(Vector3.left * 30, 1)
            .SetRelative(true);

        yield return new WaitForSeconds(.5f);
        looking = true;
        yield return new WaitForSeconds(1);
        Neck.DOLocalRotate(Vector3.right * 60, 1)
            .SetRelative(true);
        //BirdModel.DOLookAt(InitLookAt.position - InitLookAt.right * 5, 1f, AxisConstraint.Y);

        yield return new WaitForSeconds(2.5f);
        BirdModel.DOLocalRotate(Vector3.zero, 1);
        Init();
        yield return new WaitForSeconds(2.5f);

        BirdAnimator.speed = 1;
        foreach (var item in WalkTweens)
        {
            item.DOPlay();
        }
    }

    public void GotYou()
    {
        StartCoroutine(_GotYou());
    }

    private IEnumerator _GotYou()
    {
        if (lookingCoroutine != null)
            StopCoroutine(lookingCoroutine);
        Init();
        LookIK.solver.target = Player.Instance.transform;
        DOTween.To(() => LookIK.solver.IKPositionWeight, x => LookIK.solver.IKPositionWeight = x, 1, 0.5f);

        GotYouFX.SetActive(true);
        CuriousFX.SetActive(false);
        BirdModel.DOKill();
        BirdModel.DOLookAt(Player.Instance.transform.position, 0.5f, AxisConstraint.Y);
        //CameraMain.Instance.enabled = false;
        //CameraMain.Instance.LookAt(PlayerLookAt, 0.45f);
        Player.Instance.Status = PlayerStatus.Wait;
        yield return new WaitForSeconds(.5f);

        SEManager.Instance.PlaySystemSE(SystemSE.大鳥起風);
        //CameraMain.Instance.enabled = true;
        CameraMain.Instance.cameraSpeed = 10f;
        Player.Instance.transform.DOLookAt(BirdModel.position, 1f);
        GressMat.SetVector("_WindDirection", BirdModel.forward);
        GressMat.DOKill();
        GressMat.DOFloat(0.6f, "_WindGustStrength", 1);
        GressMat.DOFloat(0.15f, "_WindGustTint", 2);
        WindFXAnim.DORewind();
        WindFXAnim.DOPlay();
        WindFX.Play();
        CameraWindFX.Play();
        yield return new WaitForSeconds(.5f);
        Player.Instance.Anim.SetTrigger("WindBack");

        BirdAnimator.speed = 1;
        BirdAnimator.SetTrigger("Curious");
    }

    public void OnEnter()
    {
        playerIn = true;
        PlayerAnimEvent.Instance.StandUpEvent += Stop;
    }

    public void OnExit()
    {
        playerIn = false;
        PlayerAnimEvent.Instance.StandUpEvent -= Stop;
        if (GotYouFX.activeSelf)
        {
            Stop();
        }
    }

    public void Stop()
    {
        DOTween.To(() => LookIK.solver.IKPositionWeight, x => LookIK.solver.IKPositionWeight = x, 0, 1);

        //CameraMain.Instance.enabled = true;
        Player.Instance.Status = PlayerStatus.Moving;
        CameraMain.Instance.cameraSpeed = 200f;
        if (enabled)
        {
            BirdModel.DOLocalRotate(Vector3.zero, 1)
                .OnComplete(() =>
                {
                    BirdAnimator.speed = 1;
                    foreach (var item in WalkTweens)
                    {
                        item.DOPlay();
                    }
                });
            BirdAnimator.SetBool("Flying", false);
        }
        GotYouFX.SetActive(false);
        GressMat.DOKill();
        GressMat.DOFloat(0, "_WindGustStrength", 2);
        GressMat.DOFloat(0, "_WindGustTint", 2);
    }

    public void Ending()
    {
        UI_FullScreenFade.Instance.SetMovieMode(true);
        if (lookingCoroutine != null)
            StopCoroutine(lookingCoroutine);

        BirdAnimator.speed = 1;
        foreach (var item in WalkTweens)
        {
            item.DOPause();
        }
        StartCoroutine(d());
        IEnumerator d()
        {
            enabled = false;
            EndingCam1.SetActive(true);
            GotYouFX.SetActive(true);
            CuriousFX.SetActive(false);
            LookIK.solver.target = Player.Instance.FPCam.transform;
            DOTween.To(() => LookIK.solver.IKPositionWeight, x => LookIK.solver.IKPositionWeight = x, 1, 0.5f);
            Player.Instance.Status = PlayerStatus.Wait;
            yield return new WaitForSeconds(.5f);
            BirdModel.DOLookAt(Player.Instance.transform.position, 0.5f, AxisConstraint.Y);
            yield return new WaitForSeconds(.5f);

            SEManager.Instance.PlaySystemSE(SystemSE.大鳥飛走);
            Player.Instance.transform.DOLookAt(BirdModel.position, 1f);
            GressMat.SetVector("_WindDirection", BirdModel.forward);
            GressMat.DOKill();
            GressMat.DOFloat(0.6f, "_WindGustStrength", 1);
            GressMat.DOFloat(0.15f, "_WindGustTint", 2);
            WindFXAnim.DORewind();
            WindFXAnim.DOPlay();
            WindFX.Play();
            Player.Instance.Anim.SetTrigger("WindBack");
            BirdAnimator.SetBool("Flying", true);
            BirdAnimator.SetTrigger("Fly");
            yield return new WaitForSeconds(2f);
            GotYouFX.SetActive(false);
            DOTween.To(() => LookIK.solver.IKPositionWeight, x => LookIK.solver.IKPositionWeight = x, 0, 1);
            EndingCam2.SetActive(true);
            FlyTracker.gameObject.SetActive(true);
            BirdModel.SetParent(FlyTracker);
            BirdModel.localPosition = BirdModel.localEulerAngles = Vector3.zero;
            yield return new WaitForSeconds(3f);

            UI_FullScreenFade.Instance.SetMovieMode(false);
            EndingCam1.SetActive(false);
            EndingCam2.SetActive(false);
            Stop();
            EndingEvent.Invoke();
            yield return new WaitForSeconds(2f);
            BirdModel.gameObject.SetActive(false);
        }
    }
}