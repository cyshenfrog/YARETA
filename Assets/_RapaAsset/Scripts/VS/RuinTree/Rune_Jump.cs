using UltEvents;
using UnityEngine;

public class Rune_Jump : MonoBehaviour
{
    public int JumpNumber = 3;
    public UltEvent OnReady;
    public UltEvent OnEnd;
    public UltEvent OnClear;
    public GameObject[] CheckerPass;
    private bool ready;
    private int count;
    private float jumpCD;

    // Update is called once per frame
    private void Update()
    {
        if (!ready)
            return;
        if (count > 0)
        {
            jumpCD += Time.deltaTime;
            if (jumpCD > 1.5f)
                ResetCounting();
        }
    }

    private void ResetCounting()
    {
        count = 0;
        jumpCD = 0;
        foreach (var item in CheckerPass)
        {
            item.SetActive(false);
        }
    }

    public void Ready()
    {
        ready = true;
        Player.Instance.OnJump += CheckJump;
        OnReady.Invoke();
    }

    public void End()
    {
        ready = false;
        ResetCounting();
        Player.Instance.OnJump -= CheckJump;
        OnEnd.Invoke();
    }

    private void CheckJump()
    {
        CheckerPass[count].SetActive(true);
        jumpCD = 0;
        count++;
        if (count >= JumpNumber)
        {
            Clear();
        }
    }

    private void Clear()
    {
        OnClear.Invoke();
        End();
    }
}