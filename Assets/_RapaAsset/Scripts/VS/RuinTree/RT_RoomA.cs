using System;
using UltEvents;
using UnityEngine;

public class RT_RoomA : MonoBehaviour
{
    public RotationWall Level1;
    public RotationWall Level2;
    public float TargetAngle1 = 30;
    public float TargetAngle2 = -30;
    public GameObject EndingCutscene;
    public Pushable[] Pushables;
    public UltEvent OnCompelete;
    private bool Level1Win;
    private bool Level2Win;
    private bool AllClear;

    // Start is called before the first frame update：
    private void Start()
    {
        Level1.OnStop += CheckAns1;
        Level2.OnStop += CheckAns2;
    }

    private void CheckAns1()
    {
        if (MathF.Abs(Level1.transform.localEulerAngles.y - TargetAngle1) < 1f)
            OnLevel1Win();
    }

    private void CheckAns2()
    {
        if (MathF.Abs(Level2.transform.localEulerAngles.y - TargetAngle2) < 1f)
            OnLevel2Win();
    }

    private void OnLevel1Win()
    {
        Level1Win = true;
        if (Level2Win)
            OnAllClear();
    }

    private void OnLevel2Win()
    {
        Level2Win = true;
        if (Level1Win)
            OnAllClear();
    }

    private void OnAllClear()
    {
        if (AllClear)
            return;
        foreach (var pushable in Pushables)
        {
            pushable.Stop();
        }
        OnCompelete.Invoke();
        AllClear = true;
        EndingCutscene.SetActive(true);
        Player.Instance.Status = PlayerStatus.Wait;
        Delay.Instance.Wait(8, () =>
        {
            Player.Instance.Status = PlayerStatus.Moving;
        });
    }
}