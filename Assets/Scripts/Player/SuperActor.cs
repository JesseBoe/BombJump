using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class SuperActor : MonoBehaviour {


    public Vector2 Velocity;
    public ControllerState2D _ControllerState;

    //Actor Parameters controlls things like Gravity, how far things get knocked back and other things. It can be overrided during gameplay by setting _overrideParamaters
    public ActorParameters DefaultParamters;
    public ActorParameters Parameters { get { return _overrideParameters ?? DefaultParamters; } }
    public bool Active = true;

    public BoxCollider2D _Collider;
    private ActorParameters _overrideParameters;
    private Transform _transform;
    private List<CollisionDetails> collisionDetails = new List<CollisionDetails>(32);
    private RaycastHit2D[] allRayHits = new RaycastHit2D[32];
    //The corners of our box collider.
    private Vector2 _cornerBottomLeft, _cornerTopLeft, _cornerBottomRight;
    private float _verticalDistanceBetweenArrays, _horizontalDistanceBetweenArrays;
    private Vector2 knockBackVelocity;

    private const float skinWidth = 1f;
    public int verticalRays = 4;
    public int horizontalRays = 6;

    // Use this for initialization
    void Start ()
    {
        _ControllerState = new ControllerState2D();
        _ControllerState.IsFacingRight = true;
        _transform = gameObject.transform;
        Velocity = Vector2.zero;
        _Collider = gameObject.GetComponent<BoxCollider2D>();

        _horizontalDistanceBetweenArrays = (_Collider.size.x - skinWidth * 2) / (verticalRays - 1);
        _verticalDistanceBetweenArrays = (_Collider.size.y - skinWidth * 2) / (horizontalRays - 1);
        calculateRayOrigins();
        GameObject.FindGameObjectWithTag("Manager").GetComponent<ActorManager>().Actors.Add(this);
    }

    // Update is called once per frame
    void Update () {
		
	}

    private void FixedUpdate()
    {

    }

    public void Act()
    {
        if (Active)
        {
            if (Parameters.StarSnap)
            {
                starRide();
            }

            addVerticalVelocity(Parameters.Gravity);
            handleKnockBack();
            handleMovement(knockBackVelocity);
            handleMovement(Velocity * Time.deltaTime);
            _ControllerState.HasMoved = true;

            _ControllerState.standingOnMe.Clear();
        }
    }

    private void handleMovement(Vector2 deltaMovement)
    {
        //Here is where the magic happens. Through many raycasts our object is capable of movement.

        _ControllerState.Reset();

        if (Mathf.Abs(deltaMovement.y) > .02f)
            moveVertically(ref deltaMovement.y);
        else
            deltaMovement.y = 0f;

        if (Mathf.Abs(deltaMovement.x) > .02f)
            moveHorizontally(ref deltaMovement.x);
        else
        {
            deltaMovement.x = 0f;
        }
    }

    public void Remove()
    {
        GameObject.FindGameObjectWithTag("Manager").GetComponent<ActorManager>().Actors.Remove(this);
        GameObject.Destroy(gameObject);
    }

    private void calculateRayOrigins()
    {
        var center = new Vector2(_Collider.offset.x, _Collider.offset.y);

        _cornerTopLeft = (Vector2)_transform.position + new Vector2(center.x - _Collider.size.x / 2 + skinWidth, center.y + _Collider.size.y / 2 - skinWidth);
        _cornerBottomLeft = (Vector2)_transform.position + new Vector2(center.x - _Collider.size.x / 2 + skinWidth, center.y - _Collider.size.y / 2 + skinWidth);
        _cornerBottomRight = (Vector2)_transform.position + new Vector2(center.x + _Collider.size.x / 2 - skinWidth, center.y - _Collider.size.y / 2 + skinWidth);
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

    public float GetColliderHeightCord()
    {
        return _cornerTopLeft.y + skinWidth;
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
            if (!item.Movable || item.Movable && item.Go.GetComponent<SuperActor>().Parameters.pushPriority > Parameters.pushPriority)
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
                        Debug.Log("A object has collided over 50 times in one frame. fuck me");
                        item.Go.GetComponent<SuperActor>().Active = false;
                        Remove();
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

        if (_ControllerState.standingOnMe.Count > 0)
        {
            foreach (var item in _ControllerState.standingOnMe)
            {
                if (!item.GetComponent<SuperActor>().Parameters.IgnorePlatforms)
                {
                    item.GetComponent<SuperActor>().moveHorizontally(ref deltaMovementX);
                }
            }
        }
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
                if (_ControllerState.standingOn == null)
                {
                    _ControllerState.standingOn = item.Go;
                    if (item.Go.GetComponent<SuperActor>() && !item.Go.GetComponent<SuperActor>()._ControllerState.standingOnMe.Contains(gameObject))
                    {
                        item.Go.GetComponent<SuperActor>()._ControllerState.standingOnMe.Add(gameObject);
                    }
                }
            }
            if (!item.Movable || item.Movable && item.Go.GetComponent<SuperActor>().Parameters.pushPriority > Parameters.pushPriority)
            {
                if (item.DistanceToHit <= Mathf.Abs(deltaMovementY))
                {
                    if (max > item.DistanceToHit)
                    {
                        deltaMovementY = item.DistanceToHit * rayDirection.y;
                        max = Mathf.Abs(deltaMovementY);
                        if (_ControllerState.IsCollidingUp && Velocity.y > 0)
                        {
                            if (Parameters.Bouncy)
                            {
                                Velocity.y *= -1f;
                            }
                            else
                            {
                                Velocity.y = 0;
                            }
                        }
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
                        Remove();
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

        if (_ControllerState.IsCollidingDown && Parameters.Bouncy)
        {
            if (Velocity.y < 0)
            {
                SetVerticalVelocity(Velocity.y * -Parameters.BounceDamp);
                if (Mathf.Abs(Velocity.y) < 40f)
                {
                    SetVerticalVelocity(0);
                }
            }
            else
            {

            }
        }

        else if (_ControllerState.IsCollidingDown && Velocity.y < 0)
        {
            SetVerticalVelocity(0);
        }

        _ControllerState.pushedBy.Clear();
        _transform.Translate(0, deltaMovementY, 0, Space.World);
        calculateRayOrigins();          
    }

    private void starRide()
    {
        calculateRayOrigins();
        var rayDistance = 14f + skinWidth;
        var rayDirection = Vector2.down;
        var rayOrigin = _cornerBottomLeft;

        if (_ControllerState.standingOn != null && _ControllerState.standingOn.GetComponent<Star>())
        {
            _ControllerState.isStarRiding = true;
        }

        if (_ControllerState.isStarRiding)
        {
            bool stillOnStar = false;
            for (int i = 0; i < verticalRays; i++)
            {
                var rayVector = new Vector2(rayOrigin.x + (i * _horizontalDistanceBetweenArrays), rayOrigin.y);
                RaycastHit2D ray = Physics2D.Raycast(rayVector, rayDirection, rayDistance, LayerMask.GetMask("Star"));

                if (ray)
                {
                    if (!ray.transform.GetComponent<SuperActor>()._ControllerState.standingOnMe.Contains(gameObject))
                    {
                        ray.transform.GetComponent<SuperActor>()._ControllerState.standingOnMe.Add(gameObject);
                        addVerticalVelocity(-500f);
                        stillOnStar = true;
                    }
                    break;
                }
            }
            _ControllerState.isStarRiding = stillOnStar;
        }
    }
}
