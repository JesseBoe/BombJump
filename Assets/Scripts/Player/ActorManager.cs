using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class ActorManager : MonoBehaviour
{

    public List<SuperActor> Actors = new List<SuperActor>();

    // Use this for initialization
    void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    private void FixedUpdate()
    {
        IEnumerable<SuperActor> query = Actors.OrderBy(h => h.GetColliderHeightCord());
        foreach (var item in query)
        {
            item.Act();
        }
    }
}
