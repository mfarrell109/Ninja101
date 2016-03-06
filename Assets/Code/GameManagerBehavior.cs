using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;

public class GameManagerBehavior : MonoBehaviour
{

    void Awake()
    {
        DontDestroyOnLoad(this);
    }

	// Use this for initialization
	void Start()
    {
	
	}
	
	// Update is called once per frame
	void Update()
    {
	
	}

    public void FacebookLogin()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(FbInitCallback, OnHideUnity);
        }
    }

    private void FbInitCallback()
    {
        if (FB.IsInitialized)
        {
            Debug.Log("FB SDK Initialized");
            FB.ActivateApp();
            var permissions = new List<string>() {"public_profile"};
            Debug.Log("Logging in with FB...");
            FB.LogInWithReadPermissions(permissions, FbAuthCallback);
        }
        else
        {
            Debug.LogError("Failed to intialize the Facebook SDK.");
        }
    }

    private void FbAuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            AccessToken token = Facebook.Unity.AccessToken.CurrentAccessToken;
            Debug.Log("Welcome, " + token.UserId);
        }
        else
        {
            Debug.LogError("User either cancelled FB login, the app could not connect to the FB servers, or something bad happened...");
        }
    }

    private void OnHideUnity (bool isGameShown)
    {
        if (!isGameShown)
        {
            Time.timeScale = 0;
        }
        else
        {
            Time.timeScale = 1;
        }
    }
}
