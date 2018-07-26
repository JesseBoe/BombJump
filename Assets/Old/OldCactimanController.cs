using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class OldCactimanController : MonoBehaviour
{
    /*
    public ControllerParameters DefaultParameters; //This is set up in the unity editor
    public bool HandleCollisions;
    public ControllerParameters Parameters { get { return _overrideParameters ?? DefaultParameters; } }
    public GameObject StandingOn { get; private set; }
    public LayerMask layerMask;

    private Transform _transform;
    private Vector2 _velocity;
    private ControllerParameters _overrideParameters; //This overrides the default parameters and can be changed mid gameplay
    private BoxCollider2D _boxCollider;
    private float _jumpIn; // You can only jump after a certain amount of time has passed
    private float _dashIn;
    private float _dashTime;
    private bool pause;
    private List<RaycastHit2D> hitList = new List<RaycastHit2D>(16);
    private Vector3 truPos;

    private Vector2 _cornerBottomLeft, _cornerBottomRight, _cornerTopLeft;
    private float _verticalDistanceBetweenRays, _horizontalDistanceBetweenRays;
    private const float skinWidth = 2f;
    private const int totalHorizontalRays = 6;
    private const int totalVerticalRays = 4;

    public ControllerState2D ControllerState { get; private set; } // Keeps track of various cases such as colliding in a certain direction or moving up slopes
    public Vector2 Velocity { get { return _velocity; } }

    public void Awake()
    {
        StandingOn = null;
        truPos = transform.position;
        HandleCollisions = true;
        ControllerState = new ControllerState2D();
        ControllerState.IsDashing = false;
        _transform = transform;
        _boxCollider = GetComponent<BoxCollider2D>();
        ControllerState.IsCrashing = false;
        pause = false;

        _horizontalDistanceBetweenRays = (_boxCollider.size.x - skinWidth * 2) / (totalVerticalRays - 1);
        _verticalDistanceBetweenRays = (_boxCollider.size.y - skinWidth * 2) / (totalHorizontalRays - 1);
    }

	// Update is called once per frame
	void Update ()
    {
        _jumpIn -= Time.deltaTime;
        if (!ControllerState.IsDashing)
            _dashIn -= Time.deltaTime;
    }

    void FixedUpdate()
    {
        transform.position = truPos;
        _velocity.y += Parameters.Gravity;
        handleMovement(Velocity * Time.deltaTime);
        truPos = transform.position;
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z);
    }

    private void calculateRayOrigins()
    {
        var center = new Vector2(_boxCollider.offset.x, _boxCollider.offset.y);

        _cornerTopLeft = _transform.position + new Vector3(center.x - _boxCollider.size.x /2 + skinWidth, center.y + _boxCollider.size.y /2 - skinWidth);
        _cornerBottomRight = _transform.position + new Vector3(center.x + _boxCollider.size.x /2 - skinWidth, center.y - _boxCollider.size.y /2 + skinWidth);
        _cornerBottomLeft = transform.position + new Vector3(center.x - _boxCollider.size.x /2 + skinWidth, center.y - _boxCollider.size.y /2 + skinWidth);
    }

    public void AddForce(Vector2 force)
    {
        _velocity += force;
    }
    public void SetForce(Vector2 force)
    {
        _velocity = force;
    }
    public void SetHorizontalForce(float x)
    {
        _velocity.x = x;
    }
    public void SetVerticalForce(float y)
    {
        _velocity.y = y;
    }

    public void Jump()
    {
        SetVerticalForce(Parameters.JumpMagnitude);
        _jumpIn = Parameters.JumpFrequency;
    }

    public void Dash(int normalizedHorizontal)
    {
        float horizontalForce = 0;
        _dashTime -= Time.deltaTime;
        if (_dashTime > 0 || !ControllerState.IsGrounded)
        {
            if (ControllerState.IsGrounded)
                horizontalForce = ControllerState.IsFacingRight ? Parameters.DashSpeed : Parameters.DashSpeed * -1;
            else
            {
                if (normalizedHorizontal > 0)
                    ControllerState.IsFacingRight = true;

                else if (normalizedHorizontal < 0)
                    ControllerState.IsFacingRight = false;

                horizontalForce = ControllerState.IsFacingRight ? Parameters.DashSpeed : Parameters.DashSpeed * -1;
            }
            SetHorizontalForce(horizontalForce);
        }
        else
        {
            ControllerState.IsDashing = false;
            _dashIn = Parameters.DashFrequency;
        }
    }

    public bool CanJump()
    {
        if (Parameters.JumpRestrictions == ControllerParameters.JumpBehavior.CantJump)
            return false;

        if (ControllerState.IsCrashing)
            return false;

        if (_jumpIn <= 0)
        {
            if (ControllerState.IsGrounded && Parameters.JumpRestrictions == ControllerParameters.JumpBehavior.CanJumpOnGround)
                return true;
            else if (Parameters.JumpRestrictions == ControllerParameters.JumpBehavior.CanJumpAnywhere)
                return true;
        }
        return false;
    }

    public bool CanDash()
    {
        if (Parameters.DashRestrictions == ControllerParameters.DashBehavior.CantDash || ControllerState.IsDashing)
            return false;

        if (ControllerState.IsCrashing)
            return false;

        if (_dashIn <= 0)
        {
            if (ControllerState.IsGrounded && Parameters.DashRestrictions == ControllerParameters.DashBehavior.CanDashOnGround)
            {
                ControllerState.IsDashing = true;
                _dashTime = Parameters.DashDuration;
                return true;
            }
            if (Parameters.DashRestrictions == ControllerParameters.DashBehavior.CanDashAnywhere)
            {
                ControllerState.IsDashing = true;
                _dashTime = Parameters.DashDuration;
                return true;
            }
        }
        return false;
    }

    //This is the start of everything to do with player physics
    //Handles in this order. 1) Slopes 2)HorizontalMovement 3)VerticalMovement
    private void handleMovement(Vector2 deltaMovement)
    {
        if (pause)
            return;

        var wasGrounded = ControllerState.IsCollidingDown;
        ControllerState.Reset();

        if (HandleCollisions)
        {
            //HandlePlatforms();
            calculateRayOrigins();
            deltaMovement += handlePlatforms();

            if (deltaMovement.y < 0 && wasGrounded)
            {
                //Debug.Log("HandleSlopes?");
                //Somehow its supposed to know its on a slope? Maybe ill figure this out later
                //handleVerticalSlope(ref deltaMovement);
            }

            if (Mathf.Abs(deltaMovement.x) > .02f)
                moveHorizontally(ref deltaMovement);
            else
                deltaMovement.x = 0;

            moveVertically(ref deltaMovement);
        }
        else
        {
            hitList.Clear();
        }
        _transform.Translate(deltaMovement, Space.World);

        if (Time.deltaTime > 0)
        {
            _velocity = deltaMovement / Time.deltaTime;

            if (ControllerState.FlagCrash)
            {
                if (_velocity.y < 0)
                { _velocity = ControllerState.IsCollidingRight ? new Vector2(-300, 200) : new Vector2(300, 200); }
                else
                { _velocity = ControllerState.IsCollidingRight ? new Vector2(-300, 500) : new Vector2(300, 500); }
            }
        }


        _velocity.x = Mathf.Min(_velocity.x, Parameters.MaxVelocity.x);
        _velocity.y = Mathf.Min(_velocity.y, Parameters.MaxVelocity.y);

        //if (ControllerState.IsMovingUpSlope)
            //_velocity.y = 0;
    }

    private void handleVerticalSlope(ref Vector2 deltaMovement)
    {
        //throw new NotImplementedException();
    }

    private void moveHorizontally(ref Vector2 deltaMovement)
    {
        bool isGoingRight = deltaMovement.x > 0;
        var rayDistance = Mathf.Abs(deltaMovement.x);
        var rayDirection = isGoingRight ? Vector2.right : Vector2.left;
        var rayOrigin = isGoingRight ? _cornerBottomRight : _cornerBottomLeft;
        var collidingWithDistance = float.MaxValue;
        GameObject collideWith = gameObject;

        hitList.Clear();

        for (int i = 0; i < totalHorizontalRays; i++)
        {
            var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
            Debug.Log(rayDistance);
            Debug.DrawRay(rayVector, rayDirection * (rayDistance + skinWidth), Color.red);
            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance + skinWidth, layerMask);

            if (!raycastHit)
                continue;

            hitList.Add(raycastHit);
        }

        foreach (RaycastHit2D ray in hitList)
        {
            //Horizontal slope code goes here
            var horizontalDistanceToHit = ray.distance;
            if (horizontalDistanceToHit < collidingWithDistance)
            {
                deltaMovement.x = (Mathf.Abs(ray.distance) * rayDirection.x);
                //deltaMovement.x -= skinWidth;
                if (!isGoingRight)
                {
                    deltaMovement.x += skinWidth;
                }
                else
                {
                    deltaMovement.x -= skinWidth;
                }
                collideWith = ray.collider.gameObject;
            }

            if (isGoingRight)
            {
                ControllerState.IsCollidingRight = true;
                SetHorizontalForce(0);
            }
            else
            {
                ControllerState.IsCollidingLeft = true;
                SetHorizontalForce(0);
            }
        }

        if (ControllerState.IsDashing && ControllerState.IsCollidingLeft || ControllerState.IsDashing && ControllerState.IsCollidingRight)
        {
            dashCollide(collideWith);
        }

        /*
        if (ControllerState.IsCollidingLeft || ControllerState.IsCollidingRight)
        {
            if (collideWith.GetComponent<CactimanController>() && collideWith.GetComponent<CactimanController>().Parameters.Pushable)
            {
                float distToHit = deltaMovement.x;
                deltaMovement.x = rayDirection.x * rayDistance;
                collideWith.GetComponent<CactimanController>().MoveWithoutVelocity(new Vector2(deltaMovement.x - distToHit, 0f));
            }
        }
        
    }

    private void moveVertically(ref Vector2 deltaMovement)
    {
        bool isGoingUp = deltaMovement.y > 0;
        var rayDistance = Mathf.Abs(deltaMovement.y);
        var rayDirection = isGoingUp ? Vector2.up : Vector2.down;
        var rayOrigin = isGoingUp ? _cornerTopLeft : _cornerBottomLeft;
        var standingOnDistance = float.MaxValue;

        hitList.Clear();

        for (int i = 0; i < totalVerticalRays; i++)
        {
            var rayVector = new Vector2(rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);
            Debug.DrawRay(rayVector, rayDirection * (rayDistance + skinWidth), Color.red);
            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance + skinWidth, layerMask);

            if (!raycastHit)
                continue;

            hitList.Add(raycastHit);
        }

        if (hitList.Count == 0)
            StandingOn = null;

        foreach (RaycastHit2D ray in hitList)
        {
            if (!isGoingUp)
            {
                var verticalDistanceToHit = ray.distance;
                if (verticalDistanceToHit < standingOnDistance)
                {
                    standingOnDistance = verticalDistanceToHit;
                    StandingOn = ray.collider.gameObject;
                    deltaMovement.y = (Mathf.Abs(standingOnDistance) * rayDirection.y) + skinWidth;
                }
            }
            if (isGoingUp)
            {
                deltaMovement.y = (Mathf.Abs(ray.distance) * rayDirection.y) - skinWidth;
                ControllerState.IsCollidingUp = true;
            }
            else
            {
                ControllerState.IsCollidingDown = true;
            }
            /*
            if (!isGoingUp && deltaMovement.y > .01f)
            {
                Debug.Log("Moveing up slope?");
                ControllerState.IsMovingUpSlope = true;
            }
            
            if (rayDistance < skinWidth + .0001f)
                break;
        }
    }

    private void dashCollide(GameObject collide)
    {
        ControllerState.IsCrashing = true;
        ControllerState.IsDashing = false;
        ControllerState.FlagCrash = true;
        
        //Here I need to add code to test if collide is an enemy  and push them back
    }

    private Vector2 handlePlatforms()
    {
        if (StandingOn != null)
        {
            if (StandingOn.GetComponent<OldCactimanController>())
            {
                return StandingOn.GetComponent<OldCactimanController>().Velocity * Time.deltaTime;
            }
        }
        return Vector2.zero;
    }
    */
}
