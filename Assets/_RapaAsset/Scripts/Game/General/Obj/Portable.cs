using System.Collections;
using DG.Tweening;
using RootMotion.FinalIK;
using UltEvents;
using UnityEngine;

[RequireComponent(typeof(InteractionObject))]
public class Portable : MonoBehaviour
{
    public Transform Pivot;
    public InteractionObject interaction;
    private Rigidbody rb;
    public Collider[] cols;
    public UltEvent OnTake;
    public UltEvent OnDrop;
    public bool DontDrop;
    public bool SoftSE;

    private void Start()
    {
        interaction = GetComponent<InteractionObject>();
        rb = GetComponent<Rigidbody>();
        cols = GetComponentsInChildren<Collider>();
    }

    private void OnEnable()
    {
        //transform.parent = null;
    }

    public void SetColliders(bool active)
    {
        foreach (var collider in cols)
        {
            collider.enabled = active;
        }
    }

    public void Take(bool pause = true)
    {
        StartCoroutine(_Take(pause));
    }

    public IEnumerator _Take(bool pause = true)
    {
        if (rb) rb.isKinematic = true;
        SetColliders(false);
        GameRef.CarringObj = this;
        if (pause)
            Player.Instance.Status = PlayerStatus.Wait;
        //Player.Instance.PlayerTrigger.UnRegist(GetComponent<Interactable>());
        //Player.Instance.Anim.SetTrigger("Pick");
        yield return new WaitForSeconds(0f);
        Player_IKManager.Instance.RotatePivot(transform, Pivot);
        Player_IKManager.Instance.PlayTwoHandIK(interaction, () =>
        {
            transform.parent = Player_IKManager.Instance.TwoHandHoldPosition;
            transform.localPosition = Vector3.zero;
            //if (GetComponent<Obj_Info>())
            //    UI_ScanTextFX.Instance.ShowTextFX(GetComponent<Obj_Info>(), transform.position);

            if (pause)
                Player.Instance.Status = PlayerStatus.Moving;
        });
        OnTake.Invoke();
    }

    public void Drop(bool resumeMoving = true)
    {
        StartCoroutine(_Drop(resumeMoving));
    }

    private IEnumerator _Drop(bool resumeMoving)
    {
        Player.Instance.Status = PlayerStatus.Wait;
        //transform.DOMove(Player.Instance.transform.forward * .6f, 0.19f)
        //    .SetRelative(true);
        if (rb)
        {
            rb.isKinematic = false;
            rb.AddForce(Player.Instance.transform.forward * 10 + Vector3.up);
        }
        yield return new WaitForSeconds(0.1f);

        transform.parent = null;
        SetColliders(true);
        Player_IKManager.Instance.ResumeTwoHandIK(() =>
        {
            GameRef.CarringObj = null;
            if (resumeMoving)
                Player.Instance.Status = PlayerStatus.Moving;
        });
        OnDrop.Invoke();
    }

    public void ToPocketAndDestroy()
    {
        transform.DOScale(0, .5f)
            .SetDelay(0.5f)
            .OnComplete(() =>
            {
                SEManager.Instance.PlaySystemSE(SystemSE.回收白石);
                Player.Instance.Anim.SetTrigger("Equip");
                Player_IKManager.Instance.ResumeTwoHandIK(() =>
                {
                    GameRef.CarringObj = null;
                    Player.Instance.Status = PlayerStatus.Moving;
                });
                gameObject.SetActive(false);
            });
    }

    public void ToPocket()
    {
        transform.DOScale(0, .5f)
            .SetDelay(0.5f)
            .OnComplete(() =>
            {
                SEManager.Instance.PlaySystemSE(SystemSE.回收白石);
                Player.Instance.Anim.SetTrigger("Equip");

                transform.parent = null;
                Player_IKManager.Instance.ResumeTwoHandIK(() =>
                {
                    GameRef.CarringObj = null;
                    Player.Instance.Status = PlayerStatus.Moving;
                });
            });
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioSource.PlayClipAtPoint(SEManager.Instance.GetRandomHitSE(SoftSE ? HitSEType.Soft : HitSEType.Hard), transform.position);
    }
}