using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class JumpRefresher : MonoBehaviour {

    private bool activated = false;
    public float respawnTime = 5f;
    private float timeSpentActivated = 0f;
    private SpriteRenderer dot;

    public SpriteRenderer glow;
    public ParticleSystem ps;

	// Use this for initialization
	void Start () {
        dot = GetComponent<SpriteRenderer>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (activated)
        {
            timeSpentActivated += Time.deltaTime;
        }
	}

    private void FixedUpdate()
    {
        if (!activated)
        {
            var ray = Physics2D.CircleCast(transform.position, 8, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Player"));

            if (ray)
            {
                ray.transform.GetComponent<SuperPlayer>().CactiParameters.JumpRestrictions = CactimanParameters.JumpBehavior.CanJumpAnywhereOnce;
                activated = true;
                glow.enabled = false;
                dot.enabled = false;
                ps.Stop();
                ActorManager.instance.PlaySound("PoweringUp", 1f);
            }
        }
        else
        {
            if (timeSpentActivated >= respawnTime)
            {
                timeSpentActivated = 0f;
                activated = false;
                glow.enabled = true;
                dot.enabled = true;
                ps.Play();
            }
        }
    }
}
