using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;
using UnityEngine.EventSystems;

// singleton controller for controlling the player character
public class CharacterController : MonoBehaviour
{
    // List of avatars
    public List<GameObject> avatars = new List<GameObject>();

    // Active avatar
    private GameObject activeAvatar;

    // pathfinding agent
    private NavMeshAgent agent;

    private bool walking = false;

    // character animator
    private Animator animator;

    private Vector3 initialPosition;

    // run once when scene is loaded
    public void Init(int avatarID)
    {
        initialPosition = transform.position;
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        activeAvatar = avatars[0];
        // Need to init all avatars, since "Start" -func may not happen..
        foreach (GameObject avatarObj in avatars)
            avatarObj.GetComponent<CharacterAvatarController>().Init(0);

        ChangePlayerAvatar(avatarID);
    }    

    // reset player position to starting position
    public void ResetPosition()
    {
        Rotator.Instance.ResetPosition();
        transform.position = initialPosition;
        agent.ResetPath();
    }

    // set goal position for pathfinding
    public void SetGoal(Vector3 goal)
    {
        if(agent.isOnNavMesh) agent.SetDestination(goal);
    }

    public Vector3 GetNextPosition()
    {
        return (gameObject.transform.position + agent.desiredVelocity.normalized);
    }

    public bool GetWalking()
    {
        return walking;
    }

    public void ChangePlayerAvatar(int i)
    {
        // Save the direction the character is facing
        int dir = (int)activeAvatar.GetComponent<CharacterAvatarController>().avatarDirection;

        // Hide other avatars
        foreach (GameObject a in avatars)
        {
            a.SetActive(false);
        }

        // Set the new avatar active
        avatars[i].SetActive(true);
        avatars[i].GetComponent<CharacterAvatarController>().Init(dir);

        // Update animator
        animator = GetComponentInChildren<Animator>();
    }

    // run every frame of the game
    private void Update()
    {
        if(agent.isOnNavMesh)
        {
            if (agent.remainingDistance < 0.1f && walking)
            {
                //animator.SetBool("walking", false);
                walking = false;
            }
            else if (agent.remainingDistance > 0.1f && !walking)
            {
                //animator.SetBool("walking", true);
                walking = true;
            }
        }
        
    }

    private bool IsPointerOverUIObject()
    {
        PointerEventData eventDataCurrentPosition = new PointerEventData(EventSystem.current);
        eventDataCurrentPosition.position = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        List<RaycastResult> results = new List<RaycastResult>();
        EventSystem.current.RaycastAll(eventDataCurrentPosition, results);
        Debug.Log(results.Count > 0);
        return results.Count > 0;
    }
}
