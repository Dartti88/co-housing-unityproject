using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class Taskmanager : MonoBehaviour
{
    //0=taskList, 1=acceptedTasks_list, 2=createdTasks_list
    public int chosenTaskList;
    bool firstUpdateDone = false;
    public GameObject taskContainer;
    public GameObject availableTaskElementPrefab;
    public GameObject acceptedTaskElementPrefab;
    public GameObject createdTaskElementPrefab;

    //For testing
    int testId = 0;
    
    [SerializeField]
    private GameObject profileObject;

    private int userID;

    [SerializeField]
    [NamedArrayAttribute(new string[] { "name", "description", "cost", "quantity", "uniqueQuantity", "points", "expiry" })]
    private InputField[] inputFields; // in the following order: [taskName, description, cost, quantity, uniqueQuantity, points]
    [SerializeField]
    private Button createTaskButton;


    public Dictionary<int, Task> taskList;
    public Dictionary<int, Task> acceptedTasks_list;
    public Dictionary<int, Task> createdTasks_list;
    // Start is called before the first frame update
    void Start()
    {
        //Format the local taskList
        taskList = Client.Instance.task_list;

        //When the button to create a task is pressed we parse the input from the user to the CreateTask function
        createTaskButton.onClick.AddListener(() => 
        {
            //Set the default values for creating the task
            int quantity, uniqueQuantity, points, target; float cost;
            cost = quantity = uniqueQuantity = points = target = 0;
            //Check if the variable is empty and then parse the value from the input
            if (!string.IsNullOrWhiteSpace(inputFields[2].text)) { cost = float.Parse(inputFields[2].text, System.Globalization.NumberStyles.Float); }
            if (!string.IsNullOrWhiteSpace(inputFields[3].text)) { quantity = int.Parse(inputFields[3].text, System.Globalization.NumberStyles.Integer); }
            if (!string.IsNullOrWhiteSpace(inputFields[4].text)) { uniqueQuantity = int.Parse(inputFields[4].text, System.Globalization.NumberStyles.Integer); }
            if (!string.IsNullOrWhiteSpace(inputFields[5].text)) { points = int.Parse(inputFields[5].text, System.Globalization.NumberStyles.Integer); }
            string expiryDate = "0000-00-00";
            if (inputFields[6].text != "") expiryDate = inputFields[6].text;
            CreateTask(
                inputFields[0].text, 
                inputFields[1].text, 
                cost,
                quantity,
                uniqueQuantity,
                points,
                target,
                expiryDate);
        });

        //Get the users ID
        userID = FindObjectOfType<ProfileHandler>().userProfile.profileID;
    }

    private void Update()
    {
        ExecuteOnFirstUpdate();   
    }

    //Used for displaying the tasks in the list ingame. Called every time new content is loaded from server
    public void DisplayTasks(string emptystr)
    {
        taskList = Client.Instance.task_list;
        acceptedTasks_list = Client.Instance.acceptedTasks_list;
        createdTasks_list = Client.Instance.createdTasks_list;
        //Empties the current list
        for (int i = 0; i<taskContainer.transform.childCount; i++)
        {
            Destroy(taskContainer.transform.GetChild(i).gameObject);
        }
        Dictionary<int, Task> tempList = taskList;
        GameObject tempPrefab = availableTaskElementPrefab;
        if (chosenTaskList==1)
        {
            tempPrefab = acceptedTaskElementPrefab;
            tempList = acceptedTasks_list;
        }
        else if(chosenTaskList==2)
        {
            tempPrefab = createdTaskElementPrefab;
            tempList = createdTasks_list;
        }

        //Instantiates all the task objects from the list
        foreach (Task task in tempList.Values)
        {
            GameObject newTaskElement = Instantiate(tempPrefab, tempPrefab.transform.position, tempPrefab.transform.rotation);
            newTaskElement.transform.SetParent(taskContainer.transform, false);
            newTaskElement.GetComponent<TaskUIElement>().ShowTaskElement(
                task.taskID,
                Client.Instance.GetDisplayNameById(task.creatorID),
                task.taskName,
                task.description,
                task.cost,
                task.points,
                task.quantity,
                task.expirationDate);

        }
    }

    //Loads new tasks from server. Called by server.
    public void LoadTasks(string callbackstring)
    {
        switch (callbackstring)
        {
            case "accepted":
                chosenTaskList = 1;
                break;
            case "created":
                chosenTaskList = 2;
                break;
            default:
                chosenTaskList = 0;
                break;
        }
        Client.Instance.BeginRequest_GetAcceptedTasks(userID, null);
        Client.Instance.BeginRequest_GetCreatedTasks(userID, null);
        Client.Instance.BeginRequest_GetAvailableTasks(DisplayTasks);
        
        
        Debug.Log("New task list length: " + taskList.Count());
    }

    public void AddTask(Task newTask /*Add object here*/)
    {
        if (newTask.quantity != 0) newTask.quantity = 1;
        Client.Instance.BeginRequest_AddNewTask(newTask, LoadTasks);
        //taskList.Add(newTask.taskID, newTask);
    }
    
    public void RemoveTask(int taskId)
    {
        if(!taskList.TryGetValue(taskId, out Task tmp)) { Debug.LogWarning("Task ID not valid!"); return; }
        if (tmp.creatorID != userID) { Debug.LogWarning("Cannot delete a task you do not own!"); return; }
        taskList.Remove(taskId);
        LoadTasks("empty");
    }
    
    public void AcceptTask(int taskId, int profileId)
    {
        Task acceptedTask = taskList[taskId];
        Client.Instance.BeginRequest_AcceptTask(profileId, taskId, null);
        if(acceptedTask.quantity>1)
        {
            acceptedTask.quantity--;
        }
        else
        {
            taskList.Remove(taskId);
        }
    }

    public void CompleteTask(int taskId, int profileId)
    {
        Client.Instance.BeginRequest_CompleteTask(profileId, taskId, null);
    }

    public void TestAddButton()
    {
        CreateTask("taskTask", "asd", 100, 1, 1, 5, 0, "");
    }

    public void TestRemoveButton()
    {
        if(taskList.Count > 0) RemoveTask(taskList.First().Key);
    }

    //Placeholder function for a running int ID
    public int newId()
    {
        testId++;
        return testId;
    }


    public Task CopyTask(Task origTask)
    {
        Task newTask = new Task
        {
            creatorID = origTask.creatorID,
            taskID = origTask.taskID,
            cost = origTask.cost,
            description = origTask.description,
            targetID = origTask.targetID,
            quantity = origTask.quantity
        };
        return newTask;
    }


    //Probably obsolete version of formatting the new task
    public void ReplaceTaskComponent(GameObject origTaskObj, Task newTask)
    {
        Task origTask = origTaskObj.GetComponent<Task>();
        origTask.creatorID = newTask.creatorID;
        origTask.taskID = newTask.taskID;
        origTask.cost = newTask.cost;
        origTask.description = newTask.description;
        origTask.targetID = newTask.targetID;
        origTask.quantity = newTask.quantity;
    }

    
    // Create new task and add it to the Task List, retuns true if successful, false if not
    public bool CreateTask(string taskName, string taskText, float taskCost, int taskQuantity,  int taskUniqueQ, int taskPoints, int taskTarget, string taskExpireDate)
    {
        Task task = new Task() { 
            creatorID = userID,                      //Placeholder until profiles are implemented
            taskID = newId(),
            cost = taskCost,
            taskName = taskName,
            description = taskText,
            targetID = taskTarget,
            quantity = taskQuantity,
            uniqueQuantity = taskUniqueQ,
            points = taskPoints,
            creationDate = "",                  // FORMAT?
            expirationDate = taskExpireDate
        };

        //TODO: Add check for validity of task on client and server

        //TODO: Add task to list and sync with server
        AddTask(task);
        return true;
    }

    //-----------------------------TARPEELLINEN?------------------------------------------------------

    public bool ModifyTask(int taskId, float? taskCost = null, string taskText = null, int? taskQuantity = null, int? taskUniqueQ = null, int? taskPoints = null, int? taskTarget = null, string taskExpireDate = null)
    {
        // Get the task
        if (!taskList.TryGetValue(taskId, out Task task))
        {
            Debug.LogWarning("Task ID not valid!");
            return false;
        }

        //Check that the user is the owner of the task  
        if (task.creatorID != userID)
            return false;

        //Overwrite old data with new
        if (taskCost != null) { task.cost = (float)taskCost; }
        if (taskQuantity != null) { task.quantity = (int)taskQuantity; }
        if (taskUniqueQ != null) { task.quantity = (int)taskUniqueQ; }
        if (taskPoints != null) { task.points = (int)taskPoints; }
        if (taskTarget != null) { task.targetID = (int)taskTarget; }
        if (string.IsNullOrWhiteSpace(taskText)) { task.description = taskText; }
        if (string.IsNullOrWhiteSpace(taskExpireDate)) { task.expirationDate = taskExpireDate; }

        //TODO: Add check for validity of task on client and server & update it

        return true;
    }


    public enum ListSearchType
    {
        exact,
        includeLarger,
        includeSmaller
    }

    public List<Task> GetFilteredList(string searchTerm = null, int? cost = null, ListSearchType listSearchType = ListSearchType.exact)
    {

        //Placeholder declaration, replace with actual list of all tasks
        List<Task> tasks = taskList.Values.ToList();

        //Temporary list for 
        List<Task> tmp  = new List<Task>();


        if(!string.IsNullOrWhiteSpace(searchTerm))
        {
            tmp = tasks.Where(task => task.description.Contains(searchTerm)).ToList();
        }
        if (cost != null)
        {
            switch (listSearchType)
            {
                case ListSearchType.exact:
                    tmp.Concat(tasks.Where(task => task.cost == cost).ToList());
                    break;
                case ListSearchType.includeLarger:
                    tmp.Concat(tasks.Where(task => task.cost >= cost).ToList());
                    break;
                case ListSearchType.includeSmaller:
                    tmp.Concat(tasks.Where(task => task.cost <= cost).ToList());
                    break;
            }
        }
        return tmp;
    }

    public enum SortType
    {
        CostLowest,
        CostHighest,
        QuantityLowest,
        QuantityHighest
    }

    // Sorts the list given as a reference
    public void SortList(ref List<Task> list, SortType sortType, SortType? optionalType = null)
    {
        // Helper function to get the float value to sort by 
        Func<Task, SortType?, float> getSortVal = (Task task, SortType? sort) =>
        {
            if (sort is null) { return 0f; }
            if (sort == SortType.CostHighest || sort == SortType.CostHighest) { return task.cost; } else { return task.quantity; }
        };

        if (sortType == SortType.CostLowest || sortType == SortType.QuantityLowest)
        {
            if (optionalType == SortType.CostLowest || optionalType == SortType.QuantityLowest)
            {
                list = list.OrderBy(task => getSortVal(task, sortType)).ThenBy(task => getSortVal(task, optionalType)).ToList();
            }
            else
            {
                list = list.OrderBy(task => getSortVal(task, sortType)).ThenByDescending(task => getSortVal(task, optionalType)).ToList();
            }
        }
        else
        {
            if (optionalType == SortType.CostLowest || optionalType == SortType.QuantityLowest)
            {
                list = list.OrderByDescending(task => getSortVal(task, sortType)).ThenBy(task => getSortVal(task, optionalType)).ToList();
            }
            else
            {
                list = list.OrderByDescending(task => getSortVal(task, sortType)).ThenByDescending(task => getSortVal(task, optionalType)).ToList();
            }
        }
    }

    void ExecuteOnFirstUpdate()
    {
        if(!firstUpdateDone)
        {
            //LoadTasks("empty");
            firstUpdateDone = true;
        }
    }


}

public class NamedArrayAttribute : PropertyAttribute
{
    public readonly string[] names;
    public NamedArrayAttribute(string[] names) { this.names = names; }
}

