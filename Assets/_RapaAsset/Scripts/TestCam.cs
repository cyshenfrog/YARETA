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
        if (GameInput.Keyboard.GetKey(KeyCode.W))
        {
            transform.position += transform.forward * Time.deltaTime * Speed;
        }
        if (GameInput.Keyboard.GetKey(KeyCode.S))
        {
            transform.position -= transform.forward * Time.deltaTime * Speed;
        }
        if (GameInput.Keyboard.GetKey(KeyCode.A))
        {
            transform.position -= transform.right * Time.deltaTime * Speed;
        }
        if (GameInput.Keyboard.GetKey(KeyCode.D))
        {
            transform.position += transform.right * Time.deltaTime * Speed;
        }
        if (GameInput.Keyboard.GetKey(KeyCode.Q))
        {
            transform.position -= transform.up * Time.deltaTime * Speed;
        }
        if (GameInput.Keyboard.GetKey(KeyCode.E))
        {
            transform.position += transform.up * Time.deltaTime * Speed;
        }

        transform.localEulerAngles += 100 * (Vector3.left * GameInput.Mouse.GetAxis(1) + Vector3.up * GameInput.Mouse.GetAxis(0));

        if (GameInput.Keyboard.GetKeyDown(KeyCode.LeftShift))
        {
            Speed *= 10;
        }
        else if (GameInput.Keyboard.GetKeyUp(KeyCode.LeftShift))
        {
            Speed /= 10;
        }
    }
}