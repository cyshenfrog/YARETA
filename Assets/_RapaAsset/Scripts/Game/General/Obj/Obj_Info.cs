using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class Obj_Info : MonoBehaviour
{
    public int ID { get { return (int)GetScanData; } }

    public bool Weird;
    public bool Drawable = true;
    public bool BigObj;
    public bool GoddessWant { get { return GameManager.Instance.ScanSheet.dataArray[ID].Goddesswant; } }
    public ScanDataEnum ScanData;

    public ScanDataEnum GetScanData
    {
        set
        {
            ScanData = value;
        }
        get
        {
            if (Gold)
                return ScanData + 1;
            else if (Silver)
                return ScanData + 2;
            else
                return ScanData;
        }
    }

    public UnityEvent OnScanEvent;

    public void Upload()
    {
        if (ID == 24)
        {
            //if (PrototypeMain.Instance.WhiteStoneCount != 0)
            SaveDataManager.CurrentScore += GameManager.Instance.ScanSheet.dataArray[ID].Fullpoint;
            PrototypeMain.Instance.WhiteStoneCount++;
        }
        SaveDataManager.AddScannedData(ID, 0);
    }

    public static void Upload(int ID)
    {
        if (ID == 24)
        {
            if (PrototypeMain.Instance.WhiteStoneCount != 0)
                SaveDataManager.CurrentScore += GameManager.Instance.ScanSheet.dataArray[ID].Fullpoint;
            PrototypeMain.Instance.WhiteStoneCount++;
        }
        SaveDataManager.AddScannedData(ID, 0);
    }

    public bool Gold;

    public bool Silver;
}