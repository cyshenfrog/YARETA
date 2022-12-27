using System.Collections;
using DG.Tweening;
using UnityEngine;
using UnityEngine.UI;

using System;
using UnityEngine.Events;

public class FallingQTE : MonoBehaviour
{
    public GameObject OriginalRock;
    public GameObject Rock;
    public GameObject EventCam;
    public GameObject DodgeCam;
    public GameObject EndingCam;
    public GameObject EndingRock;
    public GameObject UI;
    public Transform DLight;
    public GameObject RockWaveGroup;
    public GameObject EndingWhiteStone;
    public GameObject OriginalWhiteStone;
    public GameObject FallingWhiteStone;

    public Transform EndingPos;
    public Transform[] RockWave1;
    public Transform[] RockWave2;
    public bool Save { get; set; }
    public BrokenRod Station;
    private Rigidbody[] FragmentRb;
    private Vector3[] FragmentInitPos;
    private bool whiteStoneTaken;

    private void Start()
    {
        FragmentRb = Rock.GetComponentsInChildren<Rigidbody>();
        FragmentInitPos = new Vector3[FragmentRb.Length];
        for (int i = 0; i < FragmentInitPos.Length; i++)
        {
            FragmentInitPos[i] = FragmentRb[i].transform.position;
        }
    }

    public void StartEvent()
    {
        StartCoroutine(Event());
        IEnumerator Event()
        {
            Player.Instance.PuzzleMode = true;
            yield return new WaitForSeconds(1f);
            Player.Instance.Status = PlayerStatus.Wait;
            EventCam.SetActive(true);
            yield return new WaitForSeconds(1f);
            Rock.SetActive(true);
            OriginalRock.SetActive(false);
            Time.timeScale = 0.25f;

            SEManager.Instance.PlaySystemSE(SystemSE.山崩開始);
            yield return new WaitForSeconds(2 * Time.timeScale);
            Time.timeScale = 1f;
            DodgeMode();
        }
    }

    private void DodgeMode()
    {
        EndingWhiteStone.SetActive(false);
        OriginalWhiteStone.SetActive(false);
        FallingWhiteStone.SetActive(true);
        DLight.localEulerAngles = new Vector3(90, DLight.localEulerAngles.y, DLight.localEulerAngles.z);
        Rock.SetActive(false);
        UI.SetActive(true);
        DodgeCam.SetActive(true);
        RockWaveGroup.SetActive(true);
        EventCam.SetActive(false);
        Player.Instance.Status = PlayerStatus.Moving;
    }

    public void Finish()
    {
        if (!UI.activeSelf)
            return;

        Player.Instance.PuzzleMode = false;
        if (Save)
        {
            Escape();
        }
        else
        {
            Time.timeScale = 1f;
            Player.Instance.Die(ResetAll, true);
        }
    }

    private void Escape()
    {
        Player.Instance.Status = PlayerStatus.Static;
        ResetEnv();
        Player.Instance.transform.SetPositionAndRotation(EndingPos.position, EndingPos.rotation);
        EndingCam.transform.position = Player.Instance.transform.position + Player.Instance.transform.forward * 3 + Vector3.up;
        EndingRock.SetActive(true);
        EndingCam.SetActive(true);
        StartCoroutine(d());
        IEnumerator d()
        {
            Time.timeScale = .7f;
            Player.Instance.Anim.SetTrigger("JumpAway");
            yield return new WaitForSeconds(1);
            SEManager.Instance.PlaySystemSE(SystemSE.山崩結束);
            Time.timeScale = 1f;
            yield return new WaitForSeconds(3);
            if (whiteStoneTaken)
                Obj_Info.Upload(24);
            else
            {
                EndingWhiteStone.SetActive(true);
                OriginalWhiteStone.SetActive(false);
                FallingWhiteStone.SetActive(false);
            }
            EndingCam.SetActive(false);
            Player.Instance.Status = PlayerStatus.Moving;
            CameraMain.Instance.CameraControl.rotation = Quaternion.identity;
        }
    }

    private void ResetAll()
    {
        ResetEnv();
        whiteStoneTaken = false;
        OriginalWhiteStone.SetActive(true);
        EndingWhiteStone.SetActive(false);
        FallingWhiteStone.SetActive(false);
        FallingWhiteStone.transform.localScale = Vector3.one * 2;

        Station.ResetStation();
        Rock.SetActive(false);
        OriginalRock.SetActive(true);
        for (int i = 0; i < FragmentInitPos.Length; i++)
        {
            FragmentRb[i].transform.position = FragmentInitPos[i];
            FragmentRb[i].velocity = Vector3.zero;
        }
        foreach (var collider in FallingWhiteStone.GetComponentsInChildren<Collider>())
        {
            collider.enabled = true;
        }
    }

    private void ResetEnv()
    {
        DodgeCam.SetActive(false);
        UI.SetActive(false);
        RockWaveGroup.SetActive(false);

        DLight.localEulerAngles = new Vector3(30, DLight.localEulerAngles.y, DLight.localEulerAngles.z);
    }

    public void WhiteStoneTake()
    {
        whiteStoneTaken = true;
    }
}