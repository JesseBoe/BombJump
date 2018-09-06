using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerCamera : MonoBehaviour {

    public SuperActor player;

    public float Floor;
    public float Roof;
    public float LeftWall;
    public float RightWall;
    public float Cameraheight;
    public float Camerahorizontal;
    public Vector2 Offset;

    private int scale = 1;
    private Vector2 baseRes = new Vector2(640, 360);
    private Vector3 pos;
    private bool locked = false;
    private bool first = true;

    public CameraType CameraMoveType;

	// Use this for initialization
	void Start ()
    {
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<SuperActor>();
        }
        if (Screen.fullScreen)
        {
            Screen.SetResolution((int)baseRes.x, (int)baseRes.y, FullScreenMode.ExclusiveFullScreen, 120);
        }
        else
        {
            Screen.SetResolution((int)baseRes.x, (int)baseRes.y, FullScreenMode.Windowed, 120);
        }
        setScale(ActorManager.instance.scale);
        transform.position = new Vector2(Camerahorizontal, Cameraheight);
        //setCameraTwo(ActorManager.instance.checkPoint.cameraSettings, player.transform.position);
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
        if (player == null)
        {
            player = GameObject.FindGameObjectWithTag("Player").GetComponent<SuperActor>();
        }

        if (!locked)
        {
            switch (CameraMoveType)
            {
                case CameraType.HorizontalRail:
                    pos = new Vector3(player.transform.position.x, Cameraheight, -100f);
                    pos.x = Mathf.Clamp(pos.x, LeftWall + 320, RightWall - 320);
                    break;
                case CameraType.VerticalRail:
                    pos = new Vector3(Camerahorizontal, player.transform.position.y + Offset.y, -100f);
                    pos.y = Mathf.Clamp(pos.y, Floor + 180, Roof - 180);
                    break;
                case CameraType.Free:
                    pos = new Vector3(player.transform.position.x + Offset.x, player.transform.position.y + Offset.y, -100f);
                    break;
                default:
                    break;
            }

            transform.position = pos;
        }

        if (Input.GetKeyDown(KeyCode.LeftBracket))
        {
            scale--;
            if (scale < 1)
            {
                scale = 1;
            }
            Vector2 newRes = baseRes * scale;
            ActorManager.instance.scale = scale;
            Screen.SetResolution((int)newRes.x, (int)newRes.y, FullScreenMode.Windowed, 120);
        }
        if (Input.GetKeyDown(KeyCode.RightBracket))
        {
            scale++;
            Vector2 newRes = baseRes * scale;
            ActorManager.instance.scale = scale;
            Screen.SetResolution((int)newRes.x, (int)newRes.y, FullScreenMode.Windowed, 120);
        }
        first = false;
    }

    public void setScale(int s)
    {
        scale = s;
        Vector2 newRes = baseRes * scale;
        Screen.SetResolution((int)newRes.x, (int)newRes.y, FullScreenMode.Windowed, 120);
    }

    public void setCameraTwo(CameraControlPoint cam, Vector2 newPosition)
    {
        LeftWall = cam.LeftWall;
        RightWall = cam.RightWall;
        Floor = cam.Floor;
        Roof = cam.Roof;
        Offset = cam.Offset;
        Camerahorizontal = cam.camerahorizontal;
        Cameraheight = cam.cameraheight;
        CameraMoveType = cam.type;
        pos = new Vector3(newPosition.x, newPosition.y, -100f);
        switch (CameraMoveType)
        {
            case CameraType.HorizontalRail:
                pos = new Vector3(player.transform.position.x, Cameraheight, -100f);
                pos.x = Mathf.Clamp(pos.x, LeftWall + 320, RightWall - 320);
                break;
            case CameraType.VerticalRail:
                pos = new Vector3(Camerahorizontal, player.transform.position.y + Offset.y, -100f);
                pos.y = Mathf.Clamp(pos.y, Floor + 180, Roof - 180);
                break;
            case CameraType.Free:
                pos = new Vector3(player.transform.position.x + Offset.x, player.transform.position.y + Offset.y, -100f);
                break;
            default:
                break;
        }
        transform.position = pos;
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

        if (first)
        {
            transform.position = new Vector2(Camerahorizontal, Cameraheight);
        }

        switch (CameraMoveType)
        {
            case CameraType.HorizontalRail:
                pos = new Vector3(player.transform.position.x, Cameraheight, -100f);
                pos.x = Mathf.Clamp(pos.x, LeftWall + 320, RightWall - 320);
                break;
            case CameraType.VerticalRail:
                pos = new Vector3(Camerahorizontal, player.transform.position.y + Offset.y, -100f);
                pos.y = Mathf.Clamp(pos.y, Floor + 180, Roof - 180);
                break;
            case CameraType.Free:
                pos = new Vector3(player.transform.position.x + Offset.x, player.transform.position.y + Offset.y, -100f);
                break;
            default:
                break;
        }

        if (!first)
        {
            locked = true;
            StartCoroutine("LerpToPos");
        }
    }

    public void loadCamera(float leftWall, float rightWall, float floor, float roof, float camerahorizontal, float cameraheight, Vector2 offset, CameraType ctype)
    {
        LeftWall = leftWall;
        RightWall = rightWall;
        Floor = floor;
        Roof = roof;
        Offset = offset;
        Camerahorizontal = camerahorizontal;
        Cameraheight = cameraheight;
        CameraMoveType = ctype;
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
            pos.x = Mathf.Clamp(pos.x, LeftWall + 320, RightWall - 320);
            pos.y = Mathf.Clamp(pos.y, Floor + 180, Roof - 180);
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
