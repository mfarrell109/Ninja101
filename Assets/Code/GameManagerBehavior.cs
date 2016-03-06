using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using Facebook.Unity;
using GameSparks.Api.Requests;
using GameSparks.Api.Responses;
using System;

namespace UnityEngine
{
    public enum UserLoginType
    {
        Facebook,
        GooglePlus
    }

    public interface NinjaUser
    {
        string GetFirstName();
        string GetLastName();
        UserLoginType GetLoginType();
        // TODO Texture GetProfilePicture();
    }

    public struct FbNinjaUser : NinjaUser
    {
        private string firstName;
        private string lastName;
        private AccessToken accessToken;

        public FbNinjaUser(string first, string last, AccessToken token)
        {
            firstName = first;
            lastName = last;
            accessToken = token;
        }

        string NinjaUser.GetFirstName()
        {
           return firstName;
        }

        string NinjaUser.GetLastName()
        {
            return lastName;
        }

        UserLoginType NinjaUser.GetLoginType()
        {
            return UserLoginType.Facebook;
        }

        public AccessToken GetFbAccessToken()
        {
            return accessToken;
        }
    }
}

public class GameManagerBehavior : MonoBehaviour
{
    public NinjaUser user;


    void Awake()
    {
        DontDestroyOnLoad(this);
    }
    
	void Start()
    {
	
	}
	
	void Update()
    {
	
	}

    // Call this method to initiate FB login and to query user for permission
    public void FacebookLogin()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(FbInitCallback, OnHideUnity);
        }
    }

    // This is called after the FB SDK has been initialized
    private void FbInitCallback()
    {
        if (FB.IsInitialized)
        {
            Debug.Log("FB SDK Initialized");
            FB.ActivateApp();
            var permissions = new List<string>() {"public_profile", "user_friends"};
            Debug.Log("Logging in with FB...");
            FB.LogInWithReadPermissions(permissions, FbAuthCallback);
        }
        else
        {
            Debug.LogError("Failed to intialize the Facebook SDK.");
        }
    }

    // Called after the user has authenticated
    private void FbAuthCallback(ILoginResult result)
    {
        if (FB.IsLoggedIn)
        {
            FB.API("/me?fields=first_name,last_name", Facebook.Unity.HttpMethod.GET, FbProfileCallback);
        }
        else
        {
            Debug.LogError("User either cancelled FB login, the app could not connect to the FB servers, or something bad happened...");
        }
    }

    // Acquires and builds a newly logged-in user
    private void FbProfileCallback(IGraphResult result)
    {
        if (result.Error != null)
        {
            Debug.LogError("FB Error: " + result.Error);
        }
        else if (!FB.IsLoggedIn)
        {
            Debug.LogError("FB Login cancelled by player");
        }
        else
        {
            string firstName = result.ResultDictionary["first_name"].ToString();
            string lastName = result.ResultDictionary["last_name"].ToString();
            user = new FbNinjaUser(firstName, lastName, Facebook.Unity.AccessToken.CurrentAccessToken);
            GreetUser();

            GameSparksLogin();

            Application.LoadLevel("GameMenu");
        }
    }

    private void GreetUser()
    {
        if (user != null)
        {
            Debug.Log("Welcome, " + user.GetFirstName() + " " + user.GetLastName() + ". You are now authenticated with " + user.GetLoginType().ToString() + ".");
        }
    }

    private void GameSparksLogin()
    {
        if (FB.IsLoggedIn)
        {
            new FacebookConnectRequest().SetAccessToken(Facebook.Unity.AccessToken.CurrentAccessToken.TokenString).Send((response) =>
            {
                if (response.HasErrors)
                {
                    Debug.Log("Something failed when connecting with Facebook: " + response.Errors.JSON);
                }
                else
                {
                    //Otherwise we are successfully logged in!
                    Debug.Log("Gamesparks Facebook Login Successful");
                    //Since we successfully logged in, we can get our account information.

                }
            });
        } 
        // Else if G+ login
    }

    private void response(AuthenticationResponse obj)
    {
        throw new NotImplementedException();
    }

    // TODO: Pulled from FB SDK example. Do we actually need this?
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
