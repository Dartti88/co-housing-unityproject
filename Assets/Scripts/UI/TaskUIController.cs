using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskUIController : MonoBehaviour
{
    public GameObject taskContainer;
    public GameObject taskElementPrefab;

    // Testinappula, poistetaan my�hemmin
    public Button testButton;

    // Task lista joka haetaan serverilt�
    // T�ytyy muuttaa dictionaryksi, jos sen saa vain siin� muodossa
    // TestTask on apuluokkana, koska en ole viel� saanut Task-luokkaa k�ytt��n
    // TestTask l�ytyy t�m�n lopusta ja sen voi poistaa heti kun Task on k�yt�ss�
    private List<TestTask> taskList;

    private void Start()
    {
        GetDataFromServer();

        // Testifunktio, joka lis�� taskListaan random taskeja; voidaan poistaa
        //PopulateTaskList();

        ShowAllTasks();

        // Testinappula; voidaan poistaa
        testButton.GetComponent<Button>().onClick.AddListener(AddTestTask);
    }

    /// <summary>
    /// Gets the newest task data from the server.
    /// </summary>
    private void GetDataFromServer()
    {
        // PUUTTUU: Serverin kanssa kommunikointi
        // Data lis�t��n t�h�n listaan: taskList

        taskList = new List<TestTask>();
    }

    /// <summary>
    /// Displays the data from the server
    /// </summary>
    private void ShowAllTasks()
    {
        // Muista muuttaa TestTask -> Task ja lista dictionaryksi, jos tarvitsee

        for (int i = 0; i < taskList.Count; i++)
        {
            GameObject newTaskElement = Instantiate(taskElementPrefab, taskElementPrefab.transform.position, taskElementPrefab.transform.rotation);
            newTaskElement.transform.SetParent(taskContainer.transform, false);
            newTaskElement.GetComponent<TaskUIElement>().ShowTaskElement(
                taskList[i].id,
                taskList[i].creator,
                taskList[i].title,
                taskList[i].desc,
                taskList[i].reward,
                taskList[i].points,
                taskList[i].quantity,
                taskList[i].expiry);
        }
    }

    // Testik�ytt��n tehty nappi, joka lis�� yhden valmiiksi luodun default taskin
    private void AddTestTask()
    {
        GameObject newTaskElement = Instantiate(taskElementPrefab, taskElementPrefab.transform.position, taskElementPrefab.transform.rotation);
        newTaskElement.transform.SetParent(taskContainer.transform, false);
        newTaskElement.GetComponent<TaskUIElement>().ShowTaskElement(1, "Potato", "Test task", "Description", 100, 40, 3, "10.10.2021");
    }

    // Testik�ytt��n tehty funktio, joka randomisoi muutaman taskin valmiiksi listaan
    private void PopulateTaskList()
    {
        int listSize = 10;
        string testDesc = "";
        int rndDesc;

        for (int i = 0; i < listSize; i++)
        {
            rndDesc = Random.Range(10, 51);
            for (int j = 0; j < rndDesc; j++) { testDesc += "Description "; }

            taskList.Add(new TestTask() { 
                id = i, 
                creator = "Test Issuer", 
                title = "Test task " + i.ToString(),
                desc = testDesc,
                reward = Random.Range(10, 200),
                points = Random.Range(5, 50),
                quantity = Random.Range(1, 6),
                expiry = Random.Range(1, 32).ToString() + "." + Random.Range(1, 13).ToString() + "." + Random.Range(2021, 2023).ToString()
            });

            testDesc = "";
        }
    }
}

// T�m� poistetaan, kun saadaan Task class k�ytt��n
public class TestTask
{
    public int id;
    public string creator;
    public string title;
    public string desc;
    public float reward;
    public float points;
    public int quantity;
    public string expiry;
}
