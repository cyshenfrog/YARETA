using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public enum ButtonAction
{
    MenuHint,
    PutDown
}

public class UI_General : UnitySingleton_D<UI_General>
{
    public GameObject[] InteractUI;

    public void ShowActionUI(ButtonAction interact)
    {
        InteractUI[(int)interact].SetActive(true);
    }

    public void CloseActionUI(ButtonAction interact)
    {
        InteractUI[(int)interact].SetActive(false);
    }
}