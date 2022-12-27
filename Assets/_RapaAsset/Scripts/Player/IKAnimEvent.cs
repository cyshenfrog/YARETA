using UnityEngine.Events;

public class IKAnimEvent : UnitySingleton<IKAnimEvent>
{
    public UnityEvent OnPullEvent;
    public UnityEvent OnReleaseEvent;

    public void OnPull()
    {
        OnPullEvent.Invoke();
    }

    public void OnRelease()
    {
        OnReleaseEvent.Invoke();
    }
}