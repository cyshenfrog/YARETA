using UnityEngine;
using System.Collections;
using DG.Tweening;

public class lb_Bird : MonoBehaviour
{
    private enum birdBehaviors
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
        InvokeRepeating("OnGroundBehaviors", Random.Range(0, 1), Random.Range(2f, 4));
    }

    private void OnEnable()
    {
        anim = gameObject.GetComponent<Animator>();
    }

    private void OnTriggerEnter(Collider other)
    {
        if (other.GetComponent<PlayerTrigger>())
        {
            Flee();
        }
    }

    private void OnGroundBehaviors()
    {
        //bird will display a behavior
        //in the perched state the bird can only sing, preen, or ruffle
        float rand = Random.value;
        if (rand < .2)
        {
            DisplayBehavior(birdBehaviors.sing);
        }
        else if (rand < .4)
        {
            DisplayBehavior(birdBehaviors.peck);
        }
        else if (rand < .6)
        {
            DisplayBehavior(birdBehaviors.preen);
        }
        else if (rand < .8)
        {
            DisplayBehavior(birdBehaviors.ruffle);
        }
        else if (rand < .9)
        {
            if (a)
                DisplayBehavior(birdBehaviors.hopRight);
            else
                DisplayBehavior(birdBehaviors.hopLeft);
            a = !a;
        }
        else if (rand <= 1)
        {
            if (b)
                DisplayBehavior(birdBehaviors.hopBackward);
            else
                DisplayBehavior(birdBehaviors.hopForward);
            b = !b;
        }
        //lets alter the agitation level of the brid so it uses a different mix of idle animation next time
        anim.SetFloat("IdleAgitated", Random.value);
    }

    private void DisplayBehavior(birdBehaviors behavior)
    {
        switch (behavior)
        {
            case birdBehaviors.sing:
                anim.SetTrigger("sing");
                break;

            case birdBehaviors.ruffle:
                anim.SetTrigger("ruffle");
                break;

            case birdBehaviors.preen:
                anim.SetTrigger("preen");
                break;

            case birdBehaviors.peck:
                anim.SetTrigger("peck");
                break;

            case birdBehaviors.hopForward:
                anim.SetInteger("hop", 1);
                break;

            case birdBehaviors.hopLeft:
                anim.SetInteger("hop", -2);
                break;

            case birdBehaviors.hopRight:
                anim.SetInteger("hop", 2);
                break;

            case birdBehaviors.hopBackward:
                anim.SetInteger("hop", -1);
                break;
        }
    }

    private void ResetHopInt()
    {
        anim.SetInteger("hop", 0);
    }

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