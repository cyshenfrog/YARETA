using NPOI.SS.Formula.Functions;
using System.Collections;
using System.Collections.Generic;
using UltEvents;
using UnityEngine;

public class Rune_TurnAround : MonoBehaviour
{
    public bool Clockwise;
    public GameObject[] CheckerPass;
    public GameObject[] CheckerWrong;
    public int RoundNumber = 3;
    public int CheckerNumber = 4;
    public UltEvent Pass;
    public UltEvent Wrong;
    public UltEvent Correct;
    private int currentID = -1;
    private int count;

    private int NextID
    {
        get
        {
            int t;
            if (Clockwise)
            {
                t = currentID + 1;
                if (t >= CheckerNumber)
                {
                    t = 0;
                }
                return t;
            }
            else
            {
                t = currentID - 1;
                if (t < 0)
                {
                    t = CheckerNumber - 1;
                }
                return t;
            }
        }
    }

    public void IDCheck(int id)
    {
        if (currentID == -1)
        {
            currentID = id;
            return;
        }
        if (id == NextID)
        {
            count++;
            Pass.Invoke();
            CheckerPass[id].SetActive(true);
            if (count == RoundNumber * 4)
            {
                Correct.Invoke();
                gameObject.SetActive(false);
            }
        }
        else
        {
            CheckerWrong[id].SetActive(true);
            count = 0;
            Wrong.Invoke();
        }

        currentID = id;
    }
}