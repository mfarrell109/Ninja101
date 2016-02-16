using UnityEngine;
using System.Collections;

namespace UnityEngine
{
    public enum AiFaceDirection // Ensure only two directions exist to convert to boolean values...
    {
        RIGHT,
        LEFT
    }
    public enum AiState
    {
        IDLE,
        RUN,
        THROW,
        JUMP,
        DEAD
    }
}

public class AiStateController : MonoBehaviour
{
    
    public float movementSpeed = 3f;
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

    private AiState state = AiState.IDLE; // Current state of the AI GameObject
    private AiFaceDirection direction = AiFaceDirection.RIGHT;
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
    }

    void LateUpdate()
    {		
        moveDelta = transform.position - lastMovePos;		
        lastMovePos.Set(transform.position.x, transform.position.y, transform.position.z);		
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
            case AiState.IDLE:
                OnIdle();
                break;
            case AiState.RUN:
                OnRun();
                break;
            case AiState.THROW:
                OnThrow();
                break;
            case AiState.JUMP:
                OnJump();
                break;
            case AiState.DEAD:
                OnDead();
                break;
        }
    }

    private void HandleGrounding()
    {
        if (IsOverGround())
        {
            SetGrounded(true);
            if (state == AiState.JUMP) // Shouldn't be jumping if grounded. Revert to idle for movement check
            {
                SetState(AiState.IDLE);
            }
        }
        else if (IsOverDeadGround())
        {
            SetState(AiState.DEAD);
        }
        else // Must be jumping or falling
        {
            SetGrounded(false);
            if (state != AiState.JUMP) // Ensure jumping state
            {
                SetState(AiState.JUMP);
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

    private void SetState(AiState newState)
    {
        state = newState;
        updateAnimator = true;
    }

    public void FaceDirection(AiFaceDirection newDirection)
    {
        if (direction != newDirection && state != AiState.DEAD)
        {
            direction = newDirection;
            spriteRenderer.flipX = System.Convert.ToBoolean((int)newDirection);
        }
    }

    private void OnIdle()
    {
        if (moveDelta.magnitude > 0.0f) // Must be moving!
        {
            if (moveDelta.y != 0.0f) // Must be jumping or falling
            {
                SetState(AiState.JUMP);
            }
            else // If you're not jumping or falling but still moving, you must be running
            {
                SetState(AiState.RUN);
            }
        }
    }

    private void OnRun()
    {
        if (moveDelta.magnitude == 0.0f) // Must be idling
        {
            SetState(AiState.IDLE);
        }
        else if (moveDelta.y != 0.0f) // Must be jumping or falling
        {
            SetState(AiState.JUMP);
        }
        else // still running
        {
            if (moveDelta.x < 0) // Running left
            {
                FaceDirection(AiFaceDirection.LEFT);
            }
            else // Running right
            {
                FaceDirection(AiFaceDirection.RIGHT);
            }
        }

    }

    private void OnThrow()
    {
        SetState(AiState.IDLE);
    }

    private void OnJump()
    {
        //  A grounding check is performed before this.
        if (moveDelta.x < 0) // Jumping/falling left
        {
            FaceDirection(AiFaceDirection.LEFT);
        }
        else // Jumping/falling right
        {
            FaceDirection(AiFaceDirection.RIGHT);
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
        SetState(AiState.DEAD);
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

    public AiState GetState()
    {
        return state;
    }

    public bool IsGrounded()
    {
        return isGrounded;
    }

    public void Jump()
    {
        if (isGrounded)
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
    }

    public void ThrowAt(GameObject target)
    {
        if (isGrounded && state == AiState.IDLE)
        {
            // Face towards target
            LookAt(target);

            if (throwTime > throwDelay) // throw objects based on time restriction
            {
                SetState(AiState.THROW);
                if (throwAnimTime > throwAnimDelay) // throw at the correct timing in the animation
                {
                    Vector3 throwPosition;
                    if (direction == AiFaceDirection.RIGHT)
                    {
                        throwPosition = rightThrowPosition.position;
                    }
                    else
                    {
                        throwPosition = leftThrowPosition.position;
                    }

                    Vector3 vecToPlayer = target.transform.position - transform.position;
                    vecToPlayer.z = 0.0f;
                    vecToPlayer.Normalize();
                    Vector2 force = new Vector2(vecToPlayer.x * throwForce, vecToPlayer.y * throwForce);

                    GameObject projectile = (GameObject)Instantiate(throwable, throwPosition, Quaternion.identity);
                    Rigidbody2D body = projectile.GetComponent<Rigidbody2D>();
                    body.AddForce(force);
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
    }

    public void LookAt(GameObject target)
    {
        Vector3 vecToTarget = target.transform.position - transform.position;
        vecToTarget.z = 0.0f;
        float dot = Vector2.Dot(vecToTarget.normalized, transform.right);
        if (dot > 0)
        {
            FaceDirection(AiFaceDirection.RIGHT);
        }
        else
        {
            FaceDirection(AiFaceDirection.LEFT);
        }
    }

    // Move in the currently-facing direction
    public void Move()
    {
        if (direction == AiFaceDirection.LEFT)
        {
            transform.Translate(-movementSpeed * Time.deltaTime, 0f, 0f);
        }
        else
        {
            transform.Translate(movementSpeed * Time.deltaTime, 0f, 0f);
        }
    }
}
