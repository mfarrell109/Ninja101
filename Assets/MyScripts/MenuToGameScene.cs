using UnityEngine;
using System.Collections;

public class MenuToGameScene : MonoBehaviour {

    public string sceneName;

    public void goToGameScene(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }

    public void quitGame()
    {
        Application.Quit();
    }

}
