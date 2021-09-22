using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataController : MonoBehaviour
{
    public TaskList task_list = new TaskList();
    //public ItemList item_list = new ItemList();
    //public ProfileList profile_list = new ProfileList();
    private static DataController _instance;

    public static DataController Instance { get { return _instance; } }
    //public Dictionary<int, Task> task_list = new Dictionary<int, Task>();
    public Dictionary<string, Item> item_list = new Dictionary<string, Item>();
    public Dictionary<string, Profile> profile_list = new Dictionary<string, Profile>();


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
