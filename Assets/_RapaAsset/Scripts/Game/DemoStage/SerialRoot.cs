using System.Collections;
using Cinemachine;
using DG.Tweening;
using UltEvents;
using UnityEngine;

public class SerialRoot : MonoBehaviour
{
    public AudioSource AudioSource;
    public CinemachineVirtualCamera Cam;
    public Animator PullAnim;
    public GameObject StartGroup;
    public GameObject MoveUI;
    public Transform StartPos;
    public Transform[] Nodes;
    public Renderer Rope;
    public ParticleSystem PullFx;
    public UltEvent FinishEvent;
    public bool KeepFPSCam;
    private bool pulling;
    private Vector3 rotateEuler = new Vector3(40, 0, 0);
    private Vector2 initMousePos;
    private int Count;

    private void Update()
    {
        if (pulling)
            return;
        rotateEuler = Vector3.Lerp(rotateEuler, rotateEuler + new Vector3(-GameInput.CameraMove.y * 30, GameInput.CameraMove.x * 30), Time.deltaTime);
        rotateEuler.Set(Mathf.Clamp(rotateEuler.x, -25, 65), Mathf.Clamp(rotateEuler.y, -45, 45), rotateEuler.z);
        Cam.transform.localRotation = Quaternion.Euler(rotateEuler);
        if (GameInput.GetButtonDown(Actions.Interact))
        {
            MoveForward();
        }
    }

    public void StartPull()
    {
        Cam.gameObject.SetActive(true);
        Player.Instance.Status = PlayerStatus.Static;
        Player.Instance.transform.LookAt(Nodes[0], Vector3.up);
        enabled = true;
        Player.Instance.Model.SetActive(false);
        initMousePos = GameInput.Mouse.screenPosition;
        StartGroup.SetActive(true);
        pulling = false;
        MoveUI.SetActive(true);
        //UI_InteractionHint.Instance.CloseIcon();
    }

    private void MoveForward()
    {
        //DOTween.To(() => rotateEuler, x => rotateEuler = x, Vector3.right * 20, 1f);
        Delay.Instance.Wait(UI_ButtonBlink.Duration, then);
        void then()
        {
            AudioSource.Play();
            pulling = true;

            rotateEuler = new Vector3(40, 0, 0);
            Cam.transform.DOLocalRotate(new Vector3(40, 0, 0), 1, RotateMode.Fast);
            float duration = (Nodes[Count].position - Player.Instance.transform.position).magnitude / 2;
            Player.Instance.transform.DOMove(Nodes[Count].position, duration)
                .SetEase(Ease.Linear);
            Player.Instance.transform.DOLookAt(Nodes[Count].position, duration * 0.43f, AxisConstraint.Y)
                .SetEase(Ease.InQuad);
            DOTween.To(() => Rope.material.mainTextureOffset, x => Rope.material.mainTextureOffset = x, Vector2.up * 5, duration)
                .SetEase(Ease.Linear)
                .SetRelative(true);
            MoveUI.SetActive(false);
            PullAnim.SetBool("Pull", true);
            var m = PullFx.main;
            m.duration = duration - .3f;
            PullFx.Play();
            StartCoroutine(_MoveForward());

            IEnumerator _MoveForward()
            {
                yield return new WaitForSeconds(duration);
                Count++;
                PullAnim.SetBool("Pull", false);

                AudioSource.Pause();

                if (Count >= Nodes.Length)
                    Finish();
                else
                {
                    pulling = false;
                    MoveUI.SetActive(true);
                }
            }
        }
    }

    public void Finish()
    {
        SEManager.Instance.PlaySystemSE(SystemSE.拔蘿蔔);
        Player.Instance.transform.DOMove(Player.Instance.transform.position - Player.Instance.transform.forward * 0.1f, 0.3f)
            .SetDelay(.5f)
            .SetLoops(2, LoopType.Yoyo)
            .OnComplete(() =>
            {
                FallDown(.5f);
                SEManager.Instance.PlaySystemSE(SystemSE.拔蘿蔔);
                Player.Instance.transform.DOMove(Player.Instance.transform.position - Player.Instance.transform.forward * 0.2f, 1f)
                    .SetDelay(0.2f)
                    .SetEase(Ease.InQuad)
                    .OnComplete(() =>
                    {
                        StartCoroutine(Delay(5));
                        CameraMain.Instance.Recenter(0);
                    });
                //Player.Instance.transform.DOMove(Player.Instance.transform.position - Player.Instance.transform.forward * 0.1f, 0.3f)
                //    .SetDelay(0.2f)
                //    .SetLoops(2, LoopType.Yoyo)
                //    .OnComplete(() =>
                //    {
                //        Player.Instance.transform.DOMove(Player.Instance.transform.position - Player.Instance.transform.forward * 0.2f, 1f)
                //            .SetDelay(0.2f)
                //            .SetEase(Ease.InQuad)
                //            .OnComplete(() =>
                //            {
                //                StartCoroutine(Delay(0));
                //                CameraMain.Instance.Recenter(0);
                //            });
                //    });
            });

        IEnumerator Delay(float d)
        {
            SEManager.Instance.PlaySystemSE(SystemSE.物品產生);
            //GetComponent<Collider>().enabled = false;
            Player.Instance.Model.SetActive(true);

            yield return new WaitForSeconds(0);
            if (KeepFPSCam)
                Cam.transform.parent = Player_IKManager.Instance.BodyIK.references.head;
            else
                Cam.gameObject.SetActive(false);
            enabled = false;
            StartGroup.SetActive(false);
            FinishEvent.Invoke();
            yield return new WaitForSeconds(d);
            Player.Instance.Status = PlayerStatus.Moving;
        }
    }

    public void FallDown(float delay)
    {
        StartCoroutine(Delay(delay));

        IEnumerator Delay(float d)
        {
            yield return new WaitForSeconds(d);

            Player.Instance.Anim.SetTrigger("FallDown");
        }
    }
}