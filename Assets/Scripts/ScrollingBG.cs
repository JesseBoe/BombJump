using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScrollingBG : MonoBehaviour {

    public float speed = 1f;
    private float loopingY = 390;
    float originYPos;
	// Use this for initialization
	void Start () {
        originYPos = transform.position.y;
	}
	
	// Update is called once per frame
	void Update () {
        if (transform.position.y <= originYPos - loopingY)
        {
            transform.position += new Vector3(0, loopingY, 0f);
        }
	}
    private void FixedUpdate()
    {
        transform.position -= new Vector3(0, speed, 0);
    }
}
