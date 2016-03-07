using UnityEngine;
using System.Collections.Generic;
using Facebook.Unity;
using GameSparks.Api.Requests;
using GameSparks.Core;
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

    public class FbNinjaUser : NinjaUser
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
    public static class UserPrefKeys
    {
        public const string USER_ID = "UserId";
        public const string FIRST_NAME = "FirstName";
        public const string LAST_NAME = "LastName";
        public const string LOGIN_TYPE = "LoginType";
        public const string CACHED_ID = "CachedId";
    }

    private class CachedUser
    {
        public string firstName;
        public string lastName;
        public UserLoginType loginType;
        public string cachedId;
    }
    
    public NinjaUser user;


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

                CachedUser cUser = GetCachedUser();
                if (cUser != null && cUser.loginType == UserLoginType.Facebook 
                && cUser.cachedId == Facebook.Unity.AccessToken.CurrentAccessToken.UserId)
                {
                    SetUser(cUser.firstName, cUser.lastName, UserLoginType.Facebook, Facebook.Unity.AccessToken.CurrentAccessToken.UserId);
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
        Application.LoadLevel("GameMenu");
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
            var permissions = new List<string>() {"public_profile", "user_friends"};
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
                    user = new FbNinjaUser(firstName, lastName, Facebook.Unity.AccessToken.CurrentAccessToken);
                    OnSocialMediaLogin();
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

    private void GreetUser()
    {
        if (user != null)
        {
            Debug.Log("Welcome, " + user.GetFirstName() + " " + user.GetLastName() + ". You are now authenticated with " + user.GetLoginType().ToString() + ".");
        }
    }

    // Retrieves the last logged on user from the cache
    // Returns null if there was a problem
    private CachedUser GetCachedUser()
    {
        CachedUser cachedUser = new CachedUser();
        try
        {
            cachedUser.firstName = PlayerPrefs.GetString(UserPrefKeys.FIRST_NAME);
            cachedUser.lastName = PlayerPrefs.GetString(UserPrefKeys.LAST_NAME);
            cachedUser.loginType = (UserLoginType)Enum.Parse(typeof(UserLoginType), PlayerPrefs.GetString(UserPrefKeys.LOGIN_TYPE));
            cachedUser.cachedId = PlayerPrefs.GetString(UserPrefKeys.CACHED_ID);
        }
        catch (Exception e)
        {
            cachedUser = null;
        }

        return cachedUser;
    }

    // Saves the user to PlayerPrefs, then builds a new current user
    private void SetUser(string firstName, string lastName, UserLoginType loginType, string userId)
    {
        PlayerPrefs.SetString(UserPrefKeys.FIRST_NAME, firstName);
        PlayerPrefs.SetString(UserPrefKeys.LAST_NAME, lastName);
        PlayerPrefs.SetString(UserPrefKeys.LOGIN_TYPE, loginType.ToString());
        PlayerPrefs.SetString(UserPrefKeys.CACHED_ID, userId);
        PlayerPrefs.Save();

        if (loginType == UserLoginType.Facebook)
        {
            if (FB.IsLoggedIn)
            {
                user = new FbNinjaUser(firstName, lastName, Facebook.Unity.AccessToken.CurrentAccessToken);
            }
            else
            {
                Debug.Log("Warning: Caching a user to PlayerPrefs that isn't actually logged in at this moment");
            }
        }
        // else if G+
    }
}
