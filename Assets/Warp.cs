using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class Warp : MonoBehaviour {

    public string NextLevel;

	// Use this for initialization
	void Start () {

	}
	
	// Update is called once per frame
	void Update () {
        RaycastHit2D raycastHit = Physics2D.CircleCast(transform.position, 14, Vector2.zero, 200f, LayerMask.GetMask("Player"));

        if (raycastHit)
        {
            ActorManager.instance.ChangeScene(NextLevel);
        }
	}
}
