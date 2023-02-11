using System.Collections;
using Cinemachine;
using UnityEngine;
using DG.Tweening;

public enum CameraMode
{
    Default,
    Aim,
    TopView,
}

public class CameraMain : UnitySingleton_D<CameraMain>
{
    public CinemachineFreeLook MainFreeLookCam;
    public CinemachineVirtualCamera AimCam;
    public CinemachineVirtualCamera TopDownCam;
    private CinemachinePOV aimCamPOV;
    private GameObject[] cameras;
    public CameraMode CurrentCameraMode;
    public bool Lock;

    private void Start()
    {
        cameras = new GameObject[3] { MainFreeLookCam.gameObject, AimCam.gameObject, TopDownCam.gameObject };
        aimCamPOV = (CinemachinePOV)AimCam.GetComponentPipeline()[1];
    }

    // Update is called once per frame
    private void Update()
    {
        if (Lock)
            return;
        switch (CurrentCameraMode)
        {
            case CameraMode.Default:
                DefaultCamRotation();
                break;

            case CameraMode.Aim:
                AimmingCamRotation();
                break;

            case CameraMode.TopView:
                break;

            default:
                break;
        }

        if (GameInput.GetButtonDown(Actions.Touch))
            Recenter(0.2f);
    }

    private void DefaultCamRotation()
    {
        MainFreeLookCam.m_XAxis.m_InputAxisValue = GameInput.CameraMove.x;
        MainFreeLookCam.m_YAxis.m_InputAxisValue = GameInput.CameraMove.y;
    }

    private void AimmingCamRotation()
    {
        aimCamPOV.m_HorizontalAxis.m_InputAxisValue = GameInput.CameraMove.x;
        aimCamPOV.m_VerticalAxis.m_InputAxisValue = GameInput.CameraMove.y;
    }

    public void Recenter(float duration)
    {
        DOTween.To(() => MainFreeLookCam.m_YAxis.Value, x => MainFreeLookCam.m_YAxis.Value = x, .5f, duration);
        DOTween.To(() => MainFreeLookCam.m_XAxis.Value, x => MainFreeLookCam.m_XAxis.Value = x, Tools.GetTrimmedEular(MainFreeLookCam.LookAt.eulerAngles.y), duration);
    }

    public void SetCameraMode(CameraMode mode)
    {
        cameras[(int)CurrentCameraMode].SetActive(false);
        CurrentCameraMode = mode;
        cameras[(int)CurrentCameraMode].SetActive(true);
        switch (mode)
        {
            case CameraMode.Default:
                break;

            case CameraMode.Aim:
                aimCamPOV.m_HorizontalRecentering.m_enabled = true;
                aimCamPOV.m_VerticalRecentering.m_enabled = true;
                Delay.Instance.Wait(0.1f, () =>
                 {
                     aimCamPOV.m_HorizontalRecentering.m_enabled = false;
                     aimCamPOV.m_VerticalRecentering.m_enabled = false;
                     aimCamPOV.m_HorizontalAxis.Value += 10;
                 });
                break;

            case CameraMode.TopView:
                break;

            default:
                break;
        }
    }
}