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
    public GameObject currentProfilePic;
    public GameObject ProfilePicture;

    public InputField profileName;
    public InputField profileDescription;
    public Text nameText;
    public Text descriptionText;

    public void Start()
    {
        profileChangeCanvas.gameObject.SetActive(false);
        profilePics.gameObject.SetActive(false);
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
        Image actual = ProfilePicture.gameObject.GetComponent<Image>();
        currentProfilePic.gameObject.GetComponent<Image>().sprite = actual.sprite;
    }

    public void ConfirmChangesToServer(){
        //
        //here the function to send all info to server
        //
        //use the index to know which pic+avatar prefab pack to use and send info to server!
        int index = int.Parse(currentProfilePic.gameObject.GetComponent<Image>().sprite.name);
        Debug.Log(index + " index");

        Image im = currentProfilePic.gameObject.GetComponent<Image>();
        ProfilePicture.gameObject.GetComponent<Image>().sprite = im.sprite;


        if (profileName.text.Length == 0)
        {
            nameText.text = "Test";
        }
        else if (profileName.text.Length > 0)
        {
            profileName.text = profileName.text.Substring(0, 1).ToUpper() + profileName.text.Substring(1).ToLower();
            nameText.text = profileName.text;
            
        }
       if (profileDescription.text.Length == 0)
       {
            descriptionText.text = "Test";
       }
       else if (profileDescription.text.Length > 0)
       {
            descriptionText.text = profileDescription.text;
            descriptionText.text = descriptionText.text.Substring(0, 1).ToUpper() + descriptionText.text.Substring(1).ToLower();
       }


        profileChangeCanvas.gameObject.SetActive(false);
    }

    public void CloseProfileInfo(){
        profileChangeCanvas.gameObject.SetActive(false);
    }

    public void OpenProfileChoosingS(){
        profilePics.gameObject.SetActive(true);
    }

}
