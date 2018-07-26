using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CactimanController : Actor
{
    /*
    public CactimanParameters _cactimanParameters;

    private float _jumpIn;
    private float _dashIn;
    private float _dashTime;


    // Use this for initialization
    protected override void Awake()
    {
        base.Awake();
        _jumpIn = _cactimanParameters.JumpFrequency;
        _dashIn = _cactimanParameters.DashFrequency;
        _dashTime = _cactimanParameters.DashDuration;
    }


    void Update ()
    {
        _jumpIn -= Time.deltaTime;
        if (!_ControllerState.IsDashing)
            _dashIn -= Time.deltaTime;
    }

    protected override void handleMovement(Vector2 deltaMovement)
    {
        if (pause)
            return;

        var wasGrounded = _ControllerState.IsCollidingDown;
        _ControllerState.Reset();

        if (handleCollisions)
        {
            calculateRayOrigins();
            transform.Translate(handlePlatforms());
            calculateRayOrigins();

            if (Mathf.Abs(deltaMovement.x) > .02f)
                moveHorizontally(ref deltaMovement);
            else
                deltaMovement.x = 0;

            _transform.Translate(new Vector2(deltaMovement.x, 0f));
            calculateRayOrigins();

            moveVertically(ref deltaMovement);
        }
        else
        {
            hitList.Clear();
        }

        if (Parameters.FreezeX)
            deltaMovement.x = 0;

        if (Parameters.FreezeY)
            deltaMovement.y = 0;

        if (Time.deltaTime > 0)
            _velocity = deltaMovement / Time.deltaTime;

        if (_ControllerState.FlagCrash)
        {
            if (_velocity.y < 0)
            { _velocity = _ControllerState.IsCollidingRight ? new Vector2(-300, 200) : new Vector2(300, 200); }
            else
            { _velocity = _ControllerState.IsCollidingRight ? new Vector2(-300, 500) : new Vector2(300, 500); }
        }

        handleKnockBack();

        _transform.Translate(new Vector2(0, deltaMovement.y), Space.World);
    }

    protected override void moveHorizontally(ref Vector2 deltaMovement)
    {
        bool isGoingRight = deltaMovement.x > 0;
        var rayDistance = Mathf.Abs(deltaMovement.x) + skinWidth;
        var rayDirection = isGoingRight ? Vector2.right : Vector2.left;
        var rayOrigin = isGoingRight ? _cornerBottomRight : _cornerBottomLeft;
        var collidingWithDistance = float.MaxValue;
        List<CollisionDetails> cd = new List<CollisionDetails>(horizontalRays);
        GameObject collideWith = gameObject;

        hitList.Clear();

        for (int i = 0; i < horizontalRays; i++)
        {
            var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenRays));
            Debug.DrawRay(rayVector, rayDistance * rayDirection, Color.red);
            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance, Parameters.layerMask);

            if (!raycastHit)
                continue;

            hitList.Add(raycastHit);
            bool pushable = false;
            if (raycastHit.transform.gameObject.GetComponent<Actor>() && raycastHit.transform.gameObject.GetComponent<Actor>().Parameters.Pushable)
                pushable = true;

            bool badprogramming = true;
            if (badprogramming)
            {
                cd.Add(new CollisionDetails(raycastHit.transform.gameObject, Mathf.Abs(raycastHit.distance) - skinWidth, pushable));
                badprogramming = false;//yeahright
            }
            bool add = false;
            foreach (CollisionDetails item in cd)
            {
                if (item.Go != raycastHit.transform.gameObject)
                {
                    add = true;
                }
            }
            if (add)
            {
                cd.Add(new CollisionDetails(raycastHit.transform.gameObject, raycastHit.distance - skinWidth, pushable));
            }
        }

        foreach (RaycastHit2D ray in hitList)
        {
            var horizontalDistanceToHit = Mathf.Abs(ray.distance) - skinWidth;

            if (horizontalDistanceToHit < collidingWithDistance)
            {
                deltaMovement.x = horizontalDistanceToHit * rayDirection.x;
                if (!isGoingRight)
                {
                    _ControllerState.IsCollidingLeft = true;
                }
                else
                {
                    _ControllerState.IsCollidingRight = true;
                }
                collideWith = ray.collider.gameObject;
                collidingWithDistance = horizontalDistanceToHit;
            }
        }

        if (_ControllerState.IsDashing && _ControllerState.IsCollidingLeft || _ControllerState.IsDashing && _ControllerState.IsCollidingRight)
        {
            dashCollide(collideWith);
        }
        else
        {
            foreach (CollisionDetails item in cd)
            {
                if (item.Movable)
                {
                    float fullmove = (Mathf.Abs(rayDistance) - skinWidth) * rayDirection.x;
                    deltaMovement.x = item.DistanceToHit * rayDirection.x + (item.Go.GetComponent<Actor>().translateHorizontal(fullmove - item.DistanceToHit * rayDirection.x));
                }
            }
        }
    }

    protected override void moveVertically(ref Vector2 deltaMovement)
    {
        bool isGoingUp = deltaMovement.y > 0;
        var rayDistance = Mathf.Abs(deltaMovement.y) + skinWidth;
        var rayDirection = isGoingUp ? Vector2.up : Vector2.down;
        var rayOrigin = isGoingUp ? _cornerTopLeft : _cornerBottomLeft;
        var standingOnDistance = float.MaxValue;

        hitList.Clear();

        for (int i = 0; i < verticalRays; i++)
        {
            var rayVector = new Vector2(rayOrigin.x + (i * _horizontalDistanceBetweenRays), rayOrigin.y);
            Debug.DrawRay(rayVector, rayDistance * rayDirection, Color.red);
            var raycastHit = Physics2D.Raycast(rayVector, rayDirection, rayDistance + skinWidth, Parameters.layerMask);

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
                var verticalDistanceToHit = ray.distance - skinWidth;
                if (verticalDistanceToHit < standingOnDistance)
                {
                    standingOnDistance = verticalDistanceToHit;
                    StandingOn = ray.collider.gameObject;
                    deltaMovement.y = standingOnDistance * rayDirection.y;
                }
            }
            if (isGoingUp)
            {
                var verticalDistanceToHit = ray.distance - skinWidth;
                if (verticalDistanceToHit < standingOnDistance)
                {
                    standingOnDistance = verticalDistanceToHit;
                    _ControllerState.IsCollidingUp = true;
                    deltaMovement.y = (Mathf.Abs(ray.distance) * rayDirection.y) - skinWidth;
                }
            }
            else
            {
                _ControllerState.IsCollidingDown = true;
            }
        }
    }


    public void Jump()
    {
        SetVerticalVelocity(_cactimanParameters.JumpMagnitude);
        _jumpIn = _cactimanParameters.JumpFrequency;
    }

    public void Dash(int normalizedHorizontal)
    {
        float horizontalForce = 0;
        _dashTime -= Time.deltaTime;
        if (_dashTime > 0 || !_ControllerState.IsGrounded)
        {
            if (_ControllerState.IsGrounded)
                horizontalForce = _ControllerState.IsFacingRight ? _cactimanParameters.DashSpeed : _cactimanParameters.DashSpeed * -1;
            else
            {
                if (normalizedHorizontal > 0)
                    _ControllerState.IsFacingRight = true;

                else if (normalizedHorizontal < 0)
                    _ControllerState.IsFacingRight = false;

                horizontalForce = _ControllerState.IsFacingRight ? _cactimanParameters.DashSpeed : _cactimanParameters.DashSpeed * -1;
            }
            SetHorizontalVelocity(horizontalForce);
        }
        else
        {
            _ControllerState.IsDashing = false;
            _dashIn = _cactimanParameters.DashFrequency;
        }
    }

    public bool CanJump()
    {
        if (_cactimanParameters.JumpRestrictions == CactimanParameters.JumpBehavior.CantJump)
            return false;

        if (_ControllerState.IsCrashing)
            return false;

        if (_jumpIn <= 0)
        {
            if (_ControllerState.IsGrounded && _cactimanParameters.JumpRestrictions == CactimanParameters.JumpBehavior.CanJumpOnGround)
                return true;
            else if (_cactimanParameters.JumpRestrictions == CactimanParameters.JumpBehavior.CanJumpAnywhere)
                return true;
        }
        return false;
    }

    public bool CanDash()
    {
        if (_cactimanParameters.DashRestrictions == CactimanParameters.DashBehavior.CantDash || _ControllerState.IsDashing)
            return false;

        if (_ControllerState.IsCrashing)
            return false;

        if (_dashIn <= 0)
        {
            if (_ControllerState.IsGrounded && _cactimanParameters.DashRestrictions == CactimanParameters.DashBehavior.CanDashOnGround)
            {
                _ControllerState.IsDashing = true;
                _dashTime = _cactimanParameters.DashDuration;
                return true;
            }
            if (_cactimanParameters.DashRestrictions == CactimanParameters.DashBehavior.CanDashAnywhere)
            {
                _ControllerState.IsDashing = true;
                _dashTime = _cactimanParameters.DashDuration;
                return true;
            }
        }
        return false;
    }

    private void dashCollide(GameObject collide)
    {
        _ControllerState.IsCrashing = true;
        _ControllerState.IsDashing = false;
        _ControllerState.FlagCrash = true;

        //Here I need to add code to test if collide is an enemy  and push them back
    }
    */
}
