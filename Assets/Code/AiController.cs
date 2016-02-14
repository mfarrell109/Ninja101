using UnityEngine;
using System.Collections;

public class AiController : MonoBehaviour {
    enum FaceDirection { // Ensure only two directions exist to convert to boolean values...
        RIGHT,
        LEFT
    }
    enum State {
        IDLE,
        RUN,
        THROW,
        JUMP,
        DEAD
    }

    private SpriteRenderer spriteRenderer; // Assumes a SpriteRenderer exists
    private Animator animator; // Animator attached to the GameObject, if any.
    private State state = State.IDLE; // Current state of the AI GameObject
    private FaceDirection direction = FaceDirection.RIGHT;
    private bool updateAnimator = true; // Flip this flag to true if you want to update the animator on the next frame
    private Vector3 moveDelta = new Vector3(); // Used to determine transition between movement states
    private Vector3 lastMovePos = new Vector3(); // Used to calculate movement delta


	// Use this for initialization
	void Start () {
        spriteRenderer = GetComponent<SpriteRenderer>();
        animator = GetComponent<Animator>();
	}
	
	// Update is called once per frame
	void Update () {
        HandleGrounding();
        HandleActionStates();
        UpdateAI();
	}

    void LateUpdate () {
        moveDelta = transform.position - lastMovePos;
        lastMovePos.Set(transform.position.x, transform.position.y, transform.position.z);
    }

    private void UpdateAI() {
        // TODO: Actually do stuff
        transform.Translate(Time.deltaTime, 0f, 0f);
    }

    private void HandleActionStates() {
        if (animator != null && updateAnimator == true) {
            animator.SetInteger("State", (int)state);
            updateAnimator = false;
        }

        switch (state) {
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

    private void HandleGrounding() {
        if (IsGrounded()) {
            if (state == State.JUMP) { // Shouldn't be jumping if grounded. Revert to idle for movement check
                SetState(State.IDLE);
            }
        }
        else { // Must be jumping or falling
            if (state != State.JUMP) { // Ensure jumping state
                SetState(State.JUMP);
            }
        }
    }

    private bool IsGrounded() {
        // TODO: check for grounding
        return true;
    }

    private void SetState(State newState) {
        state = newState;
        updateAnimator = true;
    }

    private void SetToFace(FaceDirection newDirection) {
        if (direction != newDirection) {
            direction = newDirection;
            spriteRenderer.flipX = System.Convert.ToBoolean((int)newDirection);
        }
    }

    private void OnIdle() {
        if (moveDelta.magnitude > 0.0f) { // Must be moving!
            if (moveDelta.y != 0.0f) { // Must be jumping or falling
                SetState(State.JUMP);
            }
            else { // If you're not jumping or falling but still moving, you must be running
                SetState(State.RUN);
            }
        }
    }

    private void OnRun() {
        if (moveDelta.magnitude == 0.0f) { // Must be idling
            SetState(State.IDLE);
        }
        else if (moveDelta.y != 0.0f) { // Must be jumping or falling
            SetState(State.JUMP);
        }
        else { // still running
            if (moveDelta.x < 0) { // Running left
                SetToFace(FaceDirection.LEFT);
            }
            else { // Running right
                SetToFace(FaceDirection.RIGHT);
            }
        }
        
    }

    private void OnThrow() {
        // TODO: Throw shruiken
        SetState(State.IDLE);
    }

    private void OnJump() {
        //  A grounding check is performed before this.
        if (moveDelta.x < 0) { // Jumping/falling left
            SetToFace(FaceDirection.LEFT);
        }
        else { // Jumping/falling right
            SetToFace(FaceDirection.RIGHT);
        }
    }

    private void OnDead() {
        // TODO: handle scoring and remove GameObject after a period of time, here.
    }
}
