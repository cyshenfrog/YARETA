using System.Collections;
using Cinemachine;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(CinemachineFreeLook))]
public class CM_FreeLookInput : MonoBehaviour
{
    private CinemachineFreeLook cam;

    // Start is called before the first frame update
    private void Start()
    {
        cam = GetComponent<CinemachineFreeLook>();
    }

    // Update is called once per frame
    private void Update()
    {
        cam.m_XAxis.m_InputAxisValue = GameInput.CameraMove.x;
        cam.m_YAxis.m_InputAxisValue = GameInput.CameraMove.y;

        if (GameInput.GetButtonDown(Actions.Touch))
            Recenter(0.2f);
    }

    public void Recenter(float duration)
    {
        DOTween.To(() => cam.m_YAxis.Value, x => cam.m_YAxis.Value = x, .6f, duration);
        DOTween.To(() => cam.m_XAxis.Value, x => cam.m_XAxis.Value = x, Tools.GetTrimmedEular(cam.LookAt.eulerAngles.y), duration);
    }
}