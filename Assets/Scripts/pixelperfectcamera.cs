using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class pixelperfectcamera : MonoBehaviour {

    private int screenX;
    private int screenY;
    
    // Use this for initialization
    private void Awake()
    {
        screenX = 640;
        screenY = 360;
        Screen.SetResolution(screenX, screenY, false);
        GetComponent<Camera>().orthographicSize = screenY / 2;
        transform.position = new Vector3(transform.position.x - .1f, transform.position.y - .1f, transform.position.z);
    }

    private void LateUpdate()
    {
        //transform.position = new Vector3(transform.position.x - .1f, transform.position.y - .1f, transform.position.z);
    }
}
