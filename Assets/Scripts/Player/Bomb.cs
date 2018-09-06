using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    
    private Sprite[] bombSprites;
    private Sprite[] explosionSprites;
    private SuperActor _actor;
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _explosionSpriteRenderer;
    private Throwable throwable;
    private bool wasGrounded = false;

    public float time;
    private float aniSpeed;
    public bool exploded;

    private List<string> checkCollides = new List<string>();
    private List<int> keepChecking = new List<int>();
    private LayerMask defaultmask;

    public GameObject GoExplosion;

    public BombParameters _parameters; // set in unity editor

    // Use this for initialization
    void Start ()
    {
		bombSprites = Resources.LoadAll<Sprite>("Sprites/SunrealBomb");
        explosionSprites = Resources.LoadAll<Sprite>("Sprites/Explosion");
        _explosionSpriteRenderer = GoExplosion.GetComponent<SpriteRenderer>();
        _actor = GetComponent<SuperActor>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        throwable = GetComponent<Throwable>();
        time = 0;
        aniSpeed = 2F;
        exploded = false;

        defaultmask = _actor.Parameters.layerMask;
        _actor.Parameters.layerMask = LayerMask.NameToLayer("Intangible");
        gameObject.layer = LayerMask.NameToLayer("Intangible");
    }

    // Update is called once per frame
    void Update()
    {
        if (_actor._ControllerState.IsGrounded && !wasGrounded)
        {
            ActorManager.instance.PlaySound("Land", 1f);
        }
        checkSpikes();
        wasGrounded = _actor._ControllerState.IsGrounded;
    }

    private void FixedUpdate()
    {
        bombThings();
        if (throwable.Thrown)
        {
            if (gameObject.layer == LayerMask.NameToLayer("Intangible") && !exploded)
            {
                adjustLayers();
            }

            bool isGoingRight = throwable.ThrowVelocity.x > 0;

            if (isGoingRight)
            {
                if (_actor._ControllerState.IsCollidingRight)
                {
                    throwable.ThrowVelocity = new Vector2(0, 0);
                }
            }
            else
            {
                if (_actor._ControllerState.IsCollidingLeft)
                {
                    throwable.ThrowVelocity = new Vector2(0, 0);
                }
            }

            if (_actor._ControllerState.IsCollidingDown)
            {
                throwable.ThrowVelocity = new Vector2(throwable.ThrowVelocity.x / 1.2f, 0);
                if (Mathf.Abs(throwable.ThrowVelocity.x) < 20)
                {
                    throwable.ThrowVelocity = new Vector2(0, 0);
                }
            }

            _actor.SetHorizontalVeloicty(throwable.ThrowVelocity.x);
        }
    }

    void checkSpikes()
    {
        if (!exploded)
        {
            RaycastHit2D[] hits = Physics2D.BoxCastAll((Vector2)transform.position + _actor._Collider.offset, _actor._Collider.size, 0, Vector2.zero, Mathf.Infinity, LayerMask.GetMask("Spike"));
            if (hits.Length > 0)
            {
                time = _parameters.TimeToExplode;
            }
        }
    }

    void bombThings()
    {
        time += Time.deltaTime * aniSpeed;
        if (time < _parameters.TimeToExplode)
        {
            _spriteRenderer.sprite = bombSprites[(int)Mathf.Floor(Mathf.Lerp(0, 4, time % 1))];
        }
        else
        {
            if (!exploded)
            {
                ActorManager.instance.PlaySound("BombExplosionFinal", 1);
                _actor.Active = false;
                _spriteRenderer.sprite = null;
                _actor.Parameters.layerMask = LayerMask.NameToLayer("Intangible");
                gameObject.layer = LayerMask.NameToLayer("Intangible");
                GoExplosion.transform.SetParent(null);
                Vector2 center = new Vector2(transform.position.x + _actor._Collider.offset.x, transform.position.y + _actor._Collider.offset.y );
                var explosionHitList = Physics2D.CircleCastAll(center, _parameters.EffectiveRange, Vector2.zero, 0f, LayerMask.GetMask("Player", "Bomb"), -40f, 5f);
                if (explosionHitList.Length > 0)
                {
                    foreach (var rayCastHit in explosionHitList)
                    {
                        if (rayCastHit.transform.gameObject == gameObject)
                            continue;


                        ActorManager.instance.PlaySound("Hit_Hurt", 1f);
                        Vector2 rayhitCenter = new Vector2(rayCastHit.transform.position.x + rayCastHit.transform.GetComponent<BoxCollider2D>().offset.x, rayCastHit.transform.position.y + rayCastHit.transform.GetComponent<BoxCollider2D>().offset.y);
                        float distance = Mathf.Sqrt((rayhitCenter.x - center.x) * (rayhitCenter.x - center.x) + (rayhitCenter.y - center.y) * (rayhitCenter.y - center.y));
                        var degree = Mathf.Atan2(rayhitCenter.y - center.y, rayhitCenter.x - center.x) * 180 / Mathf.PI;
                        Vector2 dir = new Vector2(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad));
                        if (dir.x == 1)
                        {
                            dir.x = 0;
                        }
                        dir.y += .65f;
                        if (dir.y > .9)
                        {
                            dir.y = .9f;
                        }
                        Mathf.Clamp(dir.x, -1f, 1f);
                        float magnitude = (_parameters.EffectiveRange - distance) / _parameters.EffectiveRange;
                        //Debug.Log(magnitude);
                        Vector2 force = new Vector2(Mathf.Lerp(_parameters.MinKnockBack.x, _parameters.MaxKnockBack.x, magnitude) * dir.x, Mathf.Lerp(_parameters.MinKnockBack.y, _parameters.MaxKnockBack.y, magnitude) * dir.y);
                        Debug.Log(force);
                        rayCastHit.transform.GetComponent<SuperActor>().KnockBack(force);
                        if (rayCastHit.transform.GetComponent<Bomb>())
                        {
                            if (!rayCastHit.transform.GetComponent<Bomb>().exploded && rayCastHit.transform.GetComponent<Bomb>().time < rayCastHit.transform.GetComponent<Bomb>()._parameters.TimeToExplode - .2f)
                            {
                                rayCastHit.transform.GetComponent<Bomb>().time = rayCastHit.transform.GetComponent<Bomb>()._parameters.TimeToExplode + Random.Range(-.2f, -.15f);
                            }
                        }
                    }
                }

                exploded = true;
            }
            if (time >= _parameters.TimeToExplode + 1)
            {
                removeBomb();
            }
            else
            {
                _explosionSpriteRenderer.sprite = explosionSprites[(int)Mathf.Floor(Mathf.Lerp(0, 10, time - _parameters.TimeToExplode))];
            }
        }
    }

    private void adjustLayers()
    {
        //This slowly activates collisions so that if spawned inside of another object everything works out.
        RaycastHit2D[] hits = Physics2D.BoxCastAll((Vector2)transform.position + _actor._Collider.offset, _actor._Collider.size, 0, Vector2.zero, Mathf.Infinity, defaultmask);
        if (hits.Length > 0)
        {
            keepChecking.Clear();
            foreach (var item in hits)
            {
                if (!checkCollides.Contains(LayerMask.LayerToName(item.transform.gameObject.layer)))
                {
                    keepChecking.Add(item.transform.gameObject.layer);
                }
            }
            for (int i = 0; i < 28; i++)
            {
                if ((defaultmask == (defaultmask | (1 << i))))
                {
                    if (!keepChecking.Contains(i))
                    {
                        checkCollides.Add(LayerMask.LayerToName(i));
                    }
                }
            }
        }
        else
        {
            for (int i = 0; i < 28; i++)
            {
                if ((defaultmask == (defaultmask | (1 << i))))
                {
                    if (!checkCollides.Contains(LayerMask.LayerToName(i)))
                    {
                        checkCollides.Add(LayerMask.LayerToName(i));
                    }
                }
            }
            gameObject.layer = LayerMask.NameToLayer("Bomb");
        }
        _actor.Parameters.layerMask = LayerMask.GetMask(checkCollides.ToArray());
        if (_actor.Parameters.layerMask == defaultmask)
        {
            gameObject.layer = LayerMask.NameToLayer("Bomb");
        }
    }

    public void removeBomb()
    {
        _actor.Remove();
        GameObject.Destroy(GoExplosion);
        GameObject.Destroy(transform.parent.gameObject);
    }

    void OnBecameInvisible()
    {
        if (throwable.Thrown)
        {
            if (isActiveAndEnabled)
            {
                StartCoroutine("destroyTimer");
            }
        }
    }

    private void OnBecameVisible()
    {
        StopCoroutine("destroyTimer");
    }

    IEnumerator destroyTimer()
    {
        float destroytime = 0;
        while (destroytime < 1)
        {
            destroytime += Time.deltaTime;
            yield return new WaitForEndOfFrame();
        }
        removeBomb();
    }
}

