using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Taskmanager : MonoBehaviour
{
    //For testing
    public int testId = 0;
    public Task testTask0;
    public Task testTask1;
    public Task testTask2;
    public Task testTask3;
    public Dictionary<int, Task> testTaskList;

    public GameObject taskObjTemplate;
    GameObject currentTaskObject;
    public GameObject listParent;

    public Dictionary<int, Task> taskList;
    // Start is called before the first frame update
    void Start()
    {
        //Format the taskList
        taskList = DataController.Instance.task_list;

        //Placeholder, used for the update method
        testTaskList = new Dictionary<int, Task>();
        testTaskList.Add(testTask0.id, testTask0);
        testTaskList.Add(testTask1.id, testTask1);
        testTaskList.Add(testTask2.id, testTask2);
        testTaskList.Add(testTask3.id, testTask3);
    }

    //Used for displaying the tasks in the list ingame. Called every time new content is loaded from server
    public void DisplayTasks()
    {
        //Empties the current list
        for(int i = 0; i<listParent.transform.childCount; i++)
        {
            Destroy(listParent.transform.GetChild(i).gameObject);
        }
        //Instantiates all the task objects from the list
        foreach (Task task in taskList.Values)
        {
            //Debug.Log("Task ID: " + obj.GetComponent<Task>().taskId);
            GameObject taskObj = Instantiate(taskObjTemplate, listParent.transform, true);
            ReplaceTaskComponent(taskObj, task);

            
        }
    }

    //Loads new tasks from server. Called by server.
    public void LoadTasks(Dictionary<int, Task> updatedTaskList)
    {
        taskList = new Dictionary<int, Task>(updatedTaskList);
        Debug.Log("New task list length: " + taskList.Count());
        DisplayTasks();
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
            bool test = taskList.Remove(taskId);
            Debug.Log("Removed??" + test);
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

    public void TestUpdateButton()
    {
        Debug.Log("TestTaskList length: " + testTaskList.Count);
        LoadTasks(testTaskList);
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





}
