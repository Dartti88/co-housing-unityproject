using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DataController : MonoBehaviour
{
    public Dictionary<string, Task> task_list = new Dictionary<string, Task>();
    public Dictionary<string, Item> item_list = new Dictionary<string, Item>();
    public Dictionary<string, Profile> profile_list = new Dictionary<string, Profile>();

    void Start()
    {
        
    }

    void Update()
    {
        
    }

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

    public Task RemoveTask(Task task)
    {
        return null;
    }

    public Item AddItem(Item item)
    {
        return null;
    }

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
