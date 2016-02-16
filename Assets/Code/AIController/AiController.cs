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

    // Use this for initialization
    void Start()
    {
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, detectionRadius);
    }
    
    // Using FixedUpdate for raycasting -- unsure if necessary. Better safe than sorry!
    void FixedUpdate()
    {
        if (stateController.GetState() != AiState.DEAD)
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
            stateController.LookAt(detectedObj);
            if (followTarget)
            {
                // Is a wall or drop in front of us?
                // If so, should we jump or turn?
                // Move after action has been taken
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
                // Is a wall or drop in front of us?
                // If so, should we jump or turn?
                // Move after action has been taken
                stateController.Move();
            } 
            // else do nothing because idle
        }
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
            RaycastHit2D hit = Physics2D.Raycast(transform.position, vecToObj, detectionRadius, targetLayer);
            if (hit.collider != null)
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
