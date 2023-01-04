using System;
using UnityEngine;

public class GameManager : UnitySingleton_D<GameManager>
{
    public ControllerSprites ControllerSprites;
    public TalkSheet TalkSheet;
    public ScanSheet ScanSheet;
    public UISheet UISheet;
    public Font EnFont;
    public Font ChFont;
    public ButtonMapping ButtonMapping;

    public bool TestMode;

    private static bool cursorvisible;

    public static bool Cursorvisible
    {
        get { return cursorvisible; }
        set
        {
            cursorvisible = value;
            if (!GameInput.UsingJoystick)
            {
                Cursor.lockState = value ? CursorLockMode.None : CursorLockMode.Locked;
                Cursor.visible = value;
            }
        }
    }

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
        //Cursor.lockState = CursorLockMode.Locked;
        //Cursorvisible = true;
        GameInput.OnSwitchController += UpdateCursor;
        GameInput.OnSwitchController += InitControllerType;
        SaveDataManager.MainCam = Camera.main;
        Screen.SetResolution(1920, 1080, true);

        if (TestMode)
        {
            SaveDataManager.TutorialPassed = true;
        }
    }

    private void InitControllerType(bool usingGamepad)
    {
        if (usingGamepad)
        {
            if (GameInput.RewiredPlayer.controllers.GetLastActiveController().name.Contains("Sony"))
                GameInput.JoyButtonType = ControllerType.PlayStation;
            if (GameInput.RewiredPlayer.controllers.GetLastActiveController().name.Contains("Nintendo"))
                GameInput.JoyButtonType = ControllerType.Switch;
            GameInput.OnSwitchController -= InitControllerType;
        }
    }

    private void UpdateCursor(bool usingController)
    {
        if (!usingController)
            Cursor.visible = Cursorvisible;
        else
            Cursor.visible = false;
    }

    private void Update()
    {
#if UNITY_EDITOR
        if (GameInput.Keyboard.GetKeyDown(KeyCode.F1))
            Cursorvisible = true;
#endif
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus)
        {
            Cursor.visible = Cursorvisible;
        }
    }
}