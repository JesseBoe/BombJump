using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour {

    bool thrown = false;
    public GameObject Player;
    public GameObject trailPrefab;
    private SuperActor _actor;
    private Animator animator;
    private starState state;
    private LayerMask defaultmask;
    public bool DoneSpawn;

    private float timePassed;
    private Vector2 throwVelocity;

    private enum starState
    {
        Spawning,
        Acitve,
        Exploding
    }



	// Use this for initialization
	void Start () {
        state = starState.Spawning;
        _actor = GetComponent<SuperActor>();
        animator = GetComponent<Animator>();
        _actor.Active = false;
        animator.Play("Spawning");
        defaultmask = _actor.Parameters.layerMask;
        _actor.Parameters.layerMask = LayerMask.NameToLayer("Intangible");
        gameObject.layer = LayerMask.NameToLayer("Intangible");
        Player = GameObject.FindGameObjectWithTag("Player");
        timePassed = 0;
        DoneSpawn = false;
	}

    void Update()
    {
        timePassed += Time.deltaTime;
        if (timePassed > .5)
        {
            DoneSpawn = true;
        }
    }

    // Update is called once per frame
    void FixedUpdate ()
    {

        if (DoneSpawn && timePassed > .05f)
        {
            Instantiate(trailPrefab, new Vector3(transform.position.x + 16, transform.position.y + 16, transform.position.z + 5), Quaternion.identity);
            timePassed = 0f;
        }
        

        if (thrown)
        {
            if (gameObject.layer == LayerMask.NameToLayer("Intangible"))
            {
                RaycastHit2D hit = Physics2D.BoxCast((Vector2)transform.position + _actor._Collider.offset, _actor._Collider.size, 0, Vector2.zero, Mathf.Infinity, defaultmask);
                if (hit)
                {

                }
                else
                {
                    _actor.Parameters.layerMask = defaultmask;
                    gameObject.layer = LayerMask.NameToLayer("Star");
                }
            }

            bool isGoingRight = throwVelocity.x > 0;

            if (isGoingRight)
            {
                if (_actor._ControllerState.IsCollidingRight)
                {
                    throwVelocity.x *= -1;
                }
            }
            else
            {
                if (_actor._ControllerState.IsCollidingLeft)
                {
                    throwVelocity.x *= -1;
                }
            }

            _actor.SetHorizontalVeloicty(throwVelocity.x);
        }
	}

    public void Throw(Vector2 velocity)
    {
        thrown = true;
        throwVelocity = velocity;
        _actor.SetVerticalVelocity(velocity.y);
        _actor.Active = true;
    }
}
