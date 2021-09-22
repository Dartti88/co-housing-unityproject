using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;



[Serializable]
public class Profile
{

    //This is only for testing. Later will get a apartmentID from server. Probably
    public static int OPEN_HOUSING_ID;
    public static string USERNAME_TAG = "Username";
    public static string PASSWORD_TAG = "Password";

    public int      id;
    public string   userName;
    public string   displayName;
    public string   password;

    public int      apartmentID;
    public int      communityID;
    public int      avatarID;
    public float    credits;
    public float    socialScore;
    public ProfileType      profileType;
    public string   creationDate;
    public string   description;
    
    /// <summary>
    /// For creating the userprofile 
    /// </summary>
    /// <param displayName="userName"></param>
    /// <param displayName="password"></param>
    /// <param displayName="createNewAccount"></param>
    public Profile(string userName, string password, bool createNewAccount = false)
    {
        if (createNewAccount)
        {
            this.userName = userName;
            password = Convert.ToBase64String(Encoding.UTF8.GetBytes(password));

            PlayerPrefs.SetString(USERNAME_TAG, userName);
            PlayerPrefs.SetString(PASSWORD_TAG, password);

            apartmentID = OPEN_HOUSING_ID++;
            communityID = 0;
            displayName = "Name not set";
            description = "Description not set.";
            avatarID = 0;
            credits = 0;
            socialScore = 0;
            profileType = (int)ProfileType.Silent;
            creationDate = DateTime.Now.ToString();
            //Upload to server

            Debug.Log("Username created: " + userName + " with password: " + password);
        }
        else if (true)//Needs Confirmation from server if there is a username/password combination
        {
            this.userName = userName;
            this.password = password;

            PlayerPrefs.SetString(USERNAME_TAG, userName);
            PlayerPrefs.SetString(PASSWORD_TAG, password);

            //Needs loaded values for the profile
            apartmentID = OPEN_HOUSING_ID++;
            communityID = 0;
            displayName = "Name not set";
            description = "Description not set.";
            avatarID = 0;
            credits = 0;
            socialScore = 0;
            profileType = (int)ProfileType.Silent;
            creationDate = DateTime.Now.ToString();

            Debug.Log("Username loaded: " + userName + " with password: " + password);
        }
    }

    /// <summary>
    /// For making Profiles from the data that we get from the server
    /// </summary>
    /// <param displayName="apartmentID"></param>
    /// <param displayName="userName"></param>
    /// <param displayName="displayName"></param>
    /// <param displayName="avatarID"></param>
    /// <param displayName="credits"></param>
    /// <param displayName="socialScore"></param>
    /// <param displayName="profileType"></param>
    public Profile(int apartmentID, int communityID, string userName, string displayName, string description, int avatarID, float socialScore, ProfileType profileType, DateTime creationDate)
    {
        this.userName = userName;
        this.apartmentID = apartmentID;
        this.communityID = communityID;
        this.displayName = displayName;
        this.description = description;
        this.avatarID = avatarID;
        this.socialScore = socialScore;
        this.profileType = profileType;
        this.creationDate = creationDate.ToString();
    }

    public Profile(int apartmentID, int communityID, string userName, string displayName, string password, string description, int avatarID, float socialScore, ProfileType profileType, DateTime creationDate)
    {
        this.userName = userName;
        this.apartmentID = apartmentID;
        this.communityID = communityID;
        this.displayName = displayName;
        this.password = password;
        this.description = description;
        this.avatarID = avatarID;
        this.socialScore = socialScore;
        this.profileType = profileType;
        this.creationDate = creationDate.ToString();
    }

    public void SaveProfile()
    {
        Debug.Log("Saved Changes! Name: " + displayName + " AvatarID: " + avatarID);
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
