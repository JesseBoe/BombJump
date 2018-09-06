using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraControlPoint : MonoBehaviour {

    public PlayerCamera cam;
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
    void Start ()
    {

	}
	
	// Update is called once per frame
	void Update () {
        if (!activated)
        {
            if (cam.CameraMoveType == PlayerCamera.CameraType.HorizontalRail)
            {
                if (Vector2.Distance(cam.transform.position, transform.position) < 320f)
                {
                    activated = true;
                    cam.setCamera(LeftWall, RightWall, Floor, Roof, camerahorizontal, cameraheight, Offset, type);
                }
            }
            if (cam.CameraMoveType == PlayerCamera.CameraType.VerticalRail)
            {
                if (Vector2.Distance(cam.transform.position, transform.position) < 180f)
                {
                    activated = true;
                    cam.setCamera(LeftWall, RightWall, Floor, Roof, camerahorizontal, cameraheight, Offset, type);
                }
            }
        }
	}
}
