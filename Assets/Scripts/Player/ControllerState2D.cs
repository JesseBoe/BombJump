using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ControllerState2D
{
    public bool IsCollidingRight { get; set; }
    public bool IsCollidingLeft { get; set; }
    public bool IsCollidingUp { get; set; }
    public bool IsCollidingDown { get; set; }
    public bool IsMovingDownSlope { get; set; }
    public bool IsMovingUpSlope { get; set; }
    public bool IsGrounded { get { return IsCollidingDown; } }
    public float SlopeAngle { get; set; }
    public bool IsFacingRight { get; set; }
    public bool HasBeenPushed { get; set; }
    public List<GameObject> pushedBy = new List<GameObject>(8);
    public GameObject standingOn;
    public List<GameObject> standingOnMe = new List<GameObject>(8);
    public bool DisablePush = false;
    public int timePushed;
    public bool HasMoved = false;
    public bool isStarRiding = false;
    public bool isPlatRide = false;
    public List<GameObject> hasCollisionsWith = new List<GameObject>();

    public bool HasCollisions { get { return IsCollidingDown || IsCollidingLeft || IsCollidingRight || IsCollidingUp; } }

    public void Reset()
    {
        IsMovingUpSlope =
            HasMoved =
            IsMovingDownSlope =
            IsCollidingLeft =
            IsCollidingRight =
            IsCollidingUp =
            IsCollidingDown = 
            HasBeenPushed = false;

        timePushed = 0;
        standingOn = null;
        pushedBy.Clear();
        SlopeAngle = 0;
    }

    public override string ToString()
    {
        return string.Format("(controller: r: {0} 1: {1} a: {2} b: {3} down-slope: {4} up-slope: {5} angle: {7}",
            IsCollidingRight,
            IsCollidingLeft,
            IsCollidingUp,
            IsCollidingDown,
            IsMovingDownSlope,
            IsMovingUpSlope,
            SlopeAngle);
    }
}
