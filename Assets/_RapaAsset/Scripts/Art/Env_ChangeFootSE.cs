using UnityEngine;

public class Env_ChangeFootSE : MonoBehaviour
{
    public AudioClip[] WalkSEOverride;

    private void OnTriggerEnter(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SEManager.Instance.CurrentWalkSE = WalkSEOverride;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.CompareTag("Player"))
        {
            SEManager.Instance.ResetWalkSE();
        }
    }
}