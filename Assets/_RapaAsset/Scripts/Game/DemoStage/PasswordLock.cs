using DG.Tweening;
using UnityEngine;
using UnityEngine.Events;

public class PasswordLock : MonoBehaviour
{
    public GameObject[] ModeGroup;
    public RectTransform SelectUI;
    public Vector2[] SelectPosition;
    public int MaxRollSide = 3;
    public int[] Ans;
    public Transform[] LockRotater;
    public UnityEvent CorrectEvent;
    private int rollID;
    private bool active;
    private int[] currentAns;

    private void Start()
    {
        currentAns = new int[MaxRollSide];
    }

    public void StartMode()
    {
        GameInput.Cursorvisible = true;
        Player.Instance.Status = PlayerStatus.Wait;
        Player.Instance.SetModelActive(false);
        foreach (var item in ModeGroup)
        {
            item.SetActive(true);
        }
        enabled = active = true;
    }

    public void FinishMode()
    {
        GameInput.Cursorvisible = false;
        Player.Instance.Status = PlayerStatus.Moving;
        Player.Instance.SetModelActive(true);
        foreach (var item in ModeGroup)
        {
            item.SetActive(false);
        }
        enabled = active = false;
    }

    private void Update()
    {
        if (!active)
            return;

        if (GameInput.GetButtonDown(Actions.Down))
        {
            Rotate(false);
        }
        else if (GameInput.GetButtonDown(Actions.Up))
        {
            Rotate(true);
        }
        else if (GameInput.GetButtonDown(Actions.Right))
        {
            ChangeRoll(true);
        }
        else if (GameInput.GetButtonDown(Actions.Left))
        {
            ChangeRoll(false);
        }
    }

    public void Rotate(bool up)
    {
        if (!active)
            return;

        SEManager.Instance.PlaySystemSE(SystemSE.轉動拉霸);
        active = false;
        currentAns[rollID] += up ? 1 : -1;

        if (currentAns[rollID] >= MaxRollSide)
        {
            currentAns[rollID] = 0;
        }
        if (currentAns[rollID] < 0)
        {
            currentAns[rollID] = 2;
        }
        LockRotater[rollID].DOLocalRotate(Vector3.right * (360f / MaxRollSide) * (up ? -1 : 1), 1, RotateMode.LocalAxisAdd)
            .SetRelative()
            .OnComplete(() => CheckAns());
    }

    private void CheckAns()
    {
        for (int i = 0; i < MaxRollSide; i++)
        {
            if (currentAns[i] != Ans[i])
            {
                active = true;
                return;
            }
        }
        CorrectEvent.Invoke();
    }

    private void ChangeRoll(bool Right)
    {
        rollID += Right ? 1 : -1;
        if (rollID >= 3)
        {
            rollID = 0;
        }
        if (rollID < 0)
        {
            rollID = 2;
        }
        SelectRow(rollID);
    }

    public void SelectRow(int ID)
    {
        rollID = ID;
        SelectUI.anchoredPosition = SelectPosition[ID];
    }
}