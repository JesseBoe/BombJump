using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Trail : MonoBehaviour
{
    private SpriteRenderer _spriteRenderer;
    public GameObject Self;

    private float timePassed;

	// Use this for initialization
	void Start ()
    {
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _spriteRenderer.color = Color.red;
        timePassed = 0f;
	}

    public void SetSprite(Sprite trailSprite)
    {
        Self.GetComponent<SpriteRenderer>().sprite = trailSprite;
    }

    // Update is called once per frame
    void Update()
    {

    }
    private void FixedUpdate()
    {
        gameObject.transform.position += new Vector3(0f, 0f, .01f);
        gameObject.transform.localScale = new Vector3(Mathf.Lerp(1, 0, timePassed), Mathf.Lerp(1, 0, timePassed), 1);
        timePassed += Time.deltaTime * 3;

        if (timePassed >= 1)
        {
            GameObject.Destroy(gameObject);
        }
    }
}
