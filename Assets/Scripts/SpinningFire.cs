using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpinningFire : MonoBehaviour {

    public GameObject FirePrefab;
    public List<Transform> pos = new List<Transform>();
    public List<GameObject> fire = new List<GameObject>();
	// Use this for initialization
	void Start () {
        for (int i = 0; i < transform.childCount; i++)
        {
            pos.Add(transform.GetChild(i));
            fire.Add(Instantiate(FirePrefab));
        }
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        transform.Rotate(new Vector3(0, 0, 5f));

        for (int i = 0; i < fire.Count; i++)
        {
            fire[i].transform.position = pos[i].position;
        }
    }
}
