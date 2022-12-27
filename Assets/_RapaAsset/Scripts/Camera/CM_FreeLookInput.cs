using System.Collections;
using Cinemachine;
using UnityEngine;
using UnityEngine.InputSystem;

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
            cam.m_XAxis.m_InputAxisValue += Mouse.current.delta.x.ReadValue();
            cam.m_YAxis.m_InputAxisValue += Mouse.current.delta.y.ReadValue();
        }

        if (Mouse.current.middleButton.wasPressedThisFrame)
            StartCoroutine(Recenter());
    }

    private IEnumerator Recenter()
    {
        cam.m_RecenterToTargetHeading.m_enabled = true;
        yield return new WaitForSeconds(2);
        cam.m_RecenterToTargetHeading.m_enabled = false;
    }
}