using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour {

    public Vector2 vel;
    private bool beenShot = false;
    private SuperActor _actor;
    private LayerMask defaultmask;
    private float lifetime = 10f;
    private float timePassed = 0f;
    private bool collided = false;

    // Use this for initialization
    void Start () {
        _actor = GetComponent<SuperActor>();
    }
	
	// Update is called once per frame
	void Update () {
        timePassed += Time.deltaTime;
        if (collided)
        {
            //Makes the bullet go away
            transform.localScale = new Vector3(Mathf.Lerp(1, 0, timePassed * 4), Mathf.Lerp(1, 0, timePassed *4), 1);
            if (timePassed >= .25f)
            {
                _actor.Remove();
            }
        }
    }

    private void FixedUpdate()
    {
        if (timePassed > lifetime)
        {
            //bullet has not collided with anything in its lifetime(10 seconds)
            _actor.Remove();
        }

        if (beenShot)
        {
            //Moves bullet
            _actor.SetVelocity(vel);

            if (_actor._ControllerState.HasCollisions && !collided)
            {
                //we hit something.
                _actor.Active = false;
                collided = true;
                timePassed = 0;
            }
        }
    }
    //This should be called anytime a bullet is instantiated
    public void shoot(Vector2 velocity)
    {
        vel = velocity;
        beenShot = true;
    }
}
