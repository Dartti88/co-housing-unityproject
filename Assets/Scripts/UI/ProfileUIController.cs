using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUIController : MonoBehaviour
{
    public ProfileHandler pHandler;
    private PlayerController playerController;

    public Canvas taskCanvas;

    public Image profileBackground;
    public Image profileChangeCanvas;
    public Button closeProfileBtn;
    public Image profilePics; //this is the grid
    public GameObject currentProfilePic;
    public GameObject ProfilePicture;
    public Image[] profilePictures;

    public InputField profileName;
    public InputField profileDescription;
    public Text nameText;
    public Text descriptionText;

    public Text taskName;
    public Text taskDesc;
    public Text taskReward;
    public Text taskQuantity;
    public Text taskDate;
    public Button CancelTask;

    public void Start()
    {
        profileChangeCanvas.gameObject.SetActive(false);
        profilePics.gameObject.SetActive(false);
        nameText.text = pHandler.GetUserProfile().userName;
        taskCanvas.gameObject.SetActive(false);
        CancelTask.gameObject.GetComponent<Button>().onClick.AddListener(CancelTasks);
        playerController = PlayerController.Instance;

    }

    //I need a one function to update the data from Profile Handler to the Profile UI. Also I didn't understand how the Avatar images should work. Notice I took the lines for this function from the ConfirmChangesToServer() function. Joel
    public void LoadOnLogin()
    {
        //this only gets the profile and sets everything up.
        nameText.text = pHandler.GetUserProfile().displayName;
        descriptionText.text = pHandler.GetUserProfile().description;
        for(int i = 0; i < 12; i++)
        {
            if (i == pHandler.GetUserProfile().avatarID)
            {
                ProfilePicture.GetComponent<Image>().sprite = profilePictures[i].sprite;
                break;
            }
            else Debug.Log("avatarID not 0-11 ");

        }
        //also has to load tasks in the future
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
        profileName.text = pHandler.GetUserProfile().displayName;
    }

    public void ConfirmChangesToServer(int index){
        

        UpdateProfileCanvas();
        
       
       //
        //here the function to send all info to server
        //


        pHandler.userProfile.avatarID = index;
        pHandler.userProfile.displayName = nameText.text;
        pHandler.userProfile.description = descriptionText.text;
        pHandler.SaveProfileChanges();

        profileChangeCanvas.gameObject.SetActive(false);
    }

    //this one prepares to send all changes to server! could also combine the two ^ but this one should NOT sync on login! instead there's a func for that on the top
    public int UpdateProfileCanvas()
    {
        //use the index to know which pic+avatar prefab pack to use and send info to server!
        int index = int.Parse(currentProfilePic.gameObject.GetComponent<Image>().sprite.name);
        Debug.Log(index + " index");
        AvatarButtonOnClick(index);

        Image im = currentProfilePic.gameObject.GetComponent<Image>();
        ProfilePicture.gameObject.GetComponent<Image>().sprite = im.sprite;
        if (profileName.text.Length == 0)
        {
            profileName.text = pHandler.GetUserProfile().displayName;
            nameText.text = pHandler.GetUserProfile().displayName;
        }
        else if (profileName.text.Length > 0)
        {
            profileName.text = profileName.text.Substring(0, 1).ToUpper() + profileName.text.Substring(1).ToLower();
            nameText.text = profileName.text;

        }
        if (profileDescription.text.Length == 0)
        {
            descriptionText.text = pHandler.GetUserProfile().description;
        }
        else if (profileDescription.text.Length > 0)
        {
            descriptionText.text = profileDescription.text;
            descriptionText.text = descriptionText.text.Substring(0, 1).ToUpper() + descriptionText.text.Substring(1).ToLower();
        }

        return index;
    }

    public void CloseProfileInfo(){
        profileChangeCanvas.gameObject.SetActive(false);
    }

    public void OpenProfileChoosingS(){
        profilePics.gameObject.SetActive(true);
    }

    public void OpenTaskCreationCanvas()
    {
        taskCanvas.gameObject.SetActive(true);
    }

    public void AddTask()
    {
        //probably needs the task controller but for now: 
        Debug.Log("Task added!");
        taskCanvas.gameObject.SetActive(false);




        //taskName.text = "";
        //taskDesc.text = "";
        //taskReward.text = "";
        //taskQuantity.text = "";
        //taskDate.text = "";
    }

    public void CancelTasks()
    {
        taskName.text = "  ";
        taskDesc.text = "  ";
        taskReward.text = "  ";
        taskQuantity.text = "  ";
        taskDate.text = "  ";
        
        taskCanvas.gameObject.SetActive(false);
        Debug.Log("canceled task");
    }

    private void AvatarButtonOnClick(int i)
    {
        playerController.ChangePlayerAvatar(i);
    }
    //TODO write a function to set image from server based on number
}
