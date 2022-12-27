using HinputClasses;
using UnityEngine;
using System;

public enum Actions
{
    Jump,
    RoketJump,
    Running,
    Interact,
    LeftChoise,
    RightChoise,
    Scan,
    DrawMode,
    Menu,
    OffWork,
    Tutorial,
    Confirm,
    Cancel,
    Neither,
    Draw,
    Drag,
    EnumLength
}

public static class GameInput
{
    public static ControllerType ControllerType;
    private static bool _usingGamepad;

    public static bool usingGamepad
    {
        get { return _usingGamepad; }
        set
        {
            if (_usingGamepad != value)
            {
                _usingGamepad = value;
                OnSwitchController?.Invoke(value);
            }
            else
                _usingGamepad = value;
        }
    }

    public static Action<bool> OnSwitchController = null;
    public static bool AnyInput { get { return Hinput.anyInput || Hinput.keyboard.anyKey || Hinput.mouse.anyClick; } }

    public static Keyboard Keyboard { get { return Hinput.keyboard; } }

    public static bool IsMove
    {
        get
        {
            if (usingGamepad)
            {
                return Move != Vector2.zero;
            }
            else
            {
                return Keyboard.W || Keyboard.A || Keyboard.S || Keyboard.D;
            }
        }
    }

    public static Vector2 Move
    {
        get
        {
            if (usingGamepad)
            {
                return Hinput.anyGamepad.leftStick;
            }
            else
            {
                return new Vector2((Keyboard.D ? 1 : 0) - (Keyboard.A ? 1 : 0), (Keyboard.W ? 1 : 0) - (Keyboard.S ? 1 : 0));
            }
        }
    }

    public static Vector3 MovementCameraSpace
    {
        get
        {
            return SaveDataManager.MainCam.transform.forward * Move.y + SaveDataManager.MainCam.transform.right * Move.x;
        }
    }

    public static bool IsCameraMove
    {
        get
        {
            if (usingGamepad)
            {
                return Hinput.anyGamepad.rightStick.distance > 0;
            }
            else
            {
                return Hinput.mouse.delta != Vector2.zero;
            }
        }
    }

    public static Vector2 CameraMove
    {
        get
        {
            if (usingGamepad)
            {
                return Hinput.anyGamepad.rightStick;
            }
            else
            {
                return Hinput.mouse.delta * 100;
            }
        }
    }

    private static ButtonMappingData mappingData;

    public static bool GetButton(Actions ButtonAction)
    {
        if (GameManager.Instance.ButtonMapping.HasMapping(ButtonAction))
            return Button(ButtonAction).pressed;
        else
            return false;
    }

    public static bool GetButtonDown(Actions ButtonAction)
    {
        if (GameManager.Instance.ButtonMapping.HasMapping(ButtonAction))
            return Button(ButtonAction).justPressed;
        else
            return false;
    }

    public static bool GetButtonUp(Actions ButtonAction)
    {
        if (GameManager.Instance.ButtonMapping.HasMapping(ButtonAction))
            return Button(ButtonAction).justReleased;
        else
            return false;
    }

    public static Pressable Button(Actions Button)
    {
        mappingData = GameManager.Instance.ButtonMapping.GetMapping(Button);
        if (usingGamepad)
        {
            if (mappingData.Button != Buttons.None)
            {
                return Hinput.anyGamepad.buttons[(int)mappingData.Button];
            }
            else if (mappingData.Axis != Axis.None)
            {
                switch (mappingData.Axis)
                {
                    case Axis.DPad_Up:
                        return Hinput.anyGamepad.dPad.up;

                    case Axis.DPad_Down:
                        return Hinput.anyGamepad.dPad.down;

                    case Axis.DPad_Left:
                        return Hinput.anyGamepad.dPad.left;

                    case Axis.DPad_Right:
                        return Hinput.anyGamepad.dPad.right;

                    case Axis.RightStick_Up:
                        return Hinput.anyGamepad.rightStick.up;

                    case Axis.RightStick_Down:
                        return Hinput.anyGamepad.rightStick.down;

                    case Axis.RightStick_Left:
                        return Hinput.anyGamepad.rightStick.left;

                    case Axis.RightStick_Right:
                        return Hinput.anyGamepad.rightStick.right;

                    case Axis.LeftStick_Up:
                        return Hinput.anyGamepad.leftStick.up;

                    case Axis.LeftStick_Down:
                        return Hinput.anyGamepad.leftStick.down;

                    case Axis.LeftStick_Left:
                        return Hinput.anyGamepad.leftStick.left;

                    case Axis.LeftStick_Right:
                        return Hinput.anyGamepad.leftStick.right;

                    case Axis.None:
                    default:
                        return null;
                }
            }
            else
                return null;
        }
        else
        {
            if (mappingData.KB != Keys.None)
            {
                return Hinput.keyboard.keys[(int)mappingData.KB];
            }
            else if (mappingData.Mouse != MouseClick.None)
            {
                switch (mappingData.Mouse)
                {
                    case MouseClick.leftClick:
                        return Hinput.mouse.leftClick;

                    case MouseClick.rightClick:
                        return Hinput.mouse.rightClick;

                    case MouseClick.middleClick:
                        return Hinput.mouse.middleClick;

                    case MouseClick.anyClick:
                        return Hinput.mouse.anyClick;

                    case MouseClick.None:
                    default:
                        return null;
                }
            }
            else
                return null;
        }
    }
}