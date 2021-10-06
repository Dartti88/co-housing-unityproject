using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.UI;


public class Taskmanager : MonoBehaviour
{
    public GameObject taskContainer;
    public GameObject taskElementPrefab;

    //For testing
    int testId = 0;

    [SerializeField]
    private InputField[] inputFields; // in the following order: [taskName, description, cost, quantity, uniqueQuantity, points]
    [SerializeField]
    private Button createTaskButton;


    public Dictionary<int, Task> taskList;
    // Start is called before the first frame update
    void Start()
    {
        //Format the local taskList
        taskList = Client.Instance.taskList;

        // When the button to create a task is pressed we parse the input from the user to the CreateTask function
        // TODO: IHAN HELVETIN ISO REWRITE
        createTaskButton.onClick.AddListener(() => 
        {

            int cost, place, quantity, uniqueQuantity, points;
            cost = place = quantity = uniqueQuantity = points = 0;

            if (!string.IsNullOrWhiteSpace(inputFields[2].text)) { cost = int.Parse(inputFields[2].text, System.Globalization.NumberStyles.Integer); }
            if (!string.IsNullOrWhiteSpace(inputFields[3].text)) { quantity = int.Parse(inputFields[3].text, System.Globalization.NumberStyles.Integer); }
            if (!string.IsNullOrWhiteSpace(inputFields[4].text)) { uniqueQuantity = int.Parse(inputFields[4].text, System.Globalization.NumberStyles.Integer); }
            if (!string.IsNullOrWhiteSpace(inputFields[5].text)) { points = int.Parse(inputFields[5].text, System.Globalization.NumberStyles.Integer); }

            CreateTask(cost, 
                inputFields[1].text, 
                place ,
                quantity, 
                inputFields[0].text,
                uniqueQuantity,
                points);
        });

    }

    //Used for displaying the tasks in the list ingame. Called every time new content is loaded from server
    public void DisplayTasks()
    {
        //Empties the current list
        for(int i = 0; i<taskContainer.transform.childCount; i++)
        {
            Destroy(taskContainer.transform.GetChild(i).gameObject);
        }
        //Instantiates all the task objects from the list
        foreach (Task task in taskList.Values)
        {
            GameObject newTaskElement = Instantiate(taskElementPrefab, taskElementPrefab.transform.position, taskElementPrefab.transform.rotation);
            newTaskElement.transform.SetParent(taskContainer.transform, false);
            newTaskElement.GetComponent<TaskUIElement>().ShowTaskElement(
                task.taskID,
                task.creatorID.ToString(),
                task.taskName,
                task.description,
                task.cost,
                task.points,
                task.quantity,
                task.expirationDate);

        }
    }

    //Loads new tasks from server. Called by server.
    public void LoadTasks(Dictionary<int, Task> updatedTaskList)
    {
        taskList = new Dictionary<int, Task>(updatedTaskList);
        Debug.Log("New task list length: " + taskList.Count());
    }

    public void AddTask(Task newTask /*Add object here*/)
    {
        //TODO: Ask from server if ok to add
        taskList.Add(newTask.taskID, newTask);
        DisplayTasks();
    }
    
    public void RemoveTask(int taskId)
    {
        if(taskList.Count > 0)
        {
            taskList.Remove(taskId);
            DisplayTasks();
            return;
        }
        Debug.Log("Tried to remove an object from an empty list");
    }
    

    void CompleteTask(int taskId, string profileId)
    {

    }

    public void TestAddButton()
    {
        CreateTask(0, "asd", 100, 1, "taskTask", 1, 5);
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
    public bool CreateTask(int taskCost, string taskText, int taskPlace, int taskTimes, string taskName, int taskUniqueQ, int points)
    {
        Task task = new Task
        {
            creatorID = 0,//Guid.NewGuid(),   //Placeholder until profiles are implemented
            taskID = newId(),//Guid.NewGuid(),                 //Placehlder, replace int with Guid?
            cost = taskCost,
            taskName = taskName,
            description = taskText,
            targetID = taskPlace,
            quantity = taskTimes,
            uniqueQuantity = taskUniqueQ,
            points = points
        };

        //TODO: Add check for validity of task on client and server

        //TODO: Add task to list and sync with server
        AddTask(task);
        return true;
    }

    public bool ModifyTask(int taskId, int taskCost, string taskText, int taskPlace, int taskTimes)
    {
        //Placeholder
        Task task = new Task();

        //TODO: Get task from list by taskId

        /*  
        //Check that the user is the owner of the task  
        if (task.ownerId != user.GetID)
            return false;
        */

        //Overwrite old data with new
        (task.cost, task.description, task.targetID, task.quantity) = 
            (taskCost, taskText, taskPlace, taskTimes);

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
        List<Task> tasks = new List<Task>();

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
}