using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public class Taskmanager : MonoBehaviour
{
    public GameObject taskContainer;
    public GameObject taskElementPrefab;

    //For testing
    int testId = 0;


    public Dictionary<int, Task> taskList;
    // Start is called before the first frame update
    void Start()
    {
        //Format the local taskList
        taskList = Client.Instance.task_list;
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
                task.id,
                task.creatorId,
                task.taskName,
                task.description,
                task.cost,
                task.points,
                task.quantity,
                task.date);

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
        taskList.Add(newTask.id, newTask);
        DisplayTasks();
    }
    
    public void RemoveTask(int taskId)
    {
        if(taskList.Count > 0)
        {
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
        CreateTask(0, "asd", 100, 1);
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
            creatorId = origTask.creatorId,
            id = origTask.id,
            cost = origTask.cost,
            description = origTask.description,
            targetId = origTask.targetId,
            quantity = origTask.quantity
        };
        return newTask;
    }


    //Probably obsolete version of formatting the new task
    public void ReplaceTaskComponent(GameObject origTaskObj, Task newTask)
    {
        Task origTask = origTaskObj.GetComponent<Task>();
        origTask.creatorId = newTask.creatorId;
        origTask.id = newTask.id;
        origTask.cost = newTask.cost;
        origTask.description = newTask.description;
        origTask.targetId = newTask.targetId;
        origTask.quantity = newTask.quantity;
    }

    // Create new task and add it to the Task List, retuns true if successful, false if not
    public bool CreateTask(int taskCost, string taskText, int taskPlace, int taskTimes)
    {
        Task task = new Task
        {
            creatorId = "test",//Guid.NewGuid(),   //Placeholder until profiles are implemented
            id = newId(),//Guid.NewGuid(),                 //Placehlder, replace int with Guid?
            cost = taskCost,
            description = taskText,
            targetId = taskPlace,
            quantity = taskTimes
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
        (task.cost, task.description, task.targetId, task.quantity) = 
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