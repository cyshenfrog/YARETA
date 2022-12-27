using UnityEngine;
using UnityEngine.EventSystems;

public class BGMPlayer : MonoBehaviour, IPointerClickHandler
{
    public bool PlayOnEnable = false;
    public bool PlayOnClick = false;
    public bool Loop = false;
    public AudioClip BgmClip;
    public BgmChannels Channel = BgmChannels.Defult;

    private void OnEnable()
    {
        if (PlayOnEnable)
        {
            PlayBGMSound();
        }
    }

    #region IPointerClickHandler implementation

    public void OnPointerClick(PointerEventData eventData)
    {
        if (PlayOnClick)
        {
            PlayBGMSound();
        }
    }

    #endregion IPointerClickHandler implementation

    private void OnMouseUpAsButton()
    {
        if (
#if UNITY_IOS || UNITY_ANDROID
			! EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)
#else
            !EventSystem.current.IsPointerOverGameObject()
#endif
        )
        {
            if (PlayOnClick)
            {
                PlayBGMSound();
            }
        }
    }

    private void PlayBGMSound()
    {
        if (BgmClip)
        {
            BgmManager.Instance.BgmSources[(int)Channel].loop = Loop;
            BgmManager.Instance.PlayBgm(BgmClip, Channel);
        }
    }
}