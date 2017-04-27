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

		Invoke ("DestroyAfterSpawn", timeToDestroy);
	}
		
	void DestroyAfterSpawn() {

		OnInteraction ();
	}

	override protected void OnInteraction() {

		SMBGameWorld.Instance.PlayParticle (transform.position, "SMBCoinParticleSystem");
		SMBGameWorld.Instance.PlaySoundEffect ((int)SMBConstants.GameWorldSoundEffects.Coin);

		base.OnInteraction ();
	}
}
