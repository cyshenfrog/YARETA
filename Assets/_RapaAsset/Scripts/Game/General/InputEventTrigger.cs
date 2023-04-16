using UltEvents;
using UnityEngine;

public class InputEventTrigger : MonoBehaviour
{
    public Actions Button;
    public UltEvent PressEvent;

    private void Update()
    {
        if (GameInput.GetButtonDown(Button))
            PressEvent.Invoke();
    }
}