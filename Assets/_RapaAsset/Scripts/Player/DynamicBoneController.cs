using System.Collections;
using UnityEngine;

public class DynamicBoneController : MonoBehaviour
{
    private DynamicBone[] DBs;
    private bool b;
    private Vector3 windForce;
    private Coroutine windLoop;

    // Start is called before the first frame update
    private void Start()
    {
        DBs = GetComponents<DynamicBone>();
    }

    public void SetWindForce(Vector3 force)
    {
        if (force == Vector3.zero)
        {
            StopCoroutine(windLoop);
            b = false;
        }
        else
        {
            windLoop = StartCoroutine(WindLoop());
        }
        windForce = force;
    }

    private IEnumerator WindLoop()
    {
        b = !b;

        foreach (var item in DBs)
        {
            item.m_Force = b ? windForce : windForce * 0.5f;
        }
        yield return new WaitForSeconds(b ? Random.Range(1, 6) : 1);
        windLoop = StartCoroutine(WindLoop());
    }
}