using UnityEngine;

public class GameManager : UnitySingleton_D<GameManager>
{
    public bool TestMode;
    public GameObject prefab;

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.F1))
        {
            var obj = Instantiate(prefab);
            obj.transform.position = Vector3.up * 2;
        }
    }

    public override void Awake()
    {
        base.Awake();

        Application.targetFrameRate = 60;
        Screen.SetResolution(1920, 1080, true);

        GameRef.MainCam = Camera.main;
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