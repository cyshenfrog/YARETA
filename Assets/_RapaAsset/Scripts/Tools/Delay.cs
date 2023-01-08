using System.Collections;
using System;
using UnityEngine;

public class Delay : UnitySingleton_D<Delay>
{
    public void Wait(float second, Action callback)
    {
        StartCoroutine(d());
        IEnumerator d()
        {
            yield return new WaitForSeconds(second);
            callback?.Invoke();
        }
    }
}