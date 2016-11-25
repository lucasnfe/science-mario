using UnityEngine;
using System.Collections;

public class SMBCoin : SMBItem {

	void OnSpawnStart() {

		transform.position += Vector3.up * 0.15f;
	}

	void OnSpawn() {

		_body.applyGravity = true;
		_body.ApplyForce (Vector2.up * 5f);

		Invoke ("DestroyAfterSpawn", 0.25f);
	}
		
	void DestroyAfterSpawn() {

		OnInteraction ();
	}

	override protected void OnInteraction() {

		SMBGameWorld.Instance.PlayParticle (transform.position);
		SMBGameWorld.Instance.PlaySoundEffect ((int)SMBConstants.GameWorldSoundEffects.Coin);

		base.OnInteraction ();
	}
}
