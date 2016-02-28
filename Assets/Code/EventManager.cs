using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour {

    public delegate void OnPlayerHit();

    public OnPlayerHit onPlayerHit;


    public void PlayerHit(int dmg)
    {
        if (onPlayerHit != null)
        {
            onPlayerHit();
        }
    }
}
