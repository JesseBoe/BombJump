using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Star : MonoBehaviour {

    public GameObject Player;
    public GameObject trailPrefab;
    public GameObject starBlastPrefab;
    private SuperActor _actor;
    private Animator animator;
    private starState state;
    private LayerMask defaultmask;

    private float timePassed;
    private List<string> checkCollides = new List<string>();
    private List<int> keepChecking = new List<int>();
    private Throwable throwable;
    private AudioSource source;
    private bool wasGrounded = false;


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
        throwable = GetComponent<Throwable>();
        timePassed = 0;
        source = GetComponent<AudioSource>();
        source.Play();
	}

    void Update()
    {
        if (_actor._ControllerState.IsGrounded && !wasGrounded)
        {
            ActorManager.instance.PlaySound("LandingFinal", 1f);
        }
        timePassed += Time.deltaTime;
        wasGrounded = _actor._ControllerState.IsGrounded;
    }

    // Update is called once per frame
    void FixedUpdate ()
    {

        if (throwable.DoneSpawning && timePassed > .05f)
        {
            Instantiate(trailPrefab, new Vector3(transform.position.x + 16, transform.position.y + 16, transform.position.z + 5), Quaternion.identity);
            timePassed = 0f;
        }

        if (throwable.Thrown)
        {

            if (gameObject.layer == LayerMask.NameToLayer("Intangible"))
            {
                adjustLayers();
            }

            bool isGoingRight = throwable.ThrowVelocity.x > 0;

            if (isGoingRight)
            {
                if (_actor._ControllerState.IsCollidingRight)
                {
                    throwable.ThrowVelocity.x *= -1;
                }
            }
            else
            {
                if (_actor._ControllerState.IsCollidingLeft)
                {
                    throwable.ThrowVelocity.x *= -1;
                }
            }

            _actor.SetHorizontalVeloicty(throwable.ThrowVelocity.x);

            foreach (var item in _actor._ControllerState.hasCollisionsWith)
            {
                if (item.layer == LayerMask.NameToLayer("Starblock"))
                {
                    item.GetComponent<StarBlock>().hit();
                    ActorManager.instance.PlaySound("TargetHit", 1f);
                    Instantiate(starBlastPrefab, transform.position + new Vector3(16, 16, -1f), Quaternion.identity);
                    removeStar();
                }
                if (item.layer == LayerMask.NameToLayer("Corruption"))
                {
                    Instantiate(starBlastPrefab, transform.position + new Vector3(16, 16, -1f), Quaternion.identity);
                    ActorManager.instance.PlaySound("Corruption", 1f);
                    removeStar();
                }
            }
        }
	}

    private void adjustLayers()
    {
        //This slowly activates collisions so that if spawned inside of another object everything works out.
        RaycastHit2D[] hits = Physics2D.BoxCastAll((Vector2)transform.position + _actor._Collider.offset, _actor._Collider.size, 0, Vector2.zero, Mathf.Infinity, defaultmask);
        if (hits.Length > 0)
        {
            keepChecking.Clear();
            foreach (var item in hits)
            {
                if (!checkCollides.Contains(LayerMask.LayerToName(item.transform.gameObject.layer)))
                {
                    keepChecking.Add(item.transform.gameObject.layer);
                }
            }
            for (int i = 0; i < 28; i++)
            {
                if ((defaultmask == (defaultmask | (1 << i))))
                {
                    if (!keepChecking.Contains(i))
                    {
                        checkCollides.Add(LayerMask.LayerToName(i));
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < 28; i++)
            {
                if ((defaultmask == (defaultmask | (1 << i))))
                {
                    if (!checkCollides.Contains(LayerMask.LayerToName(i)))
                    {
                        checkCollides.Add(LayerMask.LayerToName(i));
                    }
                }
            }
            gameObject.layer = LayerMask.NameToLayer("Star");
        }
        _actor.Parameters.layerMask = LayerMask.GetMask(checkCollides.ToArray());

        if (_actor.Parameters.layerMask == defaultmask)
        {
            gameObject.layer = LayerMask.NameToLayer("Star");
        }
    }

    public void removeStar()
    {
        _actor.Remove();
        GameObject.Destroy(transform.parent.gameObject);
    }

    void OnBecameInvisible()
    {
        if (throwable.Thrown)
        {
            if (isActiveAndEnabled)
            {
                StartCoroutine("destroyTimer");
            }
        }
    }

    private void OnBecameVisible()
    {
        StopCoroutine("destroyTimer");
    }

    IEnumerator destroyTimer()
    {
        float destroytime = 0;
        while (destroytime < .6f)
        {
            destroytime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        removeStar();
    }
}
