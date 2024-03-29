using UnityEngine;
using System.Collections;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System;

/*
Class created for all aspects to player input which includes animation
*/
public class PlayerMovement : MonoBehaviour
{

    public LeftOnMouse leftMouse;
    public RightOnMouse rightMouse;
    private Rigidbody2D myRigidBody;
    public Animator animController;
    
    /* Player attributes */
    public string sceneName;
    public static float counter = 0;
    public bool collect = false;
    public static float livesCounter = 3;
    public float moveH;
    public float jumpForce = 400f;
    public float speed = 10f;
    public float move = 10f;
    public bool faceRight = true;
    public bool isDead = false;
    bool doorOpen = false;
    bool playerStart = true;
    bool death = false;
    bool moveLeft;
    bool moveRight;

    float groundRadius = 0.8f;

    /* Different ground check */
    public bool deadGround;
    public bool grounded;
    public LayerMask whatIsGround;
    public LayerMask whatIsDeadGround;   
    public Transform groundCheck;
    public Transform deadGroundCheck;

    /* Sound files */
    private AudioSource level1audio;
    AudioClip clip1;

    private AudioSource jumpAudio;
    AudioClip clip2;

    private AudioSource coinAudio;
    AudioClip clip3;

    private AudioSource gameAudio;
    AudioClip clip4;

    void Start()
    {        
        myRigidBody = GetComponent<Rigidbody2D>();
        animController = GetComponent<Animator>();
        
        /* level1 music */
        level1audio = (AudioSource)gameObject.GetComponent<AudioSource>();
        clip1 = (AudioClip)Resources.Load("LevelBeat");
        level1audio.clip = clip1;
        level1audio.loop = false;             

        /* jump sound */
        jumpAudio = (AudioSource)gameObject.GetComponent<AudioSource>();
        clip2 = (AudioClip)Resources.Load("jump");
        jumpAudio.clip = clip2;
        jumpAudio.loop = false;

        coinAudio = (AudioSource)gameObject.GetComponent<AudioSource>();
        clip3 = (AudioClip)Resources.Load("CollectCoin");
        coinAudio.clip = clip3;
        coinAudio.loop = false;

        gameAudio = (AudioSource)gameObject.GetComponent<AudioSource>();
        clip4 = (AudioClip)Resources.Load("Level1Soundwav");
        gameAudio.clip = clip4;
        gameAudio.loop = false;

        gameAudio.PlayOneShot(clip4, 1f);
        myRigidBody.transform.Translate(0, 0, 0);

        
        
    }
    

    /* ButtonB button references this method for jumping, 
     * jumpforce variable can be changed in the editor to alter how high player jumps */
    public void Jumping()
    {
        bool grounded = Physics2D.OverlapCircle(deadGroundCheck.position, groundRadius, whatIsGround);
        
        if (grounded)
        {
            animController.SetBool("isGrounded", true);
            myRigidBody.AddForce(new Vector2(0, jumpForce));
            jumpAudio.PlayOneShot(clip2, 1f);      
        }
     }
    
    /* This method is regarding flipping the player from facing left or 
     *right depending on direction player is headed */    
    void Flip()
    {
        faceRight = !faceRight;
        Vector3 theScale = transform.localScale;
        theScale.x *= -1;
        transform.localScale = theScale;
    }

    /* IEnumerator called for player yeild deatht timer */
    IEnumerator timeDeath()
    {
        if (!death)
        {
            gameAudio.Stop();
            level1audio.PlayOneShot(clip1, 1f);
            playerDead();
            death = true;

        }

        /* If player is dead, wait for 5 seconds for animation and then destroy player object */
        if (isDead)
        {
            yield return new WaitForSeconds(5);
            myRigidBody.gameObject.active = false;
        }
        
        /* Reset the level if player dies and still has more lives */
        if (livesCounter > 0)
        {
            Application.LoadLevel(Application.loadedLevel);
        }

        /* If player dies and has not lives. Go back to main menu */
        if (livesCounter <= 0)
        {
            Application.LoadLevel("GameMenu 1");
        }
    }

    /* PlayerDead method used for what happens to player when they die */ 
    void playerDead()
    {        
        isDead = true;
        livesCounter--;
    }    

    void Update()
    {
       
        moveH = Input.GetAxis("Horizontal");
     
        bool deadGround = Physics2D.OverlapCircle(deadGroundCheck.position, groundRadius, whatIsDeadGround);
        bool grounded = Physics2D.OverlapCircle(deadGroundCheck.position, groundRadius, whatIsGround);
        animController.SetBool("isSpeeding", false);
        animController.SetBool("isGrounded", grounded);
        animController.SetFloat("vSpeed", myRigidBody.velocity.y);

        /* Player start */
        if (playerStart)
        {
            myRigidBody.transform.Translate(0,0,-0.1f);
            if (myRigidBody.transform.position.z > 1)
            {
                myRigidBody.transform.Translate(0, 0, 0);
                playerStart = false;
            }
            
        }

        /* This relates to actions to take for when begining door is open of each level.
           Keep player movement to 0 and once open door. Then player can move */
        if (doorOpen == true)
        {
            moveH = 0;
            speed = 0;
            animController.SetBool("isSpeeding", false);
            StartCoroutine(DoorTimer());
        }

        /* Player is on dead ground */
        if (deadGround)
        {              
            animController.SetBool("isDead", deadGround);            
            myRigidBody.transform.Translate(0, 0, 0);
            speed = 0f;     
            StartCoroutine("timeDeath");   
            
        }

        /* player is on the ground and keyPress spacebar */
        if (grounded && Input.GetKeyDown(KeyCode.Space) && doorOpen == false)
        {
            jumpAudio.PlayOneShot(clip2, 1f);
            animController.SetBool("isGrounded", true);
            myRigidBody.AddForce(new Vector2(0, jumpForce ));
        }

        /* keyPress "a" or onScreen LeftButton is held down */
        if (Input.GetKey("a") || leftMouse.buttonHeld2 && doorOpen == false)
        {
                moveLeft = true;
                moveRight = false;
                animController.SetBool("isSpeeding", true);
                myRigidBody.transform.Translate((moveH - 1) * Time.deltaTime * speed, 0, 0);
        }

        /* keyPress "a" not held or onScreen LeftButton is not held */
        if (Input.GetKeyUp("a") || !leftMouse && doorOpen == false )
        {  
            animController.SetBool("isSpeeding", false);
        }

        /* KeyPress "d" is held or onScreen RightButton is held */
        if (Input.GetKey("d")  || rightMouse.buttonHeld  && doorOpen == false)
        {
                moveLeft = false;
                moveRight = true;
                animController.SetBool("isSpeeding", true);
                myRigidBody.transform.Translate((moveH + 1) * Time.deltaTime * speed, 0, 0);

        }

        /* KeyPress "d" is not held or onScreen RightButton is not held */
        if (Input.GetKeyUp("d") || !rightMouse && doorOpen == false)
        {
            animController.SetBool("isSpeeding", false);
        }

        /* Pressing escape or back key on android */
        if (Input.GetKeyDown(KeyCode.Escape))
        {
            print("Escape pressed");
            Application.LoadLevel("GameMenu 1");
        }

        if (moveRight == true && !faceRight)
        {
            //if (grounded && !deadGround)
                Flip();
        }
        else if (moveLeft == true && faceRight)
        {
            //if (grounded && !deadGround)
                Flip();
        }

    } //End update()

    IEnumerator DoorTimer()
    {
        yield return new WaitForSeconds(1f);        
        myRigidBody.gameObject.active = false;
        SceneChange(sceneName);

    }

    public void SceneChange(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }

    void CollectCoin(Collider2D coinCollider)
    {
        counter++;
        Destroy(coinCollider.gameObject);
    }


    void OnTriggerEnter2D(Collider2D collider)
    {
        /* Collider used for collecting coins */
        if (collider.gameObject.CompareTag("Coins"))
        {
            CollectCoin(collider);
            collect = true;
            coinAudio.PlayOneShot(clip3,1f);
        }
        
        /* Collider happens when player dies */
        if (collider.gameObject.CompareTag("Death"))
        {
            print("Collider death");
        }

        /* Collider happens when player collides with endDoor */
        if (collider.gameObject.CompareTag("EndDoor"))
        {
            doorOpen = true;
            print("Collides with End Door");
       
        }
        
    }


}//End Class







