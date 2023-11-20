using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MinMapVector : MonoBehaviour
{
    public Transform Player;
    public Transform Camera;
    private Vector3 v;

    private void Start()
    {
        v = transform.localEulerAngles;
    }

    // Update is called once per frame
    private void Update()
    {
        v.z = Camera.eulerAngles.y - Player.eulerAngles.y;
        transform.localEulerAngles = v;
    }
}