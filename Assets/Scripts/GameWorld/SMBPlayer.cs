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
	private Rigidbody2D _rigidbody;

	void Start() {

		_rigidbody = GetComponent<Rigidbody2D> ();
	}

	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Space))
			Jump();

		if (Input.GetKey (KeyCode.LeftArrow))
			Move ((float)MoveDirection.Backward);

		if (Input.GetKey (KeyCode.RightArrow))
			Move ((float)MoveDirection.Forward);

		if (Input.GetKeyUp (KeyCode.LeftArrow) || Input.GetKeyUp (KeyCode.RightArrow)) {

			Vector2 currentVelocity = _rigidbody.velocity;
			currentVelocity.x = 0f;
			_rigidbody.velocity = currentVelocity;
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
