using UnityEngine;
using System.Collections;

public class AiController : MonoBehaviour
{
    public enum EdgeAction
    {
        DoNothing,
        Jump,
        Turn
    }
    public enum AttackAction
    {
        DoNothing,
        Throw
    }
    public enum DefaultMovement
    {
        Idle,
        Pace
    }

    [Tooltip("Required to be attached to the same GameObject")]
    public AiStateController stateController;
    [Tooltip("Target layerMask to detect and pursue")]
    public LayerMask targetLayer;
    [Tooltip("Should the AI attempt to follow the target?")]
    public bool followTarget;
    [Tooltip("The radius (from the center of the GameObject) at which the AI is aware of potential targets")]
    public float detectionRadius;
    [Tooltip("Does the AI need line of sight to detect potential target?")]
    public bool needLineOfSight;
    [Tooltip("What should the AI do when it reaches a drop, or a wall?")]
    public EdgeAction edgeAction;
    [Tooltip("Should the AI do something other than look at the target? NOTE: Cannot throw while running")]
    public AttackAction attackAction;
    [Tooltip("Should the AI pace when there's nothing in the detection radius?")]
    public DefaultMovement defaultMovement;
    public LayerMask platformLayer;
    public Transform leftEdgeDetector;
    public Transform rightEdgeDetector;
    public Transform leftWallDetector;
    public Transform rightWallDetector;
    private float platformDetectionRadius = 0.8f;
    private RaycastHit2D hit;

    // Use this for initialization
    void Start()
    {
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        if (hit != null)
        {
            Gizmos.DrawLine(transform.position, hit.point);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    
    // Using FixedUpdate for raycasting -- unsure if necessary. Better safe than sorry!
    void FixedUpdate()
    {
        if (stateController.GetState() != AiState.Dead)
        {
            UpdateAI();
        }
    }

    private void UpdateAI()
    {
        // Detect target
        GameObject detectedObj = ScanForTarget(needLineOfSight);

        // if detected, follow and throw
        if (detectedObj != null)
        {
            Debug.Log("Looking at target");
            stateController.LookAt(detectedObj);

            if (followTarget)
            {
                ReactToObstacles(true);
                stateController.Move();
            }

            if (attackAction == AttackAction.Throw)
            {
                stateController.ThrowAt(detectedObj); // The state controller will not throw if AiState is not Idle
            }
        }
        else
        {
            if (defaultMovement == DefaultMovement.Pace)
            {
                ReactToObstacles(false);
                stateController.Move();
            }
            // else do nothing because idle
        }
    }

    private void ReactToObstacles(bool following)
    {
        if (ObstacleInTheWay() && stateController.GetState() != AiState.Jump)
        {
            if (edgeAction == EdgeAction.Jump)
            {
                stateController.Jump();
            }
            else if (edgeAction == EdgeAction.Turn && !following)
            {
                Debug.Log("TURN!");
                stateController.Turn();
            }
            else
            {
                // Do nothing
            }
        }
    }

    private bool ObstacleInTheWay()
    {

        Collider2D platformCollider;
        Collider2D wallCollider;

        if (stateController.GetFacingDirection() == AiFaceDirection.LEFT)
        {
            platformCollider = Physics2D.OverlapCircle(leftEdgeDetector.position, platformDetectionRadius, platformLayer);
            wallCollider = Physics2D.OverlapCircle(leftWallDetector.position, platformDetectionRadius, platformLayer);
        }
        else
        {
            platformCollider = Physics2D.OverlapCircle(rightEdgeDetector.position, platformDetectionRadius, platformLayer);
            wallCollider = Physics2D.OverlapCircle(rightWallDetector.position, platformDetectionRadius, platformLayer);
        }

        if (platformCollider == null || wallCollider != null) // Looking for an absence of platforms, and existence of walls
        {
            return true;
        }
        return false;
    }

    private GameObject ScanForTarget(bool lineOfSight)
    {
        Collider2D collider = Physics2D.OverlapCircle(transform.position, detectionRadius, targetLayer);
        if (collider != null)
        {
            if (!lineOfSight)
            {
                return collider.gameObject;
            }

            Vector3 vecToObj = collider.gameObject.transform.position - transform.position;
            vecToObj.z = 0.0f;
            vecToObj.Normalize();
            hit = Physics2D.Raycast(transform.position, vecToObj, detectionRadius, targetLayer | platformLayer);
            if (hit.collider != null && targetLayer >> hit.collider.gameObject.layer == 1) // Right-shift targetLayermask to see if it's equal to the gameObject layer
            {
                return hit.collider.gameObject;
            }
            else
            {
                return null;
            }
        } 
        else
        {
            return null;
        }
    }
}
