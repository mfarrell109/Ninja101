using UnityEngine;
using System.Collections;
using UnityEngine.UI;

public class Lives : MonoBehaviour {

    public Rect livesRect; 
    public Text text;
    public GUIStyle guiStyle;
    public Rect scoreRect;
    //public static int score;        // The player's score.
    public Texture2D textLives;
    // Use this for initialization
    void Start () {
	
	}

    void OnGUI()
    {
        GUI.DrawTexture(new Rect(Screen.width - 160, 25, 50, 50), textLives);
        GUI.Box(new Rect(Screen.width - 160, 25, 100, 100), PlayerMovement.livesCounter.ToString(), guiStyle);
        guiStyle.fontSize = 48;

    }

    // Update is called once per frame
    void Update () {
	
	}
}
