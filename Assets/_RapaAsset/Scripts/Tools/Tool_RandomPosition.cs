using UnityEngine;

public class Tool_RandomPosition : MonoBehaviour
{
    public Transform Target;
    public BoxCollider Area;
    public float UpdateFreq = 1;
    // Start is called before the first frame update
    void Start()
    {
        InvokeRepeating("UpdatePosition", 0, UpdateFreq);
    }

    // Update is called once per frame
    void UpdatePosition()
    {
        Target.position = transform.position + Area.center +
            new Vector3(
                Random.Range(-Area.size.x * 0.5f, Area.size.x * 0.5f), 
                Random.Range(-Area.size.y * 0.5f, Area.size.y * 0.5f), 
                Random.Range(-Area.size.z * 0.5f, Area.size.z * 0.5f));
    }
}
