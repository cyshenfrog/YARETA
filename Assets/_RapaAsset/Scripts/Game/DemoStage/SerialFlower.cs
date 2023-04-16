using System;
using UnityEngine;
using UnityEngine.Events;
using DG.Tweening;

public class SerialFlower : MonoBehaviour
{
    public int ID;
    public Transform Flower;
    public GameObject PopFx;
    public UnityEvent OnTouched;
    public bool FinalFlower;
    private static Action<int> Next;
    private bool inited;

    private void Start()
    {
        if (ID == 0)
        {
            Init(0);
        }
        else
            Next += Init;
    }

    private void Init(int id)
    {
        if (ID == id)
        {
            inited = true;
            Flower.DOScale(Vector3.one * 2, 0.6f)
                .SetEase(Ease.OutBack)
                .OnComplete(() =>
                {
                    if (ID != 0)
                        SEManager.Instance.PlaySystemSE(SystemSE.物品產生);
                });

            Flower.DOLocalMoveY(0.3f, 0.3f)
                .SetEase(Ease.OutBack);
            PopFx.SetActive(true);
        }
    }

    public void Touch()
    {
        if (!inited)
            return;
        SEManager.Instance.PlaySystemSE(SystemSE.逃跑花);
        Flower.DOScale(new Vector3(0, 1, 1), 0.3f)
            .OnComplete(() =>
            {
                Next(ID + 1);
            });
        OnTouched.Invoke();
        if (FinalFlower)
        {
            PopFx.SetActive(false);
            PopFx.SetActive(true);
        }
        GetComponent<Collider>().enabled = false;
    }
}