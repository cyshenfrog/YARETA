using DG.Tweening;
using UnityEngine;

public class KlostkiStopper : MonoBehaviour
{
    public GameObject[] Wall;

    private void OnTriggerEnter(Collider other)
    {
        foreach (var item in Wall)
        {
            if (item == other.gameObject)
            {
                transform.parent.DORewind();
                break;
            }
        }
    }
}