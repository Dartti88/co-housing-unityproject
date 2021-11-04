using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemGameObject : MonoBehaviour
{
    [SerializeField]
    private int _itemID;

    //List of the tasks on the item
    public Dictionary<int, Task> itemTasks;


    public Taskmanager taskManager;

    // Start is called before the first frame update
    void Start()
    {
        taskManager = GameObject.FindWithTag("Taskmanager").GetComponent<Taskmanager>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    public void DisplayItemTasks()
    {
        taskManager.itemID = _itemID;
    }
}
