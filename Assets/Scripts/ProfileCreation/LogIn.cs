using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;
using System;
using System.Globalization;
using System.Security.Cryptography;

public class LogIn : MonoBehaviour
{
    ProfileHandler pHandler;

    public PopupManager popupManager;
    public InputField input_userName;
    public InputField input_password;
    public InputField input_passwordAgain;
    public bool rememberMe;

    string userName;
    string password;
    string passwordAgain;

    private void Start()
    {
        pHandler = FindObjectOfType<ProfileHandler>();
        if (PlayerPrefs.HasKey("RememberMe"))
        {
            rememberMe = IntToBool(PlayerPrefs.GetInt("RememberMe"));
            transform.GetChild(0).Find("Remember Me").GetComponent<Toggle>().isOn = IntToBool(PlayerPrefs.GetInt("RememberMe"));
        }
            
        if (!rememberMe)
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

        password = PasswordToHexString(password);

        Profile newProfile = new Profile(
            1, 2,
            userName, "No Name",password , "No Description",
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
        else if (response == "InvalidUsername1")
        {
            popupManager.Show("Username already taken.");
            Debug.Log("Failed to create profile: Response from server >> " + response);
        }
        else
        {
            popupManager.Show("Server error");
            Debug.Log("Failed to create profile: Response from server >> " + response);
        }
    }

    public void OnClick_LogIn()
    {
        userName = input_userName.text;
        password = PasswordToHexString(input_password.text);
        Client.Instance.BeginRequest_GetAllProfiles(OnGetProfilesForLoginRequestComplete);
        //profUIController.LoadOnLogin();
    }

    void OnLogInRequestComplete(string response)
    {
        if (response == "Failed")
        {
            popupManager.Show("Invalid username or password");
            Debug.Log("Log in Failed. Invalid username or password: Response from server >> " + response);
        }
        else if (response.Contains("Error occured"))
        {
            popupManager.Show("Server error");
            Debug.Log("Failed to create profile: Response from server >> " + response);
        }
        else // !!!IF SUCCESS!!!
        {
            // We got the player's initial position as response(formatted as "x;y;z" <- ';' being delim)
            string[] responseData = response.Split(';');
            // Fukin weird globalization issue with these, if this specific kind of conversion wasnt used...
            float px = Single.Parse(responseData[0], CultureInfo.InvariantCulture); 
            float py = Single.Parse(responseData[1], CultureInfo.InvariantCulture);
            float pz = Single.Parse(responseData[2], CultureInfo.InvariantCulture);

            Client.Instance.initLocalPlayerPos = new Vector3(px, py, pz);

            Debug.Log("Log in was successful");
            PlayerPrefs.SetString(Profile.USERNAME_TAG, userName);
            PlayerPrefs.SetString(Profile.PASSWORD_TAG, password);
            if (Client.Instance.profile_list.profiles != null && Client.Instance.profile_list.profiles.Length > 0)
                FindObjectOfType<ProfileHandler>().userProfile = Client.Instance.profile_list.profiles.Where(x => x.userName == userName).First();
            FindObjectOfType<ProfileHandler>().userProfile.password = password;
            SceneManager.LoadScene("ProfileScene");
        }
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

    public void ClearPrefs(bool state)
    {
        PlayerPrefs.SetInt("RememberMe", BoolToInt(state));
    }

    int BoolToInt(bool state)
    {
        if (state)
            return 1;
        else
            return 0;
    }

    bool IntToBool(int state)
    {
        if (state == 0)
            return false;
        else
            return true;
    }

    string PasswordToHexString(string password)
    {
        byte[] array = SHA256.Create().ComputeHash(System.Text.Encoding.ASCII.GetBytes(password));
        string text = "";
        for (int i = 0; i < array.Length; i++)
            text += $"{array[i]:X2}";
        return text;
    }
}
