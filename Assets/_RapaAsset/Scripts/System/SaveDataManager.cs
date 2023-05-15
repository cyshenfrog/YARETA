using System;
using System.Collections.Generic;
using UnityEngine;

public enum GameState
{
    Lobby,
    Wait,
    WaitConnection,
    Gaming,
    Menu,
    End
}

public enum ClearLevel
{
    清楚,
    模糊,
    在畫三小,
}

public struct ScanData
{
    public int ID;
    public ClearLevel ClearLevel;
    public ClearLevel UploadedLevel;
    public bool Uploaded;

    public ScanData(int id, ClearLevel clearLevel)
    {
        Uploaded = false;
        UploadedLevel = ClearLevel.在畫三小;
        ID = id;
        ClearLevel = clearLevel;
    }

    public ScanData(int id, ClearLevel clearLevel, bool uploaded, ClearLevel uploadedLevel)
    {
        Uploaded = uploaded;
        UploadedLevel = uploadedLevel;
        ID = id;
        ClearLevel = clearLevel;
    }

    public void SetUploadLevel(ClearLevel level)
    {
        UploadedLevel = ClearLevel;
    }
}

public class SaveData
{
}

public static class SaveDataManager
{
    #region 系統

    public static SystemLanguage language = SystemLanguage.Chinese;

    public static SystemLanguage Language
    {
        get { return language; }
        set
        {
            if (language != value)
            {
                language = value;
                OnLanguageChanged?.Invoke();
            }
        }
    }

    public static event Action OnLanguageChanged;

    public static SaveData SaveData = new SaveData();
    public static bool TutorialPassed;

    #endregion 系統

    /// <summary>
    /// 遊戲流程
    /// </summary>
    public static GameState Status = GameState.Lobby;

    #region 存檔區

    //TODO: 此區寫入存檔
    public static int TargetScore = 90;

    public static int CurrentScore;

    public static List<ScanData> ScannedData = new List<ScanData>();

    public static void UploadeData(int orderID)
    {
        if (!ScannedData[orderID].Uploaded)
        {
            ScannedData[orderID].SetUploadLevel(ScannedData[orderID].ClearLevel);
            switch (ScannedData[orderID].ClearLevel)
            {
                case ClearLevel.清楚:
                    CurrentScore += ScanSheet.Instance.dataArray[ScannedData[orderID].ID].Fullpoint;
                    break;

                case ClearLevel.模糊:
                    CurrentScore += ScanSheet.Instance.dataArray[ScannedData[orderID].ID].Point;
                    break;

                case ClearLevel.在畫三小:
                default:
                    CurrentScore += 1;
                    break;
            }
        }
        else if (ScannedData[orderID].UploadedLevel != ScannedData[orderID].ClearLevel)
        {
            switch (ScannedData[orderID].UploadedLevel)
            {
                case ClearLevel.模糊:
                    CurrentScore -= ScanSheet.Instance.dataArray[ScannedData[orderID].ID].Point;
                    break;

                case ClearLevel.在畫三小:
                    CurrentScore -= 1;
                    break;
            }
            switch (ScannedData[orderID].ClearLevel)
            {
                case ClearLevel.清楚:
                    CurrentScore += ScanSheet.Instance.dataArray[ScannedData[orderID].ID].Fullpoint;
                    break;

                case ClearLevel.模糊:
                    CurrentScore += ScanSheet.Instance.dataArray[ScannedData[orderID].ID].Point;
                    break;

                case ClearLevel.在畫三小:
                default:
                    CurrentScore += 1;
                    break;
            }

            ScannedData[orderID].SetUploadLevel(ScannedData[orderID].ClearLevel);
        }
    }

    public static void AddScannedData(int id, ClearLevel clearLevel)
    {
        if (ScannedData.Exists(x => x.ID == id))
        {
            int order = ScannedData.FindIndex(x => x.ID == id);
            if (id == 24)
            {
                UI_ScanMechine.Instance.UpdateSlot(order, id);
            }

            if (clearLevel < ScannedData[order].ClearLevel)
            {
                SEManager.Instance.PlaySystemSE(clearLevel == ClearLevel.清楚 ? SystemSE.畫好了 : SystemSE.UI確認);
                ScannedData[order] = new ScanData(id, clearLevel, true, ScannedData[order].ClearLevel);
                UI_ScanMechine.Instance.UpdateSlot(order, id);
                DrawingMode.Instance.ShowUnlockHint("+ " + ScanSheet.Instance.GetName(id, clearLevel, language), false);
            }
            else
                DrawingMode.Instance.ShowUnlockHint(ScanSheet.Instance.GetName(id, clearLevel, language), true);
        }
        else
        {
            if (id != 24)
                SEManager.Instance.PlaySystemSE(clearLevel == 0 ? SystemSE.畫好了 : SystemSE.UI確認);
            ScannedData.Add(new ScanData(id, clearLevel));
            UI_ScanMechine.Instance.AddScannedData(id);
            PrototypeMain.Instance.RemoveTargetScanData(id);
            DrawingMode.Instance.ShowUnlockHint("+ " + ScanSheet.Instance.GetName(id, clearLevel, language), false);
        }
    }

    #endregion 存檔區
}