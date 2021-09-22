using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class DataController : MonoBehaviour
{
    //public TaskList task_list = new TaskList();
    //public ItemList item_list = new ItemList();
    //public ProfileList profile_list = new ProfileList();
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


    public void LoadTasks(TaskList task_list)
    {

    }
    public void LoadObjects(/*ItemList item_list*/)
    {

    }
    public void LoadProfiles(/*ProfileList profile_list*/)
    {

    }
    public void AddTask(Task task)
    {

    }

    public void AddTask(Task task, Item item)
    {

    }

    public void RemoveTask(Task task)
    {

    }

    public void AddItem(Item item)
    {

    }

    /// <summary>
    /// Ask server to remove Task from TaskList. Archive. Add reward to profile.
    /// </summary>
    /// <param displayName="task"></param>
    /// <param displayName="profile_id"></param>
    public void CompleteTask(Task task, int profile_id)
    {

    }

    public void AddProfile(Profile profile)
    {

    }

    public void UpdateProfile(Profile profile)
    {

    }
}
