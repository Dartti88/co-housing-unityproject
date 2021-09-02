using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

// used for objects in the house that can be clicked/tapped
public class ClickableObject : MonoBehaviour
{
    // when this object is selected, player will walk to this transform location
    public Transform NaviTarget;
    // 3d object to outline when selected
    public MeshRenderer TargetMesh = null;

    private Outline outline;

    public UnityEvent OnClick;

    private void Start()
    {
        if (TargetMesh == null)
        {
            var child = GetComponentInChildren<MeshRenderer>();
            outline = child.gameObject.AddComponent<Outline>();
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
        HouseObjectController.Instance.SetSelectedObject(this);
        StartCoroutine(WaitForPlayer());
        ToggleOutline(true);
    }

    // another object is clicked or this unselected for some other reason
    public void Unselect()
    {
        ToggleOutline(false);
    }

    private void ToggleOutline(bool state)
    {
        outline.enabled = state;
    }

    // wait for player to walk close to this object before activating it
    IEnumerator WaitForPlayer()
    {
        while (HouseObjectController.Instance.CurrentSelectedObject == this)
        {
            if (Vector3.Distance(transform.position, PlayerController.Instance.transform.position) < 1.5f)
            {
                OnClick.Invoke();
                break;
            }
            yield return null;
        }
    }
}
