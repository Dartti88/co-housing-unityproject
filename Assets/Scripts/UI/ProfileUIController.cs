using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUIController : MonoBehaviour
{
    public ProfileHandler pHandler;
    public Image profileBackground;
    public Image profileChangeCanvas;
    public Button closeProfileBtn;
    public Image profilePics; //this is the grid
    public Image currentProfilePic;
    public Image[] profileImages;
    public GameObject ProfilePicture;
    public InputField profileName;
    public InputField profileDescription;
    public Text nameText;
    public Text descriptionText;
    int imageIndex;

    public void Start()
    {
        profileChangeCanvas.gameObject.SetActive(false);
        profilePics.gameObject.SetActive(false);
        
        //straight up copied this from profile-team, not in use atm though
        if (pHandler != null)
            imageIndex = pHandler.userProfile.avatarID;
        else
            imageIndex = 0;
        ChangeImage();
    }

    void ChangeImage(){
        //currentProfilePic.texture = profileImages[imageIndex];
        //pHandler.ChangeAvatarID(imageIndex);
    }

    public void CloseProfileCanvas(){
        if(profileBackground.gameObject.activeSelf == true)
        {
            profileBackground.gameObject.SetActive(false);
            closeProfileBtn.gameObject.GetComponent<Image>().color = Color.green;
        }
        else if (profileBackground.gameObject.activeSelf == false)
        {
            profileBackground.gameObject.SetActive(true);
            closeProfileBtn.gameObject.GetComponent<Image>().color = Color.red;
        }
    }

    public void ChangeProfileInfo(){
        profileChangeCanvas.gameObject.SetActive(true);
        //currentProfilePic = ProfilePicture.gameObject.GetComponent<Image>();
    }

    public void ConfirmChangesToServer(){
        Debug.Log(currentProfilePic.sprite);
        //here the function to send all info to server
        //use the index to know which pic+avatar prefab pack to use and send info to server!
        int index = int.Parse(currentProfilePic.sprite.name);
        Debug.Log(index + " index");

        Image im = currentProfilePic;
        ProfilePicture.gameObject.GetComponent<Image>().sprite = im.sprite;



        profileChangeCanvas.gameObject.SetActive(false);
    }

    public void CloseProfileInfo(){
        profileChangeCanvas.gameObject.SetActive(false);
    }

    public void OpenProfileChoosingS(){
        profilePics.gameObject.SetActive(true);
    }

    //TODO input fields

}
