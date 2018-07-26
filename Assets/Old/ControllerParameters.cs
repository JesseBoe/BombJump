using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ControllerParameters {

	public enum JumpBehavior
    {
        CanJumpOnGround,
        CanJumpAnywhere,
        CantJump
    }

    public enum DashBehavior
    {
        CanDashOnGround,
        CanDashAnywhere,
        CantDash
    }

    public bool Pushable = false;
    public bool IgnorePlatforms = false;

    public float DashDuration = .8f;
    public float DashSpeed = 300;
    public float DashFrequency = 1f;

    public Vector2 MaxVelocity = new Vector2(float.MaxValue, float.MaxValue);

    [Range(0, 90)]
    public float SlopeLimit = 30;

    public float Gravity = -50f;

    public DashBehavior DashRestrictions;
    public JumpBehavior JumpRestrictions;
    public float JumpFrequency = .25f;
    public float JumpMagnitude = 50f;
}
