using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class managerSpawner : MonoBehaviour {

    public GameObject actorManagerPrefab;

	// Use this for initialization
	void Start () {
        if (ActorManager.instance == null)
        {
            Instantiate(actorManagerPrefab);
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
