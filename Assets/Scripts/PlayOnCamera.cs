using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayOnCamera : MonoBehaviour {

    void OnBecameVisible()
    {
        GetComponentInChildren<ParticleSystem>().Play();
    }
    void OnBecameInvisible()
    {
        GetComponentInChildren<ParticleSystem>().Stop();
    }
}
