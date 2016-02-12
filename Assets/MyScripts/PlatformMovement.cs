using UnityEngine;
using System.Collections;

public class PlatformMovement : MonoBehaviour {
    public bool vertical = false;
    public float offset = 1.0f;
    public float speed = 1.0f;
    
    private Vector3 start = new Vector3();
    private Vector3 end = new Vector3();
    private float moveDistance;


	// Use this for initialization
	void Start () {
        setupTrack();
        moveDistance = Vector3.Distance(start, end);
	}
	
	// Update is called once per frame
	void Update () {
        float perCovered = Mathf.Abs((Mathf.Sin(Time.time) * 0.5f) + 0.5f);
        transform.position = Vector3.Lerp(start, end, perCovered);
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
}
