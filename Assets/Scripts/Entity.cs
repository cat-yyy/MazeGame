using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Entity 
{
    public GridPoint Point { get; set; }
    public Square Type { get; set; }

    public Entity(GridPoint spawnPoint, Square type)
    {
        Point = spawnPoint;
        Type = type; 
    }

}
