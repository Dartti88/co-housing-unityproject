using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Profile
{

    //This is only for testing. Later will get a housingID from server. Probably
    public static int OPEN_HOUSING_ID;

    private string IDtag = "PlayerProfileID";
    private Guid ID;
    private int housingID;

    public string name { get; set; }
    public int avatarID { get; set; }
    public float credits { get; private set; }
    public float socialScore { get; private set; } 
    public ProfileType profileType { get; private set; }


    public Profile()
    {
        ID = Guid.NewGuid();
        PlayerPrefs.SetString(IDtag, Convert.ToBase64String(ID.ToByteArray()));

        Debug.Log("ID created: " + ID);

        housingID = OPEN_HOUSING_ID++;
        name = "Name not set";
        avatarID = 0;
        credits = 0;
        socialScore = 0;
        profileType = ProfileType.Silent;
        //Upload to server
    }

    public Profile(Guid id)
    {
        ID = id;

        Debug.Log("ID loaded: " + ID);

        //Load these from server later
        housingID = OPEN_HOUSING_ID++;
        name = "Name not set";
        avatarID = 0;
        credits = 0;
        socialScore = 0;
        profileType = ProfileType.Silent;
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
