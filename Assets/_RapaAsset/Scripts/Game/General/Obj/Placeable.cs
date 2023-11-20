using DG.Tweening;
using UltEvents;
using UnityEngine;

[RequireComponent(typeof(Collider))]
public class Placeable : Interactable
{
    public PlaceTag TargetTag;
    public Transform PlacePoint;
    public Portable Target;
    public UltEvent OnCorrect;
    private Portable currentObj;
    private UIDataEnum initActionName;

    private void Start()
    {
        initActionName = ActionName;
    }

    public override void Interact()
    {
        if (!IsInteractable)
            return;
        if (!GameRef.CarringObj)
        {
            currentObj.Take();
            currentObj = null;
            ActionName = initActionName;
            return;
        }
        if (!GameRef.CarringObj.GetComponent<PlaceObjTag>())
            return;
        if (GameRef.CarringObj.GetComponent<PlaceObjTag>().Tag != TargetTag)
            return;

        OnInteract.Invoke();
        Player.Instance.transform.DOLookAt(transform.position, 0.5f, AxisConstraint.Y);

        currentObj = GameRef.CarringObj;
        if (currentObj == Target)
            OnCorrect.Invoke();
        ActionName = UIDataEnum.®³¨ú;
        GameRef.CarringObj.transform.parent = PlacePoint;
        GameRef.CarringObj.transform.SetLocalPositionAndRotation(Vector3.zero, Quaternion.identity);
        Player_IKManager.Instance.ResumeTwoHandIK(() =>
        {
            GameRef.CarringObj = null;
            Player.Instance.PlayerTrigger.UnRegist(this);
            Player.Instance.Status = PlayerStatus.Moving;
        });
    }
}