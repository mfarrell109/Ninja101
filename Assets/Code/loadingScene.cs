using UnityEngine;
using System.Collections;

public class loadingScene : MonoBehaviour {
    public string sceneName;
    public static float counter = 0;
    public GUIStyle guiStyle;
    private AsyncOperation operation;
    private AsyncOperation async;
    public float pc, hobar = 0;
    public Texture2D progressBar;

    void Start()
    {

    }

    void Update()
    {
        if (async != null)
        {
            pc = async.progress * 100;
            hobar = pc * Screen.width / 100;
        }
    }

    public IEnumerator loadAsync(string sceneName)
    {
        if (pc >= 90)
        {
            pc = 100;
        }

        async = Application.LoadLevelAsync(sceneName);
        yield return async;
    }

    void OnGUI()
    {
        //guiStyle.fontSize = 10;
        GUI.DrawTexture(new Rect(0, Screen.height - 50, hobar, 50), progressBar, ScaleMode.StretchToFill );
    }

    //Start game from scratch and reset all variables
    public void goToGameScene(string sceneName)
    {
        PlayerMovement.livesCounter = 3;
        PlayerMovement.counter = 0;
        StartCoroutine(loadAsync(sceneName));
    }

    //Quit game
    public void quitGame()
    {
        Application.Quit();
    }
}
