using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Throwable : MonoBehaviour {

    public bool Thrown = false;
    public Vector2 ThrowVelocity;
    public GameObject Player;
    private SuperActor _actor;
    public float TimePassedSinceThrown = 0;
    private float timePassed = 0;
    public bool DoneSpawning = false;

    // Use this for initialization
    void Start () {
        _actor = GetComponent<SuperActor>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        timePassed += Time.deltaTime;
        if (timePassed > .45)
        {
            DoneSpawning = true;
        }

        if (Thrown)
        {
            TimePassedSinceThrown += Time.deltaTime;
        }
	}

    public void Throw(Vector2 velocity)
    {
        Thrown = true;
        ThrowVelocity = velocity;
        _actor.SetVerticalVelocity(velocity.y);
        _actor.Active = true;
    }
}
