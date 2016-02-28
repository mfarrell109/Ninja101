using UnityEngine;
using System.Collections;

public class SpikeBehavior : MonoBehaviour {
    public int spikeDamage;

    private EventManager eventManager;

    // Use this for initialization
    void Start()
    {
        eventManager = GameObject.FindGameObjectWithTag("EventManager").GetComponent<EventManager>();
    }

    void OnTriggerEnter2D(Collider2D other)
    {
        if (other.gameObject.tag == "Player")
        {
            eventManager.PlayerHit(spikeDamage);
        }
    }
}
