using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class ShakableTree : MonoBehaviour
{
    public int PunchTimes = -1;
    public UnityEvent PunchEvent;

    public void Punch()
    {
        if (PunchTimes == 0)
            return;
        SEManager.Instance.PlaySystemSE(SystemSE.搖樹);
        transform.DOShakePosition(0.5f, new Vector3(0.1f, 0, 0.1f))
            .SetEase(Ease.InQuad);
        if (PunchTimes > 0)
            PunchTimes--;
        if (PunchTimes == 0)
        {
            SEManager.Instance.PlaySystemSE(SystemSE.樹上掉落);
            PunchEvent.Invoke();
        }
    }

    public void WalkToAndPunch()
    {
        Player.Instance.WalkTo(transform.position - Player.Instance.transform.forward * .8f, then);
        void then()
        {
            Player.Instance.Status = PlayerStatus.Moving;
            Player.Instance.transform.DOLookAt(transform.position, 0.5f)
                .OnComplete(Punch);
        }
    }

    public void StationFall(Transform FallTarget)
    {
        Player.Instance.WalkBack();
        SEManager.Instance.PlaySystemSE(SystemSE.樹上柱子落下);
        Player.Instance.transform.DOLookAt(FallTarget.position, .5f, AxisConstraint.Y)
            .SetDelay(1f)
            .OnPlay(() => { Player.Instance.BackFall(); });
    }
}