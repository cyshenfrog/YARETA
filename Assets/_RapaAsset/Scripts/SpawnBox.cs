using UnityEngine;

public class SpawnBox : MonoBehaviour
{
    public GameObject[] Tags;
    public int ID;

    // Update is called once per frame
    private void Update()
    {
        //if (Hinput.anyGamepad.dPad.right.justPressed)
        //{
        //    RaycastHit hit;
        //    if (Physics.Raycast(transform.position + Vector3.up, transform.TransformDirection(Vector3.forward), out hit, 1))
        //    {
        //        if (hit.transform.CompareTag("Pin"))
        //        {
        //            Destroy(hit.transform.gameObject);
        //        }
        //    }
        //    else
        //        Instantiate(Tags[ID], transform.position + transform.forward + Vector3.up, Quaternion.identity, null);
        //}
        //if (Hinput.anyGamepad.rightBumper)
        //{
        //    ID++;
        //    if (ID >= Tags.Length)
        //    {
        //        ID = 0;
        //    }
        //}
        //if (Hinput.anyGamepad.leftBumper)
        //{
        //    ID--;
        //    if (ID < 0)
        //    {
        //        ID = Tags.Length - 1;
        //    }
        //}
    }
}