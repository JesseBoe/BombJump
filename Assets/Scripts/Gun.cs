using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Gun : MonoBehaviour
{
    public float FireRate;
    public float WeaponSpread;
    public float BulletSpeed;
    public Vector3 SpawnOffset;
    public GameObject target;
    public AimType TargetType;
    public GameObject BulletPrefab; //Set in the editor.
    public Vector2 Direction;

    private float timePassed;
    public enum AimType
    {
        Direction,
        Player,
        Target
    }

	// Use this for initialization
	void Start () {
        timePassed = 0;

        //Just incase a target isnt set, we set it to player.
        if (target == null)
        {
            target = GameObject.FindGameObjectWithTag("Player");
        }
	}
	
	// Update is called once per frame
	void Update () {
        timePassed += Time.deltaTime;
	}

    private void FixedUpdate()
    {
        if (timePassed >= FireRate)
        {
            //keeps firerate in check
            timePassed = 0;

            //gets velocity based on target type
            Vector2 vel = new Vector2();
            switch (TargetType)
            {
                case AimType.Direction:
                    vel = GetBulletVelocity(Direction);
                    break;
                case AimType.Player:
                    vel = GetBulletVelocity(new Vector2(((transform.position.x + SpawnOffset.x) - target.transform.position.x - target.GetComponent<SuperActor>()._Collider.offset.x), ((transform.position.y + SpawnOffset.y) - target.transform.position.y - target.GetComponent<SuperActor>()._Collider.offset.y)) * -1);
                    break;
                case AimType.Target:
                    vel = GetBulletVelocity(new Vector2(((transform.position.x + SpawnOffset.x) - target.transform.position.x), ((transform.position.y + SpawnOffset.y) - target.transform.position.y)) * -1);
                    break;
                default:
                    vel = GetBulletVelocity(Vector2.down);
                    break;
            }

            //Shoot bullet with the velocity we got above
            Instantiate(BulletPrefab, transform.position + SpawnOffset, Quaternion.identity).GetComponent<Bullet>().shoot(vel); // You can modify the bullet class to get bullets that curve or maybe speed up as they go along
        }
    }

    private Vector2 GetBulletVelocity(Vector2 aim)
    {
        float degree = Angle(aim);
        degree += Random.Range(-WeaponSpread, WeaponSpread);
        Vector2 dir = (Vector2)(Quaternion.Euler(0, 0, degree) * Vector2.up);
        dir *= BulletSpeed;
        dir.x *= -1f;
        return dir;
    }

    public static float Angle(Vector2 p_vector2)
    {
        if (p_vector2.x < 0)
        {
            return 360 - (Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg * -1);
        }
        else
        {
            return Mathf.Atan2(p_vector2.x, p_vector2.y) * Mathf.Rad2Deg;
        }
    }
}
