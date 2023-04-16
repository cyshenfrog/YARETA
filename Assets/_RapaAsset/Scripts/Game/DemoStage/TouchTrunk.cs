using DG.Tweening;
using System.Collections;
using UnityEngine;
using UnityEngine.Events;

public class TouchTrunk : MonoBehaviour
{
    public Interactable Interaction;
    public GameObject Cam;
    public GameObject EndingCam;
    public GameObject GuidingFX;
    public Transform LookingTarget;
    public SkinnedMeshRenderer Plent;
    public GameObject JointRoot;
    public UnityEvent EndingEvent;
    public AudioClip[] Whispers;
    public Material M_Trunk;
    public GameObject Light;
    public Transform NextPosition;
    public AudioSource FairyBGM;
    private int count = 11;
    private Tween tween;
    public Color TreeColor;
    private float f;
    private Tween t;

    public void StartTouch()
    {
        Player.Instance.Status = PlayerStatus.Wait;
        Player.Instance.MoveSpeed = 0.2f;
        Player_Fairy.Instance.CanScan = true;
        JointRoot.SetActive(true);
        Player.Instance.transform.DOLookAt(transform.position, 0.5f)
            .OnComplete(callback);
        void callback()
        {
            Player.Instance.WalkTo(transform.position - Player.Instance.transform.forward * 2.7f, () =>
            {
                Player.Instance.Status = PlayerStatus.Moving;
                Cam.SetActive(true);
                Player_Fairy.Instance.StartScan();
                Player.FacingTarget = LookingTarget;
                Player.Instance.MoveMode = MoveMode.Aimming;
                Interaction.enabled = false;
            });
        }
        UI_Talk.Instance.DontLock = true;
    }

    public void Glow(bool brightEnd = false)
    {
        t.Kill();
        t = DOTween.To(() => f, x => M_Trunk.SetColor("Color_63DE112", TreeColor * x), 3, 1)
            .SetLoops(brightEnd ? 3 : 4, LoopType.Yoyo);
    }

    public void Touch()
    {
        count--;
        tween.Kill();
        tween = DOTween.To(() => Plent.GetBlendShapeWeight(0), x => Plent.SetBlendShapeWeight(0, x), (8f - count) / 8f * 60f, .5f);
        if (count == 9)
        {
            Glow();
            UI_Talk.Instance.ShowTalk((int)TalkDataEnum.樹幹呢喃1, UI_Talk.Instance.TreeColor, null, true, 3);
            AudioSource.PlayClipAtPoint(Whispers[0], transform.position);
        }
        if (count == 6)
        {
            Glow();
            UI_Talk.Instance.ShowTalk((int)TalkDataEnum.樹幹呢喃2, UI_Talk.Instance.TreeColor, null, true, 3);
            AudioSource.PlayClipAtPoint(Whispers[1], transform.position);
        }
        if (count == 3)
        {
            Glow();
            UI_Talk.Instance.ShowTalk((int)TalkDataEnum.樹幹呢喃3, UI_Talk.Instance.TreeColor, null, true, 3);
            AudioSource.PlayClipAtPoint(Whispers[2], transform.position);
        }
        if (count == 0)
        {
            Glow();
            UI_Talk.Instance.ShowTalk((int)TalkDataEnum.樹幹呢喃4, UI_Talk.Instance.TreeColor, null, true, 3);
            AudioSource.PlayClipAtPoint(Whispers[3], transform.position);
        }
        if (count <= 0)
        {
            Ending();
        }
    }

    private void Ending()
    {
        Player.Instance.Status = PlayerStatus.Wait;
        Player.Instance.MoveSpeed = 1f;
        UI_FullScreenFade.Instance.SetMovieMode(true);
        GuidingFX.SetActive(true);
        EndingCam.SetActive(true);
        JointRoot.SetActive(false);
        StartCoroutine(d());

        IEnumerator d()
        {
            tween.Kill();
            yield return new WaitForSeconds(1f);
            tween = DOTween.To(() => Plent.GetBlendShapeWeight(0), x => Plent.SetBlendShapeWeight(0, x), 100f, 3f)
                .SetEase(Ease.Linear);
            yield return new WaitForSeconds(3f);
            tween = DOTween.To(() => Plent.GetBlendShapeWeight(0), x => Plent.SetBlendShapeWeight(0, x), 0f, 2f);
            tween = DOTween.To(() => Plent.GetBlendShapeWeight(1), x => Plent.SetBlendShapeWeight(1, x), 100f, 2f);

            Plent.material.DOFloat(-1f, "_CutoffHeight", 3)
                .SetDelay(1);
            yield return new WaitForSeconds(3f);

            SEManager.Instance.PlaySystemSE(SystemSE.精靈飛出2);
            yield return new WaitForSeconds(2f);
            Player.Instance.Status = PlayerStatus.Static;
            Player.Instance.transform.position = NextPosition.position;
            Player.Instance.transform.rotation = NextPosition.rotation;
            CameraMain.Instance.Recenter(0);
            M_Trunk.color = Color.black;
            Light.SetActive(false);
            UI_FullScreenFade.Instance.SetMovieMode(false);
            Finish(false);
            yield return new WaitForSeconds(1f);
            FairyBGM.Play();
            UI_Talk.Instance.ShowTalk((int)TalkDataEnum.樹幹呢喃5, UI_Talk.Instance.TreeColor, () => UI_Talk.Instance.DontLock = false, true, 3);
            AudioSource.PlayClipAtPoint(Whispers[2], Player.Instance.transform.position);
        }
    }

    public void Finish(bool enable = true)
    {
        EndingEvent.Invoke();
        JointRoot.SetActive(false);
        Cam.SetActive(false);
        EndingCam.SetActive(false);
        Player_Fairy.Instance.CanScan = false;
        Player.FacingTarget = null;
        Player.Instance.MoveMode = MoveMode.Normal;
        Player.Instance.Status = PlayerStatus.Moving;
        Player.Instance.MoveSpeed = 1f;
        Interaction.enabled = enable;
    }
}