using UnityEngine;

public class StageMain<T> : UnitySingleton_D<T> where T : Component
{
    private void Update()
    {
        if (GameInput.Keyboard.GetKeyDown(KeyCode.F8))
            Player.Instance.Die();
    }

    // Start is called before the first frame update
    private void Start()
    {
        SaveDataManager.MainCam = Camera.main;
    }
}