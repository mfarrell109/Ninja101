﻿using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using Facebook.Unity;
using GameSparks.Api.Requests;
using GameSparks.Core;

namespace UnityEngine
{
    public enum UserLoginType
    {
        Facebook,
        GooglePlus
    }
}

public class GameManagerBehavior : MonoBehaviour
{

    public NinjaUser user;
    public float signInTimeout = 30f;

    private bool profileLoaded = false;
    private bool checkProfile = false;

    // /////////////////////
    // /  PUBLIC METHODS  //
    // /////////////////////

    void Awake()
    {
        DontDestroyOnLoad(this);

        // Initialize FB to see if there's a valid session
        if (!FB.IsInitialized)
        {
            OnAwakeFacebook();
        }
    }

    void Start()
    {

    }

    void Update()
    {
        if (checkProfile)
        {
            WaitForSignIn();

            if (profileLoaded)
            {
                checkProfile = false;
                LoadGameMenu();
            }
        }
    }

    // Call this method to initiate FB login and to query user for permission
    public void FacebookLogin()
    {
        if (!FB.IsInitialized)
        {
            FB.Init(FbInitCallback, OnHideUnity);
        }
        else
        {
            FbInitCallback();
        }
    }

    public void GameSparksLogin()
    {
        if (!GS.Available) // Attempt to return GameSparks to an available state. For some reason GS hangs on connecting occassionally.
        {
            Debug.Log("The GameSparks service is unavailable. Attempting to reset.");
            GS.Reset();
        }

        if (FB.IsLoggedIn && !GS.Authenticated)
        {
            new FacebookConnectRequest().SetAccessToken(Facebook.Unity.AccessToken.CurrentAccessToken.TokenString).Send((response) =>
            {
                if (response.HasErrors)
                {
                    Debug.Log("Something failed when connecting to GameSparks with Facebook: " + response.Errors.JSON);
                }
                else
                {
                    //Otherwise we are successfully logged in!
                    Debug.Log("Gamesparks Facebook Login Successful. " + response.DisplayName);
                    //Since we successfully logged in, we can get our account information.

                }
            });
        }
        else if (FB.IsLoggedIn && GS.Authenticated)
        {
            Debug.Log("Detected cached GameSparks session ID " + GS.GSPlatform.UserId + ". No need for re-authentication.");
        }
        // Else if G+ login
    }


    // /////////////////////
    // /  PRIVATE EVENTS  //
    // /////////////////////

    private void OnAwakeFacebook()
    {
        FB.Init(() =>
        {
            if (FB.IsInitialized && FB.IsLoggedIn) // There's a valid session!
            {
                if (Application.isMobilePlatform)
                {
                    FB.ActivateApp();
                }
                Debug.Log("Found valid FB Session. Re-using.");

                CachedUser cUser = NinjaUser.LoadCachedUser();
                if (cUser != null && cUser.loginType == UserLoginType.Facebook
                && cUser.userId == Facebook.Unity.AccessToken.CurrentAccessToken.UserId)
                {
                    SetUser(new FbNinjaUser(cUser, Facebook.Unity.AccessToken.CurrentAccessToken));
                    OnSocialMediaLogin();
                }
                else // attempt to get correct details for FB session since either no saved user, wrong login type, or wrong FB user
                {
                    FbInitCallback();
                }
            }
        },
            OnHideUnity);
    }

    private void OnSocialMediaLogin()
    {
        GreetUser();
        GameSparksLogin();

        // Ensure valid user exists before loading main menu
        if (user == null || user.firstName == null || user.picture == null)
        {
            Debug.Log("User not ready. Waiting for profile information before continuing.");
            checkProfile = true;
        }
        else
        {
            LoadGameMenu();
        }
    }

    // Pauses the game in the background while FB is being authorized
    private void OnHideUnity(bool isGameShown)
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


    // ///////////////////////////
    // /  PRIVATE FB CALLBACKS  //
    // ///////////////////////////

    // This is called after the FB SDK has been initialized via the FB login button
    private void FbInitCallback()
    {
        if (FB.IsInitialized)
        {
            if (Application.isMobilePlatform)
            {
                FB.ActivateApp();
            }
            var permissions = new List<string>() { "public_profile", "user_friends" };
            Debug.Log("Logging in with Facebook...");
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
            // Get first and last name
            FB.API("/me?fields=first_name,last_name", Facebook.Unity.HttpMethod.GET, (getResult) =>
            {
                if (getResult.Error != null)
                {
                    Debug.LogError("FB Error: " + getResult.Error);
                }
                else if (!FB.IsLoggedIn)
                {
                    Debug.LogError("FB Login cancelled by player");
                }
                // Generate new user, log into GameSparks, and load the Main Menu
                else
                {
                    string firstName = getResult.ResultDictionary["first_name"].ToString();
                    string lastName = getResult.ResultDictionary["last_name"].ToString();
                    if (user != null && user.GetLoginType() == UserLoginType.Facebook)
                    {
                        Debug.Log("Setting name for user");
                        user.SetName(firstName, lastName);
                        user.Save();
                    }
                    // TODO else if G+ login detected
                    else
                    {
                        Debug.Log("Creating user with name");
                        SetUser(new FbNinjaUser(firstName, lastName, Facebook.Unity.AccessToken.CurrentAccessToken));
                    }
                    OnSocialMediaLogin();
                }
            });

            // Get profile picture
            FB.API("/me/picture?type=normal", Facebook.Unity.HttpMethod.GET, (picResult) =>
            {
                if (picResult.Error != null)
                {
                    Debug.LogError("FB Error: " + picResult.Error);
                }
                else
                {
                    Sprite picture = Sprite.Create(picResult.Texture, new Rect(0, 0, NinjaUser.PICTURE_SIZE, NinjaUser.PICTURE_SIZE), new Vector2());

                    if (user != null && user.GetLoginType() == UserLoginType.Facebook)
                    {
                        user.picture = picture;
                        Debug.Log("Set picture");
                        user.Save();
                    }
                    // TODO else if G+ login detected
                    else
                    {
                        Debug.Log("Creating user with picture");
                        SetUser(new FbNinjaUser(picture, Facebook.Unity.AccessToken.CurrentAccessToken));
                    }
                }
            });
        }
        else
        {
            Debug.LogError("FB Error: " + result.Error);
        }
    }


    // ////////////////////////////////////////////
    // /  PRIVATE, SERVICE-INDEPENDENT METHODS  //
    // ////////////////////////////////////////////
    private float startTime = 0f;
    private float timeoutTime = 0f;

    // Once checkProfile flag is set, this checks during Update() for all profile information to be loaded before continuing
    private void WaitForSignIn()
    {
        if (startTime == 0f)
        {
            startTime = Time.fixedTime;
            timeoutTime = startTime + signInTimeout;
        }

        if (Time.fixedTime > timeoutTime)
        {
            Debug.LogError("Error: Social media logon has timed out after " + signInTimeout + "s.");
            checkProfile = false;
        }
        // Check for valid user before loading next level
        else if (user != null && user.firstName != null && user.lastName != null && user.picture != null)
        {
            Debug.Log("All information collected for user.");
            profileLoaded = true;
        }
    }

    private void GreetUser()
    {
        if (user != null)
        {
            Debug.Log("Welcome, " + user.firstName + " " + user.lastName + ". You are now authenticated with " + user.GetLoginType().ToString() + ".");
        }
    }

    // Switches to a new user and caches it
    private void SetUser(NinjaUser newUser)
    {
        user = newUser;
        user.Save();
    }

    private void LoadGameMenu()
    {
        Application.LoadLevel("GameMenu");
    }
}
