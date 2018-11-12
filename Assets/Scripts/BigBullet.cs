using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BigBullet : MonoBehaviour {

    private bool beenShot = false;
    public Vector2 vel;
    public SuperActor _actor;
    private bool collided = false;
    public GameObject movingPlatform;
    private SuperActor platform;
    public GameObject particleEffect;
    public GameObject smoke;
    private float timePassed = 0f;


	// Use this for initialization
	void Start () {
        _actor = GetComponent<SuperActor>();
	}
	
	// Update is called once per frame
	void Update () {
        if (!collided)
        {
            timePassed += Time.deltaTime;
            if (timePassed > .02f)
            {
                platform.transform.gameObject.layer = 19;
            }
        }
	}

    private void FixedUpdate()
    {
        if (collided)
        {
            particleEffect.transform.SetParent(null, true);
            Object.Destroy(particleEffect, 5f);
            _actor.Remove();

        }
        else if (beenShot)
        {
            //Moves bullet
            _actor.SetVelocity(vel);
            platform.SetVelocity(vel);

            if (_actor._ControllerState.HasCollisions && !collided)
            {
                //we hit something.
                if (timePassed > .07f)
                {
                    smoke.transform.parent = null;
                    smoke.GetComponent<ParticleSystem>().Stop();
                    GameObject.Destroy(smoke, 5f);
                    _actor.Active = false;
                    platform.Active = false;
                    collided = true;
                    particleEffect.GetComponent<ParticleSystem>().Play();
                    ActorManager.instance.PlaySound("BombExplosionFinal", 1);
                    foreach (var item in _actor._ControllerState.hasCollisionsWith)
                    {
                        if (item.GetComponent<SuperPlayer>())
                        {
                            item.GetComponent<SuperPlayer>().Die();
                        }
                    }
                    platform.Remove();
                }
            }
        }
    }

    public void shoot(Vector2 velocity)
    {
        vel = velocity;
        beenShot = true;
        platform = Instantiate(movingPlatform, transform.position, Quaternion.identity).GetComponent<SuperActor>();
        if (vel.x < 0)
        {
            float rot;
            rot = smoke.GetComponent<ParticleSystem>().shape.rotation.y;
            rot = 90f;
        }
    }

    void OnBecameInvisible()
    {
        if (beenShot)
        {
            platform.Remove();
            _actor.Remove();
        }
    }
}
