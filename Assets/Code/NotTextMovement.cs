using UnityEngine;
using System.Collections;

public class NotTextMovement : MonoBehaviour {

   private AudioSource audio;
    public Rigidbody2D textRigid;
    public float force = .1f;
    public bool move = true;
    AudioClip myaudioClip;
    // Use this for initialization
    void Start () {
        
        textRigid = GetComponent<Rigidbody2D>();

        audio = (AudioSource)gameObject.GetComponent<AudioSource>();
        myaudioClip = (AudioClip)Resources.Load("smash");
        audio.clip = myaudioClip;
        audio.loop = false;

        textRigid.transform.Translate(force, 0, 0);
  

    }
	
    void moveText()
    {
        textRigid.transform.Translate(force, 0, 0);
       
    }

    void stopText()
    {
        textRigid.transform.Translate(0, 0, 0);
        
        
    }
    
    
	// Update is called once per frame
	void Update () {
        
        if (move)
        {
            moveText();
        }
        
        if (textRigid.position.x >= -2 && move == true)
        {
            move = false;
            audio.PlayOneShot(myaudioClip, 1f);           
            force = 0;
            
            // clip.Play();
        }
    }
}
