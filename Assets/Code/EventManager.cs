using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour {

    public delegate void OnPlayerHit(int dmg);

    public OnPlayerHit onPlayerHit;


    public void PlayerHit(int dmg)
    {
        if (onPlayerHit != null)
        {
            Debug.Log("HIT");
            onPlayerHit(dmg);
        }
    }
}
