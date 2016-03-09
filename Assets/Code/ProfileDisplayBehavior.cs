using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProfileDisplayBehavior : MonoBehaviour {

    public GameManagerBehavior gameManager;

	// Use this for initialization
	void Start () {
        gameManager = GameObject.FindGameObjectWithTag("GameManager").GetComponent<GameManagerBehavior>();
        SpriteRenderer pictureRenderer = GameObject.FindGameObjectWithTag("ProfilePicture").GetComponent<SpriteRenderer>();
        Text name = GameObject.FindGameObjectWithTag("NameText").GetComponent<Text>();
        NinjaUser user = gameManager.user;
        if (user != null)
        {
            pictureRenderer.sprite = user.GetProfilePicture();
            name.text = user.GetFirstName() + " " + user.GetLastName();
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
