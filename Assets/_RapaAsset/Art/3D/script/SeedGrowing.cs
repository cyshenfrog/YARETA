using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SeedGrowing : MonoBehaviour
{
    SkinnedMeshRenderer SkinnedMeshRenderer;
    Mesh skinnedMesh;
    float growingValue = 0f;

    float cutOffValue = 2f;

    // Start is called before the first frame update

    private void Awake()
    {
        SkinnedMeshRenderer = GetComponent<SkinnedMeshRenderer>();
        skinnedMesh = GetComponent<SkinnedMeshRenderer>().sharedMesh;
    }
    void Start()
    {
      
        



    }

    // Update is called once per frame
    void Update()
    {
        growingValue += Time.deltaTime * 40f;
        Debug.Log(growingValue);
        if (growingValue > 250)
        {
            return;
        }
        else
        {
            if (growingValue < 100)
            {
                SkinnedMeshRenderer.SetBlendShapeWeight(0, growingValue);

            }
            else
            {
                SkinnedMeshRenderer.SetBlendShapeWeight(0, 200f - growingValue);
                SkinnedMeshRenderer.SetBlendShapeWeight(1, growingValue - 100f);
                cutOffValue -= Time.deltaTime * 0.6f;
                this.GetComponent<Renderer>().material.SetFloat("_CutoffHeight", cutOffValue);
            }

        }

       


    }
}
