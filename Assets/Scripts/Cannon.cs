using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

public class Cannon : MonoBehaviour {

    public bool active = false;
    public GameObject BigBulletPrefab;
    public GameObject player;
    public Vector2 FireVelocity = new Vector2(200f, 0f);
    private bool facingRight = true;
    public float reloadTime = 2f;
    private float time = 2f;
    public FireMode Mode = FireMode.AtPlayer;
    public bool Quieter = false;

    public enum FireMode
    {
        AtPlayer,
        AfterTime
    }

	// Use this for initialization
	void Start ()
    {
        player = GameObject.FindGameObjectWithTag("Player");
        if (GetComponent<SpriteRenderer>().flipX)
        {
            facingRight = false;
        }
    }
	
	// Update is called once per frame
	void Update ()
    {
        time += Time.deltaTime;
	}

    private void FixedUpdate()
    {
        if (canFire())
        {
            switch (Mode)
            {
                case FireMode.AtPlayer:
                    float dist = Mathf.Abs(transform.position.y - player.transform.position.y - 8);
                    if (dist <= 6)
                    {
                        if (facingRight && player.transform.position.x + 8 > transform.position.x)
                        {
                            fire();
                        }
                        if (!facingRight && player.transform.position.x + 8 < transform.position.x)
                        {
                            fire();
                        }
                    }
                    break;
                case FireMode.AfterTime:
                    fire();
                    break;
                default:
                    break;
            }
        }
    }

    private bool canFire()
    {
        if (active)
        {
            if (time > reloadTime)
            {
                Vector2 _corner = facingRight ? new Vector2(transform.position.x + 8, transform.position.y - 7) : new Vector2(transform.position.x - 8, transform.position.y - 7);
                var ray = facingRight ? Physics2D.Raycast(_corner, Vector2.right, 32, LayerMask.GetMask("Ground")) : Physics2D.Raycast(_corner, Vector2.left, 32, LayerMask.GetMask("Ground"));
                if (ray)
                {
                    return false;
                }
                return true;
            }
        }
        return false;
    }

    private void fire()
    {
        time = 0;
        ActorManager.instance.PlaySound("GunFire", Quieter ? 1f : 3f);
        GameObject go = Instantiate(BigBulletPrefab, transform.position, Quaternion.identity);
        if (!facingRight)
        {
            go.GetComponent<SpriteRenderer>().flipX = true;
        }
        Vector2 vel = facingRight ? FireVelocity : -FireVelocity;
        go.GetComponent<BigBullet>().shoot(vel);
    }

    void OnBecameInvisible()
    {
        active = false;
    }

    private void OnBecameVisible()
    {
        active = true;
    }
}
