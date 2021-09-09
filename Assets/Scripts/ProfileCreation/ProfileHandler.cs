using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ProfileHandler : MonoBehaviour
{
    private string GuidTag = "PlayerProfileID";
    private Profile userProfile;
    private void Awake()
    {
        //PlayerPrefs.DeleteAll();
        if (PlayerPrefs.HasKey(GuidTag)) //Load if Guid is founded
            userProfile = new Profile(new Guid(Convert.FromBase64String(PlayerPrefs.GetString(GuidTag))));
        else
            userProfile = new Profile();

    }

    public void ChangeName(TextMeshProUGUI text)
    {
        userProfile.name = text.text;
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
}
