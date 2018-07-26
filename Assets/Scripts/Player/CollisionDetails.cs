using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CollisionDetails
{
    public GameObject Go;
    public float DistanceToHit;
    public bool Movable;

    public CollisionDetails(GameObject go, float distance, bool movable)
    {
        Go = go;
        DistanceToHit = distance;
        Movable = movable;
    }
}
