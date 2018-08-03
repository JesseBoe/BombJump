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
    private float timePassedSinceThrown = 0;
    private List<string> tempLayerMasks = new List<string>();

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

        if (thrown)
        {
            timePassedSinceThrown += Time.deltaTime;
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
                RaycastHit2D[] hits = Physics2D.BoxCastAll((Vector2)transform.position + _actor._Collider.offset, _actor._Collider.size, 0, Vector2.zero, Mathf.Infinity, defaultmask);
                if (hits.Length > 0)
                {
                    bool ground = false;
                    bool player = false;
                    foreach (var item in hits)
                    {
                        if (item.transform.gameObject.layer == LayerMask.GetMask("Ground"))
                        {
                            ground = true;
                        }
                        if (item.transform.gameObject.layer == LayerMask.GetMask("Player"))
                        {
                            player = true;
                        }
                    }
                    if (!ground)
                    {
                        if (!tempLayerMasks.Contains("Ground"))
                        {
                            tempLayerMasks.Add("Ground");
                        }
                    }
                    if (!player)
                    {
                        if (!tempLayerMasks.Contains("Player") && timePassedSinceThrown > .1f)
                        {
                            tempLayerMasks.Add("Player");
                        }
                    }

                    _actor.Parameters.layerMask = LayerMask.GetMask(tempLayerMasks.ToArray());
                }
                else if (timePassedSinceThrown > .1f)
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
