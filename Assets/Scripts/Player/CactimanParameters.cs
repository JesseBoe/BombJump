using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class CactimanParameters
{
    public enum PlayerState
    {
        FullControll,
        Dashing,
        Crashing,
        InDialog,
        Dead
    }

    public enum JumpBehavior
    {
        CanJumpOnGround,
        CanJumpAnywhere,
        CantJump,
        CanJumpAnywhereOnce
    }

    public enum DashBehavior
    {
        CanDashOnGround,
        CanDashAnywhere,
        CantDash
    }

    public float DashDuration = .8f;
    public float DashSpeed = 300;
    public float DashFrequency = 1f;

    public DashBehavior DashRestrictions;
    public JumpBehavior JumpRestrictions;
    public float JumpFrequency = .25f;
    public float JumpMagnitude = 50f;

}
