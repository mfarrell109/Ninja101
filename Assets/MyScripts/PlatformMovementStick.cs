using UnityEngine;
using System.Collections;

public class PlatformMovementStick : MonoBehaviour {
    private GameObject player;
    private Vector3 lastPlatformPos;
    private bool playerTriggered = false;

    // Use this for initialization
    void Start () {
        player = GameObject.FindGameObjectWithTag("Player");
        lastPlatformPos = new Vector3(transform.position.x, transform.position.y, transform.position.z);
    }
	
	// Update is called once per frame
	void Update () {
        if (playerTriggered) {
            // offset the player to keep with the movement of the platform
            Vector3 offset = new Vector3(transform.position.x, transform.position.y, transform.position.z) - lastPlatformPos;
            player.transform.Translate(offset);
        }
        lastPlatformPos.Set(transform.position.x, transform.position.y, transform.position.z);
    }

    void OnTriggerEnter2D(Collider2D other) {
        // clear offset to get ready for the player to stick to the platform
        if (other.gameObject == player) { // but don't clear for other gameObjects that land on the platform
            playerTriggered = true;
        }
    }

    void OnTriggerExit2D(Collider2D other) {
        // clear offset to get ready for the player to stick to the platform
        if (other.gameObject == player) { // but don't clear for other gameObjects that exit the platform
            playerTriggered = false;
        }
    }
}
