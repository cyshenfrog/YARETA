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
    EnumLength
}

public static class GameInput
{
    public static Rewired.Player RewiredPlayer
    { get { return ReInput.isReady ? ReInput.players.GetPlayer(0) : null; } }

    public static Keyboard Keyboard { get { return ReInput.controllers.Keyboard; } }
    public static Mouse Mouse { get { return ReInput.controllers.Mouse; } }
    public static Joystick Joystick { get { return UsingJoystick ? (Joystick)ReInput.controllers.GetLastActiveController() : ReInput.controllers.GetJoystick(0); } }
    public static ControllerType JoyButtonType;

    private static Rewired.ControllerType lastControllerType;

    public static bool UsingJoystick
    {
        get
        {
            if (lastControllerType != ReInput.controllers.GetLastActiveControllerType())
            {
                lastControllerType = ReInput.controllers.GetLastActiveControllerType();
                OnSwitchController?.Invoke(lastControllerType == Rewired.ControllerType.Joystick);
            }
            return lastControllerType == Rewired.ControllerType.Joystick;
        }
    }

    public static Action<bool> OnSwitchController = null;

    public static bool AnyInput { get { return RewiredPlayer.GetAnyButton(); } }

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

    [RuntimeInitializeOnLoadMethod]
    public static void RegistOnControllerConnected()
    {
        foreach (var item in ReInput.controllers.GetJoysticks())
        {
            item.calibrationMap.GetAxis(3).deadZone = 0.25f;
        }
        ReInput.ControllerConnectedEvent += OnControllerConnected;
    }

    private static void OnControllerConnected(ControllerStatusChangedEventArgs obj)
    {
        if (obj.controllerType == Rewired.ControllerType.Joystick)
            ((Joystick)obj.controller).calibrationMap.GetAxis(3).deadZone = 0.25f;
    }
}