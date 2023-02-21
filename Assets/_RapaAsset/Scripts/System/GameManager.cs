using System;
using UnityEngine;

public class GameManager : UnitySingleton_D<GameManager>
{
    public bool TestMode;

    public override void Awake()
    {
        base.Awake();

        Application.targetFrameRate = 60;
        Screen.SetResolution(1920, 1080, true);

        if (TestMode)
        {
            SaveDataManager.TutorialPassed = true;
        }
    }

    private void OnApplicationFocus(bool focus)
    {
        if (focus) GameInput.UpdateCursor();
    }
}