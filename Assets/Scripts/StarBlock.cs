using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StarBlock : MonoBehaviour {

    public Sprite block;
    private BoxCollider2D _collider;
    private bool activated = false;
    private bool BeenHit = false;
    public LayerMask myMask;

	// Use this for initialization
	void Start () {
        _collider =  GetComponent<BoxCollider2D>();
	}
	
	// Update is called once per frame
	void Update ()
    {
        if (!activated)
        {
            if (BeenHit)
            {
                RaycastHit2D rayhit = Physics2D.BoxCast((Vector2)transform.position + _collider.offset, _collider.size, 0, Vector2.zero, Mathf.Infinity, myMask);
                if (!rayhit)
                {
                    gameObject.layer = LayerMask.NameToLayer("Ground");
                    activated = true;
                }
            }
        }
	}

    public void hit()
    {
        BeenHit = true;
        GetComponent<SpriteRenderer>().sprite = block;

        RaycastHit2D rayhit = Physics2D.BoxCast((Vector2)transform.position + _collider.offset, _collider.size, 0, Vector2.zero, Mathf.Infinity, myMask);
        if (!rayhit)
        {
            gameObject.layer = LayerMask.NameToLayer("Ground");
            activated = true;
        }
    }
}
