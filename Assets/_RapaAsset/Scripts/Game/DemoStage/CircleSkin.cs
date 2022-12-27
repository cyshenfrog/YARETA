using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;

public class CircleSkin : MonoBehaviour
{
    public DOTweenAnimation[] Tweens;
    public GameObject StartGroup;

    public void StartMode()
    {
        StartGroup.SetActive(true);
    }

    public void LeaveMode()
    {
        foreach (var item in Tweens)
        {
            item.DOPlayBackwards();
        }
    }
}