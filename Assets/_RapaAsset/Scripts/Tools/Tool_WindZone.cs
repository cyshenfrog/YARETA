using UnityEngine;

public class Tool_WindZone : MonoBehaviour
{
    public float Force;
    public Rigidbody OnlyTarget;

    private void OnTriggerStay(Collider other)
    {
        if (!OnlyTarget)
        {
            other.attachedRigidbody.AddForce(transform.forward * Force);
        }
        else
        {
            if (other.attachedRigidbody == OnlyTarget)
            {
                other.attachedRigidbody.AddForce(transform.forward * Force);
            }
        }
    }
}