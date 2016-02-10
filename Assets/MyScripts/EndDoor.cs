using UnityEngine;
using System.Collections;

public class EndDoor : MonoBehaviour {
 
    public Rigidbody2D door;
    public float doorForce = 0.1f;
    public float doorOpenPosition = 0;
    public float doorClosePosition = 0;
    // Use this for initialization
    bool move = false;

    IEnumerator timeDelay()
    {
        yield return new WaitForSeconds(1);
        //-0.14
        doorForce = -0.1f;
        if (door.transform.position.x < doorClosePosition)
        {
            doorForce = 0;
        }
    }

    void Start()
    {
        door = GetComponent<Rigidbody2D>();

    }



    // Update is called once per frame
    void Update()
    {
    if (move == true) {
            door.transform.Translate(doorForce, 0, 0);

            if (door.transform.position.x > doorOpenPosition)
            {
                doorForce = 0;
            }

            StartCoroutine(timeDelay());
        }
    }

    void OnTriggerEnter2D(Collider2D collider)
    {
        if (collider.gameObject.CompareTag("Player"))
        {
            move = true;
        }
        
    }
}

