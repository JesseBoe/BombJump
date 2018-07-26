using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SuperPlayer : MonoBehaviour
{
    public SuperActor player;
    public CactimanParameters CactiParameters;
    public CactimanParameters.PlayerState State;

    public GameObject BombPrefab;

    private float jumpIn;
    private float dashIn;
    private float dashTime;
    private int normalizedHorizontal;
    public Sprite[] sprites;

	// Use this for initialization
	void Start () {
        jumpIn = CactiParameters.JumpFrequency;
        player._ControllerState.IsFacingRight = true;

    }
	
	// Update is called once per frame
	void Update()
    {
        jumpIn -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && canJump())
        {
            Debug.Log("JUMPU");
            player.SetVerticalVelocity(CactiParameters.JumpMagnitude);
            jumpIn = CactiParameters.JumpFrequency;
        }

        normalizedHorizontal = 0;
        if (Input.GetKey(KeyCode.A))
        {
            normalizedHorizontal += -1;
            //If you are dashing and grounded you cant change direction. Dash will change your direction if you are in air
            if (State == CactimanParameters.PlayerState.FullControll)
                player._ControllerState.IsFacingRight = false;
        }
        if (Input.GetKey(KeyCode.D))
        {
            normalizedHorizontal += 1;
            //If you are dashing and grounded you cant change direction. Dash will change your direction if you are in air
            if (State == CactimanParameters.PlayerState.FullControll)
                player._ControllerState.IsFacingRight = true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (canDash())
            {
                State = CactimanParameters.PlayerState.Dashing;
                //Do Dashing things
            }
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            EditorApplication.isPaused = true;
        }
        if (Input.GetKeyDown(KeyCode.B))
        {
            Vector3 pos = new Vector3(Input.mousePosition.x - 320, Input.mousePosition.y - 180, 0f);
            Instantiate(BombPrefab, pos, Quaternion.identity);
        }

        if (player._ControllerState.IsFacingRight)
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[1];
        }
        else
        {
            gameObject.GetComponent<SpriteRenderer>().sprite = sprites[0];
        }

        player.SetHorizontalVeloicty(normalizedHorizontal * 400);
    }

    public void Dash(int normalizedHorizontal)
    {
        float horizontalForce = 0;
        dashTime -= Time.deltaTime; //once dashtime is below 0 dash is over
        if (dashTime > 0 || !player._ControllerState.IsGrounded)
        {
            if (player._ControllerState.IsGrounded)
                horizontalForce = player._ControllerState.IsFacingRight ? CactiParameters.DashSpeed : CactiParameters.DashSpeed * -1;
            else
            {
                if (normalizedHorizontal > 0)
                    player._ControllerState.IsFacingRight = true;

                else if (normalizedHorizontal < 0)
                    player._ControllerState.IsFacingRight = false;

                horizontalForce = player._ControllerState.IsFacingRight ? CactiParameters.DashSpeed : CactiParameters.DashSpeed * -1;
            }
            player.SetHorizontalVeloicty(horizontalForce);
        }
        else
        {
            State = CactimanParameters.PlayerState.FullControll;
            dashIn = CactiParameters.DashFrequency;
        }
    }

    private bool canJump()
    {
        if (CactiParameters.JumpRestrictions == CactimanParameters.JumpBehavior.CantJump)
            return false;

        switch (State)
        {
            case CactimanParameters.PlayerState.FullControll:
                break;
            case CactimanParameters.PlayerState.KnockedBack:
                return false;
            case CactimanParameters.PlayerState.Dashing:
                break;
            case CactimanParameters.PlayerState.Crashing:
                return false;
            case CactimanParameters.PlayerState.Dead:
                return false;
            default:
                break;
        }

        if (jumpIn < 0)
        {
            if (CactiParameters.JumpRestrictions == CactimanParameters.JumpBehavior.CanJumpAnywhere)
            {
                return true;
            }
            else if (player._ControllerState.IsGrounded && CactiParameters.JumpRestrictions == CactimanParameters.JumpBehavior.CanJumpOnGround)
            {
                return true;
            }
        }
        return false;
    }

    private bool canDash()
    {
        if (CactiParameters.DashRestrictions == CactimanParameters.DashBehavior.CantDash || State == CactimanParameters.PlayerState.Dashing)
            return false;

        switch (State)
        {
            case CactimanParameters.PlayerState.FullControll:
                break;
            case CactimanParameters.PlayerState.KnockedBack:
                return false;
            case CactimanParameters.PlayerState.Dashing:
                return false;
            case CactimanParameters.PlayerState.Crashing:
                return false;
            case CactimanParameters.PlayerState.Dead:
                return false;
            default:
                break;
        }

        if (dashIn <= 0)
        {
            if (player._ControllerState.IsGrounded && CactiParameters.DashRestrictions == CactimanParameters.DashBehavior.CanDashOnGround)
            {
                State = CactimanParameters.PlayerState.Dashing;
                dashTime = CactiParameters.DashDuration;
                return true;
            }
            if (CactiParameters.DashRestrictions == CactimanParameters.DashBehavior.CanDashAnywhere)
            {
                State = CactimanParameters.PlayerState.Dashing;
                dashTime = CactiParameters.DashDuration;
                return true;
            }
        }
        return false;
    }
}
