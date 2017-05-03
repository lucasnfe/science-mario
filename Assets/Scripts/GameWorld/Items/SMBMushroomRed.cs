using UnityEngine;
using System.Collections;

public class SMBMushroomRed : SMBItem {

	public float xSpeed = 1f;
	public float ySpeed = 1f;
	public float timeToSpawn = 1f;

	private float _side;

	protected SMBCollider _collider;

	override protected void Awake() {

		_collider = GetComponent<SMBCollider> ();

		base.Awake ();
	}

	void OnSpawnStart() {

		_collider.applyHorizCollision = false;
		_collider.applyVertCollision = false;

		_body.applyGravity = false;
	}

	void OnSpawn() {

		_body.velocity.y = ySpeed * Time.fixedDeltaTime;
		_audio.Play ();

		Invoke ("MoveRandomDirection", timeToSpawn);
	}

	// Update is called once per frame
	void Update () {

		_body.velocity.x = xSpeed * _side * Time.fixedDeltaTime;
	}

	void MoveRandomDirection () {

		_body.applyGravity = true;
		_collider.applyHorizCollision = true;
		_collider.applyVertCollision = true;

		_side = (Random.value < 0.5f ? (float)SMBConstants.MoveDirection.Backward : 
			(float)SMBConstants.MoveDirection.Forward);
		
		_body.velocity.x = xSpeed * _side * Time.fixedDeltaTime;
		_body.velocity.y = 0f;
	}

	void SolveCollision(Collider2D collider) {

		if (collider.tag == "Player") {

			collider.gameObject.SendMessage ("GrowUp");
			OnInteraction ();
		}
	}

	void OnVerticalCollisionEnter(Collider2D collider) {

		SolveCollision (collider);
	}

	void OnHorizontalCollisionEnter(Collider2D collider) {

		SolveCollision (collider);

		if (collider.tag != "Player")
			_side *= -1f;
	}
}
