using DG.Tweening;
using System.Collections;
using UnityEngine;

public class BigMonster : MonoBehaviour
{
    public GameObject Cam;
    public GameObject FPSCam;
    private bool selecting;

    public Animator MosterAnim;
    public GameObject Monster;
    public GameObject EventCam;
    public GameObject EventCam2;
    public GameObject SelectUI;
    public GameObject Poo;
    public Portable Carrot;
    public ParticleSystem FallFX;
    public GameObject OriginTrees;
    public GameObject BrokenTrees;
    public Transform EatPos;
    private float choiceCD = 5;

    // Update is called once per frame
    private void Update()
    {
        if (selecting)
        {
            choiceCD -= Time.deltaTime;
            if (choiceCD <= 0)
            {
                Run();
                return;
            }
            if (GameInput.GetButtonDown(Actions.Right))
            {
                Run();
            }
            else if (GameInput.GetButtonDown(Actions.Left))
            {
                StepBack();
            }
            return;
        }
    }

    public void HeadUp()
    {
        UI_FullScreenFade.Instance.SetMovieMode(true);
        StartCoroutine(_HeadUp());
        IEnumerator _HeadUp()
        {
            OriginTrees.SetActive(false);
            BrokenTrees.SetActive(true);
            Player.Instance.Status = PlayerStatus.Wait;
            Player.Instance.SetModelActive(false);
            yield return new WaitForSeconds(.5f);
            yield return new WaitForSeconds(0.1f);
            Cam.SetActive(true);
            SEManager.Instance.PlaySystemSE(SystemSE.樹倒鳥飛);
            yield return new WaitForSeconds(1f);
            Monster.SetActive(true);

            yield return new WaitForSeconds(1f);
            yield return new WaitForSeconds(2f);
            MosterAnim.SetTrigger("Scare");
            yield return new WaitForSeconds(.5f);
            UI_FullScreenFade.Instance.SetMovieMode(false);
            Cam.transform.DOLocalRotate(Vector3.left * 20, 0.45f)
                .SetLoops(2, LoopType.Yoyo)
                .SetRelative(true);
            //Player.Instance.WalkTo((( - Player.Instance.transform.position) * .5f) + Player.Instance.transform.position, () =>
            //{
            //    Player.Instance.Status = PlayerStatus.Wait;
            //});
            Player.Instance.SetModelActive(true);
            //Cam.transform.DOLocalRotate(Vector3.left * 60, 3, RotateMode.FastBeyond360)
            //    .SetRelative(true)
            //    .SetEase(Ease.Linear)
            //    .OnComplete(() => { StartChoice(); });
            StartChoice();
        }
    }

    private void StartChoice()
    {
        Player.Instance.Status = PlayerStatus.Wait;
        GameInput.Cursorvisible = true;
        selecting = true;
        SelectUI.SetActive(true);
    }

    private void StopChoice()
    {
        GameInput.Cursorvisible = false;
        SelectUI.SetActive(false);
        selecting = false;
    }

    public void Run()
    {
        PrototypeMain.Instance.RemoveTargetScanData(46);
        PrototypeMain.Instance.RemoveTargetScanData(47);
        SEManager.Instance.PlaySystemSE(SystemSE.UI確認);
        StopChoice();
        PlayerAnimEvent.Instance.StandUpEvent += StandUp;

        StartCoroutine(_Run());

        IEnumerator _Run()
        {
            Carrot.transform.position = Player.Instance.transform.position + Player.Instance.transform.forward;
            FPSCam.SetActive(false);
            Carrot.Take(false);
            Cam.SetActive(false);
            yield return new WaitForSeconds(.5f);
            Player.Instance.Anim.SetFloat("Speed", 0);
            Player.Instance.transform.DOLocalRotate(Vector3.up * 180, 1, RotateMode.FastBeyond360)
                .SetRelative();

            Player.Instance.LerpSpeed(1f, 2f);
            Player.Instance.Anim.speed = 1.5f;
            yield return new WaitForSeconds(.5f);
            EventCam.SetActive(true);
            yield return new WaitForSeconds(2.5f);
            EventCam.SetActive(false);
            EventCam2.SetActive(true);
            GameRef.CarringObj.Drop(false);
            yield return new WaitForSeconds(0);

            Player.Instance.Anim.speed = 1;
            Player.Instance.Anim.SetTrigger("JumpAway");

            SEManager.Instance.PlaySystemSE(SystemSE.跌倒);
            yield return new WaitForSeconds(.5f);
            FallFX.transform.position = Player.Instance.transform.position;
            FallFX.Play();
            yield return new WaitForSeconds(2);
            Player.Instance.Anim.SetFloat("Speed", 0);
            CameraMain.Instance.Recenter(0);
            Monster.SetActive(false);
            EventCam2.SetActive(false);
        }
    }

    private void StandUp()
    {
        Player.Instance.Status = PlayerStatus.Moving;
        PlayerAnimEvent.Instance.StandUpEvent -= StandUp;
    }

    public void StepBack()
    {
        SEManager.Instance.PlaySystemSE(SystemSE.UI確認);
        StopChoice();
        StartCoroutine(_StepBack());

        IEnumerator _StepBack()
        {
            Player.Instance.transform.LookAt(Monster.transform.GetChild(1));

            CameraMain.Instance.Recenter(0);
            CameraMain.Instance.Lock = true;
            Cam.SetActive(false);
            FPSCam.SetActive(false);
            yield return new WaitForSeconds(.5f);
            Carrot.Drop(false);
            yield return new WaitForSeconds(.2f);
            Carrot.GetComponent<Rigidbody>().isKinematic = true;
            Carrot.GetComponent<Interactable>().enabled = false;
            Carrot.transform.parent = EatPos;
            Carrot.transform.DOLocalMove(Vector3.zero, 2);
            Player.Instance.LerpSpeed(-0.2f);
            yield return new WaitForSeconds(3);
            Player.Instance.LerpSpeed(0);
            Player.Instance.Status = PlayerStatus.Moving;

            CameraMain.Instance.Lock = false;
            MosterAnim.SetTrigger("Eat");
            yield return new WaitForSeconds(2);
            Carrot.gameObject.SetActive(false);
            yield return new WaitForSeconds(2);
            Poo.SetActive(true);
            MosterAnim.SetTrigger("Leave");
            yield return new WaitForSeconds(1);
            MosterAnim.transform.DOLocalRotate(Vector3.up * 60, 1.2f)
                .SetLoops(3, LoopType.Incremental)
                .SetRelative(true);
            yield return new WaitForSeconds(1);
            MosterAnim.transform.DOLocalMoveZ(-25, 15)
                .SetEase(Ease.InSine)
                .SetRelative(true)
                .OnComplete(() =>
                {
                    MosterAnim.gameObject.SetActive(false);
                });
        }
    }
}