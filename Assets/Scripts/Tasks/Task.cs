using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Task : MonoBehaviour
{
    //Distinct ID for the task
    public int id;
    //The ID of the creator
    public string creatorId;
    //The target objects ID where the task is located
    public int targetId;
    //The displayed text with the task description
    public string description;
    //Cost of the task (negative, positive or zero (free))
    public float cost;
    //How many times the task can be completed
    public int quantity;

    //Dates
    public DateTime creationDate;
    public DateTime expireDate;
}
