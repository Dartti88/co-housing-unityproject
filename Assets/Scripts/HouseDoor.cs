using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// sciprt on each door in the house
public class HouseDoor : MonoBehaviour
{
    // how many people are close to the door
    private int proximityCounter;

    // when player walks into proximity of the door
    void OnTriggerEnter(Collider other)
    {
        if (other.gameObject.CompareTag("Door"))
            return;

        SetDoorState(1);
    }

    // when player walks out of proximity of the door
    void OnTriggerExit(Collider other)
    {
        if (other.gameObject.CompareTag("Door"))
            return;

        SetDoorState(-1);
    }

    public void SetDoorState(int proximityUpdate)
    {
        // if there are more than 0 players near the door, open it. Otherwise close it
        proximityCounter += proximityUpdate;
        bool open = proximityCounter > 0;
        foreach (Animation anim in GetComponentsInChildren<Animation>())
        {
            anim.PlayQueued(open ? "DoorOpen" : "DoorClose", QueueMode.CompleteOthers);
        }
    }
}
