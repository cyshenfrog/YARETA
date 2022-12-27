using UnityEngine;

public class Tool_LookCam : MonoBehaviour
{
    private Camera cam;

    // Start is called before the first frame update
    private void Start()
    {
        cam = Camera.main;
    }

    // Update is called once per frame
    private void LateUpdate()
    {
        transform.rotation = cam.transform.rotation;
    }
}