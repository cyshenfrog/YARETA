using UnityEngine;

public class RT_MainWall : MonoBehaviour
{
    public GameObject[] FX;
    private bool[] levelPassed = new bool[4];
    private bool[] rotatorPassed = new bool[4];
    private bool[] slotPassed = new bool[4];

    private void Start()
    {
        rotatorPassed[0] = true;
    }

    public void RotatorPassed(int level)
    {
        if (levelPassed[level])
            return;
        rotatorPassed[level] = true;
        if (slotPassed[level])
        {
            LevelPassed(level);
            return;
        }
    }

    public void SlotPassed(int level)
    {
        if (levelPassed[level])
            return;
        slotPassed[level] = true;
        if (rotatorPassed[level])
        {
            LevelPassed(level);
            return;
        }
    }

    public void LevelPassed(int level)
    {
        FX[level].SetActive(true);
        levelPassed[level] = true;
        if (levelPassed[0] && levelPassed[1] && levelPassed[2] && levelPassed[3])
        {
            SetPlayerStatus(PlayerStatus.Wait);
        }
    }

    public void SetPlayerStatus(PlayerStatus status)
    {
        Player.Instance.Status = status;
    }
}