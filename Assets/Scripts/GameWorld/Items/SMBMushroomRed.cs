using UnityEngine;
using System.Collections;

[RequireComponent(typeof(SMBCollider))]
public class SMBMushroomRed : SMBItem {

	public float xSpeed = 1f;
	public float ySpeed = 1f;

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

		Invoke ("MoveRandomDirection", 0.7f);
	}

	void MoveRandomDirection () {

		_body.applyGravity = true;
		_collider.applyHorizCollision = true;
		_collider.applyVertCollision = true;

		float randomSide = (Random.value < 0.5f ? -1f : 1f);
		_body.velocity.x = xSpeed * randomSide * Time.fixedDeltaTime;
		_body.velocity.y = 0f;
	}
}
