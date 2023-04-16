using DG.Tweening;
using System.Collections;
using UltEvents;
using UnityEngine;
using NaughtyAttributes;

public class BrokenRod : MonoBehaviour
{
    public Transform TouchPos;
    public Transform RodBlocksRoot;
    public Interactable Rod;
    public UltEvent OnCompelete;
    public UltEvent OnUnsealed;
    public Rigidbody[] RodBlocks;
    public bool TutorialRod;
    public bool SealedRod;
    public Material Mat;

    public Transform StandPos;

    [ShowIf("TutorialRod")]
    public GameObject Cam1;

    [ShowIf("TutorialRod")]
    public GameObject Cam2;

    [ShowIf("TutorialRod")]
    public GameObject Cam3;

    private void Start()
    {
        Mat.SetFloat("_barValue", .3f);
    }

    public void Interact()
    {
        Player.Instance.Status = PlayerStatus.Wait;
        Player.Instance.transform.DOLookAt(StandPos.position, 0f, AxisConstraint.Y)
            .OnComplete(() =>
            {
                Player.Instance.WalkTo(StandPos.position - Player.Instance.transform.forward * 1.4f, then);
            });

        //else
        //{
        //    Player.Instance.transform.DOLookAt(transform.position, 0.5f, AxisConstraint.Y)
        //        .OnComplete(() => { Player.Instance.WalkTo(transform.position - Player.Instance.transform.forward * 1.4f + Player.Instance.transform.right * 0.4f, then); });
        //}
        void then()
        {
            Recycle();
            //return;
            //StartCoroutine(delay());
            //IEnumerator delay()
            //{
            //    yield return new WaitForSeconds(.5f);
            //    Player.Instance.Pad.SetActive(true);
            //    Player_IKManager.Instance.PlaySimpleIK(TouchPos, PlayerIK.RightHand, 1, default, IKReachType.Position, .6f);
            //    Player_IKManager.Instance.PlaySimpleIK(TouchPos, PlayerIK.LeftHand, 1, default, IKReachType.Position, .6f);
            //    if (TutorialRod)
            //        Cam1?.SetActive(true);
            //    yield return new WaitForSeconds(2f);
            //    if (TutorialRod)
            //        Cam2?.SetActive(true);
            //    Mat.DOFloat(.7f, "_barValue", 4)
            //        .SetEase(Ease.InQuad);
            //    yield return new WaitForSeconds(5f);
            //    Recycle();
            //}
        }
    }

    private void Recycle()
    {
        StartCoroutine(d());

        IEnumerator d()
        {
            if (TutorialRod)
            {
                Cam3?.SetActive(true);
                UI_FullScreenFade.Instance.SetMovieMode(true);
                yield return new WaitForSeconds(1f);
            }
            //Player.Instance.transform.DOLocalRotate(Vector3.up * 45, 0.2f, RotateMode.FastBeyond360)
            //    .SetRelative(true);
            //yield return new WaitForSeconds(.2f);
            Player.Instance.Hammer.SetActive(true);

            Player.Instance.Anim.SetBool("HammerFail", SealedRod);
            Player.Instance.Anim.SetTrigger("Hammer");
            Player.Instance.Anim.applyRootMotion = true;
            //RodBlocks[0].isKinematic = false;
            yield return new WaitForSeconds(1f);
            SEManager.Instance.PlaySystemSE(SystemSE.打破柱子2);
            if (SealedRod)
            {
                transform.DOShakePosition(.5f, .1f, 50);
            }
            else
            {
                Rod.enabled = false;
                for (int i = 0; i < RodBlocks.Length; i++)
                {
                    RodBlocks[i].isKinematic = false;
                    if (i % 2 == 0)
                    {
                        //yield return new WaitForSeconds(.4f);
                    }
                }

                //Player.Instance.PlayerTrigger.UnRegist(Rod);
                OnCompelete.Invoke();
            }
            //Player_IKManager.Instance.ResumeSimpleIK(PlayerIK.RightHand, 1);
            //Player_IKManager.Instance.ResumeSimpleIK(PlayerIK.LeftHand, 1);
            yield return new WaitForSeconds(1f);
            Player.Instance.Hammer.SetActive(false);

            if (TutorialRod)
            {
                Cam1?.SetActive(false);
                Cam2?.SetActive(false);
                Cam3?.SetActive(false);
                UI_FullScreenFade.Instance.SetMovieMode(false);
            }
            Player.Instance.Status = PlayerStatus.Moving;

            //Player.Instance.Pad.SetActive(false);

            //Mat.SetFloat("_barValue", .3f);
        }
    }

    public void ResetStation()
    {
        Rod.enabled = true;
        foreach (var item in RodBlocks)
        {
            item.transform.parent = RodBlocksRoot;
            item.isKinematic = true;
            item.velocity = Vector3.zero;
            item.transform.localEulerAngles = Vector3.zero;
            item.transform.localPosition = Vector3.zero;
            item.transform.localScale = Vector3.one;
            if (item.name == "WhiteStone")
            {
                item.transform.localPosition += Vector3.up * 1.793f;
            }
        }
    }

    public void Unseal()
    {
        SealedRod = false;
        OnUnsealed.Invoke();
    }
}