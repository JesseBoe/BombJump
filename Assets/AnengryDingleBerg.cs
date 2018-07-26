using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnengryDingleBerg : MonoBehaviour {

    //Asign this in the inspector
    private SuperActor _actor;
    //Our enemy will travel back and forth between these two points;
    private Vector2 start, end;
    //If true the enemy is moving towards end. If false it is moving to start.
    bool movingRight = true;

	// Use this for initialization
	void Start () {
        _actor = GetComponent<SuperActor>();
        //The end position is 250 pix to the right of the starting position
        start = transform.position;
        end = new Vector2(transform.position.x + 500, 0f);
	}

    //We do everything movement related inside of a fixed update. This makes it so our game is very consistant. Having movement tied to an uncapped frame rate results in really bad things.
    //Right now for some reason our game fix updates at 50 frames a second. we will change that to 60 once I get less lazy.

    private void FixedUpdate()
    {
        if (movingRight)
        {
            //This is equal to 2 pixels every fixed update at 50 fps. If fps was at 60 it would be 120f to move 2 pix.
            _actor.SetHorizontalVeloicty(100f);
            //If we are 4 pixels away from our turning point we set moving right to false and start moving left
            if (transform.position.x > end.x)
            {
                movingRight = false;
            }
        }
        else
        {
            //Moving left
            _actor.SetHorizontalVeloicty(-100f);

            if (Vector2.Distance(transform.position, start) < 4f)
            {
                movingRight = true;
            }
        }
    }
}
