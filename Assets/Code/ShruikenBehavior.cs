﻿using UnityEngine;
using System.Collections;

public class ShruikenBehavior : MonoBehaviour {

    public int shruikenDamage;

    private EventManager eventManager;

    // Use this for initialization
    void Start()
    {
        eventManager = GameObject.FindGameObjectWithTag("EventManager").GetComponent<EventManager>();
    }

    void OnTriggerEnter2D (Collider2D other) 
    {
        if (other.gameObject.tag == "Player")
        {
            eventManager.PlayerHit(shruikenDamage);
        }

        Destroy(gameObject); // Doesn't matter what it hits -- gets destroyed anyway
    }
}
