using System;
using System.Collections;
using UnityEngine;

public class PlayerAnimEvent : UnitySingleton_D<PlayerAnimEvent>
{
    public Action OnPlugRodEvent;
    public Action StandUpEvent;

    public void OnStep()
    {
        if (GameInput.IsMove)
            SEManager.Instance.PlayStepSE();
    }

    public void OnPlugRod()
    {
        OnPlugRodEvent?.Invoke();
    }

    public void OnStandUp()
    {
        StandUpEvent?.Invoke();
    }

    public void PlayGrassSound()
    {
        SEManager.Instance.PlaySystemSE(SystemSE.草叢聲長);
    }

    public void OnTripOnLeaf()
    {
        SEManager.Instance.PlaySystemSE(SystemSE.落地聲);
        SEManager.Instance.PlaySystemSE(SystemSE.草叢聲短);
    }
}