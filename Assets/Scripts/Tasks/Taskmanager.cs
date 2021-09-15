using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class Taskmanager : MonoBehaviour
{
    //For testing
    public GameObject testTask0;
    public GameObject testTask1;
    public GameObject testTask2;
    public GameObject testTask3;
    public Dictionary<Guid, GameObject> testTaskList;

    public GameObject taskObjTemplate;
    GameObject currentTaskObject;
    public GameObject listParent;

    public Dictionary<Guid, GameObject> taskList;
    // Start is called before the first frame update
    void Start()
    {
        //Format the taskList
        taskList = new Dictionary<Guid, GameObject>();

        //Placeholder
        testTaskList = new Dictionary<Guid, GameObject>();
        testTaskList.Add(Guid.NewGuid(), testTask0);
        testTaskList.Add(Guid.NewGuid(), testTask1);
        testTaskList.Add(Guid.NewGuid(), testTask2);
        testTaskList.Add(Guid.NewGuid(), testTask3);
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
        foreach(GameObject obj in taskList.Values)
        {
            //Debug.Log("Task ID: " + obj.GetComponent<Task>().taskId);
            Instantiate(obj, listParent.transform,true);
        }
    }

    //Loads new tasks from server. Called by server.
    public void LoadTasks(Dictionary<Guid, GameObject> updatedTaskList)
    {
        taskList = new Dictionary<Guid, GameObject>(updatedTaskList);
        DisplayTasks();
    }

    public void AddTask(GameObject newTask /*Add object here*/)
    {
        //TODO: Ask from server if ok to add
        taskList.Add(newTask.GetComponent<Task>().taskId, newTask);
        DisplayTasks();
    }
    
    public void RemoveTask(Guid taskId)
    {
        //bool test = taskList.Remove(taskId);
        //Debug.Log("Removed??" + test);
        DisplayTasks();
    }
    

    void CompleteTask(Guid taskId, string profileId)
    {

    }

    public void TestAddButton()
    {
        GameObject newTask = taskObjTemplate;
        newTask.GetComponent<Task>().taskId = Guid.NewGuid();
        AddTask(newTask);
    }

    public void TestRemoveButton()
    {
        RemoveTask(taskList.First().Key);
    }

    public void TestUpdateButton()
    {
        Debug.Log("TestTaskList length: " + testTaskList.Count);
        LoadTasks(testTaskList);
    }






    // Create new task and add it to the Task List, retuns true if successful, false if not
    public bool CreateTask(int taskCost, string taskText, string taskPlace, int taskTimes)
    {
        Task task = new Task
        {
            ownerId = Guid.NewGuid(),   //Placeholder until profiles are implemented
            taskId = Guid.NewGuid(),                 //Placehlder, replace int with Guid?
            cost = taskCost,
            text = taskText,
            place = taskPlace,
            times = taskTimes
        };

        //TODO: Add check for validity of task on client and server

        //TODO: Add task to list and sync with server

        return true;
    }

    public bool ModifyTask(int taskId, int taskCost, string taskText, string taskPlace, int taskTimes)
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
        (task.cost, task.text, task.place, task.times) = 
            (taskCost, taskText, taskPlace, taskTimes);

        //TODO: Add check for validity of task on client and server & update it

        return true;

    }





}
