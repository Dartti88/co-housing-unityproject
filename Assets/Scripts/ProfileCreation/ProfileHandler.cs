using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;


public class ProfileHandler : MonoBehaviour
{
    public static ProfileHandler Instance;

    public Profile userProfile;

    [HideInInspector] public bool loggedOut = false;

    private void Awake()
    {
        if (Instance == null)
        {
            if (SceneManager.GetActiveScene() != SceneManager.GetSceneByName("LogIn"))
                SceneManager.LoadScene("LogIn");
            Instance = this;
            DontDestroyOnLoad(Instance);
        }
        else
        {
            Destroy(this.gameObject);
        }
    }

    public void ChangeName(string text)
    {
        userProfile.displayName = text;
    }

    public void ChangeAvatarID(int avatarID)
    {
        userProfile.avatarID = avatarID;
    }

    public void SaveProfileChanges()
    {
        Client.Instance.BeginRequest_UpdateProfile(userProfile, null);
    }

    public Profile GetUserProfile()
    {
        return userProfile;
    }

    public void LogOut()
    {
        loggedOut = true;
        userProfile = null;
        SceneManager.LoadScene("LogIn");
    }
}
