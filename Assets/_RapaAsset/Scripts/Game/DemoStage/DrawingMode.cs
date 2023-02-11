using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using UnityEngine.Events;
using UnityEngine.UI;

public class DrawingMode : UnitySingleton_D<DrawingMode>
{
    public GameObject DrawingUI;
    public GameObject LogUI;
    public GameObject OffworkUI;
    public Transform HintStartPos;
    public Text[] UnlockHints;
    public DOTweenAnimation DrawOperationIcon;
    public DOTweenAnimation FullUITween;
    public UnityEvent FirstEvent;
    public Obj_Info TutorialObj;
    public Obj_Info BlurryInfo;
    public Sprite[] DrawTextures;
    public Sprite[] BlurDrawTextures;
    public Sprite[] ShitTextures;
    public Material IpadMaterial;
    public Sprite menuIconKB;

    private int hintQueueID;
    private bool firstDraw = true;
    private bool questTutorial;
    private bool ready;
    private bool drawing;
    private Vector3 rotateEuler = new Vector3(50, 0, 0);
    private RaycastHit hit;
    private Obj_Info outInfo;
    private Action OnFinish;

    // Update is called once per frame
    private void Update()
    {
        if (!ready || drawing)
            return;
        if (PrototypeMain.Instance.Offworked)
        {
            if (GameInput.GetButtonDown(Actions.DrawMode) || GameInput.GetButtonDown(Actions.Interact))
                EndDrawingMode();
            return;
        }
        if (GameInput.GetButtonDown(Actions.DrawMode) || GameInput.GetButtonDown(Actions.Cancel))
            EndDrawingMode();
        if (GameInput.GetButtonDown(Actions.Menu))
        {
            ready = false;
            LogUI.SetActive(false);
            DrawingUI.SetActive(false);
            UI_ScanMechine.Instance.Open(() =>
            {
                LogUI.SetActive(true);
                DrawingUI.SetActive(true);
                ready = true;
            });
            return;
        }
        //rotateEuler = Vector3.Lerp(rotateEuler, rotateEuler + new Vector3(-GameInput.CameraMove.y * 30, GameInput.CameraMove.x * 30), Time.deltaTime);
        Player.Instance.FPCam.transform.localEulerAngles += Vector3.right * -GameInput.CameraMove.y * 30 * Time.deltaTime;
        Player.Instance.transform.localEulerAngles += Vector3.up * GameInput.CameraMove.x * 30 * Time.deltaTime;
        if (Physics.Raycast(GameRef.MainCam.transform.position, GameRef.MainCam.transform.forward, out hit, 100, ~(1 << 2), QueryTriggerInteraction.Ignore))
        {
            outInfo = hit.collider.transform.GetComponent<Obj_Info>();

            if (outInfo)
            {
                if (outInfo.Drawable)
                {
                    DrawOperationIcon.DOPlayForward();
                    FullUITween.DOPlayForward();
                    if (GameInput.GetButtonDown(Actions.Draw))
                        Draw(outInfo, hit.distance);
                    return;
                }
            }
        }
        DrawOperationIcon.DOPlayBackwards();
        FullUITween.DOPlayBackwards();
    }

    public void StartDrawingMode()
    {
        StartDrawingMode(null);
    }

    public void StartDrawingMode(Action OnFinishCallback = null)
    {
        SEManager.Instance.PlaySystemSE(SystemSE.Menu);
        IpadMaterial.DOFade(0, 0);
        OnFinish = OnFinishCallback;
        Player.Instance.Status = PlayerStatus.Wait;
        Player.Instance.characterController.enabled = false;
        Player.Instance.transform.localEulerAngles = new Vector3(0, CameraMain.Instance.MainFreeLookCam.m_XAxis.Value, 0);
        Player.Instance.FPCam.transform.localEulerAngles = new Vector3(CameraMain.Instance.MainFreeLookCam.transform.eulerAngles.x, 0, 0);
        rotateEuler = Player.Instance.FPCam.transform.localEulerAngles;
        Player.Instance.FPCam.SetActive(true);
        Player.Instance.Model.SetActive(false);
        if (PrototypeMain.Instance.Offworked)
        {
            OffworkUI.SetActive(true);
        }
        else
        {
            LogUI.SetActive(true);
            DrawingUI.SetActive(true);
        }
        CameraMain.Instance.enabled = false;
        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return null;
            ready = true;
        }
    }

    public void EndDrawingMode()
    {
        SEManager.Instance.PlaySystemSE(SystemSE.UI取消);
        ready = false;
        CameraMain.Instance.Recenter(0);
        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(UI_ButtonBlink.Duration);
            Player.Instance.FPCam.SetActive(false);
            Player.Instance.characterController.enabled = true;
            Player.Instance.Model.SetActive(true);
            Player.Instance.Status = PlayerStatus.Moving;
            if (PrototypeMain.Instance.Offworked)
            {
                OffworkUI.SetActive(false);
            }
            else
            {
                LogUI.SetActive(false);
                DrawingUI.SetActive(false);
            }
            CameraMain.Instance.enabled = true;
            OnFinish?.Invoke();
            yield return new WaitForSeconds(UI_ButtonBlink.Duration);
            if (questTutorial)
            {
                questTutorial = false;
                UI_Talk.Instance.SetInputThisTime(menuIconKB, GameManager.Instance.ControllerSprites.GetSprit(GameInput.JoystickBrandType, ControllerButton.Up), Actions.Menu);
                UI_Talk.Instance.ShowTalk((int)TalkDataEnum.教學_打開任務介面, () =>
                {
                    UI_ScanMechine.Instance.Lock = true;
                    Player.Instance.OpenMenu();
                    UI_Talk.Instance.BGDarker.SetActive(true);
                    Delay.Instance.Wait(1.5f, () => UI_Talk.Instance.ShowTalk((int)TalkDataEnum.教學_任務, () =>
                    {
                        UI_Talk.Instance.BGDarker.SetActive(false);
                        Player.Instance.Status = PlayerStatus.Wait;
                        UI_ScanMechine.Instance.Lock = false;
                    }));
                });
            }
        }
    }

    public void Draw(Obj_Info info, float distance)
    {
        if (firstDraw && info == TutorialObj)
        {
            firstDraw = false;
            questTutorial = true;
            FirstEvent.Invoke();
        }
        StartCoroutine(delay());

        IEnumerator delay()
        {
            yield return new WaitForSeconds(UI_ButtonBlink.Duration);
            IpadMaterial.DOFade(0, 0);
            drawing = true;
            DrawOperationIcon.DOPlayBackwards();

            Player.Instance.DrawModel.SetActive(true);
            Vector3 r = Player.Instance.FPCam.transform.localEulerAngles;
            Player.Instance.FPCam.transform.DOLocalRotate(new Vector3(50, 0, 0), 1);
            //Player.Instance.PadUp();
            yield return new WaitForSeconds(.5f);
            Player.Instance.RightHandDrawing();
            SEManager.Instance.PlaySystemSE(SystemSE.畫畫);

            yield return new WaitForSeconds(.5f);
            if (distance > (info.BigObj ? 30 : 10))
                IpadMaterial.mainTexture = ShitTextures[info.ID].texture;
            else if (distance < (info.BigObj ? 15 : 5))
                IpadMaterial.mainTexture = DrawTextures[info.ID].texture;
            else
                IpadMaterial.mainTexture = BlurDrawTextures[info.ID].texture;
            IpadMaterial.DOFade(1, 1)
                .SetLoops(2, LoopType.Yoyo);
            //UI_ScanTextFX.Instance.ShowTextFX(info, Player.Instance.transform.position + Player.Instance.transform.forward * .3f + Vector3.up);
            yield return new WaitForSeconds(1);

            ClearLevel nearLevel;

            if (distance > (info.BigObj ? 30 : 10))
                nearLevel = ClearLevel.在畫三小;
            else if (distance < (info.BigObj ? 15 : 5))
                nearLevel = ClearLevel.清楚;
            else
                nearLevel = ClearLevel.模糊;

            SaveDataManager.AddScannedData(info.ID, nearLevel);

            Player.Instance.FPCam.transform.DOLocalRotate(r, 1);
            //Player.Instance.PadDown();
            yield return new WaitForSeconds(1);
            Player.Instance.DrawModel.SetActive(false);
            drawing = false;
        }
    }

    public void ShowUnlockHint(string name, bool oldInfo)
    {
        UnlockHints[hintQueueID].DOKill();
        UnlockHints[hintQueueID].transform.DOKill();
        UnlockHints[hintQueueID].color = new Color(1, 1, 1, 0);
        UnlockHints[hintQueueID].text = name;
        UnlockHints[hintQueueID].transform.position = HintStartPos.position;

        UnlockHints[hintQueueID].transform.DOLocalMoveY(40, .5f)
            .SetRelative();
        UnlockHints[hintQueueID].DOFade(oldInfo ? 0.5f : 1, .5f);
        UnlockHints[hintQueueID].DOFade(0, .5f)
            .SetDelay(8);
        foreach (var item in UnlockHints)
        {
            if (item.color.a != 0 && item != UnlockHints[hintQueueID])
                item.transform.DOLocalMoveY(40, .5f)
                    .SetRelative();
        }
        hintQueueID++;
        if (hintQueueID >= UnlockHints.Length)
            hintQueueID = 0;
    }
}