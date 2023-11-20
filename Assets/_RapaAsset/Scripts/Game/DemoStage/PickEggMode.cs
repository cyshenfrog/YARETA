using System.Collections;
using UnityEngine;
using DG.Tweening;

public class PickEggMode : MonoBehaviour
{
    public GameObject EventCam;
    public Obj_Info StoneInfo;
    public Obj_Info EggInfo;
    public bool DrawEgg { get; set; }
    public GameObject BrokenEgg;
    public GameObject GooseAngryGroup;
    public GameObject PickUI;
    public GameObject DrawUI;
    private Goose TheGoose;
    private bool picking;
    private bool active;

    public void Resume(bool Draw = false)
    {
        if (!Draw)
            Player.Instance.Anim.SetTrigger("Resume");
        Player.Instance.SetModelActive(true);
        if (DrawEgg && TheGoose)
            Player.Instance.Egg.SetActive(false);
        else
            Player.Instance.Rock.SetActive(false);
        Player.Instance.FPCam.SetActive(false);
        DrawUI.SetActive(false);
        PickUI.SetActive(true);
        if (TheGoose)
        {
            Player_IKManager.Instance.PlayTwoHandIK(GameRef.CarringObj.interaction);
            TheGoose.GetComponentInChildren<Renderer>().enabled = true;
        }
        picking = false;
        active = true;
        Player.Instance.Status = PlayerStatus.Moving;
    }

    public void Pick()
    {
        PickUI.SetActive(false);
        if (TheGoose)
            Player_IKManager.Instance.ResumeTwoHandIK();
        Player.Instance.Anim.SetTrigger("PickAndLook");
        active = false;
        Player.Instance.Status = PlayerStatus.Wait;
        StartCoroutine(d());
        IEnumerator d()
        {
            yield return new WaitForSeconds(1f);
            if (DrawEgg && TheGoose)
                Player.Instance.Egg.SetActive(true);
            else
                Player.Instance.Rock.SetActive(true);
            yield return new WaitForSeconds(1.5f);
            if (TheGoose)
                TheGoose.GetComponentInChildren<Renderer>().enabled = false;
            DrawUI.SetActive(true);
            active = picking = true;
            //DrawingMode.Instance.StartDrawingMode();
            //Player.Instance.Model.SetActive(true);
            //DrawingMode.Instance.DrawingUI.SetActive(false);
            Player.Instance.FPCam.SetActive(true);
            Player.Instance.FPCam.transform.localEulerAngles = Vector3.right * 48;
        }
    }

    public void Draw()
    {
        DrawUI.SetActive(false);
        active = false;
        StartCoroutine(d());
        IEnumerator d()
        {
            Player.Instance.Anim.SetTrigger("Resume");
            yield return new WaitForSeconds(.5f);
            if (DrawEgg && TheGoose)
                Player.Instance.Egg.SetActive(false);
            else
                Player.Instance.Rock.SetActive(false);
            Player.Instance.SetModelActive(false);
            DrawingMode.Instance.Draw((DrawEgg && TheGoose) ? EggInfo : StoneInfo, 0);
            yield return new WaitForSeconds(3);
            Resume(true);
        }
    }

    public void StartMode()
    {
        if (GameRef.CarringObj)
        {
            TheGoose = GameRef.CarringObj.GetComponent<Goose>();
            if (TheGoose)
            {
                GameRef.CarringObj.DontDrop = true;
            }
        }
        CameraMain.Instance.Lock = true;
        Player.Instance.PuzzleMode = true;
        active = enabled = true;
        EventCam.SetActive(true);
        PickUI.SetActive(true);
        DrawUI.SetActive(false);
        DrawingMode.Instance.LogUI.SetActive(true);
        Player.Instance.MoveSpeed = 0.2f;
    }

    public void EndMode()
    {
        CameraMain.Instance.Lock = false;
        Player.Instance.PuzzleMode = false;
        DrawingMode.Instance.LogUI.SetActive(false);
        //CameraMain.Instance.Recenter(0);
        active = enabled = false;
        Player.Instance.PlayerTrigger.Rescan();
        //CameraMain.Instance.SetCameraMode(CameraMode.Default3rdPerson);
        Player.Instance.MoveSpeed = 1;
        StartCoroutine(d());
        IEnumerator d()
        {
            yield return new WaitForSeconds(0);
            EventCam.SetActive(false);
            yield return new WaitForSeconds(1f);
            Player.Instance.PlayerTrigger.Rescan();
        }
    }

    public void OnStepOnEgg()
    {
        if (!enabled)
            return;
        if (!GameRef.CarringObj)
            return;
        if (!GameRef.CarringObj.GetComponent<Goose>())
            return;
        StopAllCoroutines();
        Resume();
        SEManager.Instance.PlaySystemSE(SystemSE.蛋破掉);
        Player.Instance.Status = PlayerStatus.Wait;
        BrokenEgg.SetActive(true);
        DOTween.To(() => Player.Instance.Anim.GetLayerWeight(1), x => Player.Instance.Anim.SetLayerWeight(1, x), 0.8f, 0.2f);
        Player.Instance.Anim.SetTrigger("StepOnEgg");
        Player.Instance.WalkBack(0.1f, 0.3f);
        enabled = false;
        PickUI.SetActive(false);
        DrawUI.SetActive(false);
        StartCoroutine(d());
        IEnumerator d()
        {
            yield return new WaitForSeconds(2f);

            //Player.Instance.WalkBack(0.2f, .5f);
            GooseAngryGroup.SetActive(true);
            EndMode();
            //Player.Instance.PlayerTrigger.UnRegist(Interaction);
            Player.Instance.PlayerTrigger.Rescan();
            yield return new WaitForSeconds(.2f);
            SEManager.Instance.PlaySystemSE(SystemSE.鵝生氣);
            Player.Instance.Anim.SetTrigger("GooseBack");
            Player.Instance.Anim.SetLayerWeight(1, 0);
            yield return new WaitForSeconds(.2f);

            //TheGoose.GetComponent<Rigidbody>().isKinematic = true;
            GameRef.CarringObj.Drop();
            yield return new WaitForSeconds(.2f);
            TheGoose.transform.DOLocalRotate(Vector3.zero, 1, RotateMode.FastBeyond360);
            yield return new WaitForSeconds(2f);
            TheGoose.AI.PauseAI();
            TheGoose.gameObject.SetActive(false);
            //TheGoose.GetComponentInChildren<LookAtIK>().solver.IKPositionWeight = 0;
            //TheGoose.transform.DOMove(Player.Instance.transform.forward + Vector3.up, 5)
            //    .SetEase(Ease.Linear)
            //    .OnComplete(() => { TheGoose.gameObject.SetActive(false); });
            //TheGoose.transform.DOLookAt(TheGoose.transform.position + Player.Instance.transform.forward + Vector3.up, 0, AxisConstraint.Y);
            GooseAngryGroup.SetActive(false);
            gameObject.SetActive(false);
            Player.Instance.Status = PlayerStatus.Moving;
        }
    }

    public void OnPlayerExit()
    {
        if (TheGoose)
        {
            TheGoose = null;
            GameRef.CarringObj.DontDrop = false;
        }
        if (enabled)
        {
            EndMode();
        }
    }
}