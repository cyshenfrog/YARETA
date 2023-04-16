using UnityEngine;

public class GlowMaterialTrigger : MonoBehaviour
{
    public Shader targetShader;
    private Renderer rend;
    private MaterialPropertyBlock mpb;
    private int count;
    private string[] v3Names = new string[5] { "_position", "_position1", "_position2", "_position3", "_position4" };
    private int delayCounter;

    private void Start()
    {
        mpb = new MaterialPropertyBlock();
    }

    private void Update()
    {
        if (!rend)
            return;
        if (rend.material.shader != targetShader)
            return;
        delayCounter++;
        if (delayCounter >= 4)
        {
            WritePosToMaterial();
            delayCounter = 0;
        }
    }

    private void WritePosToMaterial()
    {
        mpb.SetVector(v3Names[count], transform.position);
        rend.SetPropertyBlock(mpb);
        count++;
        if (count >= v3Names.Length)
            count = 0;
    }

    private void ClearMatVector()
    {
        if (!rend)
            return;
        foreach (var name in v3Names)
        {
            mpb.SetVector(name, Vector3.zero);
            rend.SetPropertyBlock(mpb);
        }
        rend = null;
    }

    public void SetTarget(Component target)
    {
        if (!target)
        {
            ClearMatVector();
            return;
        }
        if (rend == target.GetComponent<Renderer>())
            return;
        else
            ClearMatVector();

        rend = target.GetComponent<Renderer>();
        if (!rend)
            return;
        if (rend.material.shader != targetShader)
            return;
        count = 0;
        mpb = new MaterialPropertyBlock();
    }

    public void SpawnGuideFX()
    {
        //PrototypeMain.Instance.SpawnGuideFX(transform);
    }
}