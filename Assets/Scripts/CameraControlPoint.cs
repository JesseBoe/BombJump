using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlPoint : MonoBehaviour {

    public PlayerCamera camera;
    private bool activated = false;

    public float Floor;
    public float Roof;
    public float LeftWall;
    public float RightWall;
    public float cameraheight;
    public float camerahorizontal;
    public Vector2 Offset;
    public PlayerCamera.CameraType type;

    public bool OnRails;

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
        if (!activated)
        {
            if (Vector2.Distance(camera.transform.position, transform.position) < 140f)
            {
                activated = true;
                camera.setCamera(LeftWall, RightWall, Floor, Roof, camerahorizontal, cameraheight, Offset, type);
            }
        }
	}
}
