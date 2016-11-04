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

		_rigidbody = GetComponent<Rigidbody2D> ();
		_collider = GetComponent<BoxCollider2D> ();
		_animator = GetComponent<Animator> ();
		_renderer = GetComponent<SpriteRenderer> ();
	}

	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Z))
			Jump();

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

		if(_isOnGround)
			_rigidbody.velocity += Vector2.up * ySpeed * Time.fixedDeltaTime;
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

	void OnCollisionEnter2D(Collision2D coll) {

		if (coll.gameObject.tag == "Platform") {

			Vector3 contactPoint = coll.collider.bounds.center;
			Vector3 center = _collider.bounds.center;

			if (center.y > contactPoint.y) {

				_isOnGround = true;
				_animator.SetBool ("isJumping", false);
			}
		}
	}

	void OnCollisionExit2D(Collision2D coll) {

		if (coll.gameObject.tag == "Platform") {

			Vector3 contactPoint = coll.collider.bounds.center;
			Vector3 center = _collider.bounds.center;

			if (center.y > contactPoint.y) {

				_isOnGround = false;
				_animator.SetBool ("isJumping", true);
			}
		}
	}
}
