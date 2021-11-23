using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class Taskmanager : MonoBehaviour
{
    public ItemGameObject taskboard;
    public bool DebugAdd;
    //0=taskList, 1=acceptedTasks_list, 2=createdTasks_list, 2=availableTasks_list, 3=itemTasks_list
    public int chosenTaskList;
    bool firstUpdateDone = false;
    public GameObject taskContainer;
    public GameObject loadingOverlay;
    public GameObject taskElementPrefab;
    public GameObject availableTaskElementPrefab;
    public GameObject acceptedTaskElementPrefab;
    public GameObject createdTaskElementPrefab;
    public GameObject addTaskUI;
    public ProfileHandler profileHandler;
    //For testing
    int testId = 0;

    [SerializeField]
    private string DefaultTaskDescription = "";

    [SerializeField]
    private GameObject profileObject;

    private int userID;
    public int itemID;

    [SerializeField]
    [NamedArrayAttribute(new string[] { "name", "description", "cost", "quantity", "uniqueQuantity", "points", "expiry" })]
    private InputField[] inputFields; // in the following order: [taskName, description, cost, quantity, uniqueQuantity, points]
    [SerializeField]
    private Button createTaskButton;


    public Dictionary<int, Task> taskList;
    public Dictionary<int, Task> acceptedTasks_list;
    public Dictionary<int, Task> createdTasks_list;
    public Dictionary<int, Task> availableTasks_list;
    public Dictionary<int, Task> itemTasks_list;

    // Start is called before the first frame update
    void Start()
    {
        profileHandler = FindObjectOfType<ProfileHandler>();
        //Format the local taskList
        taskList = Client.Instance.task_list;

        //When the button to create a task is pressed we parse the input from the user to the CreateTask function
        createTaskButton.onClick.AddListener(() =>
        {
            //Set the default values for creating the task
            int quantity, uniqueQuantity, points;
            float cost = quantity = uniqueQuantity = points = 0;
            int target = taskboard._itemID;
            string description = DefaultTaskDescription;
            string expiryDate = "0000-00-00";

            // First check if the name is set. If not, do not create the task
            if (!string.IsNullOrWhiteSpace(inputFields[0].text))
            {
                //Check if the variable is empty and then parse the value from the input
                if (!string.IsNullOrWhiteSpace(inputFields[1].text)) { description = inputFields[1].text; }
                if (!string.IsNullOrWhiteSpace(inputFields[2].text)) { cost = float.Parse(inputFields[2].text, System.Globalization.NumberStyles.Float); }
                if (!string.IsNullOrWhiteSpace(inputFields[3].text)) { quantity = int.Parse(inputFields[3].text, System.Globalization.NumberStyles.Integer); }
                if (!string.IsNullOrWhiteSpace(inputFields[4].text)) { uniqueQuantity = int.Parse(inputFields[4].text, System.Globalization.NumberStyles.Integer); }
                if (!string.IsNullOrWhiteSpace(inputFields[5].text)) { points = int.Parse(inputFields[5].text, System.Globalization.NumberStyles.Integer); }
                if (!string.IsNullOrWhiteSpace(inputFields[6].text)) { expiryDate = inputFields[6].text; }

                CreateTask(
                    inputFields[0].text,
                    description,
                    cost,
                    quantity,
                    uniqueQuantity,
                    points,
                    target,
                    expiryDate);
            }
            else
            {
                Debug.LogWarning("Cannot Create Task Without Name!");
            }
        });
    }

    private void Update()
    {
        ExecuteOnFirstUpdate();   
    }

    //Used for displaying the tasks in the list ingame. Called every time new content is loaded from server
    public void DisplayTasks(string emptystr)
    {
        int tempTaskState = 0; // default
        taskList = Client.Instance.task_list;
        acceptedTasks_list = Client.Instance.acceptedTasks_list;
        createdTasks_list = Client.Instance.createdTasks_list;
        GetAvailableTasks();

        //Empties the current list
        for (int i = 0; i<taskContainer.transform.childCount; i++)
        {
            Destroy(taskContainer.transform.GetChild(i).gameObject);
        }
        Dictionary<int, Task> tempList = taskList;
        GameObject tempPrefab = taskElementPrefab;
        if (chosenTaskList==1)
        {
            tempPrefab = acceptedTaskElementPrefab;
            tempList = acceptedTasks_list;
            tempTaskState = 1;
        }
        else if(chosenTaskList==2)
        {
            tempPrefab = createdTaskElementPrefab;
            tempList = createdTasks_list;
            tempTaskState = 2;
        }
        else if(chosenTaskList==3)
        {
            tempPrefab = availableTaskElementPrefab;
            tempList = availableTasks_list;
            tempTaskState = 0;
        }
        else if(chosenTaskList==4)
        {
            tempPrefab = availableTaskElementPrefab;
            tempList = itemTasks_list;
            tempTaskState = 0;
        }

        //Instantiates all the task objects from the list
        foreach (Task task in tempList.Values)
        {
            GameObject newTaskElement = Instantiate(tempPrefab, tempPrefab.transform.position, tempPrefab.transform.rotation);
            newTaskElement.transform.SetParent(taskContainer.transform, false);

            newTaskElement.GetComponent<TaskUIElement>().taskState = tempTaskState;
            int quantity = task.quantity;
            switch (tempTaskState)
            {
                case 0:
                    newTaskElement.GetComponent<TaskUIElement>().taskButtonText.text = "Accept Task";
                    break;
                case 1:
                    quantity = task.acceptedQuantity;
                    newTaskElement.GetComponent<TaskUIElement>().taskButtonText.text = "Complete Task";
                    break;
                case 2:
                    newTaskElement.GetComponent<TaskUIElement>().taskButtonText.text = "Delete Task";
                    break;
            }
            newTaskElement.GetComponent<TaskUIElement>().ShowTaskElement(
                task.taskID,
                Client.Instance.GetDisplayNameById(task.creatorID),
                task.taskName,
                task.description,
                task.cost,
                task.points,
                quantity,
                task.expirationDate,
                task.creatorID);
            

        }
        LoadingOverlay();
    }

    //Loads new tasks from server. Called by server.
    public void LoadTasks(string callbackstring)
    {
        LoadingOverlay();
        //Get the users ID
        userID = profileHandler.userProfile.profileID;
        Debug.Log("userID = " + userID);
        switch (callbackstring)
        {
            case "accepted":
                chosenTaskList = 1;
                Client.Instance.BeginRequest_GetAcceptedTasks(userID, DisplayTasks);
                break;
            case "created":
                chosenTaskList = 2;
                Client.Instance.BeginRequest_GetCreatedTasks(userID, DisplayTasks);
                break;
            case "available":
                chosenTaskList = 3;
                Client.Instance.BeginRequest_GetAvailableTasks(DisplayTasks);
                break;
            case "item":
                chosenTaskList = 4;
                Client.Instance.BeginRequest_GetAvailableTasks(DisplayTasks);
                break;
            default:
                switch(chosenTaskList)
                {
                    case 1:
                        Client.Instance.BeginRequest_GetAcceptedTasks(userID, DisplayTasks);
                        break;
                    case 2:
                        Client.Instance.BeginRequest_GetCreatedTasks(userID, DisplayTasks);
                        break;
                    case 3:
                    case 4:
                        Client.Instance.BeginRequest_GetAvailableTasks(DisplayTasks);
                        break;
                }
                break;
        }
        
        

        Debug.Log("New task list length: " + taskList.Count());
    }

    public void AddTask(Task newTask)
    {
        if (newTask.quantity == 0) newTask.quantity = 1;
        Client.Instance.BeginRequest_AddNewTask(newTask, null);
        //taskList.Add(newTask.taskID, newTask);
    }
    
    public void RemoveTask(int taskId)
    {
        userID = profileHandler.userProfile.profileID;
        if(!createdTasks_list.TryGetValue(taskId, out Task tmp)) { Debug.LogWarning("Task ID not valid!"); return; }
        if (tmp.creatorID != userID) { Debug.LogWarning("Cannot delete a task you do not own!"); return; }
        Client.Instance.BeginRequest_RemoveTask(profileHandler.userProfile.userName, profileHandler.userProfile.password, taskId, null);
    }

    public void AddSocialPoints(int amount)
    {   
        int current_level = profileHandler.userProfile.GetProfileLevel();
        profileHandler.userProfile.socialScore += amount;
        int new_level = profileHandler.userProfile.GetProfileLevel();

        if (new_level > current_level)
        {
            //ToDo - Trigger levelup event
        }
    }
    
    public void AcceptTask(int taskId, int profileId)
    {
        Task acceptedTask = taskList[taskId];

        if (profileHandler.userProfile.credits + acceptedTask.cost >= 0)
        {
            Client.Instance.BeginRequest_AcceptTask(profileId, taskId, null);

            if (acceptedTask.quantity > 1)
            {
                acceptedTask.quantity--;
            }
            else
            {
                taskList.Remove(taskId);
            }
        }
        else
        {
            Debug.LogWarning("Insufficient Credits!");
        }
        
    }

    public void GetAvailableTasks()
    {
        Dictionary<int, Task> tempList = new Dictionary<int, Task>();
        Dictionary<int, Task> tempItemList = new Dictionary<int, Task>();

        foreach (Task task in taskList.Values)
        {
            Debug.Log("Getting available tasks for profileID " + userID);
            if (task.creatorID != userID)
            {
                tempList.Add(task.taskID, task);
                if(task.targetID == itemID)
                {
                    tempItemList.Add(task.taskID, task);
                }
            }
        }
        availableTasks_list = tempList;
        itemTasks_list = tempItemList;
    }
    public void CompleteTask(int taskId, int profileId)
    {
        Client.Instance.BeginRequest_CompleteTask(profileId, taskId, LoadTasks);
        AddSocialPoints((int)Mathf.Ceil(taskList[taskId].points/* / acceptedTask.max_quanity*/));
    }


    //Placeholder function for a running int ID
    public int newId()
    {
        testId++;
        return testId;
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
        //Get the users ID
        userID = profileHandler.userProfile.profileID;
        taskTarget = itemID;
        if(DebugAdd)
        {
            userID = 999;
        }

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

    public void DebugButtonPressed()
    {
        if(DebugAdd)
        {
            DebugAdd = false;
        }
        else
        {
            DebugAdd = true;
        }
        
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
            addTaskUI.gameObject.SetActive(false);
            firstUpdateDone = true;
        }
    }

    public void LoadingOverlay()
    {
        if(loadingOverlay != null)
        {
            switch (loadingOverlay.activeSelf)
            {
                case true:
                    loadingOverlay.SetActive(false);
                    break;
                case false:
                    loadingOverlay.SetActive(true);
                    break;
            }
        }
    }

}

public class NamedArrayAttribute : PropertyAttribute
{
    public readonly string[] names;
    public NamedArrayAttribute(string[] names) { this.names = names; }
}

