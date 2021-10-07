using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Task
{
    //Distinct ID for the task
    public int taskID;
    //The ID of the creator
    public int creatorID;
    //Name for the task
    public string taskName;
    //The target objects ID where the task is located
    public int targetID;
    //The displayed text with the task description
    public string description;
    //Cost of the task (negative, positive or zero (free))
    public float cost;
    //Awarded points (if any)
    public int points;
    //How many times the task can be completed
    public int quantity;
    //How many times the task has been completed
    public int acceptedQuantity;
    //If one person can accept the task only X amount of times
    public int uniqueQuantity;
    //Dates
    public string creationDate;
    public string expirationDate;
}
