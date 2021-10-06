using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskUIElement : MonoBehaviour
{
    public Taskmanager taskManager;
    private int _taskId;
    private int _taskQuantity;

    public Text taskTitleText;
    public Text taskExpiryText;
    public Text taskDescriptionText;
    public Text taskRewardText;
    public Text taskPointsText;
    public Text taskIssuerText;
    public Text taskQuantityText;

    public Image taskTitleImage;
    public Image taskBackgroundImage;
    public Image taskDescImage;
    public Image taskRewardImage;
    public Image taskPointsImage;
    public Image taskIssuerImage;
    public Image taskQuantityImage;
    public Image taskButtonImage;

    public Button taskAcceptButton;

    public List<ColorSchemer> colorList = new List<ColorSchemer>();

    /// <summary>
    /// Initialize color schemes and find 'accept task' button.
    /// </summary>
    private void Start()
    {
        taskManager = GameObject.FindWithTag("Taskmanager").GetComponent<Taskmanager>();
        taskAcceptButton.GetComponent<Button>().onClick.AddListener(TaskOnClick);

        // Green (default)
        colorList.Add(new ColorSchemer(new Color32(116, 204, 74, 255), new Color32(92, 180, 50, 255), new Color32(76, 156, 39, 255), new Color32(31, 115, 0, 255)));
        // Red
        colorList.Add(new ColorSchemer(new Color32(219, 79, 75, 255), new Color32(180, 55, 50, 255), new Color32(156, 39, 42, 255), new Color32(149, 19, 16, 255)));
        // Blue
        colorList.Add(new ColorSchemer(new Color32(90, 168, 204, 255), new Color32(50, 148, 180, 255), new Color32(39, 114, 156, 255), new Color32(15, 84, 135, 255)));
        // Purple
        colorList.Add(new ColorSchemer(new Color32(184, 110, 236, 255), new Color32(157, 72, 192, 255), new Color32(140, 47, 180, 255), new Color32(101, 26, 140, 255)));
        // Yellow
        colorList.Add(new ColorSchemer(new Color32(255, 200, 54, 255), new Color32(229, 175, 10, 255), new Color32(212, 154, 0, 255), new Color32(188, 129, 17, 255)));
        // Orange
        colorList.Add(new ColorSchemer(new Color32(238, 154, 39, 255), new Color32(209, 122, 31, 255), new Color32(192, 96, 26, 255), new Color32(166, 75, 12, 255)));
        // Turquoise
        colorList.Add(new ColorSchemer(new Color32(74, 204, 188, 255), new Color32(50, 180, 165, 255), new Color32(39, 156, 148, 255), new Color32(0, 115, 109, 255)));
        // Pink
        colorList.Add(new ColorSchemer(new Color32(217, 103, 157, 255), new Color32(192, 74, 137, 255), new Color32(176, 46, 119, 255), new Color32(135, 22, 86, 255)));

        // TESTI: Random väriteema taskille, voi ottaa pois käytöstä
        // Jos haluaa jättää lopulliseen, täytyisi väriteema ehkä lähettää serverille, jotta se on aina sama per task?
        RandomizeColor(Random.Range(0, colorList.Count));
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
    public void ShowTaskElement(int taskId, string creatorName, string title, string desc, float reward, float points, int quantity, string expiryDate)
    {
        // Parametreinä annetaan vain ne tiedot, mitkä näkyvät UI:ssa + ID (jos sitä tarvitaan)
        // Taskia hyväksyessä (TaskOnClick()) tehdään erilaiset tarkistukset serverin kanssa

        // PUUTTUU: Osa tarvittavista tiedoista täytyy antaa eri muodossa parametreinä
        //          (esim. taskin tekijän nimi tulee profiilista? ja date on nyt vain string testinä)
        //          Tarvittaessa täytyy lisätä muita tietoja (tarvitaanko esim. Target (Item.Guid)?)
        //          Ei vielä tietoa, tuleeko social pointsit lopulliseen appiin, mutta niille on UI:ssa nyt paikka

        //_taskId = taskId;
        _taskQuantity = quantity;

        taskTitleText.text = title;
        taskExpiryText.text = "Expires   " + expiryDate;
        taskDescriptionText.text = desc;
        taskRewardText.text = reward.ToString(); // täytyy ehkä pyöristää
        taskPointsText.text = points.ToString(); // täytyy ehkä pyöristää
        taskIssuerText.text = creatorName;
        taskQuantityText.text = _taskQuantity.ToString();
    }

    /// <summary>
    /// Called when the accept button is clicked.
    /// </summary>
    private void TaskOnClick()
    {
        // PUUTTUU: Serverin kanssa kommunikointi. 
        // Täytyy tarkistaa, että kukaan muu ei ole ottanut taskia samaan aikaan
        // tai lähettää tieto serverille, että yksi task on otettu (jos useampi samasta on saatavilla)
        // Lisäksi pitää tarkistaa, ettei task ole vanhentunut
        // _taskId-muuttujaa voi käyttää ehkä tässä hyödyksi?

        // TESTAUSTA VARTEN: Nyt nappia painamalla quantity vain menee alaspäin ja task katoaa kun quantity = 0
        _taskQuantity -= 1;

        if(_taskQuantity < 1) { Destroy(gameObject); }
        else { taskQuantityText.text = _taskQuantity.ToString(); }
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
