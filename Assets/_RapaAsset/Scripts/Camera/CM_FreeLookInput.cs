using System.Collections;
using Cinemachine;
using UnityEngine;

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
        cam.m_XAxis.m_InputAxisValue = 0;
        cam.m_YAxis.m_InputAxisValue = 0;

        cam.m_XAxis.m_InputAxisValue += GameInput.CameraMove.x;
        cam.m_YAxis.m_InputAxisValue += GameInput.CameraMove.y;
        if (Cursor.lockState == CursorLockMode.Locked)
        {
            cam.m_XAxis.m_InputAxisValue += Rewired.ReInput.controllers.Mouse.screenPositionDelta.x;
            cam.m_YAxis.m_InputAxisValue += Rewired.ReInput.controllers.Mouse.screenPositionDelta.y;
        }

        if (Rewired.ReInput.controllers.Mouse.GetButtonDown(2))
            StartCoroutine(Recenter());
    }

    private IEnumerator Recenter()
    {
        cam.m_RecenterToTargetHeading.m_enabled = true;
        yield return new WaitForSeconds(2);
        cam.m_RecenterToTargetHeading.m_enabled = false;
    }
}