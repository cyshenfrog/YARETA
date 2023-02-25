using DG.Tweening;
using NaughtyAttributes;
using UnityEngine;

public class Player_Fairy : Player
{
    public override void OnStateChanged(PlayerStatus status)
    {
        base.OnStateChanged(status);

        if (status == PlayerStatus.Wait)
        {
            if (Scanning)
                StopScan();
        }
    }

    [BoxGroup("Player_Fairy")]
    [Space]
    public Transform InitHandPos_R;

    [BoxGroup("Player_Fairy")]
    public Transform InitHandPos_L;

    [BoxGroup("Player_Fairy")]
    public GlowMaterialTrigger GlowTrigger_R;

    [BoxGroup("Player_Fairy")]
    public GlowMaterialTrigger GlowTrigger_L;

    [BoxGroup("Player_Fairy")]
    public Vector3 size = new Vector3(.3f, .3f, .5f);

    [BoxGroup("Player_Fairy")]
    public Vector3 offset = new Vector3(0, 1.22f, 0);

    [BoxGroup("Player_Fairy")]
    public LayerMask TouchScanMask;

    [BoxGroup("Player_Fairy")]
    public float maxDistance = 1.3f;

    [BoxGroup("Player_Fairy")]
    public Shader TargetShader;

    private RaycastHit hit;
    private float handSpeed = 2;

    private bool Scanning;

    public override void Update()
    {
        base.Update();

        if (!CanScan)
        {
            if (Scanning)
                StopScan();
            return;
        }
        if (GameInput.GetButtonDown(Actions.Touch))
            StartScan();
        else if (GameInput.GetButtonUp(Actions.Touch))
            StopScan();
        if (Scanning)
        {
            //bool isHit = Physics.BoxCast(transform.position + offset + transform.right * size.x - transform.forward * 0.7f, size, transform.forward, out hit,
            //       transform.rotation, maxDistance, mask);
            //if (isHit)
            //{
            //    ScanHandTracker_R.position = Vector3.Lerp(ScanHandTracker_R.position, hit.point, Time.deltaTime * handSpeed);
            //    ScanHandTracker_R.rotation = Quaternion.Lerp(ScanHandTracker_R.rotation, Quaternion.LookRotation(hit.normal, Vector3.left), Time.deltaTime * handSpeed);
            //}
            //else
            //{
            //    ScanHandTracker_R.position = Vector3.Lerp(ScanHandTracker_R.position, InitHandPos_R.position, Time.deltaTime * handSpeed);
            //    ScanHandTracker_R.rotation = Quaternion.Lerp(ScanHandTracker_R.rotation, InitHandPos_R.rotation, Time.deltaTime * handSpeed);
            //}
            //isHit = Physics.BoxCast(transform.position + offset - transform.right * size.x - transform.forward * 0.7f, size, transform.forward, out hit,
            //      transform.rotation, maxDistance, mask);
            //if (isHit)
            //{
            //    ScanHandTracker_L.position = Vector3.Lerp(ScanHandTracker_L.position, hit.point, Time.deltaTime * handSpeed);
            //    ScanHandTracker_L.rotation = Quaternion.Lerp(ScanHandTracker_L.rotation, Quaternion.LookRotation(hit.normal, Vector3.left), Time.deltaTime * handSpeed);
            //}
            //else
            //{
            //    ScanHandTracker_L.position = Vector3.Lerp(ScanHandTracker_L.position, InitHandPos_L.position, Time.deltaTime * handSpeed);
            //    ScanHandTracker_L.rotation = Quaternion.Lerp(ScanHandTracker_L.rotation, InitHandPos_L.rotation, Time.deltaTime * handSpeed);
            //}

            Vector3 startPos = transform.position + offset + transform.right * (size.y + 0.3f) - transform.forward * 0.7f;
            bool isHitR = Physics.SphereCast(startPos, size.y, transform.forward, out hit,
                maxDistance, TouchScanMask, QueryTriggerInteraction.Ignore);
            if (isHitR)
            {
                GlowTrigger_R.SetTarget(hit.collider);
                ScanHandTracker_R.position = Vector3.Lerp(ScanHandTracker_R.position, hit.point, Time.deltaTime * handSpeed);
                ScanHandTracker_R.rotation = Quaternion.Lerp(ScanHandTracker_R.rotation, Quaternion.LookRotation(hit.normal, Vector3.left), Time.deltaTime * handSpeed);
                Player_IKManager.Instance.HeadRef.position = Vector3.Lerp(Player_IKManager.Instance.HeadRef.position, hit.point, Time.deltaTime * handSpeed);

                ScanHandTracker_L.position = Vector3.Lerp(ScanHandTracker_L.position, InitHandPos_L.position, Time.deltaTime * handSpeed);
                ScanHandTracker_L.rotation = Quaternion.Lerp(ScanHandTracker_L.rotation, InitHandPos_L.rotation, Time.deltaTime * handSpeed);
            }
            else
            {
                GlowTrigger_R.SetTarget(null);
                ScanHandTracker_R.position = Vector3.Lerp(ScanHandTracker_R.position, InitHandPos_R.position, Time.deltaTime * handSpeed);
                ScanHandTracker_R.rotation = Quaternion.Lerp(ScanHandTracker_R.rotation, InitHandPos_R.rotation, Time.deltaTime * handSpeed);

                startPos = transform.position + offset - transform.right * (size.y + 0.3f) - transform.forward * 0.7f;
                bool isHitL = Physics.SphereCast(startPos, size.y, transform.forward, out hit,
                    maxDistance, TouchScanMask, QueryTriggerInteraction.Ignore);
                if (isHitL)
                {
                    GlowTrigger_L.SetTarget(hit.collider);
                    ScanHandTracker_L.position = Vector3.Lerp(ScanHandTracker_L.position, hit.point, Time.deltaTime * handSpeed);
                    ScanHandTracker_L.rotation = Quaternion.Lerp(ScanHandTracker_L.rotation, Quaternion.LookRotation(hit.normal, Vector3.left), Time.deltaTime * handSpeed);
                    Player_IKManager.Instance.HeadRef.position = Vector3.Lerp(Player_IKManager.Instance.HeadRef.position, hit.point, Time.deltaTime * handSpeed);
                }
                else
                {
                    GlowTrigger_L.SetTarget(null);
                    ScanHandTracker_L.position = Vector3.Lerp(ScanHandTracker_L.position, InitHandPos_L.position, Time.deltaTime * handSpeed);
                    ScanHandTracker_L.rotation = Quaternion.Lerp(ScanHandTracker_L.rotation, InitHandPos_L.rotation, Time.deltaTime * handSpeed);
                    Player_IKManager.Instance.HeadRef.position = Vector3.Lerp(Player_IKManager.Instance.HeadRef.position, transform.position + transform.forward + Vector3.up * 1.7f, Time.deltaTime * handSpeed);
                }
            }
        }
    }

    public override void StartScan()
    {
        Player_IKManager.Instance.PlaySimpleIK(PlayerIK.RightHand, .5f, Ease.Linear, IKReachType.Position);
        Player_IKManager.Instance.PlaySimpleIK(PlayerIK.LeftHand, .5f, Ease.Linear, IKReachType.Position);
        Player_IKManager.Instance.StartLooking();
        foreach (var item in Scanners)
        {
            item.gameObject.SetActive(true);
        }
        Scanning = true;
        MoveMode = MoveMode.Aimming;
        MoveSpeed *= 0.5f;
        PlayerTrigger.enabled = false;
        CameraMain.Instance.SetCameraMode(CameraMode.Aim);
    }

    public override void StopScan()
    {
        Player_IKManager.Instance.ResumeSimpleIK(PlayerIK.RightHand, .5f);
        Player_IKManager.Instance.ResumeSimpleIK(PlayerIK.LeftHand, .5f);
        Player_IKManager.Instance.StopLooking();
        GlowTrigger_R.SetTarget(null);
        GlowTrigger_L.SetTarget(null);
        foreach (var item in Scanners)
        {
            item.SetActive(false);
        }
        MoveMode = MoveMode.Normal;
        Scanning = false;
        MoveSpeed *= 2f;
        PlayerTrigger.enabled = true;
        CameraMain.Instance.SetCameraMode(CameraMode.Default);
    }

    private void OnDrawGizmos()
    {
        Vector3 startPos = transform.position + offset + transform.right * (size.y + 0.3f) - transform.forward * 0.7f;
        bool isHit = Physics.SphereCast(startPos, size.y, transform.forward, out hit,
            maxDistance, TouchScanMask);
        if (isHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(startPos, transform.forward * hit.distance);
            Gizmos.DrawWireSphere(startPos + transform.forward * hit.distance, size.y);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hit.point, .2f);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(startPos, transform.forward * maxDistance);
        }
        startPos = transform.position + offset - transform.right * (size.y + 0.3f) - transform.forward * 0.7f;
        isHit = Physics.SphereCast(startPos, size.y, transform.forward, out hit,
            maxDistance, TouchScanMask);
        if (isHit)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawRay(startPos, transform.forward * hit.distance);
            Gizmos.DrawWireSphere(startPos + transform.forward * hit.distance, size.y);
            Gizmos.color = Color.green;
            Gizmos.DrawSphere(hit.point, .2f);
        }
        else
        {
            Gizmos.color = Color.green;
            Gizmos.DrawRay(startPos, transform.forward * maxDistance);
        }
        //Vector3 startPos = transform.position + transform.right * size.x + offset - transform.forward * 0.7f;
        //bool isHit = Physics.BoxCast(startPos, size, transform.forward, out hit, transform.rotation, maxDistance, mask);
        //if (isHit)
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawRay(startPos, transform.forward * hit.distance);
        //    Gizmos.DrawWireCube(startPos + transform.forward * hit.distance, size);
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawSphere(hit.point, .1f);
        //}
        //else
        //{
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawRay(startPos, transform.forward * maxDistance);
        //}
        //startPos = transform.position - transform.right * size.x + offset - transform.forward * 0.7f;
        //isHit = Physics.BoxCast(startPos, size, transform.forward, out hit,
        //    transform.rotation, maxDistance, mask);
        //if (isHit)
        //{
        //    Gizmos.color = Color.red;
        //    Gizmos.DrawRay(startPos, transform.forward * hit.distance);
        //    Gizmos.DrawWireCube(startPos + transform.forward * hit.distance, size);
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawSphere(hit.point, .1f);
        //}
        //else
        //{
        //    Gizmos.color = Color.green;
        //    Gizmos.DrawRay(startPos, transform.forward * maxDistance);
        //}
    }

    public void WindMode(bool on)
    {
        //if (on)
        //{
        //    MoveSpeed = 0.3f;
        //    EtaCloth.externalAcceleration = new Vector3(0, 10, -10);
        //    EtaCloth.randomAcceleration = new Vector3(0, 0, -10);
        //}
        //else
        //{
        //    MoveSpeed = 1f;
        //    EtaCloth.externalAcceleration = new Vector3(0, 5, 0);
        //    EtaCloth.randomAcceleration = new Vector3(0, 0, 0);
        //}
    }
}