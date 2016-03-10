using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ProfileDisplayBehavior : MonoBehaviour {

    public GameManagerBehavior gameManager;

	// Use this for initialization
	void Start () {
        GameObject gmObject = GameObject.FindGameObjectWithTag("GameManager");
        if (gmObject != null)
        {
            gameManager = gmObject.GetComponent<GameManagerBehavior>();
            SpriteRenderer pictureRenderer = GameObject.FindGameObjectWithTag("ProfilePicture").GetComponent<SpriteRenderer>();
            Text name = GameObject.FindGameObjectWithTag("NameText").GetComponent<Text>();
            NinjaUser user = gameManager.loggedInUser;
            if (user != null)
            {
                pictureRenderer.sprite = user.picture;
                name.text = user.firstName + " " + user.lastName;
            }
        }
	}
	
	// Update is called once per frame
	void Update () {
	
	}
}
