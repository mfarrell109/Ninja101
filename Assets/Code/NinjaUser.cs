using System;
using System.IO;
using Facebook.Unity;

namespace UnityEngine
{

    public static class UserPrefKeys
    {
        public const string USER_ID = "UserId";
        public const string FIRST_NAME = "FirstName";
        public const string LAST_NAME = "LastName";
        public const string LOGIN_TYPE = "LoginType";
        public const string CACHED_ID = "CachedId";
        public const string PROF_PIC = "ProfilePicture";
    }

    public abstract class NinjaUser
    {
        public static int PICTURE_SIZE = 100;

        public string firstName { get; set; }
        public string lastName { get; set; }
        public Sprite picture { get; set; }

        public NinjaUser(string first, string last, Sprite picture)
        {
            this.firstName = first;
            this.lastName = last;
            this.picture = picture;
        }

        public void SetName(string first, string last)
        {
            this.firstName = first;
            this.lastName = last;
        }
        
        private static Sprite LoadPictureFromCache()
        {
            byte[] bytes = File.ReadAllBytes(Path.Combine(Application.persistentDataPath, UserPrefKeys.PROF_PIC + ".png"));
            Texture2D image = new Texture2D(NinjaUser.PICTURE_SIZE, NinjaUser.PICTURE_SIZE);
            image.LoadImage(bytes);
            return Sprite.Create(image, new Rect(0, 0, NinjaUser.PICTURE_SIZE, NinjaUser.PICTURE_SIZE), new Vector2(50, 50));
        }

        public static CachedUser LoadCachedUser()
        {
            CachedUser cachedUser = new CachedUser();
            cachedUser.firstName = PlayerPrefs.GetString(UserPrefKeys.FIRST_NAME);
            cachedUser.lastName = PlayerPrefs.GetString(UserPrefKeys.LAST_NAME);
            cachedUser.loginType = (UserLoginType)Enum.Parse(typeof(UserLoginType), PlayerPrefs.GetString(UserPrefKeys.LOGIN_TYPE));
            cachedUser.userId = PlayerPrefs.GetString(UserPrefKeys.CACHED_ID);

            cachedUser.picture = LoadPictureFromCache();

            return cachedUser;
        }

        public abstract UserLoginType GetLoginType();
        public abstract void Save();
    }

    public class CachedUser
    {
        public string firstName;
        public string lastName;
        public UserLoginType loginType;
        public string userId;
        public Sprite picture;
    }

    public class FbNinjaUser : NinjaUser
    {
        private AccessToken accessToken;

        public FbNinjaUser(string first, string last, Sprite picture, AccessToken token) : base(first, last, picture)
        {
            this.accessToken = token;
        }

        public FbNinjaUser(string first, string last, AccessToken token) : this(first, last, null, token)
        {
        }

        public FbNinjaUser(Sprite picture, AccessToken token) : this(null, null, picture, token)
        {
        }

        public FbNinjaUser(CachedUser user, AccessToken token) : this(user.firstName, user.lastName, user.picture, token)
        {
        }

        public override UserLoginType GetLoginType()
        {
            return UserLoginType.Facebook;
        }

        public override void Save()
        {
            PlayerPrefs.SetString(UserPrefKeys.FIRST_NAME, firstName);
            PlayerPrefs.SetString(UserPrefKeys.LAST_NAME, lastName);
            PlayerPrefs.SetString(UserPrefKeys.LOGIN_TYPE, GetLoginType().ToString());
            PlayerPrefs.SetString(UserPrefKeys.CACHED_ID, accessToken.UserId);
            PlayerPrefs.Save();

            if (picture != null)
            {
                File.WriteAllBytes(
                    Path.Combine(Application.persistentDataPath, accessToken.UserId + "_" + UserPrefKeys.PROF_PIC + ".png"),
                    picture.texture.EncodeToPNG()
                );
            }
            else // write empty file to save spot
            {
                File.WriteAllBytes(
                    Path.Combine(Application.persistentDataPath, accessToken.UserId + "_" + UserPrefKeys.PROF_PIC + ".png"),
                    new byte[0]
                );
            }
        }

        public AccessToken GetAccessToken()
        {
            return accessToken;
        }
    }
}
