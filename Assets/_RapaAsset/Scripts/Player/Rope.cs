using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Obi;
using static NPOI.HSSF.Util.HSSFColor;

public class Rope : MonoBehaviour
{
    public ObiParticleGroup group;
    public float intensity = 1;
    public ObiSolver Solver;
    public ObiRope TargetRope;
    public ObiRopeCursor cursor;

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
}