using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taskmanager : MonoBehaviour
{
    //For testing
    public int timesPressed;
    public Task testTask;

    public GameObject taskObjTemplate;
    GameObject currentTaskObject;
    public GameObject listObject;

    public Dictionary<int, GameObject> taskList;
    // Start is called before the first frame update
    void Start()
    {
        taskList = new Dictionary<int, GameObject>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //Unpacks the given task and adds it to the list
    GameObject UnpackAndListTask(string json)
    {
        //Original instantiation
        //GameObject newTaskObj = Instantiate(taskObjTemplate,listObject.transform);
        
        //Placeholder for testing
        GameObject newTaskObj = Instantiate(taskObjTemplate,new Vector3(listObject.transform.position.x, listObject.transform.position.y-timesPressed*101, listObject.transform.position.z), Quaternion.identity,listObject.transform);
        timesPressed++;
        newTaskObj.GetComponent<Task>().taskId = timesPressed;
        //Placeholder ends


        Task newTask = newTaskObj.GetComponent<Task>();
        JsonUtility.FromJsonOverwrite(json, newTask);
        return newTaskObj;
    }

    //Packs the given task into a json file
    string PackTask(Task task)
    {
        string json = JsonUtility.ToJson(task);
        return json;
    }

    GameObject RemoveTaskFromList(int taskId)
    {
        GameObject tempObj;
        GameObject taskObj;
        taskList.TryGetValue(taskId, out tempObj);
        taskList.Remove(taskId);
        taskObj = tempObj;
        Destroy(tempObj);
        
        //Placeholder for testing
        if(timesPressed>0)timesPressed--;
        //Placeholder ends

        return taskObj;
    }

    void TakeTaskFromList()
    {

    }

    void UpdateList()
    {

    }

    public void TestAddButton()
    {
        string json = PackTask(testTask);
        currentTaskObject = UnpackAndListTask(json);
        taskList.Add(timesPressed, currentTaskObject);
    }

    public void TestRemoveButton()
    {
        RemoveTaskFromList(timesPressed);   
    }






    // Create new task and add it to the Task List, retuns true if successful, false if not
    public bool CreateTask(int taskCost, string taskText, string taskPlace, int taskTimes)
    {
        Task task = new Task
        {
            ownerId = Guid.NewGuid(),   //Placeholder until profiles are implemented
            taskId = 0,                 //Placehlder, replace int with Guid?
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
