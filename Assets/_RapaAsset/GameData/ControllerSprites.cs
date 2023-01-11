﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ControllerButton
{
    A,
    B,
    X,
    Y,
    R1,
    R2,
    L1,
    L2,
    Up,
    Down,
    Left,
    Right,
    LStick,
    LStick_Down,
    RStick,
    RStick_Down,
    Plus,
    Minus,
    Home,
    None
}

public enum JoyIconType
{
    XBOX,
    PlayStation,
    Switch
}

[CreateAssetMenu]
public class ControllerSprites : ScriptableObject
{
    public Sprite[] Icon_Xbox;
    public Sprite[] Icon_PS;
    public Sprite[] Icon_Switch;

    public Sprite GetSprit(JoyIconType controllerType, ControllerButton button)
    {
        switch (controllerType)
        {
            case JoyIconType.XBOX:
            default:
                return Icon_Xbox[(int)button];

            case JoyIconType.PlayStation:
                return Icon_PS[(int)button];

            case JoyIconType.Switch:
                return Icon_Switch[(int)button];
        }
    }
}