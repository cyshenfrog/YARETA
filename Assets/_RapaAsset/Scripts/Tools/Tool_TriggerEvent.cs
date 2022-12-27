using UltEvents;
using UnityEngine;
using NaughtyAttributes;

public class Tool_TriggerEvent : MonoBehaviour
{
    public bool Disable;

    [SerializeField]
    private bool _TargetPlayer;

    public bool TargetPlayer
    {
        get { return _TargetPlayer; }
        set
        {
            _TargetPlayer = value;
            if (value)
            {
                Target = Player.Instance.gameObject;
            }
        }
    }

    public bool TargetPortableObject;

    [HideIf("TargetPlayer")]
    public GameObject Target;

    [HideIf("TargetPlayer")]
    public GameObject[] Targets;

    private bool haveTarget { get { return Target || TargetPlayer; } }

    [HideIf("haveTarget")]
    public bool UseLayer = true;

    [HideIf("haveTarget")]
    public bool UseTag;

    [ShowIf("UseLayer")]
    public LayerMask TargetLayrer;

    [ShowIf("UseTag")]
    public string Tag;

    public UltEvent TriggerEvent;
    public UltEvent TriggerExitEvent;

    private void Start()
    {
        if (TargetPlayer)
        {
            Target = Player.Instance.gameObject;
        }
    }

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
        else
        {
            if (!UseLayer && !UseTag)
                return;

            if (UseLayer && TargetLayrer != (TargetLayrer | (1 << other.gameObject.layer)))
                return;

            if (UseTag && !other.CompareTag(Tag))
                return;

            TriggerEvent.Invoke();
        }
    }

    private void OnTriggerExit(Collider other)
    {
        if (Disable)
            return;
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
        else
        {
            if (!UseLayer && !UseTag)
                return;

            if (UseLayer && TargetLayrer != (TargetLayrer | (1 << other.gameObject.layer)))
                return;

            if (UseTag && !other.CompareTag(Tag))
                return;

            TriggerExitEvent.Invoke();
        }
    }
}