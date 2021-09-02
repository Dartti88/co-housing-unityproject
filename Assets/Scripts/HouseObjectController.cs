using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// singleton controller that keeps track of the clickable objects in the house
public class HouseObjectController : Singleton<HouseObjectController>
{
    [HideInInspector]
    public ClickableObject CurrentSelectedObject = null;

    // set new selected clickable object
    public void SetSelectedObject(ClickableObject clickedObject)
    {
        UnselectCurrent();
        CurrentSelectedObject = clickedObject;
    }

    public void UnselectCurrent()
    {
        if (CurrentSelectedObject != null) CurrentSelectedObject.Unselect();
    }
}
