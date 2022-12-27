using System;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_ScanMechine : UnitySingleton_D<UI_ScanMechine>
{
    public Text InputType;
    public GameObject ConfirmWindow;
    public GameObject ColumnPrefab;
    public RectTransform ListRoot;
    public RectTransform Selector;
    public Transform MenuRoot;
    public Image ItemPic;
    public Text Score;
    public Text Info;
    public float SelectorUpperBound;
    public float SelectorButtonBound;
    public float CoulmnUnitSpace;
    public Action OnTargetScore = null;
    public GameObject OffworkButton;
    public GameObject Offworked;
    public GameObject OffworkButtonInactive;
    public GameObject Tutorial;
    public GameObject ScannerRoot;
    private Action onClose = null;
    private bool open;
    private List<UI_ScanDataColumn> scanDataList = new List<UI_ScanDataColumn>();
    private List<bool> isFullDataList = new List<bool>();
    private int currentSelect;

    private void Start()
    {
        //Selector.gameObject.SetActive(GameInput.usingGamepad);
        //GameInput.OnSwitchController += (b) => { Selector.gameObject.SetActive(b); };
    }

    public void AddScannedData(int id)
    {
        scanDataList.Add(Instantiate(ColumnPrefab, ListRoot).GetComponent<UI_ScanDataColumn>());
        scanDataList[scanDataList.Count - 1].Init(id, scanDataList.Count - 1);
    }

    public void UpdateSlot(int order, int id)
    {
        scanDataList[order].Init(id, order, true);
    }

    public void Update()
    {
        if (!open)
            return;
        if (Lock)
            return;
        //if (ConfirmWindow.activeSelf)
        //{
        //    if (GameInput.GetButtonDown(Actions.Cancel))
        //    {
        //        ConfirmWindow.SetActive(false);
        //    }
        //    else if (GameInput.GetButtonDown(Actions.Confirm))
        //    {
        //        Offwork();
        //    }
        //    return;
        //}
        if (ConfirmWindow.activeSelf)
        {
            if (GameInput.GetButtonDown(Actions.Confirm))
                ConfirmOffwork(true);
            else if (GameInput.GetButtonDown(Actions.Cancel))
                ConfirmOffwork(false);

            return;
        }
        if (Tutorial.activeSelf)
        {
            if (GameInput.GetButtonDown(Actions.Tutorial) || GameInput.GetButtonDown(Actions.Menu) || GameInput.GetButtonDown(Actions.Cancel))
            {
                SwitchTab();
            }
            else if (Hinput.anyGamepad.dPad.right.justPressed || Hinput.anyGamepad.leftStick.right.justPressed || Hinput.anyGamepad.rightStick.right.justPressed)
            {
                SwitchController(true);
            }
            else if (Hinput.anyGamepad.dPad.left.justPressed || Hinput.anyGamepad.leftStick.left.justPressed || Hinput.anyGamepad.rightStick.left.justPressed)
            {
                SwitchController(false);
            }
            return;
        }
        if (Hinput.keyboard.F9.justPressed)
        {
            OffworkButton.SetActive(!OffworkButton.activeSelf);
        }
        if (GameInput.Keyboard.tab.justPressed)
        {
            Close();
        }
        if (GameInput.GetButtonDown(Actions.Tutorial))
        {
            SwitchTab();
        }
        if (Hinput.mouse.scroll.down)
        {
            ListRoot.anchoredPosition += Vector2.up * 50;
        }
        if (Hinput.mouse.scroll.up)
        {
            ListRoot.anchoredPosition += Vector2.down * 50;
        }
        //else if (GameInput.usingGamepad && GameInput.GetButtonDown(Actions.Confirm))
        //{
        //    Upload();
        //}
        //else if (Tool_SwtichDayNight.isDay && GameInput.GetButtonDown(Actions.OffWork))
        //{
        //    OnOffworkButtton();
        //}
        if (Hinput.keyboard.S.justPressed || Hinput.anyGamepad.leftStick.down.justPressed || Hinput.anyGamepad.rightStick.down.justPressed || Hinput.anyGamepad.dPad.down.justPressed || Hinput.keyboard.downArrow.justPressed)
        {
            SEManager.Instance.PlaySystemSE(SystemSE.UI選擇);
            Down();
        }
        else if (Hinput.keyboard.W.justPressed || Hinput.anyGamepad.leftStick.up.justPressed || Hinput.anyGamepad.rightStick.up.justPressed || Hinput.anyGamepad.dPad.up.justPressed || Hinput.keyboard.upArrow.justPressed)
        {
            SEManager.Instance.PlaySystemSE(SystemSE.UI選擇);
            Up();
        }
    }

    public void ConfirmOffwork(bool yes)
    {
        if (yes)
            Offwork();
        else
            MenuRoot.gameObject.SetActive(true);
        ConfirmWindow.SetActive(false);
    }

    public void SwitchController(bool Right = true)
    {
        if (Right)
        {
            if (GameInput.ControllerType == ControllerType.Switch)
                GameInput.ControllerType = ControllerType.XBOX;
            else
                GameInput.ControllerType = GameInput.ControllerType + 1;
        }
        else
        {
            if (GameInput.ControllerType == ControllerType.XBOX)
                GameInput.ControllerType = ControllerType.Switch;
            else
                GameInput.ControllerType = GameInput.ControllerType - 1;
        }
        InputType.text = GameInput.ControllerType.ToString();
        GameInput.OnSwitchController?.Invoke(GameInput.usingGamepad);
    }

    public void SwitchTab()
    {
        SEManager.Instance.PlaySystemSE(SystemSE.UI確認);
        Tutorial.SetActive(!Tutorial.activeSelf);
        ScannerRoot.SetActive(!ScannerRoot.activeSelf);
    }

    public void Offwork()
    {
        SEManager.Instance.PlaySystemSE(SystemSE.UI確認);
        if (offworked)
            return;
        offworked = true;
        PrototypeMain.Instance.OffWork();
        Tool_Coroutine.Instance.Delay(1, () =>
        {
            MenuRoot.gameObject.SetActive(true);
            Close();
            Offworked.SetActive(true);
            OffworkButton.SetActive(false);
        });
    }

    public void Open(Action OnClose = null)
    {
        UpdateInfo(currentSelect);
        CameraMain.Instance.hardLock = true;
        MenuRoot.parent.gameObject.SetActive(true);
        GameManager.Cursorvisible = true;
        SEManager.Instance.PlaySystemSE(SystemSE.Menu);
        open = true;
        onClose = OnClose;
        Tool_Coroutine.Instance.Delay(0.5f, UploadAll);
        Player.Instance.Status = PlayerStatus.Wait;
    }

    public void Close()
    {
        if (Lock)
            return;
        SEManager.Instance.PlaySystemSE(SystemSE.UI取消);
        CameraMain.Instance.hardLock = false;
        GameManager.Cursorvisible = false;
        open = false;
        MenuRoot.DOScale(Vector3.zero, 0.5f)
            .SetEase(Ease.InBack)
            .OnComplete(() =>
            {
                MenuRoot.parent.gameObject.SetActive(false);
                onClose?.Invoke();
            });
    }

    private void Upload()
    {
        if (scanDataList.Count == 0)
            return;
        scanDataList[currentSelect].Upload();
    }

    private void UploadAll()
    {
        if (scanDataList.Count == 0)
            return;

        UpdateScoreUI();
        foreach (var item in scanDataList)
        {
            item.Upload();
        }
    }

    private float scoreRef;
    private bool offworked;

    public bool Lock;

    public void UpdateScoreUI()
    {
        if (offworked)
            return;
        if (SaveDataManager.CurrentScore >= SaveDataManager.TargetScore)
        {
            OffworkButton.SetActive(true);
            OffworkButtonInactive.SetActive(false);
            OnTargetScore?.Invoke();
        }
        DOTween.To(() => scoreRef, x => { scoreRef = x; Score.text = ((int)scoreRef).ToString() + "/" + SaveDataManager.TargetScore; }, SaveDataManager.CurrentScore, 0.5f);
    }

    private void Up()
    {
        if (scanDataList.Count == 0 || currentSelect == 0)
            return;
        currentSelect--;
        MoveTo(currentSelect);
    }

    private void Down()
    {
        if (scanDataList.Count == 0 || currentSelect + 1 == scanDataList.Count)
            return;
        currentSelect++;
        MoveTo(currentSelect);
    }

    public void MoveTo(int id)
    {
        UpdateInfo(id);
        if (GameInput.usingGamepad)
        {
            if (scanDataList[id].transform.position.y > SelectorUpperBound)
            {
                ListRoot.DOAnchorPosY(-CoulmnUnitSpace, 0.2f)
                    .SetRelative(true);
            }
            else if (scanDataList[id].transform.position.y < SelectorButtonBound)
            {
                ListRoot.DOAnchorPosY(CoulmnUnitSpace, 0.2f)
                    .SetRelative(true);
            }
        }
        Selector.DOAnchorPosY(((RectTransform)scanDataList[id].transform).anchoredPosition.y, 0.2f);
    }

    private void UpdateInfo(int orderID)
    {
        if (SaveDataManager.ScannedData.Count == 0)
            return;
        switch (SaveDataManager.ScannedData[orderID].ClearLevel)
        {
            case ClearLevel.在畫三小:
                ItemPic.sprite = DrawingMode.Instance.ShitTextures[SaveDataManager.ScannedData[orderID].ID];
                Info.text = GameManager.Instance.UISheet.GetUIText(UIDataEnum.再靠近一點看能不能看清楚一點);
                break;

            case ClearLevel.模糊:
                ItemPic.sprite = DrawingMode.Instance.BlurDrawTextures[SaveDataManager.ScannedData[orderID].ID];
                Info.text = GameManager.Instance.UISheet.GetUIText(UIDataEnum.再靠近一點看能不能看清楚一點);
                break;

            case ClearLevel.清楚:
            default:
                ItemPic.sprite = DrawingMode.Instance.DrawTextures[SaveDataManager.ScannedData[orderID].ID];
                Info.text = GameManager.Instance.ScanSheet.GetInfo(SaveDataManager.ScannedData[orderID].ID, SaveDataManager.language);
                break;
        }
    }
}