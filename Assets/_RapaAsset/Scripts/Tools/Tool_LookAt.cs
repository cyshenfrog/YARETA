using UnityEngine;

public class Tool_LookAt : MonoBehaviour
{
    public Transform Target;
    public Vector3 WorldUp = Vector3.up;

    private void Update()
    {
        transform.LookAt(Target, WorldUp);
    }
}