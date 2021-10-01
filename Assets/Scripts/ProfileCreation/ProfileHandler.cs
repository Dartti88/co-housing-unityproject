using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ProfileHandler : MonoBehaviour
{
    public GameObject LogInScreen;
    private Profile userProfile;
    private string username;
    private string password;
    private string passwordRepeat;
    public ProfileUIController profUIController;
    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        if (PlayerPrefs.HasKey(Profile.USERNAME_TAG)) //Load if Guid is founded
            userProfile = new Profile(PlayerPrefs.GetString(Profile.USERNAME_TAG), PlayerPrefs.GetString(Profile.PASSWORD_TAG), false);
        else
            LogInScreen.SetActive(true);
    }

    public void ChangeName(TextMeshProUGUI text)
    {
        userProfile.displayName = text.text;
    }

    public void ChangeAvatarID(int avatarID)
    {
        userProfile.avatarID = avatarID;
    }

    public void SaveProfileChanges()
    {
        userProfile.SaveProfile();
    }

    public Profile GetUserProfile()
    {
        return userProfile;
    }

    public void UpdateUserName(TextMeshProUGUI text)
    {
        username = text.text;
    }

    public void UpdatePassword(TextMeshProUGUI text)
    {
        password = text.text;
    }
    public void UpdatePasswordRepeat(TextMeshProUGUI text)
    {
        passwordRepeat = text.text;
    }

    public void Login()
    {
        if(password != "")
            userProfile = new Profile(username, Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password)));
    }

    public void Register()
    {
        profUIController.ChangeProfileInfo();
        if (password == passwordRepeat && password != "")
            userProfile = new Profile(username, password, true);
    }
}
