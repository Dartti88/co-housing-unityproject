using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Taskmanager : MonoBehaviour
{
    public Task testTask;

    public GameObject taskObjTemplate;
    public GameObject currentTaskObject;
    public GameObject listObject;
    // Start is called before the first frame update
    void Start()
    {
        string json = PackTask(testTask);
        currentTaskObject = UnpackAndListTask(json);
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    GameObject UnpackAndListTask(string json)
    {
        GameObject newTaskObj = Instantiate(taskObjTemplate,listObject.transform);
        Task newTask = newTaskObj.GetComponent<Task>();
        JsonUtility.FromJsonOverwrite(json, newTask);
        return newTaskObj;
    }

    string PackTask(Task task)
    {
        string json = JsonUtility.ToJson(task);
        return json;
    }

    void RemoveTaskFromList()
    {

    }

    void TakeTaskFromList()
    {

    }

    void UpdateList()
    {

    }
}
