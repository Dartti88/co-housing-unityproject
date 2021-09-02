using System.Collections;
using System.Collections.Generic;
using UnityEngine;

// singleton controller to keep track of the walls in the house
public class WallController : Singleton<WallController>
{
    // list of all walls in the house
    public List<HouseWall> Walls;

    // awake is called once on app start
    private void Awake()
    {
        Walls = new List<HouseWall>();
    }
    // start is called after awake
    void Start()
    {
        RefreshWalls();
    }

    // register a house wall to the list
    public void RegisterWall(HouseWall wall)
    {
        Walls.Add(wall);
    }

    // make all the walls check if they should be hidden or not
    public void RefreshWalls()
    {
        Walls.ForEach(wall =>
        {
            wall.RefreshStatus();
        });
    }
}
