using UnityEngine;
using System.Collections;

public class BeginDoor : MonoBehaviour
{
    Rigidbody2D door;
    public float doorForce = 0.1f;
    public float doorOpenPosition = 2.4f;
    public float doorClosePosition = 0;

    public delegate void StartDoorOpened();
    public delegate void StartDoorClosed();
    public StartDoorOpened startDoorOpened;
    public StartDoorClosed startDoorClosed;

    IEnumerator timeDelay()
    {
        yield return new WaitForSeconds(1);

        doorForce = -0.1f;
        if (door.transform.localPosition.x <= doorClosePosition)
        {
            doorForce = 0;
            Vector3 newPosition = door.transform.localPosition;
            newPosition.x = doorClosePosition;
            door.transform.localPosition = newPosition;

            if (startDoorClosed != null)
            {
                startDoorClosed();
            }
        }
    }

    void Start()
    {
        door = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        door.transform.Translate(Vector3.right * doorForce);

        if (door.transform.localPosition.x > doorOpenPosition)
        {
            doorForce = 0;
            Vector3 newPosition = door.transform.localPosition;
            newPosition.x = doorOpenPosition;
            door.transform.localPosition = newPosition;
            if (startDoorOpened != null)
            {
                startDoorOpened();
            }
        }

        StartCoroutine(timeDelay());
    }
}
