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
    /// For making Profiles from the data that we get from the server
    /// </summary>
    /// <param name="apartmentID"></param>
    /// <param name="communityID"></param>
    /// <param name="userName"></param>
    /// <param name="displayName"></param>
    /// <param name="password"></param>
    /// <param name="description"></param>
    /// <param name="avatarID"></param>
    /// <param name="credits"></param>
    /// <param name="socialScore"></param>
    /// <param name="profileType"></param>
    /// <param name="creationDate"></param>
    public Profile(int apartmentID, int communityID, string userName, string displayName, string password, string description, int avatarID, float credits, float socialScore, ProfileType profileType, DateTime creationDate)
    {
        this.userName = userName;
        this.apartmentID = apartmentID;
        this.communityID = communityID;
        this.displayName = displayName;
        this.password = password;
        this.description = description;
        this.avatarID = avatarID;
        this.credits = credits;
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
