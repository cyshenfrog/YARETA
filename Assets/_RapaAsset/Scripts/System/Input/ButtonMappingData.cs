using UnityEngine;

public enum MouseClick
{
    None = -1,
    leftClick,
    rightClick,
    middleClick,
    anyClick,
}

public enum Keys
{
    None = -1,
    A, B, C, D, E, F, G, H, I, J, K, L, M, N, O, P, Q, R, S, T, U, V, W, X, Y, Z,
    digit0, digit1, digit2, digit3, digit4, digit5, digit6, digit7, digit8, digit9,
    numpad0, numpad1, numpad2, numpad3, numpad4, numpad5, numpad6, numpad7, numpad8, numpad9, numLock,
    numpadEnter, numpadDivide, numpadMultiply, numpadPlus, numpadMinus, numpadPeriod, numpadEquals,
    F1, F2, F3, F4, F5, F6, F7, F8, F9, F10, F11, F12,
    backquote, quote, semicolon, comma, period, slash, backslash, leftBracket, rightBracket, minus, equals,
    leftShift, rightShift, leftAlt, rightAlt, leftCtrl, rightCtrl, leftCommand, rightCommand,
    space, enter, tab, capsLock, backSpace, contextMenu, escape,
    leftArrow, rightArrow, upArrow, downArrow,
    pageDown, pageUp, home, end, insert, delete, printScreen, scrollLock, pause
}

public enum Buttons
{
    None = -1,
    A,
    B,
    X,
    Y,
    LB,
    RB,
    LT,
    RT,
    Back,
    Start,
    LeftStickClick,
    RightStickClick
}

public enum Axis
{
    None = -1,
    DPad_Up,
    DPad_Down,
    DPad_Left,
    DPad_Right,
    RightStick_Up,
    RightStick_Down,
    RightStick_Left,
    RightStick_Right,
    LeftStick_Up,
    LeftStick_Down,
    LeftStick_Left,
    LeftStick_Right,
}

[System.Serializable]
public class ButtonMappingData
{
    public ButtonMappingData()
    {
        Button = Buttons.None;
        Axis = Axis.None;
        KB = Keys.None;
        Mouse = MouseClick.None;
    }

    [HideInInspector]
    public string name;

    [HideInInspector]
    public Actions Type;

    [SerializeField]
    public Buttons Button;

    [SerializeField]
    public Axis Axis;

    [SerializeField]
    public Keys KB;

    [SerializeField]
    public MouseClick Mouse;
}