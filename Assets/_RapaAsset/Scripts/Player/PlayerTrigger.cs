using System.Collections.Generic;
using UnityEngine;

public class PlayerTrigger : MonoBehaviour
{
    private readonly List<Interactable> objList = new List<Interactable>();
    private Collider col;
    private bool forceClose;

    // Use this for initialization
    private void Awake()
    {
        col = GetComponent<Collider>();
    }

    // Update is called once per frame
    private void Update()
    {
        UpdateUI();
    }

    private void OnEnable()
    {
        Rescan();
    }

    private void OnDisable()
    {
        ClearUI();
    }

    private void OnTriggerEnter(Collider other)
    {
        var obj = other.transform.GetComponent<Interactable>();

        if (obj)
        {
            forceClose = false;
            if (obj.IsInteractable && !objList.Contains(obj))
            {
                objList.Add(obj);
                if (objList.Count == 1)
                    UI_InteractionHint.Instance.InitIcon(obj);
            }
        }
    }

    private void OnTriggerExit(Collider other)
    {
        var obj = other.transform.GetComponent<Interactable>();
        if (obj)
            UnRegist(obj);
    }

    private void Interact(Interactable obj)
    {
        UI_InteractionHint.Instance.Interact(obj);
        CheckObjList();
    }

    public void Interact()
    {
        if (!NearestObj)
            return;
        UI_InteractionHint.Instance.Interact(NearestObj);
        CheckObjList();
    }

    public void UnRegist(Interactable obj)
    {
        if (objList.Contains(obj))
            objList.Remove(obj);
        //Rescan();
        if (objList.Count == 0)
            ClearUI();
    }

    public void Rescan()
    {
        //GameData.FacingObject = null;
        objList.Clear();
        col.enabled = false;
        col.enabled = true;
    }

    private void UpdateUI()
    {
        if (objList.Count == 0 || forceClose)
            return;
        NearestObj.ShowIcon();
    }

    public void ClearUI(bool closeIconUntillNextTarget = false)
    {
        forceClose = closeIconUntillNextTarget;
        UI_InteractionHint.Instance.CloseIcon();
    }

    private void CheckObjList()
    {
        for (var i = 0; i < objList.Count; i++)
            if (!objList[i].IsInteractable)
            {
                objList.RemoveAt(i);
                i--;
            }
    }

    #region NearestObj

    private float angle;
    private float tempAngle;
    private int id;
    private int oldId;

    public Interactable NearestObj
    {
        get
        {
            if (objList.Count == 0)
                return null;
            if (objList.Count == 1)
                return objList[0];

            id = 0;
            for (var i = 0; i < objList.Count; i++)
            {
                tempAngle = Vector3.Angle(objList[i].transform.position - transform.position, transform.forward);
                if (i == 0)
                {
                    angle = tempAngle;
                }
                else
                {
                    if (angle > tempAngle)
                    {
                        angle = tempAngle;
                        id = i;
                    }
                }
            }
            if (oldId != id)
            {
                UI_InteractionHint.Instance.InitIcon(objList[id]);
            }
            oldId = id;
            return objList[id];
        }
    }

    #endregion NearestObj
}