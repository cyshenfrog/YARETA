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
        if (Hinput.keyboard.digit1.justPressed)
        {
            Teleport(0);
        }
        if (Hinput.keyboard.digit2.justPressed)
        {
            Teleport(1);
        }
        if (Hinput.keyboard.digit3.justPressed)
        {
            Teleport(2);
        }
        if (Hinput.keyboard.digit4.justPressed)
        {
            Teleport(3);
        }
        if (Hinput.keyboard.digit5.justPressed)
        {
            Teleport(4);
        }
        if (Hinput.keyboard.digit6.justPressed)
        {
            Teleport(5);
        }
        if (Hinput.keyboard.digit7.justPressed)
        {
            Teleport(6);
        }
        if (Hinput.keyboard.digit8.justPressed)
        {
            Teleport(7);
        }
        if (Hinput.keyboard.digit9.justPressed)
        {
            Teleport(8);
        }
        if (Hinput.keyboard.digit0.justPressed)
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