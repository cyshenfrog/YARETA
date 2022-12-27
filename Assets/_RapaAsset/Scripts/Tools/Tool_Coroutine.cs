using System.Collections;
using System;
using UnityEngine;

public class Tool_Coroutine : UnitySingleton_D<Tool_Coroutine>
{
    public void Delay(float second, Action callback)
    {
        StartCoroutine(d());
        IEnumerator d()
        {
            yield return new WaitForSeconds(second);
            callback?.Invoke();
        }
    }
}