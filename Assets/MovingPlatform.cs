using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MovingPlatform : MonoBehaviour
{
    public float speedInPix;

    public List<Transform> TravelPoints = new List<Transform>();
    private SuperActor actor;
    private int loc = 0;
    private bool movingForward;
	// Use this for initialization
	void Start () {
        actor = GetComponent<SuperActor>();
	}
	
	// Update is called once per frame
	void Update () {
        Debug.Log(TravelPoints.Count);
    }

    private void FixedUpdate()
    {
        if (transform.position == TravelPoints[loc].position)
        {
            if (movingForward && loc < TravelPoints.Count - 1)
            {
                loc++;
            }
            else if (movingForward && loc == TravelPoints.Count - 1)
            {
                movingForward = false;
            }
            if (!movingForward && loc != 0)
            {
                loc--;
            }
            else if (!movingForward && loc == 0)
            {
                movingForward = true;
            }
        }

        Vector2 dir = Vector2.zero;

        if (transform.position.y < TravelPoints[loc].position.y)
        {
            dir.y = 1;
        }
        else if (transform.position.y > TravelPoints[loc].position.y)
        {
            dir.y = -1f;
        }
        if (transform.position.x < TravelPoints[loc].position.x)
        {
            dir.x = 1;
        }
        else if (transform.position.x > TravelPoints[loc].position.x)
        {
            dir.x = -1;
        }

        float dist;
        Vector2 vel = new Vector2(speedInPix, speedInPix);

        dist = Vector2.Distance(transform.position, TravelPoints[loc].position);
        
        if (dist < speedInPix)
        {
            vel.y = dist;
        }
        if (dist < speedInPix)
        {
            vel.y = dist;
        }
        
        vel *= 50f;
        vel *= dir;
        actor.SetVelocity(vel);
    }
}
