using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// singleton controller for controlling the player character
public class PlayerController : Singleton<PlayerController>
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
    void Start()
    {
        initialPosition = transform.position;
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        activeAvatar = avatars[0];
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
        agent.SetDestination(goal);
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
        int dir = (int)activeAvatar.GetComponent<AvatarController>().avatarDirection;

        // Hide other avatars
        foreach (GameObject a in avatars)
        {
            a.SetActive(false);
        }

        // Set the new avatar active
        avatars[i].SetActive(true);
        avatars[i].GetComponent<AvatarController>().InitializeAvatar(dir);

        // Update animator
        animator = GetComponentInChildren<Animator>();
    }

    // run every frame of the game
    private void Update()
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
