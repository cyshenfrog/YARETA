using System;
using UnityEngine;

public class GameManager : UnitySingleton_D<GameManager>
{
    public bool TestMode;
    public ControllerSprites ControllerSprites;
    public TalkSheet TalkSheet;
    public ScanSheet ScanSheet;
    public UISheet UISheet;
    public Font EnFont;
    public Font ChFont;
    public ButtonMapping ButtonMapping;

    public Font GetFont()
    {
        switch (SaveDataManager.Language)
        {
            case SystemLanguage.Chinese:
            case SystemLanguage.ChineseTraditional:
            case SystemLanguage.ChineseSimplified:
                return ChFont;

            case SystemLanguage.English:
            default:
                return EnFont;
        }
    }

    public override void Awake()
    {
        base.Awake();
        Application.targetFrameRate = 60;
        GameRef.MainCam = Camera.main;
        Screen.SetResolution(1920, 1080, true);

        if (TestMode)
        {
            SaveDataManager.TutorialPassed = true;
        }
    }
}