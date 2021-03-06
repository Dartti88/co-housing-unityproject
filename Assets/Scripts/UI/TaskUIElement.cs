using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskUIElement : MonoBehaviour
{
    //0=availabe, 1=accepted, 2=created
    public int taskState;
    public Taskmanager taskManager;
    public ProfileHandler profileHandler;
    private int _taskId;
    private int _creatorId;
    private int _taskQuantity;
    public Text taskTitleText;
    public Text taskExpiryText;
    public Text taskDescriptionText;
    public Text taskRewardText;
    public Text taskPointsText;
    public Text taskIssuerText;
    public Text taskQuantityText;
    public Text taskButtonText;

    public Image taskAvatarPicture;

    public Image taskTitleImage;
    public Image taskBackgroundImage;
    public Image taskDescImage;
    public Image taskRewardImage;
    public Image taskPointsImage;
    public Image taskIssuerImage;
    public Image taskQuantityImage;
    public Image taskButtonImage;
    public Image taskAvatarImage;

    public Button taskAcceptButton;

    public List<Sprite> avatarList = new List<Sprite>();
    public List<ColorSchemer> colorList = new List<ColorSchemer>();

    /// <summary>
    /// Initialize color schemes and find 'accept task' button.
    /// </summary>
    private void Start()
    {
        profileHandler = FindObjectOfType<ProfileHandler>();
        taskManager = GameObject.FindWithTag("Taskmanager").GetComponent<Taskmanager>();
        taskAcceptButton.GetComponent<Button>().onClick.AddListener(TaskOnClick); 

        // SEASON COLORS (Order: Title, background, field, button)
        // Spring
        colorList.Add(new ColorSchemer(new Color32(137, 170, 63, 255), new Color32(209, 255, 99, 255), new Color32(255, 255, 255, 255), new Color32(137, 170, 63, 255)));
        // Summer
        colorList.Add(new ColorSchemer(new Color32(100, 155, 63, 255), new Color32(143, 223, 49, 255), new Color32(255, 255, 255, 255), new Color32(100, 155, 63, 255)));
        // Autumn
        colorList.Add(new ColorSchemer(new Color32(165, 105, 46, 255), new Color32(255, 167, 71, 255), new Color32(255, 255, 255, 255), new Color32(165, 105, 46, 255)));
        // Winter
        colorList.Add(new ColorSchemer(new Color32(110, 187, 211, 255), new Color32(140, 231, 247, 255), new Color32(255, 255, 255, 255), new Color32(110, 187, 211, 255)));

        DateHelper dh = new DateHelper();
        SeasonColor(dh.GetSeasonInt());

        //// NEW COLORS (Order: Title, background, field, button)
        //// Red
        //colorList.Add(new ColorSchemer(new Color32(246, 136, 129, 255), new Color32(228, 116, 110, 255), new Color32(246, 136, 129, 255), new Color32(214, 102, 96, 255)));
        //// Orange
        //colorList.Add(new ColorSchemer(new Color32(251, 187, 141, 255), new Color32(236, 167, 117, 255), new Color32(251, 187, 141, 255), new Color32(218, 148, 97, 255)));
        //// Green
        //colorList.Add(new ColorSchemer(new Color32(171, 196, 131, 255), new Color32(148, 177, 104, 255), new Color32(171, 196, 131, 255), new Color32(123, 152, 79, 255)));
        //// Turquoise
        //colorList.Add(new ColorSchemer(new Color32(127, 212, 179, 255), new Color32(108, 192, 159, 255), new Color32(127, 212, 179, 255), new Color32(90, 172, 140, 255)));

        // TESTI: Random v??riteema taskille, voi ottaa pois k??yt??st??
        //RandomizeColor(Random.Range(0, colorList.Count));
    }

    /// <summary>
    /// Shows the information retrieved from the server in a single task element.
    /// Called from TaskUIController.
    /// </summary>
    /// <param name="id">Task id</param>
    /// <param name="creatorName">Task issuer name</param>
    /// <param name="title">Task name/title</param>
    /// <param name="desc">Task description</param>
    /// <param name="reward">Money reward</param>
    /// <param name="points">Social points reward</param>
    /// <param name="quantity">How many of the same task are left</param>
    /// <param name="expiryDate">When the task is going to expire</param>

    public void ShowTaskElement(int taskId, string displayName, string title, string desc, float reward, float points, int quantity, string expiryDate, int avatarID, int creatorID = 0)
    {
        // Parametrein?? annetaan vain ne tiedot, mitk?? n??kyv??t UI:ssa + ID (jos sit?? tarvitaan)
        // Taskia hyv??ksyess?? (TaskOnClick()) tehd????n erilaiset tarkistukset serverin kanssa

        // PUUTTUU: Osa tarvittavista tiedoista t??ytyy antaa eri muodossa parametrein??
        //          (esim. taskin tekij??n nimi tulee profiilista? ja date on nyt vain string testin??)
        //          Tarvittaessa t??ytyy lis??t?? muita tietoja (tarvitaanko esim. Target (Item.Guid)?)
        //          Ei viel?? tietoa, tuleeko social pointsit lopulliseen appiin, mutta niille on UI:ssa nyt paikka
        if(expiryDate.Equals("0000-00-00"))
        {
            expiryDate = "Never";
        }

        _taskId = taskId;
        _creatorId = creatorID;
        _taskQuantity = quantity;

        if(expiryDate != "Never")
        {
            DateHelper dh = new DateHelper(expiryDate);
            expiryDate = dh.GetNormalDateFormat();
        }

        taskTitleText.text = title;
        taskExpiryText.text = "Expires on\n" + expiryDate;
        taskDescriptionText.text = desc;
        taskRewardText.text = reward.ToString(); // t??ytyy ehk?? py??rist????
        taskPointsText.text = points.ToString(); // t??ytyy ehk?? py??rist????
        taskIssuerText.text = displayName;
        taskQuantityText.text = _taskQuantity.ToString();
        taskAvatarPicture.sprite = avatarList[avatarID];
    }

    /// <summary>
    /// Called when the accept button is clicked.
    /// </summary>
    private void TaskOnClick()
    {
        if(taskManager)
        {
            switch(taskState)
            {
                case 0:
                    taskManager.AcceptTask(_taskId, profileHandler.userProfile.profileID);
                    
                    _taskQuantity -= 1;

                    if (_taskQuantity < 1) { Destroy(gameObject); }
                    else { taskQuantityText.text = _taskQuantity.ToString(); }
                    break;
                case 1:
                    taskManager.CompleteTask(_taskId, profileHandler.userProfile.profileID);
                    _taskQuantity -= 1;

                    if (_taskQuantity < 1) { Destroy(gameObject); }
                    else { taskQuantityText.text = _taskQuantity.ToString(); }
                    break;
                case 2:
                    taskManager.RemoveTask(_taskId);
                    Destroy(gameObject);
                    break;
            }
            
        }
        // PUUTTUU: Serverin kanssa kommunikointi. 
        // T??ytyy tarkistaa, ett?? kukaan muu ei ole ottanut taskia samaan aikaan
        // tai l??hett???? tieto serverille, ett?? yksi task on otettu (jos useampi samasta on saatavilla)
        // Lis??ksi pit???? tarkistaa, ettei task ole vanhentunut
        // _taskId-muuttujaa voi k??ytt???? ehk?? t??ss?? hy??dyksi?

        // TESTAUSTA VARTEN: Nyt nappia painamalla quantity vain menee alasp??in ja task katoaa kun quantity = 0
        
    }

    /// <summary>
    /// Called when the delete button is clicked.
    /// </summary>
    public void TaskOnDelete()
    {
        if (taskManager)
        {
            taskManager.RemoveTask(_taskId);
        }
    }

    /// <summary>
    /// Gives a color scheme according to the season to the task element.
    /// </summary>
    private void SeasonColor(int seasonInt)
    {
        taskTitleImage.color = colorList[seasonInt].titleColor;
        taskBackgroundImage.color = colorList[seasonInt].backgroundColor;
        taskDescImage.color = colorList[seasonInt].fieldColor;
        taskRewardImage.color = colorList[seasonInt].fieldColor;
        taskPointsImage.color = colorList[seasonInt].fieldColor;
        taskIssuerImage.color = colorList[seasonInt].fieldColor;
        taskQuantityImage.color = colorList[seasonInt].fieldColor;
        taskButtonImage.color = colorList[seasonInt].buttonColor;
        taskAvatarImage.color = colorList[seasonInt].backgroundColor;
    }

    /// <summary>
    /// Gives a random color scheme to the task element.
    /// </summary>
    private void RandomizeColor(int rndCol)
    {
        if (rndCol < 0) { return; }

        taskTitleImage.color = colorList[rndCol].titleColor;
        taskBackgroundImage.color = colorList[rndCol].backgroundColor;
        taskDescImage.color = colorList[rndCol].fieldColor;
        taskRewardImage.color = colorList[rndCol].fieldColor;
        taskPointsImage.color = colorList[rndCol].fieldColor;
        taskIssuerImage.color = colorList[rndCol].fieldColor;
        taskQuantityImage.color = colorList[rndCol].fieldColor;
        taskButtonImage.color = colorList[rndCol].buttonColor;
        taskAvatarImage.color = colorList[rndCol].backgroundColor;
    }
}


/// <summary>
/// Helper class to store color schemes.
/// </summary>
public class ColorSchemer
{
    public Color titleColor;
    public Color backgroundColor;
    public Color fieldColor;
    public Color buttonColor;

    public ColorSchemer(Color col1, Color col2, Color col3, Color col4)
    {
        titleColor = col1;
        backgroundColor = col2;
        fieldColor = col3;
        buttonColor = col4;
    }
}
