using System;
using System.Collections;
using System.Collections.Generic;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class PrototypeMain : UnitySingleton_D<PrototypeMain>
{
    public AudioSource FairyBGM;
    public AudioSource EndingBGM;
    public bool CanRocketJump;
    public bool CanScan { get; set; }

    public bool Offworked;
    private int whiteStoneCount;

    public int WhiteStoneCount
    { get { return whiteStoneCount; } set { whiteStoneCount = value; if (value == 12) ForceOffwork(); } }

    public bool SecretEnding;
    public bool CanSavePos { get; set; }

    public CanvasGroup LogoGroup;
    public CanvasGroup LogoRoot;
    public GameObject ComicRoot;
    public GameObject OpeningGroup;
    public GameObject[] OPCam;
    public GameObject[] OffWorkCam;
    public GameObject BGM;
    public Transform StartingPosition;
    public Transform EndingPosition;

    public Tool_SwtichDayNight Switcher;
    private bool gameStart;
    public Sprite MenuIconKB;
    public Sprite MenuIconJoy;
    public Vector3 RespawnPos;
    public TouchTrunk Trunk;
    public List<int> TargetScanData = new List<int>();
    public GameObject ThanksForPlaying;
    private int puzzelCount;
    private float menuHintCD;

    public void AddPuzzelCount()
    {
        puzzelCount++;
        if (puzzelCount >= 3)
        {
            Tool_Coroutine.Instance.Delay(2, () =>
                {
                    UI_Talk.Instance.ShowTalk((int)TalkDataEnum.恭喜白金);
                }
            );
        }
    }

    public void RemoveTargetScanData(int id)
    {
        if (TargetScanData.Contains(id))
        {
            TargetScanData.Remove(id);
            if (TargetScanData.Count == 0)
                AddPuzzelCount();
        }
    }

    public void Start()
    {
        SaveDataManager.MainCam = Camera.main;
        CanSavePos = true;
        UI_ScanMechine.Instance.OnTargetScore += TargetScoreReached;
        InvokeRepeating("UpdateRespawnPoint", 0, 5);
        TargetScanData.AddRange(new int[27] { 0, 1, 2, 9, 10, 15, 16, 20, 21, 22, 34, 35, 39, 40, 41, 42, 43, 44, 46, 47, 49, 50, 51, 52, 53, 56, 57 });
        if (GameManager.Instance.TestMode)
            return;
        Player.Instance.Status = PlayerStatus.Static;
        Player.Instance.transform.position = StartingPosition.position;
        Player.Instance.transform.rotation = StartingPosition.rotation;
        Player.Instance.Anim.SetTrigger("FaceDown");
        CameraMain.Instance.Recenter(0);
    }

    private void Update()
    {
        if (ThanksForPlaying.activeSelf)
        {
            if (GameInput.Keyboard.GetKeyDown(KeyCode.F4))
                Application.Quit();
        }
        if (Offworked)
            return;
        if (SaveDataManager.TutorialPassed && Player.Instance.Status == PlayerStatus.Moving && !Player.Instance.PuzzleMode)
        {
            if (!GameInput.IsMove && !GameInput.IsCameraMove && !GameInput.AnyInput)
            {
                if (menuHintCD >= 23 && !UI_General.Instance.InteractUI[0].activeSelf)
                    UI_General.Instance.ShowActionUI(ButtonAction.MenuHint);
                else
                    menuHintCD += Time.deltaTime;
            }
            else
            {
                menuHintCD = 0;
                UI_General.Instance.CloseActionUI(ButtonAction.MenuHint);
            }
        }
        else if (UI_General.Instance.InteractUI[0].activeSelf)
        {
            menuHintCD = 0;
            UI_General.Instance.CloseActionUI(ButtonAction.MenuHint);
        }
        if (gameStart)
            return;
        if (GameInput.AnyInput)
            GameStart();
    }

    public void SwitchEndingBGM()
    {
        FairyBGM.DOFade(0, 5);
    }

    public void Ending()
    {
        EndingBGM.Play(3);
        Tool_Coroutine.Instance.Delay(8, d);
        void d()
        {
            if (SecretEnding)
            {
                UI_Talk.Instance.ShowTalk((int)TalkDataEnum.隱藏結局, () => ThanksForPlaying.SetActive(true));
            }
            else
                ThanksForPlaying.SetActive(true);
        }
    }

    public void GameStart()
    {
        StartCoroutine(_GameStart());
    }

    public IEnumerator _GameStart()
    {
        LogoGroup.DOFade(0, 1)
            .OnComplete(() => { LogoGroup.gameObject.SetActive(false); });
        gameStart = true;
        if (GameManager.Instance.TestMode)
        {
            InitStage();
            yield break;
        }

        CameraMain.Instance.hardLock = true;
        yield return new WaitForSeconds(1);
        ComicRoot.SetActive(true);
    }

    public void ETAWakeUp()
    {
        StartCoroutine(d());
        IEnumerator d()
        {
            LogoRoot.DOFade(0, 1);
            yield return new WaitForSeconds(2f);
            OPCam[1].SetActive(true);
            yield return new WaitForSeconds(.5f);
            BGM.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            foreach (var item in OPCam)
            {
                item.SetActive(false);
            }
            Player.Instance.Anim.SetTrigger("Resume");
            LogoRoot.gameObject.SetActive(false);
            Player.Instance.Status = PlayerStatus.Moving;
            yield return new WaitForSeconds(4.5f);
            CameraMain.Instance.hardLock = false;
            //UI_Talk.Instance.ShowTalk(0, () =>
            //{
            //    InitStage();
            //});
        }
    }

    private void InitStage()
    {
        OpeningGroup.SetActive(false);
        BGM.SetActive(true);
        Player.Instance.Status = PlayerStatus.Moving;
        //Player.Instance.Anim.SetBool("TakePhone", false);
    }

    private void TargetScoreReached()
    {
    }

    public void RocketJumpTutorial()
    {
        CanRocketJump = true;
    }

    public void OffWork()
    {
        StartCoroutine(_OffWork());
    }

    private IEnumerator _OffWork()
    {
        SEManager.Instance.ResetWalkSE();
        UI_General.Instance.InteractUI[0].transform.parent.gameObject.SetActive(false);
        CanScan = true;
        Offworked = true;
        UI_FullScreenFade.Instance.BlackAuto(1, 2);
        yield return new WaitForSeconds(1);
        foreach (var item in FindObjectsOfType<Portable>())
        {
            if (item.name != "WhiteStone")
                item.gameObject.SetActive(false);
        }
        yield return new WaitForSeconds(1);
        OffWorkCam[0].SetActive(true);
        Switcher.Switch();
        Player.Instance.Status = PlayerStatus.Static;
        Player.Instance.transform.position = EndingPosition.position;
        Player.Instance.transform.rotation = EndingPosition.rotation;
        Player.Instance.Status = PlayerStatus.Wait;
        Player.Instance.LerpSpeed(0.3f, 1);
        Player.Instance.PadUp();
        yield return new WaitForSeconds(.5f);

        //UI_FullScreenFade.Instance.SetMovieMode(true);
        Player_IKManager.Instance.StartLooking();
        yield return new WaitForSeconds(2.5f);
        UI_Talk.Instance.WhisperTalk = true;
        UI_Talk.Instance.ShowTalk((int)TalkDataEnum.教學_晚上, UI_Talk.Instance.TreeColor, () =>
        {
            StopAllCoroutines();
            UI_Talk.Instance.WhisperTalk = false;
            foreach (var item in OffWorkCam)
            {
                item.SetActive(false);
            }
            Player.Instance.Status = PlayerStatus.Moving;
            Player.Instance.PadDown();
            Trunk.Glow(true);
            //FairyBGM.Play(3);
            //FairyBGM.DOFade(.5f, .5f)
            //    .SetDelay(3);
        }, false, 1, "", () => Trunk.Glow());
        Trunk.Glow();
        Player.Instance.LerpSpeed(0f, 2);
        Player_IKManager.Instance.StopLooking();

        yield return new WaitForSeconds(3);
        OffWorkCam[1].SetActive(true);

        yield return new WaitForSeconds(3);
        //UI_FullScreenFade.Instance.SetMovieMode(false);
    }

    //public void SpawnGuideFX(Transform Hand)
    //{
    //    GameObject fx = EndingGuideFxPool.Spawn(Hand.position, Quaternion.identity);
    //    fx.transform.DOMove(ExitPos.position, (ExitPos.position - Hand.position).magnitude)
    //        .SetDelay(1)
    //        .SetEase(Ease.InOutQuad)
    //        .OnComplete(() => { LeanPool.Despawn(fx, 2); });
    //}

    public void UpdateRespawnPoint()
    {
        if (!CanSavePos)
            return;
        if (Player.Instance.Status == PlayerStatus.Moving && Player.Instance.characterController.isGrounded)
        {
            RespawnPos = Player.Instance.transform.position;
        }
    }

    //public void UpdateCheckPoint(Transform checkPoint)
    //{
    //    return;
    //    for (int i = 0; i < CheckPoints.Length; i++)
    //    {
    //        if (CheckPoints[i] == checkPoint)
    //        {
    //            PlayerPrefs.SetInt("CheckPoint", i);
    //            PlayerPrefs.Save();
    //            break;
    //        }
    //    }
    //}

    private void ForceOffwork()
    {
        Tool_Coroutine.Instance.Delay(2, () =>
            {
                //UI_Talk.Instance.SetInputThisTime(MenuIconKB, MenuIconJoy, Actions.Menu);
                UI_Talk.Instance.ShowTalk((int)TalkDataEnum.強制下班, AddPuzzelCount);
            }
        );
    }

    private void OnApplicationQuit()
    {
        if (!Tool_SwtichDayNight.isDay)
            Switcher.Switch();
    }
}