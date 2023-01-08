using System;
using UnityEngine;
using UnityEngine.UI;

public enum ActionText
{
    互動,
    拔看看,
    繼續拔,
    用力拔,
}

public class UI_InteractionHint : UnitySingleton_D<UI_InteractionHint>
{
    public UI_ButtonBlink InteractButton;
    public UI_InputIcon InteractIcon;
    public Text ActionText;
    private Actions HoldButton;
    private bool hold;
    private UIDataEnum _currentText;

    private void Update()
    {
        if (hold)
        {
            if (GameInput.GetButtonUp(HoldButton))
            {
                InteractButton.Init();
            }
        }
    }

    public void SetText(UIDataEnum text, ControllerButton JoystickOverride = ControllerButton.None, Sprite KBOverride = null)
    {
        _currentText = text;
        ActionText.text = GameManager.Instance.UISheet.GetUIText(text);

        if (KBOverride)
            InteractIcon.KB = KBOverride;
        else
            InteractIcon.ResetKBIcon();

        if (JoystickOverride != ControllerButton.None)
            InteractIcon.ControllerIcon = JoystickOverride;
        else
            InteractIcon.ResetJoyIcon();

        InteractIcon.UpdateIcon();
    }

    public void UpdateIcon(Vector3 pos)
    {
        InteractButton.transform.position = SaveDataManager.MainCam.WorldToScreenPoint(pos);
    }

    public void InitIcon(Interactable obj)
    {
        HoldButton = InteractButton.Buttons = obj.InteractButton;
        hold = obj.Hold;
        InteractButton.SetHold(hold);
    }

    public void Interact(Interactable obj)
    {
        InteractButton.OnBlinkFinish = then;
        InteractButton.Press();
        void then()
        {
            obj.Interact();
            if (!obj.IsInteractable)
            {
                InteractButton.transform.position = 9999 * Vector3.down;
                Player.Instance.PlayerTrigger.UnRegist(obj);
            }
        }
    }

    public void CloseIcon()
    {
        Delay.Instance.Wait(UI_ButtonBlink.Duration, then);
        void then()
        {
            InteractButton.transform.position = 9999 * Vector3.down;
        }
    }
}