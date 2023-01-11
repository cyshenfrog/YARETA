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
    private GameObject[] cameras;

    public CameraMode CurrentCameraMode;
    public bool Lock;

    private void Start()
    {
        cameras = new GameObject[3] { MainFreeLookCam.gameObject, AimCam.gameObject, TopDownCam.gameObject };
    }

    // Update is called once per frame
    private void Update()
    {
        if (Lock)
            return;
        if (CurrentCameraMode != CameraMode.Default)
            return;
        MainFreeLookCam.m_XAxis.m_InputAxisValue = GameInput.CameraMove.x;
        MainFreeLookCam.m_YAxis.m_InputAxisValue = GameInput.CameraMove.y;

        if (GameInput.GetButtonDown(Actions.Touch))
            Recenter(0.2f);
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
    }
}