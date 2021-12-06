using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;
using UnityEngine.UI;

public class ProfileUIController : MonoBehaviour
{
    public ProfileHandler pHandler;
    private PlayerController playerController;

    //profile
    public Image profileBackground; 
    public Image profileChangeCanvas;
    public Button closeProfileBtn;
    public Image profilePics; //this is the grid
    public GameObject currentProfilePic;
    public GameObject ProfilePicture;
    [SerializeField]
    [NamedArrayAttribute(new string[] {
        "Dino"      ,
        "Penguin"   ,
        "Astronaut" ,
        "Ghost"     ,
        "Griffin"   ,
        "LadyBug"   ,
        "Cone"      ,
        "Potato"    ,
        "Slime"     ,
        "Sheep"     ,
        "Robot"     ,
        "Dragon"    })]
    public Image[] profilePictures;
    [SerializeField]
    [NamedArrayAttribute(new string[] {
        "Loading..."         ,
        "Common Squirrel"    ,
        "Fine Stoat"         ,
        "Silver Fox"         ,
        "Golden Hare"        ,
        "Guardian Bear"      ,
        "Ruby Swan"          ,
        "Diamond Ringed Seal",
        "Legendary Lynx"     ,
        "Divine Snowy Owl"   ,
        "Mythical Moose",
        "God Mode"})]
    public Sprite[] levelPictures;

    [SerializeField]
    [NamedArrayAttribute(new string[]{
        "winter",
        "spring",
        "summer",
        "autumn"})]
    public Sprite[] seasons;



    public InputField profileName;
    public InputField profileDescription;
    public Text nameText;
    public Text descriptionText;
    public Text credits;
    
    //tasks
    public Button CancelTask;
    public Button addTask;
    public Canvas taskCanvas;
    
    //info
    public GameObject info;
    public GameObject creditboard;
    
    //level
    public Button level; //also has image
    public LevelManager level_manager;
    public GameObject levelUpIcon;

    //logOut
    public Button btn;

    //chat
    public GameObject chat;



    public void Start()
    {
        pHandler = FindObjectOfType<ProfileHandler>();
        profileChangeCanvas.gameObject.SetActive(false);
        profilePics.gameObject.SetActive(false);
        levelUpIcon.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, 0);
        nameText.text = pHandler.GetUserProfile().userName;
        level.gameObject.GetComponent<Button>().onClick.AddListener(LevelSlider);
        playerController = PlayerController.Instance;
        addTask.gameObject.GetComponent<Button>().onClick.AddListener(OpenAddTask);
        btn.GetComponent<Button>().onClick.AddListener(delegate { FindObjectOfType<ProfileHandler>().LogOut(); });

        profileName.characterLimit = 20;
        profileDescription.characterLimit = 144;

        LoadOnLogin();
        profileBackground.gameObject.SetActive(true);
    }

    //I need a one function to update the data from Profile Handler to the Profile UI. Also I didn't understand how the Avatar images should work. Notice I took the lines for this function from the ConfirmChangesToServer() function. Joel
    public void LoadOnLogin()
    {
        //this only gets the profile and sets everything up.
        nameText.text = pHandler.GetUserProfile().displayName;
        descriptionText.text = pHandler.GetUserProfile().description;
        credits.text = pHandler.GetUserProfile().credits.ToString();
        UpdateCredits();
        DateHelper dh = new DateHelper();
        string seasonName = dh.GetSeasonName();
        switch (seasonName)
        {
            case "winter":
                profileBackground.sprite = seasons[0]; //we want this to be winter, must check still
                break;
            case "spring":
                profileBackground.sprite = seasons[1]; //we want this to be spring
                break;
            case "summer":
                profileBackground.sprite = seasons[2]; //we want this to be summer
                break;
            case "autumn":
                profileBackground.sprite = seasons[3]; //we want this to be autumn
                break;

            default:
                profileBackground.sprite = seasons[0]; //we want this to be winter, must check still
                break;

        }
        for (int i = 0; i < 12; i++)
        {
            if (i == pHandler.GetUserProfile().avatarID)
            {
                ProfilePicture.GetComponent<Image>().sprite = profilePictures[i].sprite;
                playerController.ChangePlayerAvatar(i);
                break;
            }
            else Debug.Log("avatarID not 0-11 ");
        }
        //so after new user opens changecanvas
        if (pHandler.GetUserProfile().displayName == "No Name")
        {
             ChangeProfileInfo();
        }
    }

    public void CloseProfileCanvas(){
        profileBackground.gameObject.SetActive(!profileBackground.gameObject.activeSelf);
    }

    public void ChangeProfileInfo(){
        profileChangeCanvas.gameObject.SetActive(true);
        Image actual = ProfilePicture.gameObject.GetComponent<Image>();
        currentProfilePic.gameObject.GetComponent<Image>().sprite = actual.sprite;
        profileName.text = pHandler.GetUserProfile().displayName;
        profileDescription.text = pHandler.GetUserProfile().description;
    }

    public void ConfirmChangesToServer(){
        

        int index = UpdateProfileCanvas();
        

        pHandler.userProfile.avatarID = index;
        Debug.Log(pHandler.userProfile.avatarID);
        pHandler.userProfile.displayName = nameText.text;
        pHandler.userProfile.description = descriptionText.text;
        pHandler.SaveProfileChanges();

        profileChangeCanvas.gameObject.SetActive(false);
    }

    //this one PREPARES to send all changes to server! could also combine the two ^ but this one should NOT sync on login! instead there's a func for that on the top
    public int UpdateProfileCanvas()
    {
        //use the index to know which pic+avatar prefab pack to use and send info to server!
        int index = int.Parse(currentProfilePic.gameObject.GetComponent<Image>().sprite.name);
        int i = index;
        Debug.Log(i);
        AvatarButtonOnClick(i);

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
    
    public void OpenAddTask()
    {
        taskCanvas.gameObject.SetActive(!taskCanvas.gameObject.activeSelf);
    }
    
    private void AvatarButtonOnClick(int i)
    {
        playerController.ChangePlayerAvatar(i);
    }

    //siirrä eri skriptiin
    public void InfoButtonClick()
    {
        info.SetActive(!info.activeSelf);
    }
    public void CreditButtonClick()
    {
        creditboard.SetActive(!creditboard.activeSelf);
    }
    public void ChatButtonClick()
    {
        chat.SetActive(!chat.activeSelf);
    }

    public void LevelSlider()
    {
        //so this has to check max and current exp from server / use another func to set them up probably
        level_manager.UpdateLevels();
    }

    public void levelUp()
    {
        //emoteGO.GetComponent<EmoteBillboard>().UseEmote(4);
        //levelUpIcon.SetActive(true);
        StartCoroutine(FadeImage());
    }

    public void UpdateCredits()
    {
        credits.text = pHandler.GetUserProfile().credits.ToString();
    }

    IEnumerator FadeImage()
    {
        levelUpIcon.gameObject.SetActive(true);

        // loop over 1 second backwards
        for (float i = 1; i >= 0; i -= Time.deltaTime)
        {
            // set color with i as alpha
            levelUpIcon.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, i);
            yield return null;
        }

    }
}
