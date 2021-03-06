using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// used for objects in the house that can be clicked/tapped
public class ClickableObject : MonoBehaviour
{
    public int doorID;
    public bool stairs;
    public bool door;
    // when this object is selected, player will walk to this transform location
    public Transform NaviTarget;
    // 3d object to outline when selected
    public MeshRenderer TargetMesh = null;

    public List<ClickableObject> TargetGroup;

    private Outline outline;

    public UnityEvent OnClick;

    public ItemGameObject itemGameObject;

    Taskmanager taskManager;

    private void Start()
    {
        taskManager = GameObject.FindWithTag("Taskmanager").GetComponent<Taskmanager>();

        if (!TargetGroup.Contains(this))TargetGroup.Add(this);
        if (GetComponent<ItemGameObject>() != null) itemGameObject = GetComponent<ItemGameObject>();
        if (TargetMesh == null)
        {
            if (GetComponent<MeshRenderer>())
            {
                Debug.Log("not in child object");
                TargetMesh = GetComponent<MeshRenderer>();
            }
            else
            {
                Debug.Log("in child object");
                TargetMesh = GetComponentInChildren<MeshRenderer>();
            }
            
            outline = TargetMesh.gameObject.AddComponent<Outline>();
        }
        else
        {

            outline = TargetMesh.gameObject.AddComponent<Outline>();

        }
        // add outline and hide it
        outline.OutlineWidth = 6;
        outline.OutlineColor = Color.white;
        outline.OutlineMode = Outline.Mode.OutlineVisible;
        outline.enabled = false;
    }

    // object was clicked, select it and enable outline
    public void Select()
    {
        HouseObjectController.Instance.SetSelectedObject(TargetGroup);
        Debug.Log("selected");
        if (itemGameObject != null) itemGameObject.ChooseItem();
        StartCoroutine(WaitForPlayer());
        ToggleGroupOutline(true);
        ShowButton();
    }

    // another object is clicked or this unselected for some other reason
    public void Unselect()
    {
        HideButton();
        if (itemGameObject != null) itemGameObject.UnchooseItem();
        ToggleGroupOutline(false);
    }

    private void ToggleGroupOutline(bool state)
    {
        if (TargetGroup != null)
        {
            foreach (ClickableObject obj in TargetGroup)
            {
                obj.ToggleOutline(state);
            }
        }
    }

    private void ToggleOutline(bool state)
    {
        outline.enabled = state;
    }

    // wait for player to walk close to this object before activating it
    IEnumerator WaitForPlayer()
    {
        while (HouseObjectController.Instance.CurrentSelectedObject == TargetGroup)
        {
            if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < 1.5f)
            {
                OnClick.Invoke();
                break;
            }
            yield return null;
        }
    }

    void ShowButton()
    {
        if (stairs) taskManager.changeFloorButton.SetActive(true);
        else if (door)
        {
            taskManager.bookRoomButton.SetActive(true);
            taskManager.doorID = doorID;
        }
        else taskManager.showTasksButton.SetActive(true);
    }

    void HideButton()
    {
        taskManager.showTasksButton.SetActive(false);
        taskManager.changeFloorButton.SetActive(false);
        taskManager.bookRoomButton.SetActive(false);
    }
}
