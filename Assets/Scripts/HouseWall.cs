using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// attached on each wall and pillar of the house
public class HouseWall : MonoBehaviour
{
    // in which camera rotations this wall is hidden
    public List<ViewDirection> DisabledDirection;

    // is the wall a door
    public bool Door;

    // top part of the wall which is disabled when it would be in the way
    private MeshRenderer TopRenderer;
    private Collider TopCollider;

    // Start is called before the first frame update
    void Start()
    {
        TopRenderer = transform.GetComponent<MeshRenderer>();
        TopCollider= gameObject.AddComponent<Collider>();
        WallController.Instance.RegisterWall(this);
    }

    public void SetVisibility(bool state)
    {
        TopRenderer.enabled = state;
        //if (!Door) TopCollider.enabled = state;
    }

    // if this wall is set to be hidden during current camera rotation, hide it
    public void RefreshStatus()
    {
        if (DisabledDirection.Contains(Rotator.Instance.CurrentDirection))
        {
            SetVisibility(false);
        }
        else
        {
            SetVisibility(true);
        }
    }
}
