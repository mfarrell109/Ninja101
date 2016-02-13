using UnityEngine;
using System.Collections;

public class TrackMovement : MonoBehaviour {
    public bool vertical = false;
    public float offset = 1.0f;
    public float speed = 1.0f;
    
    private Vector3 start = new Vector3();
    private Vector3 end = new Vector3();
    private float moveDistance;
    private Vector3 lastTrackPos = new Vector3();


    // Use this for initialization
    void Start () {
        setupTrack();
        moveDistance = Vector3.Distance(start, end);
	}
	
	// Update is called once per frame
	void Update () {
        lastTrackPos.Set(transform.position.x, transform.position.y, transform.position.z);
        float wavePos = Mathf.Sin(Time.time * speed); // movement based on a sine wave modulated by time and accelerated by speed
        float wavePosOffset = (wavePos * 0.5f) + 0.5f; // scale the wave in half to modulate between 0.5 and -0.5, then move the waveform up to modulate between 0 and 1
        transform.position = Vector3.Lerp(start, end, wavePosOffset);
	}

    void OnDrawGizmosSelected () {
        Gizmos.color = Color.gray;
        setupTrack();
        Gizmos.DrawLine(start, end);
    }

    // Setup starting and ending positions of the track to slerp between
    void setupTrack () {
        Vector3 pos = transform.position;
        start.Set(pos.x, pos.y, pos.z);
        end.Set(pos.x, pos.y, pos.z);

        if (vertical) {
            start.y -= offset;
            end.y += offset;
        }
        else {
            start.x -= offset;
            end.x += offset;
        }
    }

    public Vector3 getLastMoveDelta() {
        return transform.position - lastTrackPos;
    }
}
