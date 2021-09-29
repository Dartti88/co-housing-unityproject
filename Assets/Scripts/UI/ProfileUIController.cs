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
        //currentProfilePic = ProfilePicture.gameObject.GetComponent<Image>();
    }

    public void ConfirmChangesToServer(){
        //
        //here the function to send all info to server
        //
        //use the index to know which pic+avatar prefab pack to use and send info to server!
        int index = int.Parse(currentProfilePic.sprite.name);
        Debug.Log(index + " index");

        Image im = currentProfilePic;
        ProfilePicture.gameObject.GetComponent<Image>().sprite = im.sprite;


        //CURRENTLY FUCKS SHIT UP IF LEFT EMPTY. WHY
        if (nameText.text.Length > 0)
        {
            nameText.text = profileName.text;
            nameText.text = nameText.text.Substring(0, 1).ToUpper() + nameText.text.Substring(1).ToLower();
        }
        if (descriptionText.text.Length > 0)
        {
            descriptionText.text = profileDescription.text;
            descriptionText.text = descriptionText.text.Substring(0, 1).ToUpper() + descriptionText.text.Substring(1).ToLower();
        }

        //TODO error messages if no name or description

        profileChangeCanvas.gameObject.SetActive(false);
    }

    public void CloseProfileInfo(){
        profileChangeCanvas.gameObject.SetActive(false);
    }

    public void OpenProfileChoosingS(){
        profilePics.gameObject.SetActive(true);
    }

}
