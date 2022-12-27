using UnityEngine;

public class GooseManager : MonoBehaviour
{
    public SphereCollider[] WalkAreas;
    private Vector3 dist;
    private int id;
    private Vector3 shortest;

    public void UpdateNearestArea(Goose goose)
    {
        id = 0;
        for (int i = 0; i < WalkAreas.Length; i++)
        {
            dist = WalkAreas[i].transform.position - goose.transform.position;
            if (i == 0)
                shortest = dist;
            else
            {
                if (dist.magnitude < shortest.magnitude)
                {
                    shortest = dist;
                    id = i;
                }
            }
        }
        goose.FollowArea = WalkAreas[id];
    }
}