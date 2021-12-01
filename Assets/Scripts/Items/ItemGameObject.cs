using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ItemGameObject : MonoBehaviour
{
    [SerializeField]
    public int _itemID;
    public string groupName;
    public string location;
    //List of the tasks on the item
    public Dictionary<int, Task> itemTasks;
    [SerializeField]
    private InputField itemText;

    [HideInInspector]
    public Taskmanager taskManager;

    // Start is called before the first frame update
    void Start()
    {
        if (GameObject.FindWithTag("ItemText") != null) itemText = GameObject.FindWithTag("ItemText").GetComponent<InputField>();
        else Debug.Log("ItemText error");
        if (groupName == "") groupName = this.name;
        taskManager = GameObject.FindWithTag("Taskmanager").GetComponent<Taskmanager>();
    }

    // Update is called once per frame
    void Update()
    {
    }

    public void ChooseItem()
    {
        itemText.text = groupName;
        taskManager.itemID = _itemID;
    }

    public void UnchooseItem()
    {
        itemText.text = "";
        taskManager.UnselectItem();
    }
}
