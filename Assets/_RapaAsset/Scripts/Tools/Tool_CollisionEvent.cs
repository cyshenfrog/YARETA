using UltEvents;
using UnityEngine;
using NaughtyAttributes;

public class Tool_CollisionEvent : MonoBehaviour
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

    [HideIf("TargetPlayer")]
    public GameObject Target;

    private bool haveTarget { get { return Target || TargetPlayer; } }

    [HideIf("haveTarget")]
    public bool UseLayer;

    [HideIf("haveTarget")]
    public bool UseTag;

    [ShowIf("UseLayer")]
    public LayerMask TargetLayrer;

    [ShowIf("UseTag")]
    public string Tag;

    public UltEvent CollisionEnterEvent;
    public UltEvent CollisionExitEvent;

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

    private void OnCollisionEnter(Collision collision)
    {
        if (Disable)
            return;
        if (Target)
        {
            if (collision.gameObject == Target)
                CollisionEnterEvent.Invoke();
        }
        else
        {
            if (!UseLayer && !UseTag)
                return;

            if (UseLayer && TargetLayrer != (TargetLayrer | (1 << collision.gameObject.layer)))
                return;

            if (UseTag && !collision.gameObject.CompareTag(Tag))
                return;

            CollisionEnterEvent.Invoke();
        }
    }

    private void OnCollisionExit(Collision collision)
    {
        if (Disable)
            return;

        if (Target)
        {
            if (collision.gameObject == Target)
                CollisionExitEvent.Invoke();
        }
        else
        {
            if (!UseLayer && !UseTag)
                return;

            if (UseLayer && TargetLayrer != (TargetLayrer | (1 << collision.gameObject.layer)))
                return;

            if (UseTag && !collision.gameObject.CompareTag(Tag))
                return;

            CollisionExitEvent.Invoke();
        }
    }
}