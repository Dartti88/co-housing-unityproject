using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;

public class LogIn : MonoBehaviour
{
    ProfileHandler pHandler;

    public InputField input_userName;
    public InputField input_password;
    public InputField input_passwordAgain;
    public bool clearPrefs;

    string userName;
    string password;
    string passwordAgain;

    private void Start()
    {
        pHandler = FindObjectOfType<ProfileHandler>();
        if (clearPrefs)
        {
            PlayerPrefs.DeleteKey(Profile.USERNAME_TAG);
            PlayerPrefs.DeleteKey(Profile.PASSWORD_TAG);
        }


        if (PlayerPrefs.HasKey(Profile.USERNAME_TAG) && !pHandler.loggedOut) //Load if Prefs is founded
        {
            userName = PlayerPrefs.GetString(Profile.USERNAME_TAG);
            password = PlayerPrefs.GetString(Profile.PASSWORD_TAG);
            Client.Instance.BeginRequest_GetAllProfiles(OnGetProfilesForLoginRequestComplete);
        }
    }

    public void OnClick_CreateNewUser()
    {
        userName = input_userName.text;
        password = input_password.text;
        passwordAgain = input_passwordAgain.text;


        if (password != passwordAgain)
        {
            Debug.Log("Passwords didn't match");
            return;
        }

        Profile newProfile = new Profile(
            1, 2,
            userName, "No Name", password, "No Description",
            3, 0, 0, Profile.ProfileType.Guest,
            DateTime.Now
        );

        Client.Instance.BeginRequest_AddNewProfile(newProfile, OnCreateProfileRequestComplete);
    }
    void OnCreateProfileRequestComplete(string response)
    {
        if (response == "Success")
        {
            Debug.Log("New profile was created successfully");
            Client.Instance.BeginRequest_GetAllProfiles(OnGetProfilesForLoginRequestComplete);
        }
        else
            Debug.Log("Failed to create profile: Response from server >> " + response);
    }

    public void OnClick_LogIn()
    {
        userName = input_userName.text;
        password = input_password.text;
        Client.Instance.BeginRequest_GetAllProfiles(OnGetProfilesForLoginRequestComplete);
        //profUIController.LoadOnLogin();
    }

    void OnLogInRequestComplete(string response)
    {
        if (response == "Success")
        {
            Debug.Log("Log in was successful");
            PlayerPrefs.SetString(Profile.USERNAME_TAG, userName);
            PlayerPrefs.SetString(Profile.PASSWORD_TAG, password);
            if (Client.Instance.profile_list.profiles != null && Client.Instance.profile_list.profiles.Length > 0)
                FindObjectOfType<ProfileHandler>().userProfile = Client.Instance.profile_list.profiles.Where(x => x.userName == userName).First();
            FindObjectOfType<ProfileHandler>().userProfile.password = password;
            SceneManager.LoadScene("ProfileScene");
        }
        else
            Debug.Log("Log in Failed. Invalid username or password: Response from server >> " + response);
    }
    void OnGetProfilesForLoginRequestComplete(string response)
    {
        if (response[0] == '[')
        {
            Debug.Log("Getting Profiles List was Successful");
            Client.Instance.BeginRequest_ValidatePassword(userName, password, OnLogInRequestComplete);
        }
        else
        {
            Debug.Log("Failed to get Profiles List. response: " + response);
        }
    }

}