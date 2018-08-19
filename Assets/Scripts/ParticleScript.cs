using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleScript : MonoBehaviour {

    public ParticleSystem mySystem;

	// Use this for initialization
	void Start () {
        mySystem = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        if (!mySystem.IsAlive())
        {
            GameObject.Destroy(gameObject);
        }
    }
}
