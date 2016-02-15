using UnityEngine;
using System.Collections;

public class AiController : MonoBehaviour
{
    private enum FaceDirection // Ensure only two directions exist to convert to boolean values...
    {
        RIGHT,
        LEFT
    }
    private enum State
    {
        IDLE,
        RUN,
        THROW,
        JUMP,
        DEAD
    }

    [Tooltip("Throwing force at a specific impulse aimed at the target")]
    public float throwForce = 100f;
    [Tooltip("Object to throw")]
    public GameObject throwable;
    [Tooltip("Relative position to throw objects in the right direction")]
    public Transform rightThrowPosition;
    [Tooltip("Relative position to throw objects in the left direction")]
    public Transform leftThrowPosition;

    [Tooltip("Jumping force at a specific vertical impulse")]
    public float jumpForce = 900f;
    [Tooltip("Layer mask that determines walkable ground")]
    public LayerMask groundLayer;
    [Tooltip("Layer mask that determines damaging ground")]
    public LayerMask deadGroundLayer;
    [Tooltip("Relative position in which the sprite is considered to be on the ground")]
    public Transform groundingPosition;

    private Rigidbody2D rigidBody;
    private SpriteRenderer spriteRenderer; // Assumes a SpriteRenderer exists
    private Animator animator; // Animator attached to the GameObject, if any.
    private bool updateAnimator = true; // Flip this flag to true if you want to update the animator on the next frame

    private State state = State.IDLE; // Current state of the AI GameObject
    private FaceDirection direction = FaceDirection.RIGHT;
    private float groundRadius = 0.8f;
    private bool isGrounded = false;
    private float jumpDelay = 0.2f; // Time required between jumps
    private float jumpTime = 0.0f; // Time since last jump

    private float throwDelay = 2.0f; // Time between throws to avoid spam
    private float throwTime = 0.0f; // Time since last throw
    private float throwAnimDelay = 0.4f; // Time to wait during throw state to sync up to the animation
    private float throwAnimTime = 0.0f; // Time since throw started

    private float deathTime = 0.0f; // Time since death
    private float deathDelay = 4.0f; // Lay on ground for four seconds

    private Vector3 moveDelta = new Vector3(); // Used to determine transition between movement states
    private Vector3 lastMovePos = new Vector3(); // Used to calculate movement delta


    // Use this for initialization
    void Start()
    {
        rigidBody = GetComponent<Rigidbody2D>();
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
    }

    // Update is called once per frame
    void Update()
    {
        HandleGrounding();
        HandleActionStates();

        if (state != State.DEAD)
        {
            UpdateAI();
        }
    }

    void LateUpdate()
    {
        moveDelta = transform.position - lastMovePos;
        lastMovePos.Set(transform.position.x, transform.position.y, transform.position.z);
    }

    private void UpdateAI()
    {
        // TODO: Actually do stuff
        if (isGrounded)
        {
            SetToFace(FaceDirection.RIGHT);
            Throw();
            //transform.Translate(Time.deltaTime, 0f, 0f);
        }
    }

    private void HandleActionStates()
    {
        if (animator != null && updateAnimator == true)
        {
            if (updateAnimator == true)
            {
                animator.SetInteger("state", (int)state);
                animator.SetBool("isGrounded", isGrounded);
                updateAnimator = false;
            }
        }

        switch (state)
        {
            case State.IDLE:
                OnIdle();
                break;
            case State.RUN:
                OnRun();
                break;
            case State.THROW:
                OnThrow();
                break;
            case State.JUMP:
                OnJump();
                break;
            case State.DEAD:
                OnDead();
                break;
        }
    }

    private void HandleGrounding()
    {
        if (IsOverGround())
        {
            SetGrounded(true);
            if (state == State.JUMP) // Shouldn't be jumping if grounded. Revert to idle for movement check
            {
                SetState(State.IDLE);
            }
        }
        else if (IsOverDeadGround())
        {
            SetState(State.DEAD);
        }
        else // Must be jumping or falling
        {
            SetGrounded(false);
            if (state != State.JUMP) // Ensure jumping state
            {
                SetState(State.JUMP);
            }
        }
    }

    private void SetGrounded(bool grounded)
    {
        isGrounded = grounded;
        updateAnimator = true;
    }

    private bool IsOverGround()
    {
        return Physics2D.OverlapCircle(groundingPosition.position, groundRadius, groundLayer);
    }

    private bool IsOverDeadGround()
    {
        return Physics2D.OverlapCircle(groundingPosition.position, groundRadius, deadGroundLayer);
    }

    private void SetState(State newState)
    {
        state = newState;
        updateAnimator = true;
    }

    private void SetToFace(FaceDirection newDirection)
    {
        if (direction != newDirection && state != State.DEAD)
        {
            direction = newDirection;
            spriteRenderer.flipX = System.Convert.ToBoolean((int)newDirection);
        }
    }

    private void Jump()
    {
        if (jumpTime > jumpDelay)
        {
            rigidBody.AddForce(new Vector2(0, jumpForce));
            jumpTime = 0.0f;
        }
        else
        {
            jumpTime += Time.deltaTime;
        }
    }

    private void Throw()
    {
        if (throwTime > throwDelay) // throw objects based on time restriction
        {
            SetState(State.THROW);
            if (throwAnimTime > throwAnimDelay) // throw at the correct timing in the animation
            {
                Vector3 throwPosition;
                if (direction == FaceDirection.RIGHT)
                {
                    throwPosition = rightThrowPosition.position;
                }
                else
                {
                    throwPosition = leftThrowPosition.position;
                }

                GameObject projectile = (GameObject)Instantiate(throwable, throwPosition, Quaternion.identity);
                Rigidbody2D body = projectile.GetComponent<Rigidbody2D>();
                body.AddForce(new Vector2(throwForce, 0f));
                body.AddTorque(100f);
                throwTime = 0.0f;
                throwAnimTime = 0.0f;
            }
            else
            {
                throwAnimTime += Time.deltaTime;
            }
        }
        else
        {
            throwTime += Time.deltaTime;
        }
    }

    private void OnIdle()
    {
        if (moveDelta.magnitude > 0.0f) // Must be moving!
        {
            if (moveDelta.y != 0.0f) // Must be jumping or falling
            {
                SetState(State.JUMP);
            }
            else // If you're not jumping or falling but still moving, you must be running
            {
                SetState(State.RUN);
            }
        }
    }

    private void OnRun()
    {
        if (moveDelta.magnitude == 0.0f) // Must be idling
        {
            SetState(State.IDLE);
        }
        else if (moveDelta.y != 0.0f) // Must be jumping or falling
        {
            SetState(State.JUMP);
        }
        else // still running
        {
            if (moveDelta.x < 0) // Running left
            {
                SetToFace(FaceDirection.LEFT);
            }
            else // Running right
            {
                SetToFace(FaceDirection.RIGHT);
            }
        }

    }

    private void OnThrow()
    {
        SetState(State.IDLE);
    }

    private void OnJump()
    {
        //  A grounding check is performed before this.
        if (moveDelta.x < 0) // Jumping/falling left
        {
            SetToFace(FaceDirection.LEFT);
        }
        else // Jumping/falling right
        {
            SetToFace(FaceDirection.RIGHT);
        }
    }

    private void OnDead()
    {
        if (deathTime > deathDelay)
        {
            Destroy(gameObject);
            // Don't need to reset timer because this script will stop running
        }
        else
        {
            deathTime += Time.deltaTime;
        }
    }

    void KillSelf()
    {
        SetState(State.DEAD);
        rigidBody.velocity.Set(0.0f, 0.0f);
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        /* Collider happens when player dies */
        if (collider.gameObject.CompareTag("Death"))
        {
            KillSelf();
        }
    }
}
