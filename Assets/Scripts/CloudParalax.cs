using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CloudParalax : MonoBehaviour {

    public float len;
    public GameObject camera;
    public float scrollSpeed = -1f;
    private float offset = 0f;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update ()
    {
        while (transform.position.x + len < camera.transform.position.x)
        {
            transform.position += new Vector3(len, 0f, 0f);
        }
        while (transform.position.x - len > camera.transform.position.x)
        {
            transform.position -= new Vector3(len, 0f, 0f);
        }
	}

    private void FixedUpdate()
    {
        transform.position -= new Vector3(scrollSpeed, 0f, 0f);
    }
}
