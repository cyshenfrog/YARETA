using UnityEngine;

public class Scanner : MonoBehaviour
{
    private void OnTriggerEnter(Collider other)
    {
        Obj_Info info = other.GetComponent<Obj_Info>();
        if (!info)
            return;
        //UI_ScanTextFX.Instance.ShowTextFX(info, transform.position + Player.Instance.transform.forward * 0.3f);
    }
}