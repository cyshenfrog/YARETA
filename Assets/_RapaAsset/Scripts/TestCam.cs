using System.Collections;
using System.Collections.Generic;
using UnityEngine;

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
        if (Hinput.keyboard.W.pressed)
        {
            transform.position += transform.forward * Time.deltaTime * Speed;
        }
        if (Hinput.keyboard.S.pressed)
        {
            transform.position -= transform.forward * Time.deltaTime * Speed;
        }
        if (Hinput.keyboard.A.pressed)
        {
            transform.position -= transform.right * Time.deltaTime * Speed;
        }
        if (Hinput.keyboard.D.pressed)
        {
            transform.position += transform.right * Time.deltaTime * Speed;
        }
        if (Hinput.keyboard.Q.pressed)
        {
            transform.position -= transform.up * Time.deltaTime * Speed;
        }
        if (Hinput.keyboard.E.pressed)
        {
            transform.position += transform.up * Time.deltaTime * Speed;
        }

        transform.localEulerAngles += 100 * (Vector3.left * Hinput.mouse.delta.y + Vector3.up * Hinput.mouse.delta.x);

        if (Hinput.keyboard.leftShift.justPressed)
        {
            Speed *= 10;
        }
        else if (Hinput.keyboard.leftShift.justReleased)
        {
            Speed /= 10;
        }
    }
}