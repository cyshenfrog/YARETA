using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UltEvents;

public class KingLotus : MonoBehaviour
{
    public Transform StartPosition;
    public SkinnedMeshRenderer LotusModel;
    public UltEvent ResetEvent;
    public Transform BridgeZone;
    public UltEvent BridgeEvent;
    private Tween tween;
    private Rigidbody rb;
    private bool floating;

    private void Start()
    {
        rb = GetComponent<Rigidbody>();
    }

    private void Update()
    {
        if (!rb)
            return;
        if (!floating)
            return;
        if ((StartPosition.position - transform.position).magnitude < 2f)
        {
            StopFloating();
            return;
        }
        rb.AddForce((StartPosition.position - transform.position) * Time.deltaTime * 3);
    }

    public void TryFloatTo()
    {
        floating = true;
    }

    public void StopFloating()
    {
        floating = false;
        transform.DOKill();
        ResetEvent.Invoke();
    }

    public void SetMovingTarget(Transform t)
    {
        StartPosition = t;
    }

    public void OnDrag()
    {
        floating = false;
        rb.isKinematic = true;
        transform.DOKill();
        tween.Kill();
        tween = DOTween.To(() => LotusModel.GetBlendShapeWeight(0), x => LotusModel.SetBlendShapeWeight(0, x), 100f, 1f)
            .SetEase(Ease.InCubic);
    }

    public void OnRelease()
    {
        tween.Kill();
        tween = DOTween.To(() => LotusModel.GetBlendShapeWeight(0), x => LotusModel.SetBlendShapeWeight(0, x), 0f, 1f)
            .SetEase(Ease.InCubic);
        if (StartPosition)
        {
            float t = (transform.position - StartPosition.position).magnitude;
            transform.DOKill();
            transform.DOMoveY(StartPosition.position.y, 0.2f).
                OnComplete(() =>
                {
                    //TryFloatTo();
                    //ResetEvent.Invoke();
                    transform.DOMove(StartPosition.position, t).
                        SetDelay(0.5f)
                        .SetEase(Ease.Linear)
                        .OnComplete(() =>
                        {
                            ResetEvent.Invoke();
                            rb.isKinematic = false;
                        });
                    transform.DORotateQuaternion(StartPosition.rotation, t).
                        SetDelay(0.5f);
                });
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform == BridgeZone)
        {
            ResetEvent.Clear();
            BridgeEvent.Invoke();
            Destroy(rb);
            GetComponent<Dragable>().FinishDrag();
            SetMovingTarget(BridgeZone);
            OnRelease();
            BridgeZone.gameObject.SetActive(false);
            GetComponent<Interactable>().enabled = false;
        }
    }
}