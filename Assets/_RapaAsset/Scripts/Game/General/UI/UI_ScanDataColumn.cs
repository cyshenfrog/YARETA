using UnityEngine;
using UnityEngine.UI;
using DG.Tweening;

public class UI_ScanDataColumn : MonoBehaviour
{
    public Text Name;
    public Text Point;
    public DOTweenAnimation[] FX;
    private int orderID;
    private bool needRenew;

    private void OnEnable()
    {
        if (needRenew)
        {
            foreach (var item in FX)
            {
                item.DORewind();
                item.DOPlayForward();
            }
            needRenew = false;
        }
    }

    public void Select()
    {
        UI_ScanMechine.Instance.MoveTo(orderID);
    }

    public void Init(int ID, int order, bool renew = false)
    {
        needRenew = renew;
        orderID = order;
        switch (SaveDataManager.ScannedData[order].ClearLevel)
        {
            case ClearLevel.清楚:
                Name.text = GameData.ScanSheet.GetFullName(ID, SaveDataManager.language);
                Point.text = GameData.ScanSheet.dataArray[ID].Fullpoint.ToString();
                break;

            case ClearLevel.模糊:
                Name.text = GameData.ScanSheet.GetBlurName(ID, SaveDataManager.language);
                Point.text = GameData.ScanSheet.dataArray[ID].Point.ToString();
                break;

            case ClearLevel.在畫三小:
                Name.text = GameData.ScanSheet.GetFullName(23, SaveDataManager.language);
                Point.text = GameData.ScanSheet.dataArray[23].Fullpoint.ToString();
                break;

            default:
                break;
        }
        //switch (GameData.ScanSheet.dataArray[ID].Level)
        //{
        //    case 0:
        //    default:
        //        Name.color = Color.white;
        //        break;

        //    case 1:
        //        Name.color = Color.blue;
        //        break;

        //    case 2:
        //        Name.color = Color.magenta;
        //        break;

        //    case 3:
        //        Name.color = Color.yellow;
        //        break;
        //}
        if (ID == 24)
        {
            Name.text += " (" + PrototypeMain.Instance.WhiteStoneCount + ")";
            Point.gameObject.SetActive(true);
        }
    }

    public void Upload()
    {
        if (SaveDataManager.ScannedData[orderID].UploadedLevel == ClearLevel.清楚)
            return;
        SaveDataManager.UploadeData(orderID);
    }
}