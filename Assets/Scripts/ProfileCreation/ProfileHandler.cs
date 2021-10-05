using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using System;

public class ProfileHandler : MonoBehaviour
{
    public Profile userProfile;

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
        
    }
}
