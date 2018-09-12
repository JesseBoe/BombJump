using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cannon : MonoBehaviour {

    public bool active = false;
    public GameObject BigBulletPrefab;
    public GameObject player;
    public Vector2 FireVelocity = new Vector2(200f, 0f);
    private bool facingRight = true;
    private float reloadTime = 2f;
    private float time = 0;

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
            float dist = Mathf.Abs(transform.position.y - player.transform.position.y-8);
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
        }
    }

    private bool canFire()
    {
        if (active)
        {
            if (time > reloadTime)
            {
                return true;
            }
        }
        return false;
    }

    private void fire()
    {
        time = 0;
        GameObject go = Instantiate(BigBulletPrefab, transform.position, Quaternion.identity);
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
