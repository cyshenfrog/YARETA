using UltEvents;
using UnityEngine;

public class TreasureCell : MonoBehaviour
{
    public UltEvent OnCellUnlock;

    public void Unlock()
    {
        OnCellUnlock.Invoke();
    }
}