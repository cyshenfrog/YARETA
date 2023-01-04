using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class Debug_Teleport : MonoBehaviour
{
    public Transform[] SnapPos;
    public UltEvent PreTeleport;
    public UltEvent AfterTeleport;

    // Update is called once per frame
    private void Update()
    {
        if (GameInput.Keyboard.GetKeyDown(KeyCode.Alpha1))
        {
            Teleport(0);
        }
        if (GameInput.Keyboard.GetKeyDown(KeyCode.Alpha2))
        {
            Teleport(1);
        }
        if (GameInput.Keyboard.GetKeyDown(KeyCode.Alpha3))
        {
            Teleport(2);
        }
        if (GameInput.Keyboard.GetKeyDown(KeyCode.Alpha4))
        {
            Teleport(3);
        }
        if (GameInput.Keyboard.GetKeyDown(KeyCode.Alpha5))
        {
            Teleport(4);
        }
        if (GameInput.Keyboard.GetKeyDown(KeyCode.Alpha6))
        {
            Teleport(5);
        }
        if (GameInput.Keyboard.GetKeyDown(KeyCode.Alpha7))
        {
            Teleport(6);
        }
        if (GameInput.Keyboard.GetKeyDown(KeyCode.Alpha8))
        {
            Teleport(7);
        }
        if (GameInput.Keyboard.GetKeyDown(KeyCode.Alpha9))
        {
            Teleport(8);
        }
        if (GameInput.Keyboard.GetKeyDown(KeyCode.Alpha0))
        {
            Teleport(9);
        }
    }

    private void Teleport(int id)
    {
        PreTeleport.Invoke();
        transform.position = SnapPos[id].position;
        AfterTeleport.Invoke();
    }
}