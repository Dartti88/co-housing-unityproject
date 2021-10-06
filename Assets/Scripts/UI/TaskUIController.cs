using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class TaskUIController : MonoBehaviour
{
    public GameObject taskContainer;
    public GameObject taskElementPrefab;

    // Testinappula, poistetaan myöhemmin
    public Button testButton;

    // Task lista joka haetaan serveriltä
    // Täytyy muuttaa dictionaryksi, jos sen saa vain siinä muodossa
    // TestTask on apuluokkana, koska en ole vielä saanut Task-luokkaa käyttöön
    // TestTask löytyy tämän lopusta ja sen voi poistaa heti kun Task on käytössä
    private List<TestTask> taskList;

    private void Start()
    {
        GetDataFromServer();

        // Testifunktio, joka lisää taskListaan random taskeja; voidaan poistaa
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
        // Data lisätään tähän listaan: taskList

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

    // Testikäyttöön tehty nappi, joka lisää yhden valmiiksi luodun default taskin
    private void AddTestTask()
    {
        GameObject newTaskElement = Instantiate(taskElementPrefab, taskElementPrefab.transform.position, taskElementPrefab.transform.rotation);
        newTaskElement.transform.SetParent(taskContainer.transform, false);
        newTaskElement.GetComponent<TaskUIElement>().ShowTaskElement(1, "Potato", "Test task", "Description", 100, 40, 3, "10.10.2021");
    }

    // Testikäyttöön tehty funktio, joka randomisoi muutaman taskin valmiiksi listaan
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

// Tämä poistetaan, kun saadaan Task class käyttöön
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
