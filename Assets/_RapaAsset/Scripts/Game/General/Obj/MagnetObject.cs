using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class MagnetObject : MonoBehaviour
{
    public int ID;
    public int AttachPointNumber;
    public Dragable Dragable;
    public Transform AttachPos;
    public Transform Root;
    public GameObject AttachFX;
    public Rigidbody Physic;
    public ScanDataEnum CombinedData;
    private bool draging;
    private int count;
    private Vector3 initPos;
    private Quaternion initRot;
    public AudioSource AudioSource;

    private void Start()
    {
        initPos = Root.position;
        initRot = Root.rotation;
    }

    private void Update()
    {
        if (!draging)
            return;
        if (!GameInput.GetButton(Actions.Move) && AudioSource.isPlaying)
            AudioSource.Pause();
        if (GameInput.GetButton(Actions.Move) && !AudioSource.isPlaying)
            AudioSource.Play();
    }

    public void StartDrag()
    {
        draging = true;
    }

    public void FinishDrag()
    {
        draging = false;
        AudioSource.Pause();
    }

    public void TryAttach(MagnetObject obj)
    {
        if (obj.ID == ID)
        {
            count++;
            if (count >= 1)
            {
                CameraMain.Instance.SetCameraMode(CameraMode.TopView);
                Dragable.CamMode = CameraMode.TopView;
            }
            if (count >= AttachPointNumber * 3 + 1)
            {
                Attach(obj);
            }
        }
    }

    private void TryLeave(int id)
    {
        if (id == ID)
        {
            count--;
            if (count < 0) count = 0;
            if (count == 0)
            {
                CameraMain.Instance.SetCameraMode(CameraMode.Default);
                Dragable.CamMode = CameraMode.Default;
            }
        }
    }

    private void Attach(MagnetObject obj)
    {
        SEManager.Instance.PlaySystemSE(SystemSE.物品產生);
        draging = false;
        obj.draging = false;
        Physic.isKinematic = true;
        obj.Physic.isKinematic = true;
        obj.Dragable.GetComponentInChildren<Obj_Info>().GetScanData = CombinedData;
        Dragable.GetComponentInChildren<Obj_Info>().GetScanData = CombinedData;
        foreach (var item in Dragable.Model)
        {
            item.transform.parent = obj.AttachPos;
            item.transform.DOLocalMove(Vector3.zero, 1);
            item.transform.DOLocalRotate(Vector3.zero, .5f)
                .OnComplete(() =>
                {
                    obj.AttachFX.SetActive(true);
                    AttachFX.SetActive(true);
                });
        }
        obj.Dragable.FinishDrag();
        obj.Dragable.GetComponent<Collider>().enabled = false;
        Dragable.GetComponent<Collider>().enabled = false;
        Dragable.gameObject.SetActive(false);
        CameraMain.Instance.SetCameraMode(CameraMode.Default);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("KillY"))
        {
            Dragable.FinishDrag();
            Delay.Instance.Wait(.5f, d);
            void d()
            {
                Physic.velocity = Vector3.zero;
                Root.position = initPos;
                Root.rotation = initRot;
            }
        }
        if (!draging)
            return;
        other.GetComponent<MagnetObject>()?.TryAttach(this);
    }

    private void OnTriggerExit(Collider other)
    {
        if (!draging)
            return;
        other.GetComponent<MagnetObject>()?.TryLeave(ID);
    }
}