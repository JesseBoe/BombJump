using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class ActorParameters
{
    public LayerMask layerMask;

    public bool Pushable = false;
    public int pushPriority = 0;
    public bool Bouncy = false;
    public bool IgnorePlatforms = false;
    public bool FreezeY = false;
    public bool FreezeX = false;

    public Vector2 MaxVelocity = new Vector2(float.MaxValue, float.MaxValue);

    [Range(0, 90)]
    public float SlopeLimit = 30;

    public float Gravity = -50f;
    public float KnockBackDamp = .93f;
    public float BounceDamp = .9f;
}
