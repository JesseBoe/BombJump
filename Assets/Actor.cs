using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Actor : MonoBehaviour
{
    /*
    public ActorParameters DefaultParameters; //This is set up in the unity editor
    public ControllerState2D _ControllerState;
    public GameObject StandingOn { get; set; }
    public Vector2 Velocity { get { return _velocity; } }

    public ActorParameters Parameters { get { return _overrideParameters ?? DefaultParameters; } }


    protected bool noCollide;
    protected bool handleCollisions;
    protected Transform _transform;
    protected Vector2 _velocity;
    protected BoxCollider2D _boxCollider;
    protected bool pause;
    protected List<RaycastHit2D> hitList = new List<RaycastHit2D>(16);
    protected Vector3 truePosition;
    protected ActorParameters _overrideParameters; //This overrides the default parameters and can be changed mid gameplay
    protected Vector2 knockBackVelocity;
    private List<CollisionDetails> collisionDetails = new List<CollisionDetails>(16);

    protected Vector2 _cornerBottomLeft, _cornerBottomRight, _cornerTopLeft;
    protected float _verticalDistanceBetweenRays, _horizontalDistanceBetweenRays;

    protected const float skinWidth = 2f;
    protected const int verticalRays = 6;
    protected const int horizontalRays = 6;

    protected virtual void Awake()
    {
        _ControllerState = new ControllerState2D();
        noCollide = true;
        handleCollisions = true;
        _transform = gameObject.transform;
        truePosition = transform.position;
        _velocity = Vector2.zero;
        _boxCollider = gameObject.GetComponent<BoxCollider2D>();
        pause = false;
        knockBackVelocity = Vector2.zero;

        _horizontalDistanceBetweenRays = (_boxCollider.size.x - skinWidth * 2) / (verticalRays - 1);
        _verticalDistanceBetweenRays = (_boxCollider.size.y - skinWidth * 2) / (horizontalRays - 1);
    }

    private void FixedUpdate()
    {
        transform.position = truePosition;
        _velocity.y += Parameters.Gravity;
        _velocity += knockBackVelocity;
        handleMovement(_velocity * Time.deltaTime);
        truePosition = transform.position;
        transform.position = new Vector3(Mathf.Round(transform.position.x), Mathf.Round(transform.position.y), transform.position.z);
    }

    protected virtual void handleMovement(Vector2 deltaMovement)
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

            if (Parameters.FreezeX)
                deltaMovement.x = 0;

            _transform.Translate(new Vector2(deltaMovement.x, 0f));
            calculateRayOrigins();

            moveVertically(ref deltaMovement);
        }
        else
        {
            hitList.Clear();
        }

        if (Parameters.FreezeY)
            deltaMovement.y = 0;

        if (Time.deltaTime > 0)
            _velocity = deltaMovement / Time.deltaTime;

        handleKnockBack();

        _transform.Translate(new Vector2(0, deltaMovement.y), Space.World);
    }
    
    protected virtual void moveHorizontally(ref Vector2 deltaMovement)
    {
        bool isGoingRight = deltaMovement.x > 0;
        var rayDistance = Mathf.Abs(deltaMovement.x) + skinWidth;
        var rayDirection = isGoingRight ? Vector2.right : Vector2.left;
        var rayOrigin = isGoingRight ? _cornerBottomRight : _cornerBottomLeft;
        var collidingWithDistance = float.MaxValue;
        List<CollisionDetails> cd = new List<CollisionDetails>(horizontalRays);
        GameObject collideWith = gameObject;
        bool badprogramming = true;

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

            if (badprogramming)
            {
                cd.Add(new CollisionDetails(raycastHit.transform.gameObject, raycastHit.distance - skinWidth, pushable));
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

        foreach (CollisionDetails item in cd)
        {
            if (item.Movable)
            {
                float fullmove = (Mathf.Abs(rayDistance) - skinWidth) * rayDirection.x;
                deltaMovement.x = item.DistanceToHit * rayDirection.x + (item.Go.GetComponent<Actor>().translateHorizontal(fullmove - item.DistanceToHit * rayDirection.x));
            }
        }
    }

    protected virtual void moveVertically(ref Vector2 deltaMovement)
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

    protected void calculateRayOrigins()
    {
        var center = new Vector2(_boxCollider.offset.x, _boxCollider.offset.y);

        _cornerTopLeft = _transform.position + new Vector3(center.x - _boxCollider.size.x / 2 + skinWidth, center.y + _boxCollider.size.y / 2 - skinWidth);
        _cornerBottomRight = _transform.position + new Vector3(center.x + _boxCollider.size.x / 2 - skinWidth, center.y - _boxCollider.size.y / 2 + skinWidth);
        _cornerBottomLeft = transform.position + new Vector3(center.x - _boxCollider.size.x / 2 + skinWidth, center.y - _boxCollider.size.y / 2 + skinWidth);
    }

    public void AddHorizontalVelocity(float vel)
    {
        _velocity.x += vel;
    }
    public void AddVerticalVelocity(float vel)
    {
        _velocity.y += vel;
    }
    public void SetHorizontalVelocity(float vel)
    {
        _velocity.x = vel;
    }
    public void SetVerticalVelocity(float vel)
    {
        _velocity.y = vel;
    }

    public float translateHorizontal(float distance)
    {
        calculateRayOrigins();
        float moveBy = distance;
        bool isGoingRight = moveBy > 0;
        var rayDistance = Mathf.Abs(moveBy) + skinWidth;
        var rayDirection = isGoingRight ? Vector2.right : Vector2.left;
        var rayOrigin = isGoingRight ? _cornerBottomRight : _cornerBottomLeft;
        var collidingWithDistance = float.MaxValue;
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
        }

        foreach (RaycastHit2D ray in hitList)
        {
            var horizontalDistanceToHit = Mathf.Abs(ray.distance) - skinWidth;

            if (horizontalDistanceToHit < collidingWithDistance)
            {
                moveBy = horizontalDistanceToHit * rayDirection.x;
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

        if (collideWith.GetComponent<Actor>() && collideWith.GetComponent<Actor>().Parameters.Pushable && gameObject != collideWith)
        {
            float push = distance - (collidingWithDistance * rayDirection.x);
            moveBy = moveBy + collideWith.GetComponent<Actor>().translateHorizontal(distance - moveBy);
            _transform.Translate(moveBy, 0f, 0f);
            adjustTruePos();
            return (moveBy);
        }
        else
        {
            _transform.Translate(new Vector3(moveBy, 0f, 0f));
            adjustTruePos();
            return (moveBy);
        }
    }

    public void KnockBack(Vector2 force)
    {
        knockBackVelocity.x += force.x;
        AddVerticalVelocity(force.y);
    }

    protected void handleKnockBack()
    {
        knockBackVelocity.x *= Parameters.KnockBackDamp;
        if (Mathf.Abs(knockBackVelocity.x) <= 50f)
            knockBackVelocity.x = 0;
    }

    protected Vector2 handlePlatforms()
    {
        if (Parameters.IgnorePlatforms)
        {
            return Vector2.zero;
        }

        if (StandingOn != null)
        {
            if (StandingOn.GetComponent<Actor>())
            {
                
                Vector2 vel = StandingOn.GetComponent<Actor>().Velocity * Time.deltaTime;
                if (Parameters.FreezeX)
                    vel.x = 0;
                if (Parameters.FreezeY)
                    vel.y = 0;

                return vel;
            }
        }

        return Vector2.zero;
    }

    protected void adjustTruePos()
    {
        truePosition = _transform.position;
    }
    */
}
