using UnityEngine;
using NaughtyAttributes;

public class Tool_CollisionSE : MonoBehaviour
{
    public LayerMask IgnoreLayrer;
    public bool UseFootSE;

    [HideIf("UseFootSE")]
    public AudioClip[] CollisionSE;

    [HideIf("UseFootSE")]
    public bool SpacialBlend = true;

    [HideIf("UseFootSE")]
    public float Volumn = 1;

    private void OnCollisionEnter(Collision collision)
    {
        if (IgnoreLayrer == (IgnoreLayrer | (1 << collision.gameObject.layer)))
            return;
        if (UseFootSE)
            SEManager.Instance.PlayStepSE();
        else if (SpacialBlend)
        {
            int randomIndex = 0;
            if (CollisionSE.Length > 1)
                randomIndex = Random.Range(0, CollisionSE.Length);
            AudioSource.PlayClipAtPoint(CollisionSE[randomIndex], transform.position, Volumn);
        }
        else
            SEManager.Instance.Play(CollisionSE, Volumn);
    }
}