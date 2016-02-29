using UnityEngine;
using System.Collections;

public class BoltTeslaBehavior : MonoBehaviour
{

    public int boltDamage;

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
            eventManager.PlayerHit(boltDamage);
        }
    }
}
