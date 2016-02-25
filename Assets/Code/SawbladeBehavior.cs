using UnityEngine;
using System.Collections;

public class SawbladeBehavior : MonoBehaviour
{
    public float rotationSpeed = 100.0f;
    public bool reverseRotation = false;

    private GameObject player;
    private TrackMovement trackScript;

    // Use this for initialization
    void Start()
    {
        trackScript = GetComponent<TrackMovement>();
        player = GameObject.FindGameObjectWithTag("Player");
    }

    // Update is called once per frame
    void Update()
    {
        float axisDelta = GetAxisDelta(trackScript.GetLastMoveDelta());

        /* if anyone can find a better way to do this, feel free. This is speed bound by
        * time as well as how fast the blade is moving on the track.
        * AFAIK to make this better we need a min/max speed that the blade can go on the track
        */
        float dps = (rotationSpeed * Time.deltaTime) * (rotationSpeed * axisDelta);
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
}
