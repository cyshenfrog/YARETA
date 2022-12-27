using UnityEngine;
using System.Collections;

public class Goose : MonoBehaviour
{
    public AudioSource AudioSource;
    public Nav_Follow AI;
    public SphereCollider FollowArea;

    public Vector2 WalkGap;

    private float cd = 5;

    // Update is called once per frame
    private void Update()
    {
        cd -= Time.deltaTime;
        if (cd < 0)
        {
            cd = Random.Range(WalkGap.x, WalkGap.y);
            RandomMove();
        }
    }

    private void Start()
    {
        StartCoroutine(RandHonk());
    }
    IEnumerator RandHonk()
    {
        Honk();
        yield return new WaitForSeconds(Random.value * 10 + 10);
        StartCoroutine(RandHonk());
    }
    public void Honk()
    {
        AudioSource.pitch = Random.Range(0.9f,1.1f);
        AudioSource.Play();
    }
    private void RandomMove()
    {
        AI.Target = FollowArea.transform.position + new Vector3(Random.Range(-FollowArea.radius, FollowArea.radius), 0, Random.Range(-FollowArea.radius, FollowArea.radius));
    }
}