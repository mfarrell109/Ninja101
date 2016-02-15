using UnityEngine;
using System.Collections;

public class AiController : MonoBehaviour
{
    [Tooltip("Required to be attached to the same GameObject")]
    public AiStateController stateController;
    [Tooltip("Target to pursue")]
    public GameObject target;

    // Use this for initialization
    void Start()
    {
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }
    }

    // Update is called once per frame
    void Update()
    {
        if (stateController.GetState() != AiState.DEAD)
        {
            UpdateAI();
        }
    }

    private void UpdateAI()
    {
        // TODO: Actually do stuff
        if (stateController.IsGrounded())
        {
            stateController.FaceDirection(AiFaceDirection.RIGHT);
            stateController.ThrowAt(target);
        }
    }
}
