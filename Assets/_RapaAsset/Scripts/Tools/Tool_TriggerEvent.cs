using Sirenix.OdinInspector;
using UltEvents;
using UnityEngine;

public enum FilterType
{
    None,
    Layer,
    Tag,
    PlayerTag,
    GameObject,
}

public class Tool_TriggerEvent : MonoBehaviour
{
    public FilterType Filter;
    public bool Disable;

    public bool TargetPortableObject;

    [ShowIf("Filter", FilterType.GameObject)]
    public GameObject Target;

    [ShowIf("Filter", FilterType.GameObject)]
    public GameObject[] Targets;

    [ShowIf("Filter", FilterType.Layer)]
    public LayerMask TargetLayrer;

    [ShowIf("Filter", FilterType.Tag)]
    public string Tag;

    public UltEvent TriggerEvent;
    public UltEvent TriggerExitEvent;

    public void SetDisable(bool b)
    {
        Disable = b;
    }

    private void OnTriggerEnter(Collider other)
    {
        if (Disable)
            return;
        if (TargetPortableObject)
        {
            if (other.GetComponentInChildren<Portable>())
            {
                TriggerEvent.Invoke();
            }
        }
        switch (Filter)
        {
            case FilterType.None:
                break;

            case FilterType.Layer:
                if (TargetLayrer == (TargetLayrer | (1 << other.gameObject.layer)))
                    TriggerEvent.Invoke();

                break;

            case FilterType.Tag:
                if (other.CompareTag(Tag))
                    TriggerEvent.Invoke();
                break;

            case FilterType.PlayerTag:
                if (other.CompareTag("Player"))
                    TriggerEvent.Invoke();
                break;

            case FilterType.GameObject:
                if (Target)
                {
                    if (other.gameObject == Target)
                        TriggerEvent.Invoke();
                }
                else if (Targets.Length != 0)
                {
                    foreach (var item in Targets)
                    {
                        if (other.gameObject == item)
                            TriggerEvent.Invoke();
                    }
                }
                break;

            default:
                break;
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Disable)
            return;
        switch (Filter)
        {
            case FilterType.None:
                break;

            case FilterType.Layer:
                if (TargetLayrer == (TargetLayrer | (1 << other.gameObject.layer)))
                    TriggerExitEvent.Invoke();
                break;

            case FilterType.Tag:
                if (other.CompareTag(Tag))
                    TriggerExitEvent.Invoke();
                break;

            case FilterType.PlayerTag:
                if (other.CompareTag("Player"))
                    TriggerExitEvent.Invoke();
                break;

            case FilterType.GameObject:
                if (Target)
                {
                    if (other.gameObject == Target)
                        TriggerExitEvent.Invoke();
                }
                else if (Targets.Length != 0)
                {
                    foreach (var item in Targets)
                    {
                        if (other.gameObject == item)
                            TriggerExitEvent.Invoke();
                    }
                }
                break;

            default:
                break;
        }
    }
}