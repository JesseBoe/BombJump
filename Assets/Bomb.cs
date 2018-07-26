using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : MonoBehaviour {
    
    private Sprite[] bombSprites;
    private Sprite[] explosionSprites;
    private SuperActor _actor;
    private SpriteRenderer _spriteRenderer;
    private SpriteRenderer _explosionSpriteRenderer;

    public float time;
    private float aniSpeed;
    public bool exploded;

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
        time = 0;
        aniSpeed = 2F;
        exploded = false;
    }
	
	// Update is called once per frame
	void Update ()
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
                _actor.Active = false;
                _spriteRenderer.sprite = null;
                _actor.Parameters.layerMask = LayerMask.NameToLayer("Intangible");
                gameObject.layer = LayerMask.NameToLayer("Intangible");
                GoExplosion.transform.SetParent(null);
                Vector2 offSet = new Vector2(56, 67);
                Vector2 center = (Vector2)GoExplosion.transform.position + offSet;
                var explosionHitList = Physics2D.CircleCastAll(center, _parameters.EffectiveRange, Vector2.zero, 0f, LayerMask.GetMask("Player", "Bomb"), -15f, 15f);

                if (explosionHitList.Length > 0)
                {
                    foreach (var rayCastHit in explosionHitList)
                    {
                        if (rayCastHit.transform.gameObject == gameObject)
                            continue;
                        Vector2 closest = rayCastHit.collider.bounds.ClosestPoint(center);
                        float distance = Mathf.Sqrt((closest.x - center.x) * (closest.x - center.x) + (closest.y - center.y) * (closest.y - center.y));
                        var degree = Mathf.Atan2(closest.y - center.y, closest.x - center.x) * 180 / Mathf.PI;
                        Vector2 dir = new Vector2(Mathf.Cos(degree * Mathf.Deg2Rad), Mathf.Sin(degree * Mathf.Deg2Rad));
                        dir.y += .65f;
                        if (dir.y > .9)
                        {
                            dir.y = .9f;
                        }
                        float horizontalSkew = 0;
                        if (rayCastHit.transform.position.x < transform.position.x)
                        {
                            horizontalSkew -= .35f;
                        }
                        else if (rayCastHit.transform.position.x > transform.position.x)
                        {
                            horizontalSkew += .35f;
                        }
                        dir.x += horizontalSkew;
                        Mathf.Clamp(dir.x, -1f, 1f);
                        float magnitude = (_parameters.EffectiveRange - distance) / _parameters.EffectiveRange;
                        Vector2 force = new Vector2(Mathf.Lerp(_parameters.MinKnockBack.x, _parameters.MaxKnockBack.x, magnitude) * dir.x, Mathf.Lerp(_parameters.MinKnockBack.y, _parameters.MaxKnockBack.y, magnitude) * dir.y);

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
                GameObject.Destroy(GoExplosion);
                GameObject.Destroy(gameObject);
            }
            else
            {
                _explosionSpriteRenderer.sprite = explosionSprites[(int)Mathf.Floor(Mathf.Lerp(0, 10, time - _parameters.TimeToExplode))];
            }
        }
	}
}
