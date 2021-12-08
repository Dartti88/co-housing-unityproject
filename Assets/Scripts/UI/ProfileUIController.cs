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
    public Button addTaskBtn;
    public Canvas addTaskCanvas;
    public Canvas CanvasTaskUI;
    
    //info
    public GameObject info;
    public GameObject creditboard;
    
    //level
    public Button level; //also has image
    public LevelManager level_manager;
    public GameObject levelUpIcon;
    public GameObject emoteGO;
    public GameObject levelUpText;

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
        levelUpIcon.gameObject.SetActive(false);
        nameText.text = pHandler.GetUserProfile().userName;
        level.gameObject.GetComponent<Button>().onClick.AddListener(LevelSlider);
        playerController = PlayerController.Instance;
        addTaskBtn.gameObject.GetComponent<Button>().onClick.AddListener(OpenAddTask);
        btn.GetComponent<Button>().onClick.AddListener(delegate { FindObjectOfType<ProfileHandler>().LogOut(); });
        chat.gameObject.SetActive(false);
        profileName.characterLimit = 20;
        profileDescription.characterLimit = 144;
        profileBackground.gameObject.SetActive(true);
        LoadOnLogin();
        
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
                Debug.Log("season not loading");
                break;

        }
        for (int i = 0; i < 12; i++)
        {
            if (pHandler.GetUserProfile().avatarID == i)
            {
                ProfilePicture.GetComponent<Image>().sprite = profilePictures[i].sprite;
                playerController.ChangePlayerAvatar(i);
                break;
            }
            else Debug.Log("avatarID not " + i + ", avatarID is" + pHandler.GetUserProfile().avatarID);
        }

        CanvasTaskUI.gameObject.SetActive(false);
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
        addTaskCanvas.gameObject.SetActive(!addTaskCanvas.gameObject.activeSelf);
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
        level_manager.UpdateLevels();
    }

    public void levelUp()
    {
        Debug.Log("level gained");
        levelUpIcon.SetActive(true);
        CanvasTaskUI.gameObject.SetActive(false);
        emoteGO.GetComponent<EmoteBillboard>().UseEmote(6);
        StartCoroutine(ShowImage());
        //StartCoroutine(FadeImage());
    }

    public void UpdateCredits()
    {
        credits.text = pHandler.GetUserProfile().credits.ToString();
    }

    IEnumerator FadeImage()
    {
        // loop over 3 seconds backwards
        for (float i = 1; i >= 0; i -= Time.deltaTime/5)
        {
            // set color with i as alpha
            levelUpIcon.gameObject.GetComponent<Image>().color = new Color(1, 1, 1, i);
            levelUpText.gameObject.GetComponent<Text>().color = new Color(0, 0, 0, i);
            foreach(Transform child in levelUpText.transform)
            {
                child.gameObject.GetComponent<Text>().color = new Color(0, 0, 0, i);
            }
            yield return null;
        }
        levelUpIcon.SetActive(false);
        levelUpText.SetActive(false);

    }
    IEnumerator ShowImage()
    {
        // loop over 1 second backwards
        for (float i = 1; i >= 0; i -= Time.deltaTime/2)
        {
            yield return null;
            StartCoroutine(FadeImage());
        }

    }
}
