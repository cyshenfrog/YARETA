using UnityEngine;
using UnityEngine.Events;
using NaughtyAttributes;

public class TalkTrigger : MonoBehaviour
{
    public TalkDataEnum Talk;
    public UnityEvent OnCompelete;
    public bool OverrideKey;
    public bool MovieMode;

    [ShowIf("OverrideKey")]
    public Actions OverrideAction;

    [ShowIf("OverrideKey")]
    public ControllerButton OverrideInputJoy;

    [ShowIf("OverrideKey")]
    public Sprite OverrideInputKB;

    public void ShowTalk()
    {
        if (MovieMode)
            UI_FullScreenFade.Instance.SetMovieMode(true);
        if (OverrideKey)
            UI_Talk.Instance.SetInputThisTime(OverrideInputKB, GameData.ControllerSprites.GetSprit(GameInput.JoystickBrandType, OverrideInputJoy), OverrideAction);
        UI_Talk.Instance.ShowTalk((int)Talk, () =>
        {
            OnCompelete.Invoke();
            if (MovieMode)
                UI_FullScreenFade.Instance.SetMovieMode(false);
        });
    }
}