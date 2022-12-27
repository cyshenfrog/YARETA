using DG.Tweening;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Playables;
using UnityEngine.Rendering;

public class LakeGoddess : MonoBehaviour
{
    public bool SecretPool;
    public GameObject GoddessGroup;
    public GameObject SecretGroup;
    public GameObject LieGroup;
    public GameObject UI;
    public Transform GoddessModel;

    public Material GoldMat;
    public Material SilverMat;

    public Transform GoldPosition;
    public Transform SilverPosition;
    public Transform GivePosition;
    public Transform FX;
    public Volume PostFX;
    private Obj_Info originObj;
    private List<Obj_Info> redundentPool = new List<Obj_Info>();
    private List<Transform> giveObj = new List<Transform>();
    private bool selecting;
    private GameObject gold;
    private GameObject silver;
    private bool selectGold;
    private bool showing;
    private bool disabled;
    private int carrotCount;
    private int grapeCount;

    private void Update()
    {
        if (!selecting)
            return;
        if (GameInput.GetButtonDown(Actions.RightChoise))
            Select(0);
        else if (GameInput.GetButtonDown(Actions.LeftChoise)) Select(1);
        else if (GameInput.GetButtonDown(Actions.Neither)) Select(2);
    }

    private void OnTriggerEnter(Collider other)
    {
        var obj = other.GetComponent<Obj_Info>();
        if (obj)
        {
            if (!showing)
                Show(obj);
            else
                redundentPool.Add(obj);
        }
    }

    public void Show(Obj_Info obj)
    {
        if (disabled)
            return;
        if (!obj.GoddessWant)
            return;

        showing = true;
        originObj = obj;

        Player.Instance.Status = PlayerStatus.Wait;
        Player.Instance.PuzzleMode = true;
        Player.Instance.Model.SetActive(false);
        SEManager.Instance.PlaySystemSE(SystemSE.小女神登場);
        if (!SecretPool)
        {
            //FX.localPosition = new Vector3(FX.localPosition.x, -1.14f, FX.localPosition.z);
            //GoddessGroup.transform.position = Player.Instance.transform.position + Player.Instance.transform.forward * 2;
            GoddessGroup.transform.DOLookAt(Player.Instance.transform.position, 0, AxisConstraint.Y);
        }
        GoddessGroup.SetActive(true);

        gold = Instantiate(originObj.gameObject);
        gold.GetComponentInChildren<MeshRenderer>().material = GoldMat;
        gold.GetComponentInChildren<Obj_Info>().Gold = true;
        gold.GetComponentInChildren<Obj_Info>().Silver = false;
        gold.GetComponentInChildren<Rigidbody>().isKinematic = true;

        silver = Instantiate(originObj.gameObject);
        silver.GetComponentInChildren<MeshRenderer>().material = SilverMat;
        silver.GetComponentInChildren<Obj_Info>().Gold = false;
        silver.GetComponentInChildren<Obj_Info>().Silver = true;
        silver.GetComponentInChildren<Rigidbody>().isKinematic = true;

        FreezeObj(originObj.GetComponentInChildren<Rigidbody>());
        StartCoroutine(d());
        IEnumerator d()
        {
            yield return 0;
            gold.transform.SetParent(GoldPosition);
            gold.transform.localPosition = Vector3.zero;
            gold.transform.localRotation = Quaternion.identity;
            silver.transform.SetParent(SilverPosition);
            silver.transform.localPosition = Vector3.zero;
            silver.transform.localRotation = Quaternion.identity;
            yield return new WaitForSeconds(1);
            Player.Instance.Status = PlayerStatus.Wait;
            originObj.GetComponentInChildren<Rigidbody>().isKinematic = true;
        }
    }

    public void SecreatShow()
    {
        if (disabled)
            return;
        disabled = true;
        SEManager.Instance.PlaySystemSE(SystemSE.大女神出場);
        PrototypeMain.Instance.SecretEnding = true;
        UI_FullScreenFade.Instance.SetMovieMode(true);
        Player.Instance.Status = PlayerStatus.Wait;
        Player.Instance.Model.SetActive(false);
        GoddessModel.DOLocalMoveY(-0.7f, 3)
            .SetRelative()
            .SetLoops(2, LoopType.Yoyo);
        SecretGroup.SetActive(true);

        StartCoroutine(d2());
        IEnumerator d2()
        {
            yield return new WaitForSeconds(5);
            UI_FullScreenFade.Instance.SetMovieMode(false);
            PostFX.gameObject.SetActive(true);
            DOTween.To(() => PostFX.weight, x => PostFX.weight = x, 1, 2f);

            yield return new WaitForSeconds(2);
            SEManager.Instance.PlaySystemSE(SystemSE.女神呢喃);
            UI_Talk.Instance.ShowTalk((int)TalkDataEnum.女神告誡, Color.red, () =>
            {
                DOTween.To(() => PostFX.weight, x => PostFX.weight = x, 0, 2f)
                    .OnComplete(() => PostFX.gameObject.SetActive(false));
                SecretGroup.SetActive(false);
                Player.Instance.Status = PlayerStatus.Moving;
                Player.Instance.Model.SetActive(true);
            });
        }
    }

    public void ShowTalk()
    {
        //ToDo
        //if (selectGold)
        //    GoldCam.SetActive(true);
        //else
        //    SilverCam.SetActive(true);
        if (PrototypeMain.Instance.SecretEnding)
            return;

        UI_Talk.Instance.ShowTalk((int)TalkDataEnum.女神開場, StartSelect, true, 1, GameManager.Instance.ScanSheet.dataArray[originObj.ID].Name);
    }

    public void StartSelect()
    {
        Player.Instance.Status = PlayerStatus.Wait;
        UI.SetActive(true);
        selecting = true;
        GameManager.Cursorvisible = true;
    }

    // 0 gold, 1 silver, 2 none
    public void Select(int i)
    {
        SEManager.Instance.PlaySystemSE(SystemSE.UI確認);
        Player.Instance.Model.SetActive(true);
        GameManager.Cursorvisible = false;
        selecting = false;
        bool honest = false;
        switch (i)
        {
            case 0:
                honest = originObj.Gold;
                break;

            case 1:
                honest = originObj.Silver;
                break;

            case 2:
                honest = !originObj.Gold && !originObj.Silver;
                break;

            default:
                break;
        }
        if (honest)
        {
            UI.SetActive(false);
            GivePosition.gameObject.SetActive(true);
            EndingTalk();
            //GiveAll();
        }
        else
            YouLie();
        //selecting = false;
        //if (yes)
        //{
        //    if ((selectGold && originObj.Silver) || (!selectGold && originObj.Gold) || (!originObj.Silver && !originObj.Gold))
        //    {
        //        YouLie();
        //    }
        //    else
        //    {
        //        Give(selectGold ? GoldPosition.GetChild(0) : SilverPosition.GetChild(0));
        //    }
        //}
        //else
        //{
        //    if (!selectGold)
        //    {
        //        selectGold = true;
        //        ShowTalk();
        //    }
        //    else
        //    {
        //        if (!originObj.Gold && !originObj.Silver)
        //        {
        //            GiveAll();
        //        }
        //        else
        //        {
        //            UI_Talk.Instance.ShowTalk(3, () => { Close(); }, true);
        //        }
        //    }
        //}
    }

    private void Give(Transform obj)
    {
        giveObj.Add(obj);
        UI.SetActive(false);

        GivePosition.gameObject.SetActive(true);
        obj.SetParent(GivePosition, false);
    }

    private void GiveAll()
    {
        //SEManager.Instance.PlaySystemSE(SystemSE.女神贈與2);
        if (originObj.Silver || originObj.Gold)
        {
            Destroy(originObj);
        }
        else
        {
            originObj.transform.SetParent(GivePosition, false);
            originObj.transform.localPosition = Vector3.zero;
            originObj.GetComponentInChildren<Rigidbody>().isKinematic = false;
            giveObj.Add(originObj.transform);
        }
        gold.transform.SetParent(GivePosition, false);
        gold.transform.localPosition = Vector3.right;
        gold.GetComponentInChildren<Rigidbody>().isKinematic = false;
        silver.transform.SetParent(GivePosition, false);
        silver.transform.localPosition = Vector3.left;
        silver.GetComponentInChildren<Rigidbody>().isKinematic = false;
        giveObj.Add(gold.transform);
        giveObj.Add(silver.transform);
    }

    private void YouLie()
    {
        UI_Talk.Instance.ShowTalk(4, () => Close(), true, 3);
        UI.SetActive(false);
        LieGroup.SetActive(true);
        if (originObj.ScanData == ScanDataEnum.蘿蔔)
            carrotCount++;
        if (originObj.ScanData == ScanDataEnum.葡萄)
            grapeCount++;
        Tool_Coroutine.Instance.Delay(0.5f, () =>
        {
            Destroy(originObj);
            Destroy(silver);
            Destroy(gold);
        });

        if (originObj.ScanData == ScanDataEnum.蘿蔔 && carrotCount < 3)
            return;
        if (originObj.ScanData == ScanDataEnum.葡萄 && grapeCount < 4)
            return;

        if (originObj.ScanData == ScanDataEnum.便便)
        {
            PrototypeMain.Instance.RemoveTargetScanData(46);
            PrototypeMain.Instance.RemoveTargetScanData(47);
        }
        PrototypeMain.Instance.RemoveTargetScanData(originObj.ID);
        PrototypeMain.Instance.RemoveTargetScanData(originObj.ID + 1);
        PrototypeMain.Instance.RemoveTargetScanData(originObj.ID + 2);
    }

    private void FreezeObj(Rigidbody rb)
    {
        StartCoroutine(d());
        IEnumerator d()
        {
            rb.GetComponentInChildren<Collider>().isTrigger = true;
            yield return new WaitForSeconds(1);
            rb.isKinematic = true;
            rb.GetComponentInChildren<Collider>().isTrigger = false;
        }
    }

    public void EndingTalk()
    {
        UI_Talk.Instance.ShowTalk(3, () =>
        {
            GiveAll();
            Close();
            SEManager.Instance.PlaySystemSE(SystemSE.物品產生);
        }, true, 2);
    }

    private void Close()
    {
        foreach (var item in giveObj)
        {
            item.SetParent(null);
        }

        giveObj.Clear();
        showing = false;
        GivePosition.gameObject.SetActive(false);
        LieGroup.SetActive(false);
        GoddessGroup.SetActive(false);
        Player.Instance.PuzzleMode = false;
        Player.Instance.Status = PlayerStatus.Moving;
    }
}