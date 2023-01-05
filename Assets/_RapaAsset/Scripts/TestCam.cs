using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Rewired;

public class TestCam : MonoBehaviour
{
    public float Speed = 10;

    // Start is called before the first frame update
    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    // Update is called once per frame
    private void Update()
    {
        if (ReInput.controllers.Keyboard.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * Time.deltaTime * Speed;
        }
        if (ReInput.controllers.Keyboard.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * Time.deltaTime * Speed;
        }
        if (ReInput.controllers.Keyboard.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * Time.deltaTime * Speed;
        }
        if (ReInput.controllers.Keyboard.GetKey(KeyCode.D))
        {
            transform.position += transform.right * Time.deltaTime * Speed;
        }
        if (ReInput.controllers.Keyboard.GetKey(KeyCode.Q))
        {
            transform.position -= transform.up * Time.deltaTime * Speed;
        }
        if (ReInput.controllers.Keyboard.GetKey(KeyCode.E))
        {
            transform.position += transform.up * Time.deltaTime * Speed;
        }

        transform.localEulerAngles += 100 * (Vector3.left * ReInput.controllers.Mouse.screenPositionDelta.x + Vector3.up * ReInput.controllers.Mouse.screenPositionDelta.y);

        if (ReInput.controllers.Keyboard.GetKeyDown(KeyCode.LeftShift))
        {
            Speed *= 10;
        }
        else if (ReInput.controllers.Keyboard.GetKeyUp(KeyCode.LeftShift))
        {
            Speed /= 10;
        }
    }
}