using UnityEngine;
using System.Collections;

public class SMBPlayer : MonoBehaviour {

	public float xSpeed = 1f;
	public float ySpeed = 5f;

	private bool _isOnGround = false;

	// Custom components
	private Animator       _animator;
	private Rigidbody2D    _rigidbody;
	private BoxCollider2D  _collider;
	private SpriteRenderer _renderer;

	void Awake() {

		_animator = GetComponent<Animator> ();
		_rigidbody = GetComponent<Rigidbody2D> ();
		_collider = GetComponent<BoxCollider2D> ();
		_renderer = GetComponent<SpriteRenderer> ();
	}

	// Update is called once per frame
	void Update () {

		_isOnGround = IsOnGround ();
		_animator.SetBool ("isJumping", !_isOnGround);

		if (Input.GetKeyDown (KeyCode.Z))
			Jump ();

		if (Input.GetKey (KeyCode.LeftArrow)) {

			Move ((float)SMBConstants.MoveDirection.Backward);
			_animator.SetBool ("isMoving", true);
		}

		if (Input.GetKey (KeyCode.RightArrow)) {

			Move ((float)SMBConstants.MoveDirection.Forward);
			_animator.SetBool ("isMoving", true);
		}

		if (Input.GetKeyUp (KeyCode.LeftArrow) || Input.GetKeyUp (KeyCode.RightArrow)) {

			Vector2 currentVelocity = _rigidbody.velocity;
			currentVelocity.x = 0f;
			_rigidbody.velocity = currentVelocity;

			_animator.SetBool ("isMoving", false);
		}
	}

	void Jump() {

		if (_isOnGround) {
			
			_rigidbody.velocity += Vector2.up * ySpeed * Time.fixedDeltaTime;
		}
	}

	void Move(float side) {

		Vector2 currentVelocity = _rigidbody.velocity;
		currentVelocity.x = (xSpeed * side) * Time.fixedDeltaTime;
		_rigidbody.velocity = currentVelocity;

		if (side == (float)SMBConstants.MoveDirection.Forward)
			_renderer.flipX = false;

		if (side == (float)SMBConstants.MoveDirection.Backward)
			_renderer.flipX = true;

		// Lock player x position
		Vector3 playerPos = transform.position;
		playerPos.x = Mathf.Clamp (playerPos.x, SMBGameWorld.Instance.LockLeftX - SMBGameWorld.Instance.TileMap.size, 
			SMBGameWorld.Instance.LockRightX);
		transform.position = playerPos;
	}

	bool IsOnGround() {

		Vector2 rayOrigin = _collider.bounds.center;
		rayOrigin.y -= _collider.bounds.extents.y + 0.01f;
		RaycastHit2D ray = Physics2D.Raycast(rayOrigin, -Vector2.up, 0.01f);

		return (ray.collider && ray.collider.tag == "Platform");
	}
}
