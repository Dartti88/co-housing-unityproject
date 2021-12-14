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
    public Animator animator;

    private Vector3 initialPosition;

    private Vector3 previousPosition;
    public float currentSpeed;

    // run once when scene is loaded
    void Start()
    {
        initialPosition = Client.Instance.initLocalPlayerPos;
        animator = GetComponentInChildren<Animator>();
        agent = GetComponent<NavMeshAgent>();
        activeAvatar = avatars[0];
        ChangePlayerAvatar(Client.Instance.pHandler.userProfile.avatarID);
        animator.logWarnings = false;
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
        Client.Instance.BeginRequest_UpdateProfileStatus(Client.Instance.profileHandler.GetComponent<ProfileHandler>().userProfile.profileID, 1, goal, null);

        Debug.Log("<GOAL>: " + goal);
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

        //Animation speed slowing
        if(!animator.GetBool("always move"))
        {
            Vector3 curMove = transform.position - previousPosition;
            currentSpeed = curMove.magnitude / Time.deltaTime;
            previousPosition = transform.position;

            AnimatorStateInfo animState = animator.GetCurrentAnimatorStateInfo(0);
            float currentTime = animState.normalizedTime % 1;
            //Debug.Log(currentTime);
            if (currentTime <= 0.1f && currentSpeed <= 0.1f)
            {
                animator.speed = 0;
            }
            else
            {
                animator.speed = 1;//Mathf.Clamp(currentSpeed, 0.5f, 1);
            }
        }


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
