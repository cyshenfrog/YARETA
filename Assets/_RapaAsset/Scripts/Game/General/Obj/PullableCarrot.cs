using System;
using System.Collections;
using DG.Tweening;
using RootMotion.FinalIK;
using UltEvents;
using UnityEngine;

public class PullableCarrot : MonoBehaviour
{
    private static bool First = false;//true;
    private static Action OnFirstTryed;
    public bool DirectPullOut;
    private int PullTimes = 1;
    public GameObject Fruit;
    public Transform ModelRoot;
    public Transform Pivot;
    public SkinnedMeshRenderer Renderer;
    public UltEvent OnCompelete;

    private int count;
    private Rigidbody rb;
    private Collider col;
    private InteractionObject interaction;
    private bool canPull;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        col = GetComponent<Collider>();
        interaction = GetComponent<InteractionObject>();
        if (!DirectPullOut)
        {
            OnFirstTryed += SetPullHardMode;
        }
    }

    private void Update()
    {
        if (!canPull)
            return;
        if (GameInput.GetButtonDown(Actions.Interact))
        {
            Pull();
        }
    }

    private void SetPullHardMode()
    {
        GetComponent<Interactable>().UseOverrideName = false;
        PullTimes = 1;
    }

    public void Hold()
    {
        StartCoroutine(_Hold());
    }

    private IEnumerator _Hold()
    {
        col.enabled = false;
        Player.Instance.Status = PlayerStatus.Wait;
        //Player.Instance.Rigidbody.isKinematic = true;
        Player.Instance.Anim.applyRootMotion = false;
        Player_IKManager.Instance.RotatePivot(transform, Pivot);
        Player.Instance.transform.DOKill();
        yield return new WaitForSeconds(.1f);
        Player_IKManager.Instance.PlaySimpleIK(PlayerIK.Body);
        Player_IKManager.Instance.PlayTwoHandIK(interaction, null, false);
        Player.Instance.transform.DOMove(transform.position - Player.Instance.transform.forward * 0.4f, .5f)
            .SetEase(Ease.InOutQuad);
        yield return new WaitForSeconds(.2f);
        Player_IKManager.Instance.RotatePivot(transform, Pivot, .3f);
        ModelRoot.DOLookAt(Player.Instance.transform.position, 0.3f, AxisConstraint.Y, Vector3.up)
            .SetEase(Ease.OutBack);
        yield return new WaitForSeconds(.2f);
        Pull();
    }

    private void Pull()
    {

        SEManager.Instance.PlaySystemSE(SystemSE.拔蘿蔔);
        canPull = false;
        count++;
        //UI_InteractionHint.Instance.CloseIcon();
        if (count == PullTimes)
        {
            if (DirectPullOut)
            {
                Player_IKManager.Instance.BodyRef.DOMove(Player.Instance.transform.forward * -0.2f, .5f)
                    .SetRelative();
                Pivot.DOMove(Player.Instance.transform.forward * -0.2f, .5f)
                    .SetRelative();
                DOTween.To(() => Renderer.GetBlendShapeWeight(0), x => Renderer.SetBlendShapeWeight(0, x), 50f, .4f)
                    .OnComplete(() => { PullOut(); });
            }
            else
            {
                if (First)
                {
                    First = false;
                    count = 0;
                    GetComponent<Interactable>().enabled = true;
                    OnFirstTryed?.Invoke();
                    EndMode(true);
                }
                else
                {
                    Player_IKManager.Instance.BodyRef.DOMove(Player.Instance.transform.forward * -0.2f, .8f)
                        .SetRelative()
                        .SetEase(Ease.InQuad);
                    Pivot.DOMove(Player.Instance.transform.forward * -0.2f, .8f)
                        .SetRelative()
                        .SetEase(Ease.InQuad);
                    DOTween.To(() => Renderer.GetBlendShapeWeight(0), x => Renderer.SetBlendShapeWeight(0, x), 50f, .7f)
                        .SetEase(Ease.InQuad)
                        .OnComplete(() =>
                        {
                            EndModeForSerialRoot();
                        });
                }
            }
        }
        else
        {
            Player_IKManager.Instance.BodyRef.DOMove(Player.Instance.transform.forward * -0.2f, 0.5f)
                .SetRelative()
                .SetEase(Ease.InQuad)
                .SetLoops(2, LoopType.Yoyo);
            Pivot.DOMove(Player.Instance.transform.forward * -0.2f, 0.5f)
                .SetRelative()
                .SetEase(Ease.InQuad)
                .SetLoops(2, LoopType.Yoyo);
            DOTween.To(() => Renderer.GetBlendShapeWeight(0), x => Renderer.SetBlendShapeWeight(0, x), 50f, 0.5f)
                .SetEase(Ease.InQuad)
                .SetLoops(2, LoopType.Yoyo)
                .OnComplete(() =>
                {
                    canPull = true;
                    UI_InteractionHint.Instance.SetText(UIDataEnum.拉);
                    UI_InteractionHint.Instance.UpdateIcon(transform.position + Vector3.up * 0.5f);
                });
        }
    }

    private void PullOut()
    {
        if (DirectPullOut)
        {
            SEManager.Instance.PlaySystemSE(SystemSE.拔根聲音LOOP);
            Tool_Coroutine.Instance.Delay(1, () => { SEManager.Instance.Stop(); SEManager.Instance.PlaySystemSE(SystemSE.物品產生); });
        }
        else
            SEManager.Instance.PlaySystemSE(SystemSE.物品產生);
        rb.isKinematic = false;
        rb.velocity = Vector3.up * 6 - Player.Instance.transform.forward;
        rb.AddTorque(Player.Instance.transform.forward * 100);
        Fruit.SetActive(true);
        EndMode();
    }

    private void EndMode(bool first = false)
    {
        StartCoroutine(D());

        IEnumerator D()
        {
            Player.Instance.Anim.applyRootMotion = true;
            Player.Instance.Anim.SetFloat("Speed", -.2f);
            Player_IKManager.Instance.ResumeSimpleIK(PlayerIK.Body, .5f, Ease.OutQuad);

            //Player.Instance.Rigidbody.isKinematic = false;
            yield return new WaitForSeconds(.1f);
            Player_IKManager.Instance.ResumeTwoHandIK();
            col.isTrigger = false;
            yield return new WaitForSeconds(1f);

            Player.Instance.Status = PlayerStatus.Moving;
            if (!first)
            {
                OnCompelete?.Invoke();
                yield return new WaitForSeconds(1.5f);
            }
            rb.isKinematic = true;
            col.enabled = first;
        }
    }

    private void EndModeForSerialRoot()
    {
        StartCoroutine(D());

        IEnumerator D()
        {
            Player.Instance.Anim.applyRootMotion = true;
            Player_IKManager.Instance.ResumeSimpleIK(PlayerIK.Body, .5f, Ease.OutQuad);
            yield return new WaitForSeconds(.1f);
            Player_IKManager.Instance.ResumeTwoHandIK();
            OnCompelete?.Invoke();
            gameObject.SetActive(false);
        }
    }
}