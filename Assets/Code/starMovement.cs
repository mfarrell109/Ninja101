using UnityEngine;
using System.Collections;

public class starMovement : MonoBehaviour
{
    private AudioSource audio;
    private AudioSource audio2;
    public AudioClip myaudioClip;
    public AudioClip myaudioClip2;

    public Rigidbody2D textRigid;
    public float starForce = 0f;
    bool move = true;
    bool soundOn = true;
    private Vector3 offset;
    //private object arget;

    IEnumerator timeDelay()
    {
        yield return new WaitForSeconds(3f);
        
   
        if (soundOn)
        {
            audio.PlayOneShot(myaudioClip, 1f);
            soundOn = false;
        }
        if (move)
        {
            moveText();
        }
        if (textRigid.position.x <= 3.2  && move)
        {
            audio2.PlayOneShot(myaudioClip2, 1f);
            move = false;
            
        }

    }
    // Use this for initialization
    void Start()
    {
      
        textRigid = GetComponent<Rigidbody2D>();

        audio = (AudioSource)gameObject.GetComponent<AudioSource>();
        //myaudioClip = (AudioClip)Resources.Load("whooshSound");
        audio.clip = myaudioClip;
        audio.loop = false;

        audio2 = (AudioSource)gameObject.GetComponent<AudioSource>();
        //myaudioClip2 = (AudioClip)Resources.Load("knifeSound");
        audio2.clip = myaudioClip2;
        audio2.loop = false;


    }

    void moveText()
    {
   
        transform.RotateAround(transform.position, Vector3.forward, 15);
        textRigid.AddForce(Vector3.left * starForce);
      
    }

    void stopText()
    {
        textRigid.transform.Translate(0,0,0);

        
    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine("timeDelay");
        textRigid.Sleep();

    }
}
