using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerCamera : MonoBehaviour {

    public SuperActor player;

    public float Floor;
    public float Roof;
    public float LeftWall;
    public float RightWall;
    public float Cameraheight;
    public float Camerahorizontal;
    public Vector2 Offset;


    private Vector3 pos;
    private bool locked = false;

    public CameraType CameraMoveType;

	// Use this for initialization
	void Start ()
    {
		
	}

    public enum CameraType
    {
        HorizontalRail,
        VerticalRail,
        Free
    }
	
	// Update is called once per frame
	void Update ()
    {
        if (!locked)
        {
            switch (CameraMoveType)
            {
                case CameraType.HorizontalRail:
                    pos = new Vector3(player.transform.position.x, Cameraheight, -30f);
                    pos.x = Mathf.Clamp(pos.x, LeftWall + 320, RightWall - 320);
                    break;
                case CameraType.VerticalRail:
                    pos = new Vector3(Camerahorizontal, player.transform.position.y + Offset.y, -30f);
                    pos.y = Mathf.Clamp(pos.y, Floor + 180, Roof - 180);
                    break;
                case CameraType.Free:
                    pos = new Vector3(player.transform.position.x + Offset.x, player.transform.position.y + Offset.y, -30f);
                    break;
                default:
                    break;
            }

            transform.position = pos;
        }
    }

    public void setCamera(float leftWall, float rightWall, float floor, float roof, float camerahorizontal, float cameraheight, Vector2 offset, CameraType ctype)
    {
        LeftWall = leftWall;
        RightWall = rightWall;
        Floor = floor;
        Roof = roof;
        Offset = offset;
        Camerahorizontal = camerahorizontal;
        Cameraheight = cameraheight;
        CameraMoveType = ctype;

        locked = true;
        StartCoroutine("LerpToPos");
    }

    IEnumerator LerpToPos()
    {
        float timeToSnap = .7f;
        float time = 0;
        Vector3 oldPos = transform.position;
        while (time < timeToSnap)
        {
            switch (CameraMoveType)
            {
                case CameraType.HorizontalRail:
                    pos = new Vector3(player.transform.position.x, Cameraheight, -30f);
                    break;
                case CameraType.VerticalRail:
                    pos = new Vector3(Camerahorizontal, player.transform.position.y + Offset.y, -30f);
                    break;
                case CameraType.Free:
                    break;
                default:
                    break;
            }

            time += Time.deltaTime;
            transform.position = Vector3.Lerp(oldPos, pos, time / timeToSnap);
            transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), pos.z);
            yield return new WaitForEndOfFrame();
        }
        transform.position = new Vector3(Mathf.Round(pos.x), Mathf.Round(pos.y), pos.z);
        locked = false;
    }

    private void FixedUpdate()
    {
        
    }
}
