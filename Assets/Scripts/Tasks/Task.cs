using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Task
{
    //Distinct ID for the task
    public int id;
    //The ID of the creator
    public int creatorId;
    //Name for the task
    public string taskName;
    //The target objects ID where the task is located
    public int targetId;
    //The displayed text with the task description
    public string description;
    //Cost of the task (negative, positive or zero (free))
    public float cost;
    //Awarded points (if any)
    public float points;
    //How many times the task can be completed
    public int quantity;

    //Dates
    public DateTime creationDate;
    public DateTime expireDate;
    public string date;
}
