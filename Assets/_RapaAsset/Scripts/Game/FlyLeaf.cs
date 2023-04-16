using System.Collections;
using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Rigidbody))]
public class FlyLeaf : MonoBehaviour
{
    public Transform StandPos;
    public Transform Target;
    public Dragable Dragable;
    public float FlyForce = 10;
    public bool FlyHigh;
    private Rigidbody rb;
    private bool flying;
    private bool playerOn;
    private float shift;
    private float t;
    private float recenterCD;
    private bool playerIn;
    private float vSpeed;
    private Vector3 dir;
    private Vector3 initPos;

    // Start is called before the first frame update
    private void Start()
    {
        rb = GetComponent<Rigidbody>();
        initPos = transform.position;
    }

    // Update is called once per frame
    private void Update()
    {
        if (GameInput.Keyboard.GetKeyDown(KeyCode.F9))
        {
            Landing();
            transform.rotation = Quaternion.identity;
            transform.position = initPos;
        }
        if (flying)
        {
            if (GameInput.GetButtonDown(Actions.Jump))
            {
                ReleasePlayer();
            }
            //rotation
            dir = Target.position - transform.position;
            dir.y = 0;
            if (playerOn && GameInput.GetButton(Actions.Move))
            {
                //if (Vector3.Angle(transform.forward, dir) < 170)

                shift += GameInput.Move.x * .5f;
                //transform.localEulerAngles += Vector3.up * GameInput.Move.x * .5f;
                transform.Rotate(Vector3.up, GameInput.Move.x * .5f);
                recenterCD = 0;
            }
            else if (recenterCD < 10)
            {
                shift = Vector3.Angle(transform.forward, dir) * (shift < 0 ? -1 : 1);
                recenterCD += Time.deltaTime;
                transform.rotation = Quaternion.Lerp(transform.rotation, Quaternion.LookRotation(dir), Time.deltaTime / 2);
            }
            transform.localEulerAngles = new Vector3(transform.localEulerAngles.x, transform.localEulerAngles.y, Mathf.Clamp(-shift / 3, -30, 30));

            //speed
            if (t < 1)
            {
                t += Time.deltaTime * .1f;
            }
            if (vSpeed > -3)
            {
                vSpeed -= Time.deltaTime * 3;
            }
            rb.velocity = Vector3.Lerp(rb.velocity, transform.forward * FlyForce + Vector3.up * vSpeed, t);
        }
    }

    public void Interact()
    {
        if (playerIn)
            Fly(FlyHigh);
        else
            Dragable.StartDrag();
    }

    public void Fly(bool blowUp)
    {
        StartCoroutine(_Fly(blowUp));
    }

    public IEnumerator _Fly(bool blowUp)
    {
        rb.useGravity = false;
        Player.Instance.WalkTo(StandPos, () =>
        {
            Player.Instance.Status = PlayerStatus.Static;
            Player.Instance.transform.parent = transform;
        });
        yield return new WaitForSeconds(1);
        rb.freezeRotation = true;
        transform.DOLookAt(Target.position, 1, AxisConstraint.Y);
        transform.DOMoveY(.3f, 1)
            .SetEase(Ease.OutQuad)
            .SetRelative(true);
        yield return new WaitForSeconds(1);
        playerOn = flying = true;
        t = 0;
        vSpeed = blowUp ? 15 : 1f;
    }

    public void Landing()
    {
        flying = false;

        rb.useGravity = true;
        rb.freezeRotation = false;
        ReleasePlayer();
    }

    private void ReleasePlayer()
    {
        playerOn = false;
        Player.Instance.transform.parent = null;
        Player.Instance.Status = PlayerStatus.Moving;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.CompareTag("Water"))
            return;
        if (flying)
            Landing();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            playerIn = true;
    }

    private void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Player"))
            playerIn = false;
    }
}