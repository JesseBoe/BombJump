using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SuperActor : MonoBehaviour {

    public Vector2 Velocity;
    public ControllerState2D _ControllerState;

    public ActorParameters DefaultParamters;
    public ActorParameters Parameters { get { return _overrideParameters ?? DefaultParamters; } }
    public bool Active = true;

    private BoxCollider2D _collider;
    private ActorParameters _overrideParameters;
    private Transform _transform;
    private List<CollisionDetails> collisionDetails = new List<CollisionDetails>(32);
    private RaycastHit2D[] allRayHits = new RaycastHit2D[32];
    private Vector2 _cornerBottomLeft, _cornerTopLeft, _cornerBottomRight;
    private float _verticalDistanceBetweenArrays, _horizontalDistanceBetweenArrays;
    private Vector2 knockBackVelocity;

    private const float skinWidth = 1f;
    private const int verticalRays = 4;
    private const int horizontalRays = 6;

    // Use this for initialization
    void Start ()
    {
        _ControllerState = new ControllerState2D();
        _transform = gameObject.transform;
        Velocity = Vector2.zero;
        _collider = gameObject.GetComponent<BoxCollider2D>();

        _horizontalDistanceBetweenArrays = (_collider.size.x - skinWidth * 2) / (verticalRays - 1);
        _verticalDistanceBetweenArrays = (_collider.size.y - skinWidth * 2) / (horizontalRays - 1);
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void FixedUpdate()
    {
        if (Active)
        {
            addVerticalVelocity(Parameters.Gravity);
            handleKnockBack();
            handleMovement(knockBackVelocity);
            handleMovement(Velocity * Time.deltaTime);
        }
    }

    private void handleMovement(Vector2 deltaMovement)
    {
        _ControllerState.Reset();

        if (Mathf.Abs(deltaMovement.x) > .02f)
            moveHorizontally(ref deltaMovement.x);
        else
            deltaMovement.x = 0f;

        if (Mathf.Abs(deltaMovement.y) > .02f)
            moveVertically(ref deltaMovement.y);
        else
            deltaMovement.y = 0f;
    }

    private void calculateRayOrigins()
    {
        var center = new Vector2(_collider.offset.x, _collider.offset.y);

        _cornerTopLeft = (Vector2)_transform.position + new Vector2(center.x - _collider.size.x / 2 + skinWidth, center.y + _collider.size.y / 2 - skinWidth);
        _cornerBottomLeft = (Vector2)_transform.position + new Vector2(center.x - _collider.size.x / 2 + skinWidth, center.y - _collider.size.y / 2 + skinWidth);
        _cornerBottomRight = (Vector2)_transform.position + new Vector2(center.x + _collider.size.x / 2 - skinWidth, center.y - _collider.size.y / 2 + skinWidth);
    }

    public void SetVelocity(Vector2 vel)
    {
        Velocity = vel;
    }

    public void addVerticalVelocity(float vel)
    {
        Velocity.y += vel;
    }

    public void SetVerticalVelocity(float vel)
    {
        Velocity.y = vel;
    }

    public void SetHorizontalVeloicty(float vel)
    {
        Velocity.x = vel;
    }

    public void KnockBack(Vector2 force)
    {
        knockBackVelocity.x += force.x;
        addVerticalVelocity(force.y);
    }

    protected void handleKnockBack()
    {
        knockBackVelocity.x *= Parameters.KnockBackDamp;
        if (Mathf.Abs(knockBackVelocity.x) <= 3f)
        {
            knockBackVelocity.x = 0;
        }
    }

    private void moveHorizontally(ref float deltaMovementX)
    {
        if (!Active)
        {
            return;
        }
        calculateRayOrigins();
        bool isGoingRight = deltaMovementX > 0;
        var rayDistance = Mathf.Abs(deltaMovementX) + skinWidth;
        var rayDirection = isGoingRight ? Vector2.right : Vector2.left;
        var rayOrigin = isGoingRight ? _cornerBottomRight : _cornerBottomLeft;

        collisionDetails.Clear();

        for (int i = 0; i < horizontalRays; i++)
        {
            var rayVector = new Vector2(rayOrigin.x, rayOrigin.y + (i * _verticalDistanceBetweenArrays));
            Debug.DrawRay(rayVector, rayDistance * rayDirection, Color.red);
            allRayHits = Physics2D.RaycastAll(rayVector, rayDirection, rayDistance, Parameters.layerMask);

            if (allRayHits.Length == 0)
                continue;

            foreach (var raycastHit in allRayHits)
            {
                if (raycastHit.transform.gameObject == gameObject)
                {
                    //ignored
                }
                else
                {
                    bool pushable = false;
                    if (raycastHit.transform.GetComponent<SuperActor>())
                    {
                        pushable = raycastHit.transform.GetComponent<SuperActor>().Parameters.Pushable;
                    }
                    collisionDetails.Add(new CollisionDetails(raycastHit.transform.gameObject, raycastHit.distance - skinWidth, pushable));
                }
            }
        }

        IEnumerable<CollisionDetails> ordered = collisionDetails.OrderBy(h => h.DistanceToHit);
        float max = rayDistance + skinWidth + .01f;
        foreach (CollisionDetails item in ordered)
        {
            if (item.DistanceToHit >= max)
            {
                continue;
            }
            if (isGoingRight)
            {
                _ControllerState.IsCollidingRight = true;
            }
            else
            {
                _ControllerState.IsCollidingLeft = true;
            }
            if (!item.Movable)
            {
                if (item.DistanceToHit <= Mathf.Abs(deltaMovementX))
                {
                    deltaMovementX = item.DistanceToHit * rayDirection.x;
                    max = Mathf.Abs(deltaMovementX);
                }
            }
            else
            {
                if (item.Movable && !item.Go.GetComponent<SuperActor>()._ControllerState.DisablePush)
                {
                    if (item.Go.GetComponent<SuperActor>()._ControllerState.timePushed > 50)
                    {
                        Debug.Log("YOOOO");
                        item.Go.GetComponent<SuperActor>().Active = false;
                        GameObject.Destroy(item.Go);
                        continue;
                    }

                    else if (max > item.DistanceToHit && !item.Go.GetComponent<SuperActor>()._ControllerState.pushedBy.Contains(gameObject))
                    {
                        item.Go.GetComponent<SuperActor>()._ControllerState.DisablePush = true;
                        item.Go.GetComponent<SuperActor>()._ControllerState.timePushed += 1;
                        item.Go.GetComponent<SuperActor>()._ControllerState.pushedBy.Add(gameObject);
                        float fullmove = (Mathf.Abs(rayDistance) - skinWidth);
                        float amountToMoveItem = (fullmove - item.DistanceToHit) * rayDirection.x;
                        item.Go.GetComponent<SuperActor>().moveHorizontally(ref amountToMoveItem);
                        deltaMovementX = item.DistanceToHit * rayDirection.x + amountToMoveItem;
                        max = Mathf.Abs(deltaMovementX);
                    }
                }
            }
        }

        foreach (CollisionDetails item in ordered)
        {
            if (item.Go.GetComponent<SuperActor>())
            {
                item.Go.GetComponent<SuperActor>()._ControllerState.DisablePush = false;
            }
        }
        _ControllerState.pushedBy.Clear();
        _transform.Translate(deltaMovementX, 0, 0, Space.World);
        //_transform.position = new Vector3(transform.position.x, Mathf.Round(transform.position.y), transform.position.z);
        calculateRayOrigins();
    }

    private void moveVertically(ref float deltaMovementY)
    {
        if (!Active)
        {
            return;
        }
        calculateRayOrigins();
        bool isGoingUp = deltaMovementY > 0;
        var rayDistance = Mathf.Abs(deltaMovementY) + skinWidth;
        var rayDirection = isGoingUp ? Vector2.up : Vector2.down;
        var rayOrigin = isGoingUp ? _cornerTopLeft : _cornerBottomLeft;

        collisionDetails.Clear();

        for (int i = 0; i < verticalRays; i++)
        {
            var rayVector = new Vector2(rayOrigin.x + (i * _horizontalDistanceBetweenArrays), rayOrigin.y);
            Debug.DrawRay(rayVector, rayDistance * rayDirection, Color.red);
            allRayHits = Physics2D.RaycastAll(rayVector, rayDirection, rayDistance, Parameters.layerMask);

            if (allRayHits.Length == 0)
                continue;

            foreach (var raycastHit in allRayHits)
            {
                if (raycastHit.transform.gameObject == gameObject)
                {
                    //ignored
                }
                else
                {
                    bool pushable = false;
                    if (raycastHit.transform.GetComponent<SuperActor>())
                    {
                        pushable = raycastHit.transform.GetComponent<SuperActor>().Parameters.Pushable;
                    }
                    collisionDetails.Add(new CollisionDetails(raycastHit.transform.gameObject, raycastHit.distance - skinWidth, pushable));
                }
            }

        }

        IEnumerable<CollisionDetails> ordered = collisionDetails.OrderBy(h => h.DistanceToHit);
        float max = rayDistance + skinWidth + .01f;

        foreach (CollisionDetails item in ordered)
        {
            if (isGoingUp)
            {
                _ControllerState.IsCollidingUp = true;
            }
            else
            {
                _ControllerState.IsCollidingDown = true;
                SetVerticalVelocity(0f);
            }
            if (!item.Movable)
            {
                if (item.DistanceToHit <= Mathf.Abs(deltaMovementY))
                {
                    if (max > item.DistanceToHit)
                    {
                        deltaMovementY = item.DistanceToHit * rayDirection.y;
                        max = Mathf.Abs(deltaMovementY);
                    }
                }
            }
            else
            {
                if (item.Movable && !item.Go.GetComponent<SuperActor>()._ControllerState.DisablePush)
                {
                    if (item.Go.GetComponent<SuperActor>()._ControllerState.timePushed > 50)
                    {
                        Debug.Log("YOOOO");
                        item.Go.GetComponent<SuperActor>().Active = false;
                        GameObject.Destroy(item.Go);
                        continue;
                    }

                    else if (max > item.DistanceToHit && !item.Go.GetComponent<SuperActor>()._ControllerState.pushedBy.Contains(gameObject))
                    {
                        item.Go.GetComponent<SuperActor>()._ControllerState.DisablePush = true;
                        item.Go.GetComponent<SuperActor>()._ControllerState.timePushed += 1;
                        item.Go.GetComponent<SuperActor>()._ControllerState.pushedBy.Add(gameObject);
                        float fullmove = (Mathf.Abs(rayDistance) - skinWidth);
                        float amountToMoveItem = (fullmove - item.DistanceToHit) * rayDirection.y;
                        item.Go.GetComponent<SuperActor>().moveVertically(ref amountToMoveItem);
                        deltaMovementY = item.DistanceToHit * rayDirection.y + amountToMoveItem;
                        max = Mathf.Abs(deltaMovementY);
                    }

                }
            }
        }

        foreach (CollisionDetails item in ordered)
        {
            if (item.Go.GetComponent<SuperActor>())
            {
                item.Go.GetComponent<SuperActor>()._ControllerState.DisablePush = false;
            }
        }
        _ControllerState.pushedBy.Clear();
        _transform.Translate(0, deltaMovementY, 0, Space.World);
        calculateRayOrigins();
    }
}
