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
    public GameObject DialogBox;

    public GameObject BombPrefab;
    public GameObject StarPrefab;
    public GameObject DeathPrefab;

    private bool firstFrame = true;
    private float jumpIn;
    private float dashIn;
    private float dashTime;
    private int normalizedHorizontal;
    private Animator animator;
    private holdObjectType holdType;
    private float timeInAir;
    private bool Jumping;
    private float timeJump;
    private float timeOnStar;
    private DialogManager dm;
    private bool wasGrounded;
    private bool collideUpPlaying = false;
    private float timeholdingobject = 0f;
    private DpadButtons DpadButtons;

    private enum holdObjectType
    {
        Nothing, Bomb, Star
    }

	// Use this for initialization
	void Start () {
        DpadButtons = new DpadButtons();
        ActorManager.instance.resettingScene = false; //ZZZ
        if (firstFrame)
        {
            if (ActorManager.instance.checkPoint.checkPointNumber == -1)
            {
                ActorManager.instance.checkPoint.checkPointNumber = 0;
                ActorManager.instance.checkPoint.playerSpawnPosition = transform.position;
            }

            transform.position = ActorManager.instance.checkPoint.playerSpawnPosition;
            firstFrame = false;
        }
        holdType = holdObjectType.Nothing;
        jumpIn = CactiParameters.JumpFrequency;
        Jumping = false;
        timeJump = 0;
        dm = FindObjectOfType<DialogManager>();
        DialogBox = GameObject.FindGameObjectWithTag("panel");

    }

    private void FixedUpdate()
    {
        if (Jumping)
        {
            timeJump += Time.deltaTime;
            player.SetVerticalVelocity((Mathf.Lerp(250, 350, timeJump * 5)) * Time.deltaTime * 47f);
        }
    }

    public void Die()
    {
        if (State != CactimanParameters.PlayerState.Dead)
        {
            State = CactimanParameters.PlayerState.Dead;
            ActorManager.instance.PlaySound("Hit_Hurt", 1f);
            ActorManager.instance.PlaySoundDeley("Hit_Hurt", .8f, .15f);
            ActorManager.instance.PlaySoundDeley("PlayerDeath", .8f, .22f);
            Instantiate(DeathPrefab, transform.position + new Vector3(16f, 16f, -1f), Quaternion.identity);
        }
    }

    // Update is called once per frame
    void Update()
    {
        DpadButtons.GetDpad();
        if (player._ControllerState.IsCollidingUp && !collideUpPlaying)
        {
            ActorManager.instance.PlaySound("Land", 1f);
            collideUpPlaying = true;
        }
        if (player._ControllerState.IsGrounded && !wasGrounded)
        {
            collideUpPlaying = false;
            if (CactiParameters.JumpRestrictions == CactimanParameters.JumpBehavior.CanJumpAnywhereOnce)
            {
                CactiParameters.JumpRestrictions = CactimanParameters.JumpBehavior.CanJumpOnGround;
            }
            ActorManager.instance.PlaySound("Land", 1f);
        }
        if (!player._ControllerState.isStarRiding)
        {
            timeOnStar = 0f;
        }
        else
        {
            timeOnStar += Time.deltaTime;
        }
        if (State == CactimanParameters.PlayerState.Dead)
        {
            GO_PlayerSprite.GetComponent<Animator>().Play("Die");
            gameObject.layer = LayerMask.NameToLayer("Intangible");
            player.Active = false;

            switch (holdType)
            {
                case holdObjectType.Nothing:
                    break;
                case holdObjectType.Bomb:
                    heldObject.GetComponent<Bomb>().removeBomb();
                    break;
                case holdObjectType.Star:
                    heldObject.GetComponent<SuperActor>().Remove();
                    break;
                default:
                    break;
            }
            heldObject = null;
            holdType = holdObjectType.Nothing;
        }
        else if (State == CactimanParameters.PlayerState.InDialog)
        {
            getDialogInput();
            checkSpikes();
        }
        else
        {
            GetInput();
            checkSpikes();
        }
        wasGrounded = player._ControllerState.IsCollidingDown;
    }

    private void checkSpikes()
    {
        RaycastHit2D[] hits = Physics2D.BoxCastAll((Vector2)transform.position + player._Collider.offset, player._Collider.size, 0, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Spike", "Bullet"));
        if (hits.Length > 0)
        {
            if (State != CactimanParameters.PlayerState.Dead)
            {
                State = CactimanParameters.PlayerState.Dead;
                ActorManager.instance.PlaySound("Hit_Hurt", 1f);
                ActorManager.instance.PlaySoundDeley("Hit_Hurt", .8f, .15f);
                ActorManager.instance.PlaySoundDeley("PlayerDeath", .8f, .22f);
                Instantiate(DeathPrefab, transform.position + new Vector3(16f, 16f, -1f), Quaternion.identity);
            }
        }
    }

    private void getDialogInput()
    {
        //Input is taken in the dialog manager gameobject
        normalizedHorizontal = 0;
        player.SetHorizontalVeloicty(0);
        GO_PlayerSprite.GetComponent<Animator>().Play("Idle");
        if (!dm.InDialog)
        {
            State = CactimanParameters.PlayerState.FullControll;
        }
    }

    private void GetInput()
    {
        if (player._ControllerState.IsGrounded)
        {
            timeInAir = 0f;
        }
        else
        {
            timeInAir += Time.deltaTime;
        }
        bool hasjumped = false;
        jumpIn -= Time.deltaTime;

        if (player._ControllerState.IsGrounded && heldObject == null)
        {
            if (Input.GetKeyDown(KeyCode.UpArrow) || DpadButtons.firstUp)
            {
                RaycastHit2D[] hits = Physics2D.BoxCastAll((Vector2)transform.position + player._Collider.offset, player._Collider.size, 0, Vector2.zero, Mathf.Infinity);
                if (hits.Length > 0)
                {
                    bool enterDialog = false;
                    foreach (var item in hits)
                    {
                        if (item.transform.gameObject.GetComponent<PowerPickUp>())
                        {
                            item.transform.gameObject.GetComponent<PowerPickUp>().pickedUp();
                        }
                        if (item.transform.gameObject.GetComponent<DialogTrigger>())
                        {
                            enterDialog = true;
                            State = CactimanParameters.PlayerState.InDialog;
                            DialogBox.transform.parent.transform.position = new Vector2(item.transform.position.x + 65, item.transform.position.y - 380);
                            DialogBox.GetComponent<Animator>().Play("DialogBoxAnim");
                            item.transform.gameObject.GetComponent<DialogTrigger>().TriggerDialogue();
                            break;
                        }
                    }
                    if (enterDialog)
                    {
                        return;
                    }
                }
            }
        }


        if (Input.GetKeyDown(KeyCode.Space) || Input.GetButtonDown("AButton") && canJump())
        {
            //player.SetVerticalVelocity(CactiParameters.JumpMagnitude);
            player.Parameters.StarSnap = false;
            //player.addVerticalVelocity(CactiParameters.JumpMagnitude);
            jumpIn = CactiParameters.JumpFrequency;
            player.Parameters.IgnorePlatforms = true;
            hasjumped = true;
            Jumping = true;
            timeJump = 0;
            ActorManager.instance.PlaySound("JumpFinal", .8f);

        }

        if (Jumping)
        {
            if (player._ControllerState.IsCollidingUp)
            {
                player.Parameters.StarSnap = true;
                player.Parameters.IgnorePlatforms = false;
                Jumping = false;
            }
            if (Input.GetKey(KeyCode.Space) || Input.GetButton("AButton"))
            {
                
            }
            else
            {
                player.Parameters.StarSnap = true;
                player.Parameters.IgnorePlatforms = false;
                Jumping = false;
            }
            if (player._ControllerState.IsCollidingDown && timeJump > .01f)
            {
                player.Parameters.StarSnap = true;
                player.Parameters.IgnorePlatforms = false;
                Jumping = false;
            }
            if (timeJump > .21f)
            {
                player.Parameters.StarSnap = true;
                player.Parameters.IgnorePlatforms = false;
                Jumping = false;
            }
        }

        normalizedHorizontal = 0;
        if (Input.GetKey(KeyCode.LeftArrow) || DpadButtons.left)
        {
            normalizedHorizontal += -1;
            //If you are dashing and grounded you cant change direction. Dash will change your direction if you are in air
            if (State == CactimanParameters.PlayerState.FullControll)
                player._ControllerState.IsFacingRight = false;
        }
        if (Input.GetKey(KeyCode.RightArrow) || DpadButtons.right)
        {
            normalizedHorizontal += 1;
            //If you are dashing and grounded you cant change direction. Dash will change your direction if you are in air
            if (State == CactimanParameters.PlayerState.FullControll)
                player._ControllerState.IsFacingRight = true;
        }

        if (timeOnStar != 0 && timeOnStar <= .15f)
        {
            normalizedHorizontal = 0;
        }

        if (Input.GetKeyDown(KeyCode.S) || Input.GetButton("XButton") && ActorManager.instance.hasStar && holdType == holdObjectType.Nothing)
        {
            holdType = holdObjectType.Star;
            Vector3 pos = new Vector3(transform.position.x - 4, transform.position.y + 9f, transform.position.z + 1f);
            heldObject = Instantiate(StarPrefab, pos, Quaternion.identity).GetComponentInChildren<SuperActor>();
        }
        if (Input.GetKeyDown(KeyCode.D) || Input.GetButtonDown("YButton") && ActorManager.instance.hasBomb && holdType == holdObjectType.Nothing)
        {
            holdType = holdObjectType.Bomb;
            Vector3 pos = new Vector3(transform.position.x - 4, transform.position.y + 9f, transform.position.z + 1f);
            heldObject = Instantiate(BombPrefab, pos, Quaternion.identity).GetComponentInChildren<SuperActor>();
        }

        if (hasjumped)
        {
            GO_PlayerSprite.GetComponent<Animator>().Play("Jump");
        }
        else if (normalizedHorizontal != 0 && State == CactimanParameters.PlayerState.FullControll && player._ControllerState.IsGrounded && jumpIn != CactiParameters.JumpFrequency && jumpIn <= CactiParameters.JumpFrequency - .2f)
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
        if (holdType != holdObjectType.Nothing && heldObject != null)
        {
            bool buttonReleased = false;

            switch (holdType)
            {
                case holdObjectType.Nothing:
                    //This never happens
                    break;

                case holdObjectType.Bomb:

                    heldObject.transform.position = new Vector3(transform.position.x - 20, transform.position.y - 2, transform.position.z + 1);
                    if (Input.GetKey(KeyCode.D) || Input.GetButton("YButton"))
                    {

                    }
                    else
                    {
                        buttonReleased = true;
                    }
                    break;

                case holdObjectType.Star:

                    heldObject.transform.parent.position = new Vector3(transform.position.x - 4, transform.position.y + 9f, transform.position.z + 1);
                    if (Input.GetKey(KeyCode.S) || Input.GetButton("XButton"))
                    {

                    }
                    else
                    {
                        buttonReleased = true;
                    }
                    break;

                default:
                    break;
            }

            if (!buttonReleased)
            {
                timeholdingobject += Time.deltaTime;
            }
            else
            {
                timeholdingobject = 0f;
            }

            if (heldObject.GetComponent<Throwable>().DoneSpawning && buttonReleased)
            {
                //Throwing
                Vector2 throwVelocity = new Vector2();

                if (player._ControllerState.IsFacingRight)
                {
                    if (Input.GetKey(KeyCode.UpArrow) || DpadButtons.up)
                    {
                        throwVelocity = new Vector2(150, 400);
                    }
                    else if (Input.GetKey(KeyCode.DownArrow) || DpadButtons.down)
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
                    if (Input.GetKey(KeyCode.UpArrow) || DpadButtons.up)
                    {
                        throwVelocity = new Vector2(-150, 400);
                    }
                    else if (Input.GetKey(KeyCode.DownArrow) || DpadButtons.down)
                    {
                        throwVelocity = new Vector2(-150, -400);
                    }
                    else
                    {
                        throwVelocity = new Vector2(-250, 200);
                    }
                }

                heldObject.GetComponentInChildren<Throwable>().Throw(throwVelocity);
                holdType = holdObjectType.Nothing;
                heldObject = null;
            }
            else if (buttonReleased)
            {
                //LetGoTooEarly
                switch (holdType)
                {
                    case holdObjectType.Nothing:
                        break;
                    case holdObjectType.Bomb:
                        heldObject.GetComponent<Bomb>().removeBomb();
                        break;
                    case holdObjectType.Star:
                        heldObject.GetComponent<Star>().removeStar();
                        break;
                    default:
                        break;
                }
                heldObject = null;
                holdType = holdObjectType.Nothing;
            }
        }
        else if (heldObject == null)
        {
            holdType = holdObjectType.Nothing;
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
            else if (CactiParameters.JumpRestrictions == CactimanParameters.JumpBehavior.CanJumpAnywhereOnce)
            {
                CactiParameters.JumpRestrictions = CactimanParameters.JumpBehavior.CanJumpOnGround;
                return true;
            }
            else if (timeInAir < .05f && CactiParameters.JumpRestrictions == CactimanParameters.JumpBehavior.CanJumpOnGround)
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
