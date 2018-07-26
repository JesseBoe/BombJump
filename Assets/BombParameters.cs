using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class BombParameters
{
    public Vector2 MaxKnockBack;
    public Vector2 MinKnockBack;

    public float TimeToExplode;
    public float EffectiveRange;
}
