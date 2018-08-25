using UnityEngine;
using System.Collections;

public class Baloon : MonoBehaviour
{
    private SuperActor actor;
    private float timespentwithanobjectstandingontopofmycollider = 0;
    // Use this for initialization
    void Start()
    {
        actor = GetComponent<SuperActor>();
    }

    // Update is called once per frame
    void Update()
    {
        
        if (actor._ControllerState.standingOnMe.Count > 0)
        {
            if (timespentwithanobjectstandingontopofmycollider > .03f)
            {
                actor.DefaultParamters.Gravity = -26f;
            }
            else
            {
                actor.DefaultParamters.Gravity = 0f;
            }
            timespentwithanobjectstandingontopofmycollider += Time.deltaTime;
        }
        else
        {
            timespentwithanobjectstandingontopofmycollider = 0f;
            actor.DefaultParamters.Gravity = 1f;
        }
        
    }
}
