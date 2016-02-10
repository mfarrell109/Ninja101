using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Score : MonoBehaviour {


    public Rect scoreRect; 
    //public static int score;        // The player's score.
    public Text text;               // Reference to the Text component.
    public GUIStyle guiStyle;
    //public PlayerMovement player;
    public Texture2D textCoin;
    
    void OnGUI()
    {

        guiStyle.fontSize = 48;
        GUI.DrawTexture(new Rect(Screen.width / 48, 25, 50, 50), textCoin);
        GUI.Box(new Rect((Screen.width / 48) + 70, 25, 100, 100), PlayerMovement.counter.ToString(), guiStyle);
    }
    
}
