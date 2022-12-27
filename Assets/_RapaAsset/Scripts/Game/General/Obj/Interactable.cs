using DG.Tweening;
using UltEvents;
using UnityEngine;
using NaughtyAttributes;

public class Interactable : MonoBehaviour
{
    public bool Hold;
    public bool UseOverrideName;
    public bool OverrideIcon;

    [ShowIf("OverrideIcon")]
    public Sprite KB;

    [ShowIf("OverrideIcon")]
    public ControllerButton ControllerButton;

    [ShowIf("UseOverrideName")]
    public UIDataEnum OverrideActionName;

    public Actions InteractButton = Actions.Interact;
    public UIDataEnum ActionName;

    private void Start()
    {
    }

    public bool IsInteractable
    {
        get
        {
            if (!enabled || !gameObject.activeSelf)
                return false;
            return true;
        }
    }

    public bool BigObj;

    public UltEvent OnInteract;
    private Vector3 worldPos;

    public virtual void Interact()
    {
        if (!IsInteractable)
            return;
        //Player.Instance.PlayerTrigger.ClearUI(true);
        OnInteract.Invoke();
        Player.Instance.transform.DOLookAt(transform.position, 0.5f, AxisConstraint.Y);
    }

    public virtual void ShowIcon()
    {
        if (OverrideIcon)
            UI_InteractionHint.Instance.SetText(UseOverrideName ? OverrideActionName : ActionName, ControllerButton, KB);
        else
            UI_InteractionHint.Instance.SetText(UseOverrideName ? OverrideActionName : ActionName);
        if (BigObj)
        {
            worldPos = transform.position;
            worldPos.Set(worldPos.x, Player.Instance.transform.position.y + 1.6f, worldPos.z);
        }

        UI_InteractionHint.Instance.UpdateIcon(BigObj ? worldPos : transform.position);
    }
}