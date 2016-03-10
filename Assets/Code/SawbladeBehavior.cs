using UnityEngine;
using System.Collections;

public class SawbladeBehavior : MonoBehaviour
{
    public int sawbladeDamage;

    // Maximum rotation speed (should be >= 0)
    public float rotationSpeed = 100.0f;

    // Whether blades should _permanently_ spin in opposite direction
    public bool reverseRotation = false;

    // True: spin up and down depending on distance from offset edge
    // False: constant speed regardless of position
    public bool variableRotationSpeed = true;

    // If true, blade comes to a halt then reverses direction;
    // if false, blade comes to a halt then resumes in the same direction
    public bool canChangeDirection = true;

    private TrackMovement trackScript;
    private EventManager eventManager;

    // Use this for initialization
    void Start()
    {
        trackScript = GetComponent<TrackMovement>();
        eventManager = GameObject.FindGameObjectWithTag("EventManager").GetComponent<EventManager>();
    }

    // Update is called once per frame
    void Update()
    {
        float dps = 0.0f;

        if (variableRotationSpeed)
        {
            // Between -trackScript.speed/10 and +trackScript.speed/10
            float axisDelta = GetAxisDelta(trackScript.GetLastMoveDelta()) * 100f;

            /* if anyone can find a better way to do this, feel free. This is speed bound by
            * time as well as how fast the blade is moving on the track.
            * AFAIK to make this better we need a min/max speed that the blade can go on the track
            */
            dps = rotationSpeed * axisDelta * Time.deltaTime;

            if (!canChangeDirection)
            {
                dps = Mathf.Abs(dps);
            }
        }
        else
        {
            dps = rotationSpeed * Time.deltaTime * 100f / (trackScript.offset * 2f);
        }

        transform.Rotate(0.0f, 0.0f, reverseRotation ? -dps : dps);
    }

    float GetAxisDelta(Vector3 moveDelta)
    {
        if (moveDelta.x != 0.0f)
        {
            return moveDelta.x;
        }
        else
        {
            return moveDelta.y;
        }
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            eventManager.PlayerHit(sawbladeDamage);
        }
    }
}
