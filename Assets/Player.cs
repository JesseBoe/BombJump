using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

public class Player : MonoBehaviour
{
    /*
    private Sprite[] _cactimanSprites;
    private GameObject _trailPrefab;
    private CactimanController _controller;
    private SpriteRenderer _spriteRenderer;

    private int normalizedHorizontal;

	// Use this for initialization
	void Start ()
    {
        _controller = GetComponent<CactimanController>();
        _spriteRenderer = GetComponent<SpriteRenderer>();
        _controller._ControllerState.IsFacingRight = true;
        _trailPrefab = Resources.Load<GameObject>("Prefabs/CactimanTrail");
        _cactimanSprites = Resources.LoadAll<Sprite>("Sprites/CactimanSpriteSheet");
    }
	
	// Update is called once per frame
	void Update ()
    {
        handleInput();
        if (_controller._ControllerState.HasFullControl)
        {
            _spriteRenderer.color = Color.white;
            _controller.SetHorizontalVelocity(200 * normalizedHorizontal);
        }
        else if (_controller._ControllerState.IsDashing)
        {
            trail();
            _spriteRenderer.color = Color.red;
            _controller.Dash(normalizedHorizontal);
        }  
        else if (_controller._ControllerState.IsCrashing)
        {
            if (_controller._ControllerState.IsGrounded && !_controller._ControllerState.FlagCrash)
            {
                _controller._ControllerState.IsCrashing = false;
            }
        }
        _spriteRenderer.sprite = _controller._ControllerState.IsFacingRight ? _cactimanSprites[2] : _cactimanSprites[0];

	}

    private void handleInput()
    {

        if (_controller.CanJump() && Input.GetKeyDown(KeyCode.Space))
            _controller.Jump();

        normalizedHorizontal = 0;

        if (Input.GetKey(KeyCode.A))
        {
            normalizedHorizontal -= 1;

            //If you are dashing and grounded you cant change direction. Dash will change your direction if you are in air
            if (_controller._ControllerState.HasFullControl)
                _controller._ControllerState.IsFacingRight = false;
        }
        if (Input.GetKey(KeyCode.D))
        {
            normalizedHorizontal += 1;

            //If you are dashing and grounded you cant change direction. Dash will change your direction if you are in air
            if (_controller._ControllerState.HasFullControl)
                _controller._ControllerState.IsFacingRight = true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse1))
        {
            if (_controller.CanDash())
                _controller._ControllerState.IsDashing = true;
        }

        if (Input.GetKeyDown(KeyCode.Mouse0))
        {
            EditorApplication.isPaused = true;
        }
    }

    private void trail()
    {
        GameObject cactiTrail = (GameObject)Instantiate(_trailPrefab, new Vector3(transform.position.x, transform.position.y, transform.position.z + 1f), Quaternion.identity);
        cactiTrail.gameObject.GetComponent<Trail>().SetSprite(_controller._ControllerState.IsFacingRight ? _cactimanSprites[3] : _cactimanSprites[1]);
    }
    */
}
