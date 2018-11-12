using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Sign : MonoBehaviour {

    public CheckPoint checkPoint;

    private BoxCollider2D myCollider;
    private bool activated = false;
    private bool arrowInvis = true;
    // Use this for initialization
    void Start()
    {
        myCollider = GetComponent<BoxCollider2D>();
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll((Vector2)transform.position + myCollider.offset, myCollider.size, 0, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Player"));
        if (!activated)
        {
            if (hits.Length > 0)
            {
                activated = true;
                if (ActorManager.instance.checkPoint.checkPointNumber < checkPoint.checkPointNumber)
                {
                    ActorManager.instance.checkPoint = checkPoint;
                }
            }
        }
        if (hits.Length > 0)
        {
            if (arrowInvis)
            {
                GetComponent<Animator>().Play("StandingNearFirst");
                arrowInvis = false;
            }
        }
        else
        {
            GetComponent<Animator>().Play("Sign");
            arrowInvis = true;
        }
    }
}
