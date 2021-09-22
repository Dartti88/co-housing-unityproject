using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataController : MonoBehaviour
{
    public TaskList task_list = new Dictionary<string, Task>();
    private static DataController _instance;

    [Serializable]
    public class ProfilesContainer
    {
        public Profile[] profiles;
    }
    public ProfilesContainer profile_list = new ProfilesContainer();


    public static DataController Instance {  get { return _instance; } }

    public Dictionary<int, Task> task_list = new Dictionary<int, Task>();
    

    private void Awake()
    {
        if (_instance != null && _instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            _instance = this;
        }
    }
    void Start()
    {
        
    }

    void Update()
    {
        
    }
    /*
    public TaskList LoadTasks(TaskList task_list)
    {
        return null;
    }
    public ItemList LoadObjects(ItemList item_list)
    {
        return null;
    }
    public ProfileList LoadProfiles(ProfileList profile_list)
    {
        return null;
    }
    public Task AddTask(Task task, [Item])
    {
        return null;
    }
   
    public Task AddTask(Task task, Item item)
    {
        return null;
    }
     */
    public Task RemoveTask(Task task)
    {
        return null;
    }
    /*
    public Item AddItem(Item item)
    {
        return null;
    }
    */
    public Task CompleteTask(Task task, int profile_id)
    {
        return null;
    }

    public Profile AddProfile(Profile profile)
    {
        return null;
    }

    public Profile UpdateProfile(Profile profile)
    {
        return null;
    }

}
