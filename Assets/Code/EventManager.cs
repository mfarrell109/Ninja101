using UnityEngine;
using System.Collections;

public class EventManager : MonoBehaviour {

    public delegate void OnPlayerHit(int dmg);
    public delegate void OnCoinCollected();

    public OnPlayerHit onPlayerHit;
    public OnCoinCollected onCoinCollected;


    public void PlayerHit(int dmg)
    {
        if (onPlayerHit != null)
        {
            onPlayerHit(dmg);
        }
    }

    public void CollectCoin()
    {
        if (onCoinCollected != null)
        {
            onCoinCollected();
        }
    }
}
