using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UI_Begining : MonoBehaviour
{
    public GameObject EnglishUI;
    public GameObject ChineseUI;
    public GameObject LanguageRoot;
    public GameObject InputRoot;
    private bool IsCh;

    private void Start()
    {
        English();
    }

    private void Update()
    {
        if (GameInput.GetButtonDown(Actions.Right))
        {
            Chinese();
        }
        else if (GameInput.GetButtonDown(Actions.Left))
        {
            English();
        }
        else if (GameInput.GetButtonDown(Actions.Confirm))
        {
            Confirm();
        }
    }

    public void English()
    {
        IsCh = false;
        EnglishUI.SetActive(true);
        ChineseUI.SetActive(false);
    }

    public void Chinese()
    {
        IsCh = true;
        EnglishUI.SetActive(false);
        ChineseUI.SetActive(true);
    }

    public void Confirm()
    {
        SaveDataManager.Language = IsCh ? SystemLanguage.Chinese : SystemLanguage.English;
        LanguageRoot.SetActive(false);
        InputRoot.SetActive(true);
        GameInput.Cursorvisible = false;
    }
}