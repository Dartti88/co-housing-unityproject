using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[Serializable]
public class Task : MonoBehaviour
{
    public Guid ownerId;
    public string text;
    public int cost;
    public string place;
    public int times;
}
