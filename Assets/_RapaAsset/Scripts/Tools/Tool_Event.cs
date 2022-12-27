using System.Collections;
using UltEvents;
using UnityEngine;
using NaughtyAttributes;

public class Tool_Event : MonoBehaviour
{
    public bool PlayOnEnable = true;
    public bool SetOnEnable { set { PlayOnEnable = value; } }
    public float Delay;
    public bool Loop;

    [ShowIf("Loop")]
    public int LoopCircle = 1;

    [ShowIf("Loop")]
    public float LoopGap = 1;

    private bool breakFlag;
    private int count;

    public UltEvent DelayEvent;
    public bool PlayOnce;
    private IEnumerator delay;

    private void OnEnable()
    {
        if (PlayOnEnable)
        {
            PlayEvent();
        }
    }

    public void PlayEvent()
    {
        if (PlayOnce && delay != null)
            return;
        breakFlag = false;
        delay = _PlayEvent();
        StartCoroutine(delay);
    }

    public void StopEvent()
    {
        if (delay != null)
        {
            breakFlag = true;
            StopCoroutine(delay);
            delay = null;
        }
    }

    public IEnumerator _PlayEvent()
    {
        yield return new WaitForSeconds(Delay);
        if (breakFlag)
        {
            breakFlag = false;
            yield break;
        }
        if (Loop)
        {
            while (count < LoopCircle || LoopCircle < 0)
            {
                if (breakFlag)
                {
                    breakFlag = false;
                    yield break;
                }
                count++;
                DelayEvent.Invoke();
                yield return new WaitForSeconds(LoopGap);
            }
        }
        else
            DelayEvent.Invoke();
    }
}