using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;
using Cinemachine;

public class CarrotSlot : MonoBehaviour
{
    public UnityEvent TriggerEvent;
    private static int count;

    public void OnTriggerEnter(Collider other)
    {
        if (!other.GetComponent<Obj_Info>())
            return;
        if (!other.GetComponent<Interactable>())
            return;
        if (other.GetComponent<Obj_Info>().ScanData == ScanDataEnum.蘿蔔 && other.GetComponent<Interactable>().enabled)
        {
            count++;
            GetComponent<Collider>().enabled = false;
            //other.GetComponent<Collider>().enabled = false;
            other.GetComponent<Interactable>().enabled = false;
            other.GetComponent<Rigidbody>().isKinematic = true;
            other.transform.DOMove(transform.position, 0.5f);
            if (count == 3)
            {
                SEManager.Instance.PlaySystemSE(SystemSE.機關解謎);
                TriggerEvent.Invoke();
                DOTween.To(() => ((CinemachineBasicMultiChannelPerlin)CameraMain.Instance.GetCurrentCam().GetComponentPipeline()[2]).m_AmplitudeGain,
                    x => ((CinemachineBasicMultiChannelPerlin)CameraMain.Instance.GetCurrentCam().GetComponentPipeline()[2]).m_AmplitudeGain = x, .2f, 1);

                DOTween.To(() => ((CinemachineBasicMultiChannelPerlin)CameraMain.Instance.GetCurrentCam().GetComponentPipeline()[2]).m_AmplitudeGain,
                        x => ((CinemachineBasicMultiChannelPerlin)CameraMain.Instance.GetCurrentCam().GetComponentPipeline()[2]).m_AmplitudeGain = x, 0, 1)
                    .SetDelay(4);
            }
        }
    }
}