using UnityEngine;
using System.Collections;

public class changeTitleScene : MonoBehaviour
{

    IEnumerator timeDelay()
    {
        yield return new WaitForSeconds(9);
        Application.LoadLevel("GameMenu");

    }
    // Use this for initialization
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        StartCoroutine("timeDelay");
    }
}
