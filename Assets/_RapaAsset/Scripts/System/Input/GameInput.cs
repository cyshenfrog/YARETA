using UnityEngine;
using System;
using Rewired;

public enum Actions
{
    Jump,
    RoketJump,
    Run,
    Interact,
    Left,
    Right,
    Touch,
    DrawMode,
    Menu,
    OffWork,
    Tutorial,
    Confirm,
    Cancel,
    Neither,
    Draw,
    Drag,
    ToggleRun,
    Up,
    Down,
    Move,
    EnumLength
}

public static class GameInput
{
    //Rewire Player Ref
    public static Rewired.Player RewiredPlayer
    { get { return ReInput.isReady ? ReInput.players.GetPlayer(0) : null; } }

    //Device And Joystick Icon Type

    public static Keyboard Keyboard { get { return ReInput.controllers.Keyboard; } }
    public static Mouse Mouse { get { return ReInput.controllers.Mouse; } }
    public static Joystick Joystick { get { return UsingJoystick ? (Joystick)ReInput.controllers.GetLastActiveController() : ReInput.controllers.GetJoystick(0); } }
    public static JoyIconType JoyButtonType;

    //Input States

    public static Action<bool> OnSwitchController = null;

    private static ControllerType lastControllerType;

    public static bool UsingJoystick
    {
        get
        {
            if (lastControllerType != ReInput.controllers.GetLastActiveControllerType())
            {
                lastControllerType = ReInput.controllers.GetLastActiveControllerType();
                OnSwitchController?.Invoke(lastControllerType == ControllerType.Joystick);
                UpdateCursor();
            }
            return lastControllerType == ControllerType.Joystick;
        }
    }

    private static bool cursorvisible;

    public static bool Cursorvisible
    {
        get { return cursorvisible; }
        set
        {
            cursorvisible = value;
            if (!UsingJoystick)
            {
                Cursor.visible = value;
            }
        }
    }

    #region Inputs

    public static bool AnyInput { get { if (Joystick != null) return Joystick.GetAnyButton() || Keyboard.GetAnyButton(); else return Keyboard.GetAnyButton(); } }

    public static bool IsMove
    {
        get
        {
            return RewiredPlayer.GetAxis2D("MoveX", "MoveY") != Vector2.zero;
        }
    }

    public static Vector2 Move
    {
        get
        {
            return RewiredPlayer.GetAxis2D("MoveX", "MoveY");
        }
    }

    private static Vector3 _movementCameraSpace;

    public static Vector3 MovementCameraSpace
    {
        get
        {
            _movementCameraSpace = GameRef.MainCam.transform.forward * Move.y + GameRef.MainCam.transform.right * Move.x;
            _movementCameraSpace.y = 0;
            return _movementCameraSpace;
        }
    }

    public static bool IsCameraMove
    {
        get
        {
            return RewiredPlayer.GetAxis2D("CameraX", "CameraY") != Vector2.zero;
        }
    }

    public static Vector2 CameraMove
    {
        get
        {
            return RewiredPlayer.GetAxis2D("CameraX", "CameraY");
        }
    }

    public static bool GetButton(Actions ButtonAction)
    {
        return RewiredPlayer.GetButton(ButtonAction.ToString());
    }

    public static bool GetButtonDown(Actions ButtonAction)
    {
        return RewiredPlayer.GetButtonDown(ButtonAction.ToString());
    }

    public static bool GetButtonUp(Actions ButtonAction)
    {
        return RewiredPlayer.GetButtonUp(ButtonAction.ToString());
    }

    #endregion Inputs

    [RuntimeInitializeOnLoadMethod]
    private static void Init()
    {
        foreach (var item in ReInput.controllers.GetJoysticks())
        {
            item.calibrationMap.GetAxis(3).deadZone = 0.25f;
        }
        ReInput.ControllerConnectedEvent += OnControllerConnected;
        OnSwitchController += _OnSwitchController;
    }

    private static void _OnSwitchController(bool useJoycon)
    {
        if (useJoycon)
        {
            if (RewiredPlayer.controllers.GetLastActiveController().name.Contains("Sony"))
                JoyButtonType = JoyIconType.PlayStation;
            if (RewiredPlayer.controllers.GetLastActiveController().name.Contains("Nintendo"))
                JoyButtonType = JoyIconType.Switch;
        }
    }

    private static void OnControllerConnected(ControllerStatusChangedEventArgs obj)
    {
        if (obj.controllerType == ControllerType.Joystick)
            ((Joystick)obj.controller).calibrationMap.GetAxis(3).deadZone = 0.25f;
    }

    public static void UpdateCursor()
    {
        if (UsingJoystick)
            Cursor.visible = false;
        else
        {
            Cursor.lockState = Cursorvisible ? CursorLockMode.None : CursorLockMode.Locked;
            Cursor.visible = Cursorvisible;
        }
    }
}