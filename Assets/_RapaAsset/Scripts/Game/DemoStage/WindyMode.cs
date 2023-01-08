using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using Cinemachine.Utility;
using DG.Tweening;
using UltEvents;

public class WindyMode : MonoBehaviour
{
    public Transform StartPos;
    public UltEvent WindyEvent1;
    public UltEvent WindyEvent;
    public GameObject EventCam;
    public Transform Leaf;
    public CinemachineVirtualCamera FinalCam;

    public void StopShaking()
    {
        DOTween.To(() => ((CinemachineBasicMultiChannelPerlin)FinalCam.GetComponentPipeline()[2]).m_AmplitudeGain,
                x => ((CinemachineBasicMultiChannelPerlin)FinalCam.GetComponentPipeline()[2]).m_AmplitudeGain = x, 0, 1)
            .SetDelay(2);
        DOTween.To(() => ((CinemachineTransposer)FinalCam.GetComponentPipeline()[0]).m_XDamping,
                x => ((CinemachineTransposer)FinalCam.GetComponentPipeline()[0]).m_XDamping
                    = ((CinemachineTransposer)FinalCam.GetComponentPipeline()[0]).m_YDamping
                    = ((CinemachineTransposer)FinalCam.GetComponentPipeline()[0]).m_ZDamping
                    = x, 3, 3)
            .SetDelay(3);
    }

    public void StartWindy()
    {
        Delay.Instance.Wait(2, d0);
        void d0()
        {
            SEManager.Instance.PlaySystemSE(SystemSE.大鳥飛走);
            Player.Instance.Status = PlayerStatus.Static;
            UI_FullScreenFade.Instance.SetMovieMode(true);
            Player.Instance.transform.SetParent(StartPos);
            Player.Instance.transform.localPosition = Vector3.zero;
            Player.Instance.transform.localEulerAngles = Vector3.zero;
            Player.Instance.transform.DOLocalMoveZ(0.5f, 2)
                .SetEase(Ease.InOutSine)
                .SetRelative(true);
            Player.Instance.Anim.SetTrigger("Aganist");
            WindyEvent1.Invoke();
        }
        StartCoroutine(d());
        IEnumerator d()
        {
            yield return new WaitForSeconds(4);
            Player.Instance.Status = PlayerStatus.Wait;
            //Player.Instance.transform.DOLocalRotate(Vector3.up * 180, 2, RotateMode.FastBeyond360);
            Player.Instance.Anim.SetTrigger("MTWind");
            WindyEvent.Invoke();
            //Player.Instance.Status = PlayerStatus.Static;
        }
        Delay.Instance.Wait(6.5f, d2);
        void d2()
        {
            SEManager.Instance.PlaySystemSE(SystemSE.葉子起飛);
            Player.Instance.Status = PlayerStatus.Static;

            Player.Instance.transform.parent = Leaf;
        }
    }

    public void EndWind()
    {
        SEManager.Instance.PlaySystemSE(SystemSE.跌倒);
        Player.Instance.transform.parent = null;
        Player.Instance.transform.localEulerAngles = Vector3.up * 260;
        Player.Instance.Anim.SetTrigger("GetUp");
        Delay.Instance.Wait(3.5f, d2);
        void d2()
        {
            UI_FullScreenFade.Instance.SetMovieMode(false);
            EventCam.SetActive(false);
            CameraMain.Instance.Recenter(0);
            Player.Instance.Status = PlayerStatus.Moving;

            Obj_Info.Upload(24);
        };
    }
}