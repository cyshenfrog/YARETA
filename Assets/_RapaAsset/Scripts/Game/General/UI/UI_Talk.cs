using System;
using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

public class UI_Talk : UnitySingleton_D<UI_Talk>
{
    public Text Dialog;
    public Text Dialog2;
    public GameObject BGDarker;
    public Image Underline;
    public RectTransform TalkGroup;
    public Image Button;
    public Color TreeColor;
    public bool WhisperTalk;
    public bool DontLock;
    public AudioClip[] Whispers;

    private string[] currentTalks;
    private string replacer = "";
    private int talkCount;
    private Action TalkFinish = null;
    private Action onNextTalk = null;
    private bool talking;
    private bool autoMode;
    private float autoTalkGapTime;
    private bool overrideInput;
    private Sprite overrideInputKB;
    private Sprite overrideInputJoy;
    private Actions overrideInputAction;
    private Coroutine AutoTalking;
    private Color targetColor = Color.white;
    private bool split;

    private void Start()
    {
        UpdateTextSize();
        SaveDataManager.OnLanguageChanged += UpdateTextSize;
    }

    private void UpdateTextSize()
    {
        Dialog.font = GameManager.Instance.GetFont();
        Dialog.transform.localScale = SaveDataManager.Language == SystemLanguage.English ? Vector3.one : (Vector3.one - Vector3.right * 0.1f);
    }

    private void Update()
    {
        if (autoMode)
            return;
        if (talking)
        {
            if (overrideInput && talkCount == currentTalks.Length)
            {
                if (GameInput.GetButtonDown(overrideInputAction))
                    Next();
            }
            else if (GameInput.GetButtonDown(Actions.Interact))
            {
                Next();
            }
        }
    }

    private void UpdateOverrideIcon(bool IsController)
    {
        if (overrideInput && talkCount == currentTalks.Length)
            Button.sprite = IsController ? overrideInputJoy : overrideInputKB;
    }

    public void SetInputThisTime(Sprite OverrideInputKB, Sprite OverrideInputJoy, Actions Input)
    {
        overrideInput = true;
        overrideInputKB = OverrideInputKB;
        overrideInputJoy = OverrideInputJoy;
        //Button.sprite = GameInput.usingGamepad ? OverrideInputJoy : OverrideInputKB;
        GameInput.OnSwitchController += UpdateOverrideIcon;
        overrideInputAction = Input;
    }

    public void ShowTalk(int StartId, Color textColor, Action OnFinish = null, bool Auto = false, float AutoTalkDuration = 1, string ReplaceText = "", Action OnNextTalk = null)
    {
        onNextTalk = OnNextTalk;
        targetColor = Dialog.color = Underline.color = textColor;
        ShowTalk(StartId, OnFinish, Auto, AutoTalkDuration, ReplaceText);
    }

    public void ShowTalk(int StartId, Action OnFinish = null, bool Auto = false, float AutoTalkDuration = 1, string ReplaceText = "")
    {
        if (WhisperTalk)
        {
            SEManager.Instance.Play(Whispers[0]);
        }
        if (talking && AutoTalking != null)
        {
            StopCoroutine(AutoTalking);
        }
        autoTalkGapTime = AutoTalkDuration;
        TalkFinish = OnFinish;
        talkCount = 0;
        autoMode = Auto;
        talking = !autoMode;
        replacer = ReplaceText;
        ShowTalk(GameManager.Instance.TalkSheet.GetTalks(StartId));
    }

    public void ShowTalk(string[] talks)
    {
        if (!DontLock)
        {
            Player.Instance.Status = PlayerStatus.Wait;
            Player.Instance.Slowdown = true;
        }
        currentTalks = talks;
        Dialog.text = "";
        Dialog2.text = "";
        PlayText();
        TalkGroup.gameObject.SetActive(true);
    }

    private IEnumerator _AutoNext()
    {
        yield return new WaitForSeconds(autoTalkGapTime);
        Next();
    }

    public void Next()
    {
        if (!autoMode)
            SEManager.Instance.PlaySystemSE(SystemSE.UI選擇);
        if (talkCount == currentTalks.Length)
        {
            CloseTalk();
            return;
        }
        else if (WhisperTalk)
        {
            SEManager.Instance.Play(Whispers[talkCount]);
        }
        if (DOTween.IsTweening(Dialog) || (split && DOTween.IsTweening(Dialog2)))
        {
            if (!autoMode)
            {
                Button.DOKill();
                Button.color = targetColor;
            }
            DOTween.Kill(Dialog);
            if (split)
            {
                DOTween.Kill(Dialog2);
                string[] s = currentTalks[talkCount].Split('*');
                Dialog.text = s[0];
                Dialog2.text = s[1];
            }
            else
                Dialog.text = Tool_StringTool.CheckLineBreak(currentTalks[talkCount]);
            talkCount++;
            if (overrideInput && talkCount == currentTalks.Length)
                UpdateOverrideIcon(GameInput.UsingJoystick);
            if (autoMode)
                AutoTalking = StartCoroutine(_AutoNext());
        }
        else if (!autoMode)
        {
            if (!DOTween.IsTweening(Button))
            {
                Button.color = targetColor;
                Button.DOFade(0, 0.1f)
                    .SetLoops(2, LoopType.Yoyo)
                    .OnComplete(PlayText);
            }
        }

        split = false;
        onNextTalk?.Invoke();
    }

    private void PlayText()
    {
        Button.color = Color.clear;
        string[] s = currentTalks[talkCount].Split('*');
        split = currentTalks[talkCount].Contains("*");
        Dialog.text = "";
        Dialog2.text = "";
        Dialog.DOText(CheckString(Tool_StringTool.CheckLineBreak(s[0]), replacer), split ? 0.7f : 1)
            .SetEase(Ease.Linear)
            .OnComplete(() =>
            {
                if (split)
                {
                    Dialog2.DOText(CheckString(Tool_StringTool.CheckLineBreak(s[1]), replacer), .3f)
                        .SetEase(Ease.Linear);
                }
                talkCount++;
                if (overrideInput && talkCount == currentTalks.Length)
                    UpdateOverrideIcon(GameInput.UsingJoystick);
                if (autoMode)
                    AutoTalking = StartCoroutine(_AutoNext());
                else
                    Button.color = targetColor;
            });
    }

    public void CloseTalk()
    {
        Player.Instance.Status = PlayerStatus.Moving;

        targetColor = Button.color = Dialog.color = Underline.color = Color.white;
        TalkFinish?.Invoke();
        onNextTalk = null;
        talking = false;
        talkCount = 0;
        if (overrideInput)
        {
            overrideInput = false;
            GameInput.OnSwitchController -= UpdateOverrideIcon;
            Button.GetComponent<UI_InputIcon>().Switch(GameInput.UsingJoystick);
        }
        TalkGroup.gameObject.SetActive(false);
    }

    private string CheckString(string s, string r)
    {
        return s.Replace("*", r);
    }
}