using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomInfo : MonoBehaviour
{
    public string databaseRoomName;
    public string roomName;
    public float size;
    public float creditPerHour;
    public float creditPerDay;
    public int floor; // 0 = outside, 1 = first, 2 = second, 3 = third
    public Sprite roomPicture;
}
