using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataController : MonoBehaviour
{
    public TaskList task_list = new Dictionary<string, Task>();
    private static DataController _instance;

    public static DataController Instance {  get { return _instance; } }

    public Dictionary<int, Task> task_list = new Dictionary<int, Task>();
    //public Dictionary<string, Item> item_list = new Dictionary<string, Item>();
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
