using System;
using System.Collections;
using UnityEngine;
using UnityEngine.EventSystems;
using NaughtyAttributes;

public class SEPlayer : MonoBehaviour, IPointerClickHandler
{
    [Header("------觸發開關-----")]
    public bool PlayOnEnable;

    public bool PlayOnClick;

    [Header("------碰撞標籤-----")]
    public LayerMask TargetLayrer;

    public string TargetTag;

    [Header("------音源控制-----")]
    public bool CustumSE;

    public SystemSE SystemSE;

    [ShowIf("CustumSE")]
    public AudioClip[] Audios = new AudioClip[1];

    [ShowIf("CustumSE")]
    public bool SpacialBlend;

    public bool RandomPitch;
    public float delay;
    public float Volumn = 1;

    public void SetVolumn(float v)
    {
        Volumn = v;
    }

    [ShowIf("CustumSE")]
    public float LowPitchRange = .95f;

    [ShowIf("CustumSE")]
    public float HighPitchRange = 1.05f;

    private void OnEnable()
    {
        if (PlayOnEnable && Audios.Length != 0)
        {
            PlaySE();
        }
    }

    #region IPointerClickHandler implementation

    public void OnPointerClick(PointerEventData eventData)
    {
        if (PlayOnClick)
        {
            PlaySE();
        }
    }

    #endregion IPointerClickHandler implementation

    private void OnMouseUpAsButton()
    {
        if (PlayOnClick)
        {
            if (
#if UNITY_IOS || UNITY_ANDROID
			! EventSystem.current.IsPointerOverGameObject(Input.GetTouch(0).fingerId)
#else
                !EventSystem.current.IsPointerOverGameObject()
#endif
            )
                PlaySE();
        }
    }

    private void _PlaySE(float delay, Vector3 pos)
    {
        if (!CustumSE)
        {
            SEManager.Instance.PlaySystemSE(SystemSE, Volumn, RandomPitch);
        }
        else if (SpacialBlend)
        {
            int randomIndex = 0;
            if (Audios.Length > 1)
                randomIndex = UnityEngine.Random.Range(0, Audios.Length);
            AudioSource.PlayClipAtPoint(Audios[randomIndex], pos, Volumn);
        }
        else
            SEManager.Instance.Play(Audios, Volumn, RandomPitch);
    }

    public void PlaySE()
    {
        if (delay > 0)
            Tool_Coroutine.Instance.Delay(delay, () => _PlaySE(delay, transform.position));
        else
            _PlaySE(delay, transform.position);
    }

    public void PlaySE(Vector3 position)
    {
        if (delay > 0)
            Tool_Coroutine.Instance.Delay(delay, () => _PlaySE(delay, transform.position));
        else
            _PlaySE(delay, position);
    }

    private void OnTriggerEnter(Collider other)
    {
        if (TargetLayrer != (TargetLayrer | (1 << other.gameObject.layer)) && (String.IsNullOrEmpty(TargetTag) ? true : !other.gameObject.CompareTag(TargetTag)))
            return;
        PlaySE(other.transform.position);
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (TargetLayrer != (TargetLayrer | (1 << collision.gameObject.layer)) && (String.IsNullOrEmpty(TargetTag) ? true : !collision.gameObject.CompareTag(TargetTag)))
            return;
        PlaySE(collision.transform.position);
    }
}