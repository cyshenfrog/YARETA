using DG.Tweening;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RotationWall : MonoBehaviour
{
    public Pushable PushForward;
    public Pushable PushBack;
    public float Speed = 10f;
    public Action OnStop;
    private bool rotating;
    private bool isFront;

    // Start is called before the first frame update：
    private void Start()
    {
        PushForward.OnPush += OnPushForward;
        PushForward.OnStop += OnStopRotation;
        PushBack.OnPush += OnPushBack;
        PushBack.OnStop += OnStopRotation;
    }

    // Update is called once per frame
    private void Update()
    {
        if (rotating)
        {
            if (isFront)
            {
                transform.Rotate(Vector3.up, Speed * Time.deltaTime);
            }
            else
            {
                transform.Rotate(Vector3.up, -Speed * Time.deltaTime);
            }
        }
    }

    private void OnPushForward()
    {
        isFront = true;
        rotating = true;
    }

    private void OnPushBack()
    {
        isFront = false;
        rotating = true;
    }

    private void OnStopRotation()
    {
        rotating = false;
        OnStop?.Invoke();
    }
}