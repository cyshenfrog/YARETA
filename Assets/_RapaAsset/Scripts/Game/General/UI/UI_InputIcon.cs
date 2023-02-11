using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using NaughtyAttributes;

[RequireComponent(typeof(Image))]
public class UI_InputIcon : MonoBehaviour
{
    public ControllerButton ControllerIcon;

    public Sprite Controller
    { get { return GameManager.Instance.ControllerSprites.GetSprit(GameInput.JoystickBrandType, ControllerIcon); } }

    public Sprite KB;
    private ControllerButton ori_C;
    private Sprite ori_KB;
    public bool CustumSize_Joy;
    public bool CustumSize_KB;

    [ShowIf("CustumSize_Joy")]
    public Vector2 JoySize;

    [ShowIf("CustumSize_KB")]
    public Vector2 KBSize;

    private Vector2 originalSize;

    [HideInInspector]
    private Image icon;

    private void Awake()
    {
        ori_C = ControllerIcon;
        ori_KB = KB;
        icon = GetComponent<Image>();
        originalSize = icon.rectTransform.sizeDelta;
        Switch(GameInput.UsingJoystick);
        GameInput.OnSwitchController += Switch;
    }

    public void ResetKBIcon()
    {
        KB = ori_KB;
        Switch(GameInput.UsingJoystick);
    }

    public void ResetJoyIcon()
    {
        ControllerIcon = ori_C;
        Switch(GameInput.UsingJoystick);
    }

    public void UpdateIcon()
    {
        Switch(GameInput.UsingJoystick);
    }

    public void Switch(bool isController)
    {
        var s = isController ? Controller : KB;
        if (s)
        {
            icon.sprite = s;
            icon.rectTransform.sizeDelta = originalSize;
            if (isController && CustumSize_Joy)
                icon.rectTransform.sizeDelta = JoySize;
            if (!isController && CustumSize_KB)
                icon.rectTransform.sizeDelta = KBSize;
            gameObject.SetActive(true);
        }
        else
            gameObject.SetActive(false);
    }

    private void OnDestroy()
    {
        GameInput.OnSwitchController -= Switch;
    }
}