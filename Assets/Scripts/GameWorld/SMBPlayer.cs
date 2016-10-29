using UnityEngine;
using System.Collections;

public class SMBPlayer : MonoBehaviour {

	public float xSpeed = 1f;
	public float ySpeed = 5f;

	private Rigidbody2D _rigidbody;

	void Start() {

		_rigidbody = GetComponent<Rigidbody2D> ();
	}

	// Update is called once per frame
	void Update () {

		if(Input.GetKeyDown(KeyCode.Space))
			Jump();

		if (Input.GetKey (KeyCode.LeftArrow))
			Move (-1f);

		if (Input.GetKey (KeyCode.RightArrow))
			Move (1f);

		if (Input.GetKeyUp (KeyCode.LeftArrow) || Input.GetKeyUp (KeyCode.RightArrow)) {

			Vector2 currentVelocity = _rigidbody.velocity;
			currentVelocity.x = 0f;
			_rigidbody.velocity = currentVelocity;
		}

	}

	void Jump() {

		_rigidbody.velocity += Vector2.up * ySpeed * Time.fixedDeltaTime;
	}

	void Move(float side) {

		Vector2 currentVelocity = _rigidbody.velocity;
		currentVelocity.x = (xSpeed * side) * Time.fixedDeltaTime;
		_rigidbody.velocity = currentVelocity;
	}


}
