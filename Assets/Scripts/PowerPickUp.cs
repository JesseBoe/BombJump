using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PowerPickUp : MonoBehaviour {

    public Powers power;

    public enum Powers
    {
        Star, Bomb
    }

	// Use this for initialization
	void Start () {
		
	}
	
	// Update is called once per frame
	void Update () {
		
	}

    public void pickedUp()
    {
        switch (power)
        {
            case Powers.Star:
                ActorManager.instance.hasStar = true;
                break;
            case Powers.Bomb:
                ActorManager.instance.hasBomb = true;
                break;
            default:
                break;
        }

        ActorManager.instance.PlaySound("PowerUpGetFinal", 1f);
    }
}
