using UnityEngine;
using System.Collections;

public class BeginDoor : MonoBehaviour {

    Rigidbody2D door;
    public float doorForce = 0.1f;
    public float doorOpenPosition = 0;
    public float doorClosePosition = 0;


    IEnumerator timeDelay()
    {
        yield return new WaitForSeconds(1);
      
        doorForce = -0.1f;
        if(door.transform.position.x < doorClosePosition)
        {
            doorForce = 0;
        }
    }

    void Start () {
        door = GetComponent<Rigidbody2D>();    
	  
	}
	
	// Update is called once per frame
	void Update () {

        door.transform.Translate(doorForce,0,0);

        if (door.transform.position.x > doorOpenPosition)
        {
            doorForce = 0;
        }

        StartCoroutine(timeDelay());	    
	}
}
