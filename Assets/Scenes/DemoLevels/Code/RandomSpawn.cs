using UnityEngine;
using System.Collections;

public class RandomSpawn : MonoBehaviour {

    public GameObject shot;
    public Transform shotSpawn;
    public float spawnRate;

    private float nextFire;
    void Update()
    {
        //random number here
        int ranNum = Random.Range(1, 50);
        if (ranNum == 1)
        {
            if (Time.time > nextFire)
            {
                nextFire = Time.time + spawnRate;
                Instantiate(shot, shotSpawn.position, shotSpawn.rotation);
            }
        }

    }
}
