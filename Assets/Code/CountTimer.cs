using UnityEngine;
using UnityEngine.UI;
using System.Collections;




public class CountTimer : MonoBehaviour{

    
    public static float Timer = 0.0f;        // The player's score.
    public Text text;               // Reference to the Text component.
    private float sec;
    private string timeDisplay;
    public GUIStyle guiStyle;

    void Start()
    {
      
     
        
    }
    void OnGUI()
    {

        GUI.Box(new Rect((Screen.width / 2) - 75, 25, 100, 100), "Timer:" + timeDisplay, guiStyle);
        guiStyle.fontSize = 48;

    }



    void Update()
    {
       
        sec += Time.deltaTime;        
        timeDisplay = sec.ToString("F0");      

    }


}
