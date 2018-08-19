using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BackgroundScroll : MonoBehaviour {

    Vector3 originalpos;
    int i;

	// Use this for initialization
	void Start () {
        originalpos = transform.position;
	}
	
	// Update is called once per frame
	void Update () {
		
	}
    private void FixedUpdate()
    {
        i++;
        if (i % 2 == 0)
        {
            transform.Translate(-1f, 1f, 0f);
        }
        if (i >= 128)
        {
            transform.Translate(64f, -64f, 0f);
            i = 0;
        }
    }
}
