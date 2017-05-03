using UnityEngine;
using System.Collections;

public class SMBCoin : SMBItem {

	public float spawnForce = 1f;
	public float posVertDelta = 0.25f;
	public float timeToDestroy = 0.25f;

	void OnSpawnStart() {

		transform.position += Vector3.up * posVertDelta;
	}

	void OnSpawn() {

		_body.applyGravity = true;
		_body.ApplyForce (Vector2.up * spawnForce);

		Invoke ("SolveCollision", timeToDestroy);
	}
		
	void SolveCollision() {

		SMBGameWorld.Instance.PlayParticle (transform.position, "SMBCoinParticleSystem");
		SMBGameWorld.Instance.PlaySoundEffect ((int)SMBConstants.GameWorldSoundEffects.Coin);

		Destroy (gameObject);
	}

	protected override void OnVerticalCollisionEnter(Collider2D collider) {

		SolveCollision ();
		base.OnVerticalCollisionEnter (collider);
	}

	protected override void OnHorizontalCollisionEnter(Collider2D collider) {

		SolveCollision ();
		base.OnHorizontalCollisionEnter (collider);
	}
}
