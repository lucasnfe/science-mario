using UnityEngine;
using System.Collections;

public class SMBPlayer : MonoBehaviour {

	enum MoveDirection {
		Forward  =  1,
		Backward = -1
	}

	public float xSpeed = 1f;
	public float ySpeed = 5f;

	private bool _isOnGround = false;

	// Custom components
	private Rigidbody2D    _rigidbody;
	private Animator       _animator;
	private SpriteRenderer _renderer;

	void Start() {

		_rigidbody = GetComponent<Rigidbody2D> ();
		_animator = GetComponent<Animator> ();
		_renderer = GetComponent<SpriteRenderer> ();
	}

	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Space))
			Jump();

		if (Input.GetKey (KeyCode.LeftArrow)) {

			Move ((float)MoveDirection.Backward);
			_animator.SetBool ("isMoving", true);
		}

		if (Input.GetKey (KeyCode.RightArrow)) {

			Move ((float)MoveDirection.Forward);
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

		if (side == (float)MoveDirection.Forward)
			_renderer.flipX = false;

		if (side == (float)MoveDirection.Backward)
			_renderer.flipX = true;
	}

	void OnCollisionEnter2D(Collision2D coll) {

		if (coll.gameObject.tag == "Platform") {

			foreach (ContactPoint2D contact in coll.contacts) {
				if (contact.normal == Vector2.up) {
					_isOnGround = true;
					break;
				}
			}
		}
	}

	void OnCollisionExit2D(Collision2D coll) {

		if (coll.gameObject.tag == "Platform") {

			foreach (ContactPoint2D contact in coll.contacts) {
				if (contact.normal == Vector2.up) {
					_isOnGround = false;
					break;
				}
			}
		}
	}
}
