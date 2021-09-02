using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

// singleton controller for controlling the player character
public class PlayerController : Singleton<PlayerController>
{
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

    // run every frame of the game
    private void Update()
    {
        if (agent.remainingDistance < 0.1f && walking)
        {
            animator.SetBool("walking", false);
            walking = false;
        }
        else if (agent.remainingDistance > 0.1f && !walking)
        {
            animator.SetBool("walking", true);
            walking = true;
        }
    }
}
