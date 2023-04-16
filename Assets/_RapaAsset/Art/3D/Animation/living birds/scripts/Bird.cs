using UnityEngine;
using DG.Tweening;

[RequireComponent(typeof(Animator))]
public class Bird : MonoBehaviour
{
    private enum BirdAnims
    {
        sing,
        preen,
        ruffle,
        peck,
        hopForward,
        hopBackward,
        hopLeft,
        hopRight,
    }

    public AudioClip song1;
    public AudioClip song2;
    public AudioClip flyAway;

    private Animator anim;

    private bool a;
    private bool b;

    private void Start()
    {
        InvokeRepeating("RandomAnim", Random.Range(0, 1), Random.Range(2f, 4));
        anim = gameObject.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerTrigger>())
        {
            Flee();
        }
    }

    private void RandomAnim()
    {
        //bird will display a behavior
        //in the perched state the bird can only sing, preen, or ruffle
        float rand = Random.value;
        if (rand < .2)
        {
            PlayAnim(BirdAnims.sing);
        }
        else if (rand < .4)
        {
            PlayAnim(BirdAnims.peck);
        }
        else if (rand < .6)
        {
            PlayAnim(BirdAnims.preen);
        }
        else if (rand < .8)
        {
            PlayAnim(BirdAnims.ruffle);
        }
        else if (rand < .9)
        {
            if (a)
                PlayAnim(BirdAnims.hopRight);
            else
                PlayAnim(BirdAnims.hopLeft);
            a = !a;
        }
        else if (rand <= 1)
        {
            if (b)
                PlayAnim(BirdAnims.hopBackward);
            else
                PlayAnim(BirdAnims.hopForward);
            b = !b;
        }
        //lets alter the agitation level of the brid so it uses a different mix of idle animation next time
        anim.SetFloat("IdleAgitated", Random.value);
    }

    private void PlayAnim(BirdAnims anim)
    {
        switch (anim)
        {
            case BirdAnims.sing:
                this.anim.SetTrigger("sing");
                break;

            case BirdAnims.ruffle:
                this.anim.SetTrigger("ruffle");
                break;

            case BirdAnims.preen:
                this.anim.SetTrigger("preen");
                break;

            case BirdAnims.peck:
                this.anim.SetTrigger("peck");
                break;

            case BirdAnims.hopForward:
                this.anim.SetInteger("hop", 1);
                break;

            case BirdAnims.hopLeft:
                this.anim.SetInteger("hop", -2);
                break;

            case BirdAnims.hopRight:
                this.anim.SetInteger("hop", 2);
                break;

            case BirdAnims.hopBackward:
                this.anim.SetInteger("hop", -1);
                break;
        }
    }

    //Called from AnimEvent
    private void ResetHopInt()
    {
        anim.SetInteger("hop", 0);
    }

    //Called from AnimEvent
    private void PlaySong()
    {
        if (Random.value < .5)
        {
            AudioSource.PlayClipAtPoint(song1, transform.position);
        }
        else
        {
            AudioSource.PlayClipAtPoint(song2, transform.position);
        }
    }

    private void Flee()
    {
        PrototypeMain.Instance.RemoveTargetScanData(49);
        CancelInvoke("OnGroundBehaviors");
        anim.SetTrigger("flee");
        Vector3 farAwayTarget = transform.position;
        farAwayTarget = new Vector3(Random.Range(-50, 50) * transform.localScale.x, 30 * transform.localScale.x, Random.Range(-50, 50) * transform.localScale.x);
        float scale = transform.localScale.x;
        float duration = (farAwayTarget).magnitude * .3f;
        transform.DOMove(farAwayTarget, duration)
            .SetRelative(true)
            .SetDelay(0.2f)
            .OnComplete(() => { Destroy(gameObject); });
        transform.DOLookAt(farAwayTarget, 1f);
        AudioSource.PlayClipAtPoint(flyAway, transform.position);
        anim.applyRootMotion = false;
    }
}