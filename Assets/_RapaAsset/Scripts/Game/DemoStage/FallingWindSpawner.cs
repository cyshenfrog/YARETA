using UnityEngine;

public class FallingWindSpawner : MonoBehaviour
{
    public GameObject[] Prefabs;
    public float TimeMin = 1;
    public float TimeMax = 4;
    private float cd;

    private void OnEnable()
    {
        cd = 1;
    }

    private void Init()
    {
        cd = Random.Range(TimeMin, TimeMax);
    }

    private void Update()
    {
        if (cd > 0)
        {
            cd -= Time.deltaTime;
            if (cd <= 0)
            {
                Instantiate(Prefabs[Random.Range(0, Prefabs.Length)], transform.position, Quaternion.identity);
                Init();
            }
        }
    }
}