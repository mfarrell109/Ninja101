using UnityEngine;
using System.Collections;

public class loadTitle : MonoBehaviour
{
    public Rigidbody2D title;
    public string sceneName;
    private float sec = 0;

    // Use this for initialization
    void Start()
    {
        title = GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        sec += Time.deltaTime;
        if (sec < 1 && sec >= 0)
        {
            title.transform.position = new Vector3(0, -2, 0);
        }

        if (sec > 1 && sec < 2)
        {
            title.transform.position = new Vector3(0, -2, 100);
        }

        if (sec >= 2)
        {
            sec = 0;
        }
    }

    public void goGameScene(string sceneName)
    {
        Application.LoadLevel(sceneName);
    }
}
