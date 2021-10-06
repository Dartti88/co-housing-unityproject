using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ProfileHandler : MonoBehaviour
{
    public ProfileUIController pUI;
    public GameObject LogInScreen;
    public Profile userProfile;
    private string username;
    private string password;
    private string passwordRepeat;
    public ProfileUIController profUIController;
    private void Awake()
    {

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



    //public void Login()
    //{
    //    if(password != "")
    //        userProfile = new Profile(username, Convert.ToBase64String(System.Text.Encoding.UTF8.GetBytes(password)));
    //}

    //public void Register()
    //{
    //    profUIController.ChangeProfileInfo();
    //    if (password == passwordRepeat && password != "")
    //        userProfile = new Profile(username, password, true);

    //    pUI.ChangeProfileInfo();

    //}
}
