using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using System;
using UnityEngine.UI;

public class TestObj : MonoBehaviour
{
    public InputField input_userName;
    public InputField input_displayName;
    public InputField input_password;
    
    void Start()
    {}
    
    void Update()
    {}


    public void OnClick_CreateNewUser()
    {
        string userName = input_userName.text;
        string displayName = input_displayName.text;
        string password = input_password.text;

        Profile newProfile = new Profile(
            1, 2, 
            userName, displayName, password, "Insert description here", 
            3, 0, Profile.ProfileType.Guest, 
            DateTime.Now
        );

        Client.Instance.BeginRequest_AddNewProfile(newProfile, OnCreateProfileRequestComplete);
    }
    void OnCreateProfileRequestComplete(string response)
    {
        if (response == "Success")
            Debug.Log("New profile was created successfully");
        else
            Debug.Log("Failed to create profile: Response from server >> " + response);
    }


    public void OnClick_LogIn()
    {
        string userName = input_userName.text;
        string password = input_password.text;
        Client.Instance.BeginRequest_ValidatePassword(userName, password, OnLogInRequestComplete);
    }
    void OnLogInRequestComplete(string response)
    {
        if (response == "Success")
            Debug.Log("Log in was successful");
        else
            Debug.Log("Log in Failed. Invalid username or password: Response from server >> " + response);
    }
}