using UnityEngine;
using System.Collections;

public class ShruikenBehavior : MonoBehaviour {

	// Use this for initialization
	void Start ()
    {
	
	}
	
	// Update is called once per frame
	void Update ()
    {
	
	}

    void OnTriggerEnter2D (Collider2D other) // Doesn't matter what it hits -- gets destroyed anyway
    {
        Destroy(gameObject);
    }
}
