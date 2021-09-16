using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class Profile
{

    //This is only for testing. Later will get a housingID from server. Probably
    public static int OPEN_HOUSING_ID;
    public static string USERNAME_TAG = "Username";
    public static string PASSWORD_TAG = "Password";


    
    private string _userName;
    private string _password;
    
    public int housingID { get; private set; }
    public int communityID { get; private set; }
    public string name { get; set; }
    public string description { get; set; }
    public int avatarID { get; set; }
    public float credits { get; private set; }
    public float socialScore { get; private set; } 
    public ProfileType profileType { get; private set; }
    public DateTime creationDate { get; private set; }

    /// <summary>
    /// For creating the userprofile 
    /// </summary>
    /// <param name="userName"></param>
    /// <param name="password"></param>
    /// <param name="createNewAccount"></param>
    public Profile(string userName, string password, bool createNewAccount = false)
    {
        if (createNewAccount)
        {
            _userName = userName;
            _password = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

            PlayerPrefs.SetString(USERNAME_TAG, _userName);
            PlayerPrefs.SetString(PASSWORD_TAG, _password);

            housingID = OPEN_HOUSING_ID++;
            communityID = 0;
            name = "Name not set";
            description = "Description not set.";
            avatarID = 0;
            credits = 0;
            socialScore = 0;
            profileType = ProfileType.Silent;
            creationDate = DateTime.Now;
            //Upload to server

            Debug.Log("Username created: " + _userName + " with password: " + _password);
        }
        else if (true)//Needs Confirmation from server if there is a username/password combination
        {
            _userName = userName;
            _password = password;

            PlayerPrefs.SetString(USERNAME_TAG, _userName);
            PlayerPrefs.SetString(PASSWORD_TAG, _password);

            //Needs loaded values for the profile
            housingID = OPEN_HOUSING_ID++;
            communityID = 0;
            name = "Name not set";
            description = "Description not set.";
            avatarID = 0;
            credits = 0;
            socialScore = 0;
            profileType = ProfileType.Silent;
            creationDate = DateTime.Now;

            Debug.Log("Username loaded: " + _userName + " with password: " + _password);
        }
    }

    /// <summary>
    /// For making Profiles from the data that we get from the server
    /// </summary>
    /// <param name="housingID"></param>
    /// <param name="userName"></param>
    /// <param name="name"></param>
    /// <param name="avatarID"></param>
    /// <param name="credits"></param>
    /// <param name="socialScore"></param>
    /// <param name="profileType"></param>
    public Profile(int housingID, int communityID, string userName, string name, string description, int avatarID, float socialScore, ProfileType profileType, DateTime creationDate)
    {
        _userName = userName;
        this.housingID = housingID;
        this.communityID = communityID;
        this.name = name;
        this.description = description;
        this.avatarID = avatarID;
        this.socialScore = socialScore;
        this.profileType = profileType;
        this.creationDate = creationDate;
    }

    public void SaveProfile()
    {
        Debug.Log("Saved Changes! Name: " + name + " AvatarID: " + avatarID);
        //Send profile changes to server
    }

    public enum ProfileType
    {
        Silent,
        Guest,
        Resident,
        Manager
    }
}
