using UnityEngine;
using Obi;

public class Rope : UnitySingleton_DR<Rope>
{
    public ObiParticleGroup group;
    public float intensity = 1;
    public ObiSolver Solver;
    public ObiRope TargetRope;
    public ObiRopeCursor cursor;

    private void Start()
    {
        transform.parent = null;
    }

    private void Update()
    {
        if (Input.GetKey(KeyCode.UpArrow))
        {
            if (TargetRope.restLength > 6.5f)
                cursor.ChangeLength(TargetRope.restLength - 1f * Time.deltaTime);
        }

        if (Input.GetKey(KeyCode.DownArrow))
        {
            cursor.ChangeLength(TargetRope.restLength + 1f * Time.deltaTime);
        }
    }

    public void SetRopeLength(float deltaLength)
    {
        cursor.ChangeLength(TargetRope.restLength + deltaLength);
    }
}