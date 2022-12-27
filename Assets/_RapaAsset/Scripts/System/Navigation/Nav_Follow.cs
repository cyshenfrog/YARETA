using UnityEngine;
using UnityEngine.AI;

[RequireComponent(typeof(NavMeshAgent))]
public class Nav_Follow : MonoBehaviour
{
    private Vector3 target;

    public Vector3 Target
    {
        get { return target; }
        set { target = value; UpdateTarget(); }
    }

    public bool KeepFollow;
    private bool pause;
    private NavMeshAgent agent;
    private float updateCD = .5f;

    private void Start()
    {
        agent = GetComponent<NavMeshAgent>();
    }

    private void Update()
    {
        if (!KeepFollow || pause)
            return;
        updateCD -= Time.deltaTime;
        if (updateCD < 0)
        {
            UpdateTarget();
            updateCD = .5f;
        }
    }

    public void UpdateTarget()
    {
        if (pause)
            return;
        agent.destination = target;
    }

    public void StartAI()
    {
        pause = false;
        agent.enabled = true;
    }

    public void PauseAI()
    {
        pause = true;
        agent.enabled = false;
    }
}