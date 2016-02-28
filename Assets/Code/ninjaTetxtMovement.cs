using UnityEngine;
using System.Collections;

public class ninjaTetxtMovement : MonoBehaviour
{

    public Rigidbody2D starRigid;
    public float force = .1f;
    public bool move = true;

    private AudioSource audio;
    AudioClip myaudioClip;

    IEnumerator timeDelay()
    {
        yield return new WaitForSeconds(2);

        if (move)
        {
            moveText();
        }

        if (starRigid.position.x >= 0 && move == true)
        {
            move = false;
            audio.PlayOneShot(myaudioClip, 1f);
            force = 0;

        }

    }
    // Use this for initialization
    void Start()
    {
        starRigid = GetComponent<Rigidbody2D>();

        audio = (AudioSource)gameObject.GetComponent<AudioSource>();
        myaudioClip = (AudioClip)Resources.Load("smash");
        audio.clip = myaudioClip;
        audio.loop = false;
    }

    void moveText()
    {
        starRigid.transform.Translate(force, 0, 0);
    }

    void stopText()
    {
        starRigid.transform.Translate(0, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine("timeDelay");

    }
}
