using UnityEngine;

[RequireComponent(typeof(Rigidbody))]
public class Tool_PhysicForce : MonoBehaviour
{
    public bool Lock { get; set; }
    private Rigidbody RB;
    public Vector3 ForceDir;
    public Vector3 Torque;
    public bool AddOnAwake;
    public bool AddOnFixUpdate;

    private void Awake()
    {
        RB = GetComponent<Rigidbody>();
        if (AddOnAwake)
            AddForce();
    }

    private void FixedUpdate()
    {
        if (!AddOnFixUpdate)
            return;

        AddForce();
    }

    public void AddForce()
    {
        if (Lock)
            return;
        RB.isKinematic = false;
        RB.AddForce(ForceDir);
        RB.AddTorque(Torque);
    }
}