using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class SuperPlayer : MonoBehaviour
{
    public SuperActor player;
    public SuperActor heldObject;
    public GameObject GO_PlayerSprite;
    public CactimanParameters CactiParameters;
    public CactimanParameters.PlayerState State;

    public GameObject BombPrefab;
    public GameObject StarPrefab;

    private float jumpIn;
    private float dashIn;
    private float dashTime;
    private int normalizedHorizontal;
    private Animator animator;
    private bool holdingObject;

	// Use this for initialization
	void Start () {
        jumpIn = CactiParameters.JumpFrequency;
    }
	
	// Update is called once per frame
	void Update()
    {
        bool hasjumped = false;
        jumpIn -= Time.deltaTime;
        if (Input.GetKeyDown(KeyCode.Space) && canJump())
        {
            player.SetVerticalVelocity(CactiParameters.JumpMagnitude);
            jumpIn = CactiParameters.JumpFrequency;
            hasjumped = true;
        }

        normalizedHorizontal = 0;
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            normalizedHorizontal += -1;
            //If you are dashing and grounded you cant change direction. Dash will change your direction if you are in air
            if (State == CactimanParameters.PlayerState.FullControll)
                player._ControllerState.IsFacingRight = false;
        }
        if (Input.GetKey(KeyCode.RightArrow))
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
            Vector3 pos = new Vector3(Input.mousePosition.x - 320, Input.mousePosition.y - 180, -20f);
            Instantiate(BombPrefab, pos, Quaternion.identity);
        }

        if (Input.GetKeyDown(KeyCode.S))
        {
            holdingObject = true;
            Vector3 pos = new Vector3(transform.position.x -4, transform.position.y + 9f, transform.position.z + 1f);
            heldObject = Instantiate(StarPrefab, pos, Quaternion.identity).GetComponentInChildren<SuperActor>();
        }

        if (hasjumped)
        {
            GO_PlayerSprite.GetComponent<Animator>().Play("Jump");
        }
        else if (normalizedHorizontal != 0 && State == CactimanParameters.PlayerState.FullControll && player._ControllerState.IsGrounded && jumpIn != CactiParameters.JumpFrequency && jumpIn <= CactiParameters.JumpFrequency -.2f)
        {
            GO_PlayerSprite.GetComponent<Animator>().Play("Run");
        }
        else if (jumpIn != CactiParameters.JumpFrequency && player._ControllerState.IsGrounded && jumpIn <= CactiParameters.JumpFrequency - .2f)
        {
            GO_PlayerSprite.GetComponent<Animator>().Play("Idle");
        }

        if (player._ControllerState.IsFacingRight)
        {
            GO_PlayerSprite.GetComponent<SpriteRenderer>().flipX = false;
            GO_PlayerSprite.transform.position = transform.position;
        }
        else
        {
            GO_PlayerSprite.GetComponent<SpriteRenderer>().flipX = true;
            GO_PlayerSprite.transform.position = transform.position + new Vector3(24, 0, 0);
        }

        player.SetHorizontalVeloicty(normalizedHorizontal * 150);
        if (holdingObject && heldObject != null)
        {
            if (Input.GetKey(KeyCode.S))
            {
                heldObject.transform.parent.position = new Vector3(transform.position.x - 4, transform.position.y + 9f, transform.position.z + 1);
            }
            else if (heldObject.GetComponent<Star>().DoneSpawn)
            {
                //Throwing
                Vector2 throwVelocity = new Vector2();

                if (player._ControllerState.IsFacingRight)
                {
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        throwVelocity = new Vector2(150, 400);
                    }
                    else if (Input.GetKey(KeyCode.DownArrow))
                    {
                        throwVelocity = new Vector2(150, -400);
                    }
                    else
                    {
                        throwVelocity = new Vector2(250, 200);
                    }
                }
                else
                {
                    if (Input.GetKey(KeyCode.UpArrow))
                    {
                        throwVelocity = new Vector2(-150, 400);
                    }
                    else if (Input.GetKey(KeyCode.DownArrow))
                    {
                        throwVelocity = new Vector2(-150, -400);
                    }
                    else
                    {
                        throwVelocity = new Vector2(-250, 200);
                    }
                }

                heldObject.GetComponentInChildren<Star>().Throw(throwVelocity);
                holdingObject = false;
                heldObject = null;
            }
            else
            {
                //LetGoTooEarly
                GameObject.Destroy(heldObject.gameObject);
                heldObject = null;
                holdingObject = false;
            }
        }
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
